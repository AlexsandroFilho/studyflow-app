# RAG de Anatomia

## Configuração local

Mantenha os segredos fora do repositório. Configure `GEMINI_API_KEY` no ambiente ou inclua `Ai:GeminiApiKey` apenas em `appsettings.Development.json`.

O projeto usa `gemini-embedding-001` com 1536 dimensões, mantendo compatibilidade com a coluna vetorial existente, e `gemini-3.5-flash-lite` para revisões. Os modelos podem ser trocados nas configurações `Ai:EmbeddingModel` e `Ai:ChatModel`.

Para usar Supabase Storage, configure também `SupabaseStorage:Url` e `SupabaseStorage:ServiceRoleKey` em `appsettings.Development.json`. Sem esses valores, os PDFs são copiados para `App_Data/fontes-anatomia` durante o desenvolvimento.

## Funcionalidades disponíveis

- `POST /api/v1/notas/{notaId}/revisoes`: revisa a nota sem alterá-la e registra o histórico.
- `POST /api/v1/temas/{temaId}/resumos`: gera e salva um resumo fundamentado do tema.
- `GET /api/v1/temas/{temaId}/resumos`: consulta o histórico de resumos do tema autenticado.

A revisão usa a nota atual e suas conexões diretas. O resumo de tema usa todas as notas atribuídas ao tema e apenas as conexões visuais internas a ele. Em ambos os casos, as notas são contexto; as referências exibidas sempre vêm do acervo oficial recuperado.

## Banco de dados

O PostgreSQL precisa permitir a extensão `vector`. A migration `AddAnatomiaRag` cria a extensão, as tabelas de fontes/chunks/histórico e o índice HNSW para similaridade por cosseno.

```powershell
dotnet ef database update -- --environment Development
```

## Ingestão de um PDF pesquisável

```powershell
dotnet run -- ingest-anatomia --file "C:\materiais\anatomia.pdf" --title "Anatomia Humana" --author "Autor" --version "1a ed." --subject "Sistema muscular"
```

O mesmo arquivo é identificado pelo hash do conteúdo: uma nova ingestão substitui os chunks da fonte em vez de duplicá-los.

Durante a ingestão com Gemini, os embeddings são gerados em ritmo controlado e salvos em lotes. Se a API informar que a cota temporária foi atingida, o processo aguarda e tenta novamente. Caso ele seja interrompido, execute o mesmo comando: a fonte incompleta retoma pelos chunks que já foram persistidos.

## Revisões

Com a API em execução e um usuário autenticado, use:

```text
POST /api/v1/notas/{notaId}/revisoes
GET  /api/v1/notas/{notaId}/revisoes
```

A revisão utiliza apenas a nota solicitada, suas conexões diretas do mesmo usuário e chunks de fontes oficiais publicadas. A anotação original nunca é alterada.

## Resumos de tema

```text
POST /api/v1/temas/{temaId}/resumos
GET  /api/v1/temas/{temaId}/resumos
```

O resumo inclui todas as notas do tema selecionado. As setas continuam sendo recursos visuais do Canvas e também ajudam a IA a explicar relações, mas não excluem notas sem conexão. Apenas conexões cujas duas notas pertencem ao tema entram nesse contexto.

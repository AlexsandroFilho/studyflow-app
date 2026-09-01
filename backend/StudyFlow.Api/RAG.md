# RAG de Anatomia

## Configuração local

Mantenha os segredos fora do repositório. Configure `GEMINI_API_KEY` no ambiente ou inclua `Ai:GeminiApiKey` apenas em `appsettings.Development.json`.

O projeto usa `gemini-embedding-001` com 1536 dimensões, mantendo compatibilidade com a coluna vetorial existente, e `gemini-2.5-flash-lite` para revisões. Os modelos podem ser trocados nas configurações `Ai:EmbeddingModel` e `Ai:ChatModel`.

Para usar Supabase Storage, configure também `SupabaseStorage:Url` e `SupabaseStorage:ServiceRoleKey` em `appsettings.Development.json`. Sem esses valores, os PDFs são copiados para `App_Data/fontes-anatomia` durante o desenvolvimento.

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

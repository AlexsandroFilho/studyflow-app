# StudyFlow

Plataforma full stack para organizar estudos, conectar anotações e revisar conteúdos com apoio de IA.

O projeto combina uma API em ASP.NET Core com uma interface em React e TypeScript. Também inclui uma funcionalidade RAG para ingestão de PDFs e consulta contextual de conteúdos.

## Tecnologias

**Back-end**

- .NET 10 e ASP.NET Core
- Entity Framework Core
- PostgreSQL e pgvector
- Autenticação JWT
- FluentValidation
- BCrypt
- Gemini API
- Testes automatizados

**Front-end**

- React
- TypeScript
- Vite
- Tailwind CSS
- React Hook Form e Zod
- Axios

## Funcionalidades

- Cadastro e autenticação de usuários
- Criação, edição e organização de notas por temas
- Conexões entre notas em formato de grafo
- Autosave de anotações
- Rotas protegidas no front-end
- Revisão de notas com IA
- Ingestão de PDFs para criação de uma base de conhecimento
- Busca contextual com embeddings e PostgreSQL/pgvector
- Testes de serviços da API

## Estrutura

```text
studyflow/
├── backend/
│   ├── StudyFlow.Api/
│   └── StudyFlow.Api.Tests/
└── frontend/
```

## Como executar localmente

### Pré-requisitos

- .NET SDK 10
- Node.js LTS
- PostgreSQL com a extensão `vector`
- Uma chave de API do Gemini

### Back-end

```bash
cd backend/StudyFlow.Api
dotnet restore
dotnet ef database update
dotnet run
```

Configure as variáveis necessárias — banco de dados, JWT e Gemini — em variáveis de ambiente ou em `appsettings.Development.json`. Nunca versione credenciais.

### Front-end

```bash
cd frontend
npm install
npm run dev
```

## RAG e ingestão de PDFs

A funcionalidade RAG utiliza embeddings para indexar conteúdo de PDFs no PostgreSQL com `pgvector`. Para ingerir um arquivo, execute o comando disponível na API:

```bash
dotnet run -- ingest-anatomia --file "caminho/arquivo.pdf" --title "Título do material"
```

Consulte a documentação em [`backend/StudyFlow.Api/RAG.md`](backend/StudyFlow.Api/RAG.md) para os detalhes de configuração.

## Testes

```bash
cd backend
dotnet test
```

## Autor

Alexsandro Alves de Souza Filho  
[LinkedIn](https://www.linkedin.com/in/alexsandrofilho14/) · [GitHub](https://github.com/AlexsandroFilho)

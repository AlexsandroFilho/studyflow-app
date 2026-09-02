using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.DTOs;

public sealed record ReferenciaAnatomiaDto(Guid FonteId, string Fonte, int Pagina, string? Secao, string? Assunto);
public sealed record ApontamentoRevisaoDto(string Tipo, string Trecho, string Explicacao, string? Sugestao);
public sealed record ResultadoRevisaoNotaDto(
    StatusRevisaoNota Status,
    string Resumo,
    IReadOnlyList<string> PontosCorretos,
    IReadOnlyList<ApontamentoRevisaoDto> Apontamentos,
    IReadOnlyList<ReferenciaAnatomiaDto> Referencias);
public sealed record RevisaoNotaResponseDto(Guid Id, int NotaId, ResultadoRevisaoNotaDto Resultado, string Modelo, DateTime DataCriacao);
public sealed record ContextoTemaNotaDto(int NotaId, string Titulo, string Conteudo);
public sealed record ContextoTemaConexaoDto(
    int ConexaoId,
    int NotaOrigemId,
    string TituloOrigem,
    int NotaDestinoId,
    string TituloDestino,
    string? Rotulo);
public sealed record ContextoTemaDto(
    int TemaId,
    string Nome,
    string? Descricao,
    IReadOnlyList<ContextoTemaNotaDto> Notas,
    IReadOnlyList<ContextoTemaConexaoDto> Conexoes);
public sealed record RelacaoResumoTemaDto(
    int ConexaoId,
    int NotaOrigemId,
    string TituloOrigem,
    int NotaDestinoId,
    string TituloDestino,
    string? Rotulo,
    string Descricao);
public sealed record ResultadoResumoTemaDto(
    StatusResumoTema Status,
    string Resumo,
    IReadOnlyList<string> PontosChave,
    IReadOnlyList<RelacaoResumoTemaDto> Relacoes,
    IReadOnlyList<ReferenciaAnatomiaDto> Referencias);
public sealed record ResumoTemaResponseDto(Guid Id, int TemaId, ResultadoResumoTemaDto Resultado, string Modelo, DateTime DataCriacao);
public sealed record PerguntaQuizGeradaDto(
    string Enunciado,
    IReadOnlyList<string> Alternativas,
    int IndiceRespostaCorreta,
    string Explicacao,
    IReadOnlyList<ReferenciaAnatomiaDto> Referencias);
public sealed record ResultadoGeracaoQuizTemaDto(StatusQuizTema Status, string Mensagem, IReadOnlyList<PerguntaQuizGeradaDto> Perguntas);
public sealed record PerguntaQuizResponseDto(Guid Id, int Ordem, string Enunciado, IReadOnlyList<string> Alternativas);
public sealed record QuizTemaResponseDto(
    Guid Id,
    int TemaId,
    StatusQuizTema Status,
    string Mensagem,
    IReadOnlyList<PerguntaQuizResponseDto> Perguntas,
    string Modelo,
    DateTime DataCriacao);
public sealed record RespostaPerguntaQuizRequestDto(Guid PerguntaId, int IndiceAlternativa);
public sealed record CriarTentativaQuizRequestDto(IReadOnlyList<RespostaPerguntaQuizRequestDto> Respostas);
public sealed record CorrecaoPerguntaQuizDto(
    Guid PerguntaId,
    int Ordem,
    string Enunciado,
    IReadOnlyList<string> Alternativas,
    int IndiceAlternativaSelecionada,
    int IndiceRespostaCorreta,
    bool Acertou,
    string Explicacao,
    IReadOnlyList<ReferenciaAnatomiaDto> Referencias);
public sealed record TentativaQuizTemaResponseDto(
    Guid Id,
    Guid QuizId,
    int QuantidadeAcertos,
    int QuantidadeQuestoes,
    double Percentual,
    IReadOnlyList<CorrecaoPerguntaQuizDto> Correcoes,
    DateTime DataCriacao);
public sealed record ContextoAnatomiaDto(Guid ChunkId, Guid FonteId, string Fonte, int Pagina, string? Secao, string? Assunto, string Texto, double Similaridade);
public sealed record ContextoNotaDto(int NotaId, string Titulo, string Conteudo, IReadOnlyList<ContextoNotaConectadaDto> Conexoes);
public sealed record ContextoNotaConectadaDto(int NotaId, string Titulo, string Conteudo, string? Rotulo);
public sealed record FonteIngestaoRequest(string CaminhoPdf, string Titulo, string? Autor, string Versao, string? Assunto, string? Subassunto);
public sealed record FonteIngestaoResultado(Guid FonteId, int QuantidadeChunks, bool Reindexada);
public sealed record ChunkAnatomiaDto(string Texto, int Pagina, string? Secao);

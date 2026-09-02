export type StatusResumoTema = "gerado" | "evidenciaInsuficiente";

export interface ReferenciaResumoTema {
  fonteId: string;
  fonte: string;
  pagina: number;
  secao?: string | null;
  assunto?: string | null;
}

export interface RelacaoResumoTema {
  conexaoId: number;
  notaOrigemId: number;
  tituloOrigem: string;
  notaDestinoId: number;
  tituloDestino: string;
  rotulo?: string | null;
  descricao: string;
}

export interface ResultadoResumoTema {
  status: StatusResumoTema;
  resumo: string;
  pontosChave: string[];
  relacoes: RelacaoResumoTema[];
  referencias: ReferenciaResumoTema[];
}

export interface ResumoTema {
  id: string;
  temaId: number;
  resultado: ResultadoResumoTema;
  modelo: string;
  dataCriacao: string;
}

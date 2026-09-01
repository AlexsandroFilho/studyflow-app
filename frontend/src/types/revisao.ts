export type StatusRevisaoNota = "evidenciaInsuficiente" | "confirmada" | "possuiDivergencias" | "incompleta";

export interface ReferenciaAnatomia {
  fonteId: string;
  fonte: string;
  pagina: number;
  secao?: string | null;
  assunto?: string | null;
}

export interface ApontamentoRevisao {
  tipo: string;
  trecho: string;
  explicacao: string;
  sugestao?: string | null;
}

export interface ResultadoRevisaoNota {
  status: StatusRevisaoNota;
  resumo: string;
  pontosCorretos: string[];
  apontamentos: ApontamentoRevisao[];
  referencias: ReferenciaAnatomia[];
}

export interface RevisaoNota {
  id: string;
  notaId: number;
  resultado: ResultadoRevisaoNota;
  modelo: string;
  dataCriacao: string;
}

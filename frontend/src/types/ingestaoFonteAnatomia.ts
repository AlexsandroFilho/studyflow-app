export type StatusIngestaoFonteAnatomia = "pendente" | "processando" | "concluida" | "falhou";

export interface IngestaoFonteAnatomia {
  id: string;
  titulo: string;
  autor?: string | null;
  versao: string;
  assunto: string;
  subassunto?: string | null;
  status: StatusIngestaoFonteAnatomia;
  mensagemErro?: string | null;
  quantidadeChunks: number;
  fonteAnatomiaId?: string | null;
  dataCriacao: string;
  dataInicio?: string | null;
  dataConclusao?: string | null;
}

export interface CriarIngestaoFonteAnatomiaForm {
  arquivo: File;
  titulo: string;
  autor?: string;
  versao: string;
  assunto: string;
  subassunto?: string;
}

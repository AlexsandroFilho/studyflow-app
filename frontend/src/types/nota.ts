export interface Nota {
  id: number;
  temaId: number;
  titulo: string;
  conteudo: string;
  resumoIa?: string | null;
  dataCriacao?: string | null;
  dataAtualizacao?: string | null;
  nomeTema?: string | null;
}

export interface NotaRequestDto {
  titulo: string;
  conteudo: string;
  temaId: number;
  resumoIa?: string | null;
}

export interface NotaUpdateDto {
  titulo: string;
  conteudo: string;
  temaId: number;
  resumoIa?: string | null;
}

export interface NotaResponseDto {
  id: number;
  titulo: string;
  conteudo: string;
  resumoIa?: string | null;
  dataCriacao: string;
  temaId: number;
  nomeTema?: string | null;
}

export interface CreateNotaDto {
  titulo: string;
  conteudo: string;
  temaId: number;
  resumoIa?: string | null;
}
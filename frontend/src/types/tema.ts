export interface Tema {
  id: number;
  nome: string;
  descricao?: string | null;
  data_criacao?: string | null;
}

export interface TemaRequestDto {
  nome: string;
  descricao?: string | null;
}

export interface TemaResponseDto {
  id: number;
  nome: string;
  descricao?: string | null;
  data_criacao?: string | null;
}

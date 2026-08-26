import { Nota } from "./nota";

export interface Conexao {
  id: number;
  notaOrigemId: number;
  notaDestinoId: number;
  rotulo?: string | null;
  dataCriacao?: string | null;
}

export interface ConexaoRequestDto {
  notaOrigemId: number;
  notaDestinoId: number;
  rotulo?: string | null;
}

export interface NotaConectadaRequestDto {
  notaOrigemId: number;
  titulo: string;
  conteudo: string;
  temaId?: number | null;
  rotulo?: string | null;
}

export interface NotaConectadaResponseDto {
  nota: Nota;
  conexao: Conexao;
}
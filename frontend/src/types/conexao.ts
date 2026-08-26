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

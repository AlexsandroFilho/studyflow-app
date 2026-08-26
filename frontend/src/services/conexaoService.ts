import { api } from './api';
import {
  Conexao,
  ConexaoRequestDto,
} from '../types/conexao';

export const conexaoService = {
  async listar(temaId?: number | null): Promise<Conexao[]> {
    const response = await api.get<Conexao[]>('/conexoes', {
      params: temaId ? { temaId } : undefined,
    });
    return response.data;
  },

  async conectar(dto: ConexaoRequestDto): Promise<Conexao> {
    const response = await api.post<Conexao>('/conexoes', dto);
    return response.data;
  },

  async desconectarPorId(conexaoId: number): Promise<void> {
    await api.delete(`/conexoes/${conexaoId}`);
  },

  async desconectarPorPar(origemId: number, destinoId: number): Promise<void> {
    await api.delete('/conexoes/par', {
      params: { origemId, destinoId },
    });
  },
};
import { api } from './api';
import {
  Conexao,
  ConexaoRequestDto,
  NotaConectadaRequestDto,
  NotaConectadaResponseDto,
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

  async criarNotaConectada(dto: NotaConectadaRequestDto): Promise<NotaConectadaResponseDto> {
    const response = await api.post<NotaConectadaResponseDto>('/conexoes/nota-conectada', dto);
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
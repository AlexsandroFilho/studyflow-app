import { api } from './api';
import { Tema, TemaRequestDto, TemaResponseDto } from '../types/tema';

export const temaService = {
  async listar(): Promise<Tema[]> {
    const response = await api.get<TemaResponseDto[]>('/temas');
    return response.data;
  },

  async buscarPorId(id: number): Promise<Tema> {
    const response = await api.get<TemaResponseDto>(`/temas/${id}`);
    return response.data;
  },

  async criar(dto: TemaRequestDto): Promise<Tema> {
    const response = await api.post<TemaResponseDto>('/temas', dto);
    return response.data;
  },

  async atualizar(id: number, dto: TemaRequestDto): Promise<Tema> {
    const response = await api.put<TemaResponseDto>(`/temas/${id}`, dto);
    return response.data;
  },

  async deletar(id: number): Promise<void> {
    await api.delete(`/temas/${id}`);
  },
};
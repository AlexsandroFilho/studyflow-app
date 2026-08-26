import { api } from './api';
import { Nota, NotaRequestDto, NotaResponseDto, NotaUpdateDto } from '../types/nota';

export const notaService = {
  async listarTodas(): Promise<Nota[]> {
    const response = await api.get<NotaResponseDto[]>('/notas');
    return response.data;
  },

  async listarPorTema(temaId: number): Promise<Nota[]> {
    const response = await api.get<NotaResponseDto[]>(`/notas/tema/${temaId}`);
    return response.data;
  },

  async buscarPorId(id: number): Promise<Nota> {
    const response = await api.get<NotaResponseDto>(`/notas/${id}`);
    return response.data;
  },

  async criar(dto: NotaRequestDto): Promise<Nota> {
    const response = await api.post<NotaResponseDto>('/notas', dto);
    return response.data;
  },

  async atualizar(id: number, dto: NotaUpdateDto): Promise<void> {
    await api.put(`/notas/${id}`, dto);
  },

  async deletar(id: number): Promise<void> {
    await api.delete(`/notas/${id}`);
  },
};
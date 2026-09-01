import { api } from "./api";
import { RevisaoNota } from "../types/revisao";

export const revisaoService = {
  async criar(notaId: number): Promise<RevisaoNota> {
    const response = await api.post<RevisaoNota>(`/notas/${notaId}/revisoes`);
    return response.data;
  },
  async listar(notaId: number): Promise<RevisaoNota[]> {
    const response = await api.get<RevisaoNota[]>(`/notas/${notaId}/revisoes`);
    return response.data;
  },
};

import { api } from "./api";
import { ResumoTema } from "../types/resumoTema";

export const resumoTemaService = {
  async criar(temaId: number): Promise<ResumoTema> {
    const response = await api.post<ResumoTema>(`/temas/${temaId}/resumos`);
    return response.data;
  },

  async listar(temaId: number): Promise<ResumoTema[]> {
    const response = await api.get<ResumoTema[]>(`/temas/${temaId}/resumos`);
    return response.data;
  },
};

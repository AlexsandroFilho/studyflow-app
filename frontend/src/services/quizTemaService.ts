import { api } from "./api";
import { CriarTentativaQuizRequest, QuizTema, TentativaQuiz } from "../types/quizTema";

export const quizTemaService = {
  async criar(temaId: number): Promise<QuizTema> {
    const response = await api.post<QuizTema>(`/temas/${temaId}/quizzes`);
    return response.data;
  },

  async listar(temaId: number): Promise<QuizTema[]> {
    const response = await api.get<QuizTema[]>(`/temas/${temaId}/quizzes`);
    return response.data;
  },

  async criarTentativa(quizId: string, request: CriarTentativaQuizRequest): Promise<TentativaQuiz> {
    const response = await api.post<TentativaQuiz>(`/quizzes/${quizId}/tentativas`, request);
    return response.data;
  },

  async listarTentativas(quizId: string): Promise<TentativaQuiz[]> {
    const response = await api.get<TentativaQuiz[]>(`/quizzes/${quizId}/tentativas`);
    return response.data;
  },
};

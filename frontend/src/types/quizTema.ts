export type StatusQuizTema = "gerado" | "evidenciaInsuficiente";

export interface ReferenciaQuiz {
  fonteId: string;
  fonte: string;
  pagina: number;
  secao?: string | null;
  assunto?: string | null;
}

export interface PerguntaQuiz {
  id: string;
  ordem: number;
  enunciado: string;
  alternativas: string[];
}

export interface QuizTema {
  id: string;
  temaId: number;
  status: StatusQuizTema;
  mensagem: string;
  perguntas: PerguntaQuiz[];
  modelo: string;
  dataCriacao: string;
}

export interface CorrecaoPerguntaQuiz {
  perguntaId: string;
  ordem: number;
  enunciado: string;
  alternativas: string[];
  indiceAlternativaSelecionada: number;
  indiceRespostaCorreta: number;
  acertou: boolean;
  explicacao: string;
  referencias: ReferenciaQuiz[];
}

export interface TentativaQuiz {
  id: string;
  quizId: string;
  quantidadeAcertos: number;
  quantidadeQuestoes: number;
  percentual: number;
  correcoes: CorrecaoPerguntaQuiz[];
  dataCriacao: string;
}

export interface CriarTentativaQuizRequest {
  respostas: Array<{ perguntaId: string; indiceAlternativa: number }>;
}

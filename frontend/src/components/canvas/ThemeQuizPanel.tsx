import React, { useEffect, useState } from "react";
import { ArrowLeft, ArrowRight, BookOpenCheck, CheckCircle2, CircleHelp, Loader2, Plus, RotateCcw, X, XCircle } from "lucide-react";
import { QuizTema, TentativaQuiz } from "../../types/quizTema";

interface ThemeQuizPanelProps {
  quiz: QuizTema | null;
  quizzes: QuizTema[];
  tentativas: TentativaQuiz[];
  enviando: boolean;
  onSelectQuiz: (quiz: QuizTema) => void;
  onSelectTentativa: (tentativa: TentativaQuiz | null) => void;
  onSubmit: (respostas: Record<string, number>) => Promise<void>;
  onGenerate: () => Promise<void>;
  gerando: boolean;
  tentativaSelecionada: TentativaQuiz | null;
  onClose: () => void;
}

export const ThemeQuizPanel: React.FC<ThemeQuizPanelProps> = ({
  quiz, quizzes, tentativas, enviando, onSelectQuiz, onSelectTentativa, onSubmit, onGenerate, gerando, tentativaSelecionada, onClose,
}) => {
  const [indiceAtual, setIndiceAtual] = useState(0);
  const [respostas, setRespostas] = useState<Record<string, number>>({});

  useEffect(() => {
    setIndiceAtual(0);
    setRespostas({});
    onSelectTentativa(null);
  }, [quiz?.id]);

  const pergunta = quiz?.perguntas[indiceAtual];
  const todasRespondidas = quiz?.perguntas.every(item => respostas[item.id] !== undefined) ?? false;

  return (
    <aside className="w-[420px] shrink-0 border-l border-slate-200 bg-white overflow-y-auto p-5 space-y-5 select-text">
      <div className="flex items-start justify-between gap-3">
        <div><h2 className="font-bold text-slate-800">Quiz do tema</h2><p className="text-xs text-slate-500 mt-1">Cinco questões fundamentadas no acervo oficial.</p></div>
        <button onClick={onClose} className="p-1.5 text-slate-500 hover:bg-slate-100 rounded-lg" title="Fechar quiz"><X className="w-4 h-4" /></button>
      </div>

      <button onClick={onGenerate} disabled={gerando} className="flex w-full items-center justify-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-xs font-semibold text-white disabled:opacity-60">
        {gerando ? <Loader2 className="w-4 h-4 animate-spin" /> : <Plus className="w-4 h-4" />}{gerando ? "Gerando quiz..." : "Gerar novo quiz"}
      </button>

      {quizzes.length > 1 && <section>
        <label className="text-xs font-bold uppercase tracking-wide text-slate-500">Quiz gerado</label>
        <select value={quiz?.id ?? ""} onChange={event => { const item = quizzes.find(x => x.id === event.target.value); if (item) onSelectQuiz(item); }} className="mt-2 w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-xs">
          {quizzes.map(item => <option key={item.id} value={item.id}>{new Date(item.dataCriacao).toLocaleString("pt-BR")}</option>)}
        </select>
      </section>}

      {!quiz && !gerando && <p className="text-sm text-slate-600">Nenhum quiz foi gerado para este tema.</p>}
      {quiz?.status === "evidenciaInsuficiente" && <div className="rounded-lg border border-slate-200 bg-slate-50 p-3 text-sm text-slate-700"><CircleHelp className="inline w-4 h-4 mr-2" />{quiz.mensagem}</div>}

      {quiz?.status === "gerado" && tentativaSelecionada && <ResultadoTentativa tentativa={tentativaSelecionada} onRefazer={() => onSelectTentativa(null)} />}

      {quiz?.status === "gerado" && !tentativaSelecionada && pergunta && <>
        <div className="flex items-center justify-between text-xs text-slate-500"><span>Questão {indiceAtual + 1} de {quiz.perguntas.length}</span><span>{Object.keys(respostas).length}/{quiz.perguntas.length} respondidas</span></div>
        <div className="h-1.5 overflow-hidden rounded-full bg-slate-100"><div className="h-full bg-blue-600 transition-all" style={{ width: `${((indiceAtual + 1) / quiz.perguntas.length) * 100}%` }} /></div>
        <section>
          <h3 className="text-sm font-semibold leading-relaxed text-slate-800">{pergunta.enunciado}</h3>
          <div className="mt-4 space-y-2">
            {pergunta.alternativas.map((alternativa, indice) => <button key={indice} onClick={() => setRespostas(anteriores => ({ ...anteriores, [pergunta.id]: indice }))} className={`w-full rounded-lg border p-3 text-left text-sm transition-colors ${respostas[pergunta.id] === indice ? "border-blue-500 bg-blue-50 text-blue-900" : "border-slate-200 hover:border-blue-300 hover:bg-slate-50"}`}><span className="mr-2 font-semibold">{String.fromCharCode(65 + indice)}.</span>{alternativa}</button>)}
          </div>
        </section>
        <div className="flex items-center justify-between gap-2">
          <button onClick={() => setIndiceAtual(x => Math.max(0, x - 1))} disabled={indiceAtual === 0} className="flex items-center gap-1 rounded-lg border border-slate-200 px-3 py-2 text-xs font-semibold disabled:opacity-40"><ArrowLeft className="w-4 h-4" />Anterior</button>
          {indiceAtual < quiz.perguntas.length - 1
            ? <button onClick={() => setIndiceAtual(x => Math.min(quiz.perguntas.length - 1, x + 1))} className="flex items-center gap-1 rounded-lg bg-blue-600 px-3 py-2 text-xs font-semibold text-white">Próxima<ArrowRight className="w-4 h-4" /></button>
            : <button onClick={() => onSubmit(respostas)} disabled={!todasRespondidas || enviando} className="rounded-lg bg-blue-600 px-3 py-2 text-xs font-semibold text-white disabled:opacity-50">{enviando ? "Corrigindo..." : "Finalizar quiz"}</button>}
        </div>
      </>}

      {quiz?.status === "gerado" && tentativas.length > 0 && <section className="border-t border-slate-200 pt-4">
        <label className="text-xs font-bold uppercase tracking-wide text-slate-500">Tentativas anteriores</label>
        <div className="mt-2 space-y-2">{tentativas.map(item => <button key={item.id} onClick={() => onSelectTentativa(item)} className="flex w-full items-center justify-between rounded-lg border border-slate-200 px-3 py-2 text-xs hover:bg-slate-50"><span>{new Date(item.dataCriacao).toLocaleString("pt-BR")}</span><strong>{item.quantidadeAcertos}/{item.quantidadeQuestoes}</strong></button>)}</div>
      </section>}
    </aside>
  );
};

const ResultadoTentativa: React.FC<{ tentativa: TentativaQuiz; onRefazer: () => void }> = ({ tentativa, onRefazer }) => <div className="space-y-5">
  <div className="rounded-xl border border-blue-200 bg-blue-50 p-4 text-center"><p className="text-3xl font-bold text-blue-700">{tentativa.percentual}%</p><p className="mt-1 text-sm text-blue-900">{tentativa.quantidadeAcertos} de {tentativa.quantidadeQuestoes} questões corretas</p></div>
  <button onClick={onRefazer} className="flex items-center gap-2 rounded-lg border border-slate-200 px-3 py-2 text-xs font-semibold"><RotateCcw className="w-4 h-4" />Responder novamente</button>
  {tentativa.correcoes.map(item => <section key={item.perguntaId} className={`rounded-lg border p-3 ${item.acertou ? "border-emerald-200" : "border-red-200"}`}>
    <p className="flex gap-2 text-sm font-semibold text-slate-800">{item.acertou ? <CheckCircle2 className="w-4 h-4 text-emerald-600 shrink-0" /> : <XCircle className="w-4 h-4 text-red-600 shrink-0" />}{item.ordem}. {item.enunciado}</p>
    <p className="mt-2 text-xs text-slate-600">Sua resposta: {item.alternativas[item.indiceAlternativaSelecionada]}</p>
    {!item.acertou && <p className="mt-1 text-xs text-emerald-700">Correta: {item.alternativas[item.indiceRespostaCorreta]}</p>}
    <p className="mt-2 text-sm leading-relaxed text-slate-700">{item.explicacao}</p>
    {item.referencias.map((ref, index) => <p key={`${ref.fonteId}-${index}`} className="mt-2 flex gap-1 text-xs text-slate-500"><BookOpenCheck className="w-3.5 h-3.5 shrink-0 text-blue-600" />{ref.fonte} · página {ref.pagina}{ref.secao ? ` · ${ref.secao}` : ""}</p>)}
  </section>)}
</div>;

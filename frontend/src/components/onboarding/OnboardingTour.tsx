import React, { useEffect, useMemo, useState } from "react";
import { ArrowLeft, ArrowRight, Check, Sparkles, X } from "lucide-react";

interface TourStep {
  target?: string;
  title: string;
  description: string;
}

interface OnboardingTourProps {
  isOpen: boolean;
  onClose: (dontShowAgain: boolean) => Promise<void>;
}

const STEPS: TourStep[] = [
  {
    title: "Bem-vindo ao StudyFlow",
    description: "Em poucos passos, você vai conhecer o fluxo para organizar seus estudos e usar os recursos de IA.",
  },
  {
    target: "novo-tema",
    title: "Comece criando um tema",
    description: "Os temas agrupam as notas de um mesmo assunto e definem o contexto usado nos resumos e quizzes.",
  },
  {
    target: "nova-nota",
    title: "Registre o que está estudando",
    description: "Crie notas dentro dos temas. No Canvas, uma nota nova aparece no centro da área que você está visualizando.",
  },
  {
    target: "canvas",
    title: "Organize seu mapa mental",
    description: "Arraste as notas para organizá-las, mova o fundo para navegar e use os pontos dos cartões para criar conexões.",
  },
  {
    target: "temas",
    title: "Selecione um tema",
    description: "Ao escolher um tema, o Canvas mostra suas notas e libera as funcionalidades de IA daquele conteúdo.",
  },
  {
    target: "ia-tema",
    title: "Estude com apoio da IA",
    description: "Gere um resumo fundamentado no acervo oficial ou responda a um quiz criado a partir das notas do tema.",
  },
];

const HIGHLIGHT_PADDING = 8;
const CARD_WIDTH = 360;

export const OnboardingTour: React.FC<OnboardingTourProps> = ({ isOpen, onClose }) => {
  const [stepIndex, setStepIndex] = useState(0);
  const [dontShowAgain, setDontShowAgain] = useState(false);
  const [targetRect, setTargetRect] = useState<DOMRect | null>(null);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const step = STEPS[stepIndex];

  useEffect(() => {
    if (!isOpen) return;
    setStepIndex(0);
    setDontShowAgain(false);
    setError(null);
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) return;

    const updateTarget = () => {
      const element = step.target
        ? document.querySelector<HTMLElement>(`[data-tour="${step.target}"]`)
        : null;
      setTargetRect(element?.getBoundingClientRect() ?? null);
    };

    updateTarget();
    window.addEventListener("resize", updateTarget);
    return () => window.removeEventListener("resize", updateTarget);
  }, [isOpen, step]);

  const cardPosition = useMemo(() => {
    if (!targetRect) return { left: "50%", top: "50%", transform: "translate(-50%, -50%)" };

    const left = Math.min(
      Math.max(16, targetRect.left + targetRect.width / 2 - CARD_WIDTH / 2),
      window.innerWidth - CARD_WIDTH - 16
    );
    const hasSpaceBelow = targetRect.bottom + 250 < window.innerHeight;
    return {
      left,
      top: hasSpaceBelow ? targetRect.bottom + 18 : Math.max(16, targetRect.top - 238),
      transform: "none",
    };
  }, [targetRect]);

  if (!isOpen) return null;

  const close = async () => {
    setSaving(true);
    setError(null);
    try {
      await onClose(dontShowAgain);
    } catch {
      setError("Não foi possível salvar sua preferência. Tente novamente.");
    } finally {
      setSaving(false);
    }
  };

  const highlight = targetRect
    ? {
        left: targetRect.left - HIGHLIGHT_PADDING,
        top: targetRect.top - HIGHLIGHT_PADDING,
        width: targetRect.width + HIGHLIGHT_PADDING * 2,
        height: targetRect.height + HIGHLIGHT_PADDING * 2,
      }
    : null;

  return (
    <div className="fixed inset-0 z-[100]" role="dialog" aria-modal="true" aria-label="Guia de uso do StudyFlow">
      {highlight ? (
        <>
          <div className="fixed left-0 right-0 top-0 bg-slate-950/55 backdrop-blur-[2px]" style={{ height: Math.max(0, highlight.top) }} />
          <div className="fixed bottom-0 left-0 right-0 bg-slate-950/55 backdrop-blur-[2px]" style={{ top: highlight.top + highlight.height }} />
          <div className="fixed left-0 bg-slate-950/55 backdrop-blur-[2px]" style={{ top: highlight.top, width: Math.max(0, highlight.left), height: highlight.height }} />
          <div className="fixed right-0 bg-slate-950/55 backdrop-blur-[2px]" style={{ top: highlight.top, left: highlight.left + highlight.width, height: highlight.height }} />
          <div
            className="pointer-events-none fixed rounded-xl border-2 border-blue-400 shadow-[0_0_0_4px_rgba(59,130,246,0.25),0_12px_40px_rgba(15,23,42,0.35)]"
            style={highlight}
          />
        </>
      ) : (
        <div className="absolute inset-0 bg-slate-950/55 backdrop-blur-[2px]" />
      )}

      <section
        className="fixed w-[360px] max-w-[calc(100vw-32px)] rounded-2xl border border-slate-200 bg-white p-5 shadow-2xl"
        style={cardPosition}
      >
        <div className="mb-4 flex items-start justify-between gap-3">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-blue-600 text-white">
            <Sparkles className="h-4 w-4" />
          </div>
          <button onClick={close} disabled={saving} className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-700" title="Fechar guia">
            <X className="h-4 w-4" />
          </button>
        </div>

        <p className="mb-1 text-[10px] font-bold uppercase tracking-wider text-blue-600">
          Passo {stepIndex + 1} de {STEPS.length}
        </p>
        <h2 className="text-lg font-bold text-slate-900">{step.title}</h2>
        <p className="mt-2 text-sm leading-6 text-slate-600">{step.description}</p>

        <label className="mt-5 flex cursor-pointer items-center gap-2.5 rounded-lg bg-slate-50 px-3 py-2.5 text-xs text-slate-600">
          <input
            type="checkbox"
            checked={dontShowAgain}
            onChange={(event) => setDontShowAgain(event.target.checked)}
            className="h-4 w-4 rounded border-slate-300 text-blue-600"
          />
          Não mostrar este guia novamente
        </label>

        {error && <p className="mt-2 text-xs text-red-600">{error}</p>}

        <div className="mt-5 flex items-center justify-between">
          <button
            onClick={() => setStepIndex((current) => Math.max(0, current - 1))}
            disabled={stepIndex === 0 || saving}
            className="flex items-center gap-1.5 rounded-lg px-3 py-2 text-xs font-semibold text-slate-600 hover:bg-slate-100 disabled:invisible"
          >
            <ArrowLeft className="h-3.5 w-3.5" /> Anterior
          </button>

          {stepIndex === STEPS.length - 1 ? (
            <button onClick={close} disabled={saving} className="flex items-center gap-1.5 rounded-lg bg-blue-600 px-4 py-2 text-xs font-semibold text-white hover:bg-blue-700 disabled:opacity-60">
              <Check className="h-3.5 w-3.5" /> {saving ? "Salvando..." : "Concluir"}
            </button>
          ) : (
            <button
              onClick={() => setStepIndex((current) => current + 1)}
              disabled={saving}
              className="flex items-center gap-1.5 rounded-lg bg-blue-600 px-4 py-2 text-xs font-semibold text-white hover:bg-blue-700"
            >
              Próximo <ArrowRight className="h-3.5 w-3.5" />
            </button>
          )}
        </div>
      </section>
    </div>
  );
};

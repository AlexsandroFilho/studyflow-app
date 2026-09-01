import React from "react";
import { AlertTriangle, BookOpenCheck, CheckCircle2, CircleHelp, X } from "lucide-react";
import { RevisaoNota } from "../../types/revisao";

interface ReviewPanelProps {
  revisao: RevisaoNota;
  onClose: () => void;
}

const statusConfig = {
  confirmada: { label: "Conteúdo confirmado", icon: CheckCircle2, color: "text-emerald-700 bg-emerald-50 border-emerald-200" },
  possuiDivergencias: { label: "Pontos para revisar", icon: AlertTriangle, color: "text-amber-700 bg-amber-50 border-amber-200" },
  incompleta: { label: "Conteúdo incompleto", icon: CircleHelp, color: "text-blue-700 bg-blue-50 border-blue-200" },
  evidenciaInsuficiente: { label: "Evidência insuficiente", icon: CircleHelp, color: "text-slate-700 bg-slate-50 border-slate-200" },
};

export const ReviewPanel: React.FC<ReviewPanelProps> = ({ revisao, onClose }) => {
  const config = statusConfig[revisao.resultado.status] ?? statusConfig.evidenciaInsuficiente;
  const Icon = config.icon;
  return (
    <aside className="w-[360px] shrink-0 border-l border-slate-200 bg-white overflow-y-auto p-5 space-y-5 select-text">
      <div className="flex items-start justify-between gap-3">
        <div>
          <h2 className="font-bold text-slate-800">Revisão de Anatomia</h2>
          <p className="text-xs text-slate-500 mt-1">Sugestões baseadas no acervo oficial.</p>
        </div>
        <button onClick={onClose} className="p-1.5 text-slate-500 hover:bg-slate-100 rounded-lg" title="Fechar revisão"><X className="w-4 h-4" /></button>
      </div>

      <div className={`flex items-center gap-2 rounded-lg border px-3 py-2 text-xs font-semibold ${config.color}`}>
        <Icon className="w-4 h-4" /> {config.label}
      </div>

      <section>
        <h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Resumo</h3>
        <p className="text-sm text-slate-700 leading-relaxed">{revisao.resultado.resumo}</p>
      </section>

      {revisao.resultado.pontosCorretos.length > 0 && <section>
        <h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Pontos corretos</h3>
        <ul className="space-y-2 text-sm text-slate-700">{revisao.resultado.pontosCorretos.map((ponto, index) => <li key={index} className="flex gap-2"><CheckCircle2 className="w-4 h-4 text-emerald-600 shrink-0 mt-0.5" />{ponto}</li>)}</ul>
      </section>}

      {revisao.resultado.apontamentos.length > 0 && <section>
        <h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Sugestões</h3>
        <div className="space-y-3">{revisao.resultado.apontamentos.map((item, index) => <div key={index} className="rounded-lg border border-slate-200 p-3 text-sm"><p className="font-semibold text-slate-800">{item.trecho}</p><p className="text-slate-600 mt-1 leading-relaxed">{item.explicacao}</p>{item.sugestao && <p className="text-blue-800 bg-blue-50 mt-2 p-2 rounded">Sugestão: {item.sugestao}</p>}</div>)}</div>
      </section>}

      {revisao.resultado.referencias.length > 0 && <section>
        <h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Fontes consultadas</h3>
        <ul className="space-y-2">{revisao.resultado.referencias.map((referencia, index) => <li key={`${referencia.fonteId}-${index}`} className="flex gap-2 text-xs text-slate-600"><BookOpenCheck className="w-4 h-4 text-blue-600 shrink-0" /><span><strong>{referencia.fonte}</strong><br />Página {referencia.pagina}{referencia.secao ? ` · ${referencia.secao}` : ""}</span></li>)}</ul>
      </section>}
    </aside>
  );
};

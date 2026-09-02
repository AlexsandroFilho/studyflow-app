import React from "react";
import { BookOpenCheck, CheckCircle2, CircleHelp, GitFork, ListChecks, X } from "lucide-react";
import { ResumoTema } from "../../types/resumoTema";

interface ThemeSummaryPanelProps {
  resumo: ResumoTema | null;
  historico: ResumoTema[];
  onSelect: (resumo: ResumoTema) => void;
  onClose: () => void;
}

export const ThemeSummaryPanel: React.FC<ThemeSummaryPanelProps> = ({ resumo, historico, onSelect, onClose }) => {
  const possuiResumo = resumo?.resultado.status === "gerado";

  return (
    <aside className="w-[360px] shrink-0 border-l border-slate-200 bg-white overflow-y-auto p-5 space-y-5 select-text">
      <div className="flex items-start justify-between gap-3">
        <div><h2 className="font-bold text-slate-800">Resumo do tema</h2><p className="text-xs text-slate-500 mt-1">Sintetizado com base no acervo oficial.</p></div>
        <button onClick={onClose} className="p-1.5 text-slate-500 hover:bg-slate-100 rounded-lg" title="Fechar resumo"><X className="w-4 h-4" /></button>
      </div>

      {historico.length > 1 && <section>
        <label className="text-xs font-bold uppercase tracking-wide text-slate-500">Histórico</label>
        <select value={resumo?.id ?? ""} onChange={(event) => { const item = historico.find(x => x.id === event.target.value); if (item) onSelect(item); }} className="mt-2 w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-xs text-slate-800 focus:outline-none focus:border-blue-500">
          {historico.map(item => <option key={item.id} value={item.id}>{new Date(item.dataCriacao).toLocaleString("pt-BR")}</option>)}
        </select>
      </section>}

      {!resumo && <p className="text-sm text-slate-600">Nenhum resumo foi gerado para este tema.</p>}
      {resumo && <>
        <div className={`flex items-center gap-2 rounded-lg border px-3 py-2 text-xs font-semibold ${possuiResumo ? "text-emerald-700 bg-emerald-50 border-emerald-200" : "text-slate-700 bg-slate-50 border-slate-200"}`}>
          {possuiResumo ? <CheckCircle2 className="w-4 h-4" /> : <CircleHelp className="w-4 h-4" />}{possuiResumo ? "Resumo fundamentado" : "Evidência insuficiente"}
        </div>
        <section><h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Resumo</h3><p className="text-sm text-slate-700 leading-relaxed">{resumo.resultado.resumo}</p></section>
        {resumo.resultado.pontosChave.length > 0 && <section><h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Pontos-chave</h3><ul className="space-y-2 text-sm text-slate-700">{resumo.resultado.pontosChave.map((ponto, index) => <li key={index} className="flex gap-2"><ListChecks className="w-4 h-4 text-blue-600 shrink-0 mt-0.5" />{ponto}</li>)}</ul></section>}
        {resumo.resultado.relacoes.length > 0 && <section><h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Relações no mapa</h3><div className="space-y-3">{resumo.resultado.relacoes.map(relacao => <div key={relacao.conexaoId} className="rounded-lg border border-slate-200 p-3 text-sm"><p className="flex items-center gap-2 font-semibold text-slate-800"><GitFork className="w-4 h-4 text-blue-600 shrink-0" />{relacao.tituloOrigem} → {relacao.tituloDestino}</p>{relacao.rotulo && <p className="mt-1 text-xs text-slate-500">{relacao.rotulo}</p>}<p className="mt-2 text-slate-600 leading-relaxed">{relacao.descricao}</p></div>)}</div></section>}
        {resumo.resultado.referencias.length > 0 && <section><h3 className="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Fontes consultadas</h3><ul className="space-y-2">{resumo.resultado.referencias.map((referencia, index) => <li key={`${referencia.fonteId}-${index}`} className="flex gap-2 text-xs text-slate-600"><BookOpenCheck className="w-4 h-4 text-blue-600 shrink-0" /><span><strong>{referencia.fonte}</strong><br />Página {referencia.pagina}{referencia.secao ? ` · ${referencia.secao}` : ""}</span></li>)}</ul></section>}
      </>}
    </aside>
  );
};

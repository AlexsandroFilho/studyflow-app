import React from "react";
import { Folder, Search, Plus, Trash2, Edit2, Network, PanelLeftClose, PanelLeftOpen } from "lucide-react";
import { Tema } from "../../types/tema";
import { Nota } from "../../types/nota";
import { getThemeColor } from "../../hooks/useCanvas";

interface SidebarProps {
  temas: Tema[];
  notas: Nota[];
  selectedTemaId: number | null;
  onSelectTema: (temaId: number | null) => void;
  onOpenCreateTema: () => void;
  onEditTema: (tema: Tema) => void;
  onDeleteTema: (tema: Tema) => void;
  searchTerm: string;
  onSearchChange: (term: string) => void;
  isCollapsed: boolean;
  onToggle: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({
  temas,
  notas,
  selectedTemaId,
  onSelectTema,
  onOpenCreateTema,
  onEditTema,
  onDeleteTema,
  searchTerm,
  onSearchChange,
  isCollapsed,
  onToggle,
}) => {
  const getNotaCount = (temaId: number) =>
    notas.filter((n) => n.temaId === temaId).length;

  if (isCollapsed) {
    return (
      <aside className="w-11 bg-white border-r border-slate-200 h-full z-20 shrink-0 shadow-sm flex flex-col items-center pt-3">
        <button onClick={onToggle} title="Mostrar temas" className="rounded-lg p-2 text-slate-500 hover:bg-slate-100 hover:text-slate-800 transition-colors">
          <PanelLeftOpen className="h-4 w-4" />
        </button>
      </aside>
    );
  }

  return (
    <aside data-tour="temas" className="w-64 bg-white border-r border-slate-200 flex flex-col h-full z-20 select-none shrink-0 shadow-sm">
      <div className="p-3 border-b border-slate-200">
        <div className="flex items-center gap-2">
          <div className="relative flex-1">
          <Search className="w-3.5 h-3.5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            placeholder="Buscar notas e conexões..."
            className="w-full bg-slate-50 border border-slate-200 rounded-lg pl-8 pr-3 py-1.5 text-xs text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-blue-500 transition-colors"
          />
          </div>
          <button onClick={onToggle} title="Ocultar temas" className="rounded-md p-1.5 text-slate-500 hover:bg-slate-100 hover:text-slate-800 transition-colors">
            <PanelLeftClose className="h-4 w-4" />
          </button>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto p-2 space-y-1">
        <div className="flex items-center justify-between px-2 pt-3 pb-2">
          <span className="text-[10px] font-semibold text-slate-500 uppercase tracking-wider flex items-center gap-1.5">
            <Folder className="w-3 h-3 text-slate-500" />
            Temas de Estudo
          </span>
          <button
            onClick={onOpenCreateTema}
            className="p-0.5 rounded text-slate-500 hover:text-slate-700 hover:bg-slate-100 transition-colors"
            title="Novo Tema"
          >
            <Plus className="w-3.5 h-3.5" />
          </button>
        </div>

        <button
          onClick={() => onSelectTema(null)}
          className={`w-full flex items-center justify-between px-2.5 py-2 rounded-lg text-xs font-medium transition-colors ${
            selectedTemaId === null
              ? "bg-blue-50 text-blue-700 border border-blue-200 font-semibold shadow-sm"
              : "text-slate-600 hover:bg-slate-50 hover:text-slate-800"
          }`}
        >
          <div className="flex items-center gap-2">
            <span
              className={`w-1.5 h-1.5 rounded-full ${
                selectedTemaId === null ? "bg-blue-600" : "bg-slate-400"
              }`}
            />
            Todos os temas
          </div>
          <span className="text-[10px] px-1.5 py-0.5 rounded bg-slate-100 text-slate-600 font-mono border border-slate-200">
            {notas.length}
          </span>
        </button>

        {temas.map((tema) => {
          const isSelected = selectedTemaId === tema.id;
          const color = getThemeColor(tema.id);
          const count = getNotaCount(tema.id);

          return (
            <div
              key={tema.id}
              onClick={() => onSelectTema(tema.id)}
              className={`group flex items-center justify-between px-2.5 py-2 rounded-lg text-xs cursor-pointer transition-colors ${
                isSelected
                  ? "bg-blue-50 text-blue-700 border border-blue-200 font-semibold shadow-sm"
                  : "text-slate-600 hover:bg-slate-50 hover:text-slate-800"
              }`}
            >
              <div className="flex items-center gap-2 min-w-0 flex-1">
                <span
                  className="w-2 h-2 rounded-full shrink-0 shadow-sm"
                  style={{ backgroundColor: color }}
                />
                <span className="truncate">{tema.nome}</span>
              </div>

              <div className="flex items-center gap-1 shrink-0">
                <span className="text-[10px] px-1.5 py-0.5 rounded bg-slate-100 text-slate-600 font-mono border border-slate-200 group-hover:hidden">
                  {count}
                </span>

                <div className="hidden group-hover:flex items-center gap-0.5">
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onEditTema(tema);
                    }}
                    className="p-1 rounded text-slate-500 hover:text-slate-700 hover:bg-slate-100 transition-colors"
                    title="Editar"
                  >
                    <Edit2 className="w-3 h-3" />
                  </button>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onDeleteTema(tema);
                    }}
                    className="p-1 rounded text-slate-500 hover:text-red-500 hover:bg-red-50 transition-colors"
                    title="Excluir"
                  >
                    <Trash2 className="w-3 h-3" />
                  </button>
                </div>
              </div>
            </div>
          );
        })}

        {temas.length === 0 && (
          <div className="px-2 py-6 text-center">
            <p className="text-[11px] text-slate-500">Nenhum tema criado ainda.</p>
            <button
              onClick={onOpenCreateTema}
              className="mt-2 text-[11px] text-blue-600 hover:underline font-semibold"
            >
              Criar primeiro tema →
            </button>
          </div>
        )}
      </div>

      <div className="p-3 border-t border-slate-200 bg-white">
        <div className="flex items-center justify-center gap-1.5 text-[11px] text-slate-500">
          <Network className="w-3.5 h-3.5 text-blue-500" />
          <span>Grafo de Conhecimento Ativo</span>
        </div>
      </div>
    </aside>
  );
};

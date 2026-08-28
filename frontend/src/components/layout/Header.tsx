import React from "react";
import { LayoutGrid, BookOpen, Plus, Network, LogOut } from "lucide-react";

export type ViewMode = "canvas" | "editor";

interface HeaderProps {
  viewMode: ViewMode;
  setViewMode: (mode: ViewMode) => void;
  onOpenCreateNota: () => void;
  onOpenCreateTema: () => void;
  totalNotas: number;
  totalTemas: number;
  totalConexoes: number;
  onLogout: () => void;
}

export const Header: React.FC<HeaderProps> = ({
  viewMode,
  setViewMode,
  onOpenCreateNota,
  onOpenCreateTema,
  totalNotas,
  totalTemas,
  totalConexoes,
  onLogout,
}) => {
  return (
    <header className="h-14 border-b border-slate-200 bg-white px-5 flex items-center justify-between z-30 select-none shrink-0 shadow-sm">
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center shadow-sm border border-blue-500/30">
          <Network className="w-4 h-4 text-white" />
        </div>
        <div>
          <span className="font-bold text-sm tracking-tight text-slate-800">
            Study<span className="text-blue-600">Flow</span>
          </span>
          <p className="text-[10px] text-slate-500 leading-none mt-0.5 font-medium">
            {totalNotas} notas · {totalTemas} temas · {totalConexoes} conexões
          </p>
        </div>
      </div>

      <div className="flex items-center bg-slate-100 border border-slate-200 p-0.5 rounded-lg shadow-sm">
        <button
          onClick={() => setViewMode("canvas")}
          className={`flex items-center gap-2 px-3.5 py-1.5 rounded-md text-xs font-medium transition-all ${
            viewMode === "canvas"
              ? "bg-white text-slate-800 shadow-sm font-semibold border border-slate-200"
              : "text-slate-500 hover:text-slate-800 hover:bg-white"
          }`}
        >
          <LayoutGrid className="w-3.5 h-3.5" />
          Quadro Canvas
        </button>

        <button
          onClick={() => setViewMode("editor")}
          className={`flex items-center gap-2 px-3.5 py-1.5 rounded-md text-xs font-medium transition-all ${
            viewMode === "editor"
              ? "bg-white text-slate-800 shadow-sm font-semibold border border-slate-200"
              : "text-slate-500 hover:text-slate-800 hover:bg-white"
          }`}
        >
          <BookOpen className="w-3.5 h-3.5" />
          Modo Foco
        </button>
      </div>

      <div className="flex items-center gap-2">
        <button
          onClick={onOpenCreateTema}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-slate-700 bg-white border border-slate-200 hover:bg-slate-50 transition-colors"
        >
          <Plus className="w-3.5 h-3.5 text-slate-500" />
          Novo Tema
        </button>

        <button
          onClick={onOpenCreateNota}
          className="flex items-center gap-1.5 px-3.5 py-1.5 rounded-lg text-xs font-semibold bg-blue-600 text-white hover:bg-blue-700 border border-blue-600 shadow-sm transition-all"
        >
          <Plus className="w-3.5 h-3.5 font-bold" />
          Nova Nota
        </button>
        <button
          onClick={onLogout}
          title="Sair"
          className="rounded-lg border border-slate-200 p-2 text-slate-500 transition-colors hover:bg-red-50 hover:text-red-600"
        >
          <LogOut className="h-4 w-4" />
        </button>
      </div>
    </header>
  );
};

import React from "react";
import { LayoutGrid, BookOpen, Plus, Network } from "lucide-react";

export type ViewMode = "canvas" | "editor";

interface HeaderProps {
  viewMode: ViewMode;
  setViewMode: (mode: ViewMode) => void;
  onOpenCreateNota: () => void;
  onOpenCreateTema: () => void;
  totalNotas: number;
  totalTemas: number;
  totalConexoes: number;
}

export const Header: React.FC<HeaderProps> = ({
  viewMode,
  setViewMode,
  onOpenCreateNota,
  onOpenCreateTema,
  totalNotas,
  totalTemas,
  totalConexoes,
}) => {
  return (
    <header className="h-14 border-b border-[#526D82]/50 bg-[#161B22] px-5 flex items-center justify-between z-30 select-none shrink-0 shadow-md">
      {/* Brand */}
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 rounded-lg bg-[#526D82] flex items-center justify-center shadow-md border border-[#9DB2BF]/30">
          <Network className="w-4 h-4 text-[#DDE6ED]" />
        </div>
        <div>
          <span className="font-bold text-sm tracking-tight text-[#DDE6ED]">
            Study<span className="text-[#9DB2BF]">Flow</span>
          </span>
          <p className="text-[10px] text-[#9DB2BF] leading-none mt-0.5 font-medium">
            {totalNotas} notas · {totalTemas} temas · {totalConexoes} conexões
          </p>
        </div>
      </div>

      {/* Switcher de Visão */}
      <div className="flex items-center bg-[#27374D] border border-[#526D82]/60 p-0.5 rounded-lg shadow-inner">
        <button
          onClick={() => setViewMode("canvas")}
          className={`flex items-center gap-2 px-3.5 py-1.5 rounded-md text-xs font-medium transition-all ${
            viewMode === "canvas"
              ? "bg-[#526D82] text-[#DDE6ED] shadow-sm font-semibold border border-[#9DB2BF]/40"
              : "text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#31435D]"
          }`}
        >
          <LayoutGrid className="w-3.5 h-3.5" />
          Quadro Canvas
        </button>

        <button
          onClick={() => setViewMode("editor")}
          className={`flex items-center gap-2 px-3.5 py-1.5 rounded-md text-xs font-medium transition-all ${
            viewMode === "editor"
              ? "bg-[#526D82] text-[#DDE6ED] shadow-sm font-semibold border border-[#9DB2BF]/40"
              : "text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#31435D]"
          }`}
        >
          <BookOpen className="w-3.5 h-3.5" />
          Modo Foco
        </button>
      </div>

      {/* Ações Globais */}
      <div className="flex items-center gap-2">
        <button
          onClick={onOpenCreateTema}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-[#DDE6ED] bg-[#27374D] border border-[#526D82] hover:bg-[#31435D] transition-colors"
        >
          <Plus className="w-3.5 h-3.5 text-[#9DB2BF]" />
          Novo Tema
        </button>

        <button
          onClick={onOpenCreateNota}
          className="flex items-center gap-1.5 px-3.5 py-1.5 rounded-lg text-xs font-semibold bg-[#526D82] text-[#DDE6ED] hover:bg-[#9DB2BF] hover:text-[#161B22] border border-[#526D82] shadow-md transition-all"
        >
          <Plus className="w-3.5 h-3.5 font-bold" />
          Nova Nota
        </button>
      </div>
    </header>
  );
};

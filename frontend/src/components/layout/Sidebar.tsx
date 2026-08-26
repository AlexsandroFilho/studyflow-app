import React from "react";
import { Folder, Search, Plus, Trash2, Edit2, Network } from "lucide-react";
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
}) => {
  const getNotaCount = (temaId: number) =>
    notas.filter((n) => n.temaId === temaId).length;

  return (
    <aside className="w-64 bg-[#161B22] border-r border-[#526D82]/50 flex flex-col h-full z-20 select-none shrink-0 shadow-md">
      {/* Campo de Busca */}
      <div className="p-3 border-b border-[#526D82]/40">
        <div className="relative">
          <Search className="w-3.5 h-3.5 text-[#9DB2BF] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            placeholder="Buscar notas e conexões..."
            className="w-full bg-[#27374D] border border-[#526D82] rounded-lg pl-8 pr-3 py-1.5 text-xs text-[#DDE6ED] placeholder-[#9DB2BF] focus:outline-none focus:border-[#9DB2BF] transition-colors"
          />
        </div>
      </div>

      {/* Lista de Temas */}
      <div className="flex-1 overflow-y-auto p-2 space-y-1">
        {/* Cabeçalho da seção */}
        <div className="flex items-center justify-between px-2 pt-3 pb-2">
          <span className="text-[10px] font-semibold text-[#9DB2BF] uppercase tracking-wider flex items-center gap-1.5">
            <Folder className="w-3 h-3 text-[#9DB2BF]" />
            Temas de Estudo
          </span>
          <button
            onClick={onOpenCreateTema}
            className="p-0.5 rounded text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#27374D] transition-colors"
            title="Novo Tema"
          >
            <Plus className="w-3.5 h-3.5" />
          </button>
        </div>

        {/* Todos os Temas */}
        <button
          onClick={() => onSelectTema(null)}
          className={`w-full flex items-center justify-between px-2.5 py-2 rounded-lg text-xs font-medium transition-colors ${
            selectedTemaId === null
              ? "bg-[#27374D] text-[#DDE6ED] border border-[#526D82] font-semibold shadow-sm"
              : "text-[#9DB2BF] hover:bg-[#27374D]/60 hover:text-[#DDE6ED]"
          }`}
        >
          <div className="flex items-center gap-2">
            <span
              className={`w-1.5 h-1.5 rounded-full ${
                selectedTemaId === null ? "bg-[#DDE6ED]" : "bg-[#526D82]"
              }`}
            />
            Todos os temas
          </div>
          <span className="text-[10px] px-1.5 py-0.5 rounded bg-[#161B22] text-[#9DB2BF] font-mono border border-[#526D82]/40">
            {notas.length}
          </span>
        </button>

        {/* Temas individuais */}
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
                  ? "bg-[#27374D] text-[#DDE6ED] border border-[#526D82] font-semibold shadow-sm"
                  : "text-[#9DB2BF] hover:bg-[#27374D]/50 hover:text-[#DDE6ED]"
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
                <span className="text-[10px] px-1.5 py-0.5 rounded bg-[#161B22] text-[#9DB2BF] font-mono border border-[#526D82]/40 group-hover:hidden">
                  {count}
                </span>

                {/* Ações no Hover */}
                <div className="hidden group-hover:flex items-center gap-0.5">
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onEditTema(tema);
                    }}
                    className="p-1 rounded text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#526D82] transition-colors"
                    title="Editar"
                  >
                    <Edit2 className="w-3 h-3" />
                  </button>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onDeleteTema(tema);
                    }}
                    className="p-1 rounded text-[#9DB2BF] hover:text-red-400 hover:bg-red-950/40 transition-colors"
                    title="Excluir"
                  >
                    <Trash2 className="w-3 h-3" />
                  </button>
                </div>
              </div>
            </div>
          );
        })}

        {/* Estado Vazio */}
        {temas.length === 0 && (
          <div className="px-2 py-6 text-center">
            <p className="text-[11px] text-[#9DB2BF]">Nenhum tema criado ainda.</p>
            <button
              onClick={onOpenCreateTema}
              className="mt-2 text-[11px] text-[#DDE6ED] hover:underline font-semibold"
            >
              Criar primeiro tema →
            </button>
          </div>
        )}
      </div>

      {/* Rodapé da Sidebar */}
      <div className="p-3 border-t border-[#526D82]/40 bg-[#161B22]">
        <div className="flex items-center justify-center gap-1.5 text-[11px] text-[#9DB2BF]">
          <Network className="w-3.5 h-3.5 text-[#9DB2BF]" />
          <span>Grafo de Conhecimento Ativo</span>
        </div>
      </div>
    </aside>
  );
};

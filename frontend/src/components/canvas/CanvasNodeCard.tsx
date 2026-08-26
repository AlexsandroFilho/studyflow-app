import React from "react";
import { CanvasNode } from "../../types/canvas";
import { Tema } from "../../types/tema";
import { BookOpen, Edit3, Trash2, Calendar, Plus, Link2 } from "lucide-react";

interface CanvasNodeCardProps {
  node: CanvasNode;
  temas: Tema[];
  onMouseDown: (e: React.MouseEvent, nodeId: number) => void;
  onOpenInEditor: (notaId: number) => void;
  onEditNota: (node: CanvasNode) => void;
  onDeleteNota: (node: CanvasNode) => void;
  onCreateConnectedNote: (sourceNode: CanvasNode) => void;
  onStartConnecting: (e: React.MouseEvent, sourceNodeId: number) => void;
  isConnectingSource?: boolean;
}

export const CanvasNodeCard: React.FC<CanvasNodeCardProps> = ({
  node,
  temas,
  onMouseDown,
  onOpenInEditor,
  onEditNota,
  onDeleteNota,
  onCreateConnectedNote,
  onStartConnecting,
  isConnectingSource = false,
}) => {
  const tema = temas.find((t) => t.id === node.data.temaId);

  const formatDate = (dateStr?: string | null) => {
    if (!dateStr) return "";
    try {
      return new Date(dateStr).toLocaleDateString("pt-BR", {
        day: "2-digit",
        month: "short",
      });
    } catch {
      return "";
    }
  };

  const themeColor = node.color || "#526D82";

  return (
    <div
      style={{
        transform: `translate(${node.position.x}px, ${node.position.y}px)`,
        outline: node.isSelected || isConnectingSource ? "2px solid #9DB2BF" : "none",
        outlineOffset: "2px",
        boxShadow: node.isSelected
          ? "0 8px 24px rgba(0, 0, 0, 0.45), 0 0 0 1px #9DB2BF"
          : "0 4px 14px rgba(0, 0, 0, 0.35)",
      }}
      onMouseDown={(e) => onMouseDown(e, node.id)}
      onDoubleClick={(e) => {
        e.stopPropagation();
        onOpenInEditor(node.id);
      }}
      className={`absolute w-72 rounded-xl bg-[#27374D] border ${
        node.isSelected ? "border-[#9DB2BF]" : "border-[#526D82]"
      } p-5 cursor-grab active:cursor-grabbing transition-colors duration-150 select-none group z-10 hover:border-[#9DB2BF]`}
    >
      {/* Botão de Criação de Nota Conectada (+) na lateral direita */}
      <button
        onClick={(e) => {
          e.stopPropagation();
          onCreateConnectedNote(node);
        }}
        className="absolute -right-3.5 top-1/2 -translate-y-1/2 w-7 h-7 rounded-full bg-[#526D82] hover:bg-[#9DB2BF] text-[#DDE6ED] hover:text-[#161B22] border-2 border-[#161B22] flex items-center justify-center shadow-md transition-all transform hover:scale-110 z-20"
        title="Criar nova nota conectada a esta"
      >
        <Plus className="w-4 h-4 font-bold" />
      </button>

      {/* Tag do Tema & Ações Rápidas */}
      <div className="flex items-center justify-between gap-2 mb-3">
        <span
          className="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-md text-[10px] font-semibold uppercase tracking-wider"
          style={{
            backgroundColor: `${themeColor}33`,
            color: "#DDE6ED",
            border: `1px solid ${themeColor}88`,
          }}
        >
          <span
            className="w-1.5 h-1.5 rounded-full shrink-0"
            style={{ backgroundColor: themeColor }}
          />
          {tema?.nome || `Tema #${node.data.temaId}`}
        </span>

        {/* Ações rápidas no Hover */}
        <div className="flex items-center gap-0.5 opacity-0 group-hover:opacity-100 transition-opacity">
          {/* Ligar a outra nota */}
          <button
            onClick={(e) => onStartConnecting(e, node.id)}
            className="p-1.5 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#526D82]/50 transition-colors"
            title="Ligar a outra nota"
          >
            <Link2 className="w-3.5 h-3.5" />
          </button>

          <button
            onClick={(e) => {
              e.stopPropagation();
              onOpenInEditor(node.id);
            }}
            className="p-1.5 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#526D82]/50 transition-colors"
            title="Abrir em Modo Foco"
          >
            <BookOpen className="w-3.5 h-3.5" />
          </button>

          <button
            onClick={(e) => {
              e.stopPropagation();
              onEditNota(node);
            }}
            className="p-1.5 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#526D82]/50 transition-colors"
            title="Editar Nota"
          >
            <Edit3 className="w-3.5 h-3.5" />
          </button>

          <button
            onClick={(e) => {
              e.stopPropagation();
              onDeleteNota(node);
            }}
            className="p-1.5 rounded-md text-[#9DB2BF] hover:text-red-400 hover:bg-red-950/40 transition-colors"
            title="Excluir Nota"
          >
            <Trash2 className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      {/* Título */}
      <h4 className="text-sm font-bold text-[#DDE6ED] leading-snug mb-2 line-clamp-1 group-hover:text-white transition-colors">
        {node.data.titulo}
      </h4>

      {/* Snippet de Conteúdo */}
      <p className="text-xs text-[#9DB2BF] line-clamp-3 leading-relaxed mb-4">
        {node.data.conteudo}
      </p>

      {/* Rodapé */}
      <div className="flex items-center justify-between pt-3 border-t border-[#526D82]/50 text-[11px] text-[#9DB2BF]">
        <div className="flex items-center gap-1">
          <Calendar className="w-3 h-3 text-[#9DB2BF]" />
          <span>{formatDate(node.data.dataCriacao) || "—"}</span>
        </div>
        <span className="opacity-0 group-hover:opacity-75 transition-opacity text-[10px]">
          2× para abrir
        </span>
      </div>
    </div>
  );
};

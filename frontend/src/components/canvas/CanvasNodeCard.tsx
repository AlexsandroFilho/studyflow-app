import React from "react";
import { CanvasNode } from "../../types/canvas";
import { AnchorSide } from "../../hooks/useCanvas";
import { Tema } from "../../types/tema";
import { Sparkles, Calendar, Tag, Trash2, Edit3, ExternalLink } from "lucide-react";
import { toPlainMarkdown } from "../../utils/markdown";

interface CanvasNodeCardProps {
  node: CanvasNode;
  temas: Tema[];
  onMouseDown: (e: React.MouseEvent, nodeId: number) => void;
  onOpenInEditor: (notaId: number) => void;
  onEditNota: (node: CanvasNode) => void;
  onDeleteNota: (node: CanvasNode) => void;
  onStartConnecting: (e: React.MouseEvent, sourceNodeId: number, sourceSide: AnchorSide) => void;
  onFinishConnecting: (e: React.MouseEvent, targetNodeId: number, targetSide: AnchorSide) => void;
  isConnecting?: boolean;
  isConnectingSource?: boolean;
}

export const CanvasNodeCard: React.FC<CanvasNodeCardProps> = ({
  node,
  temas,
  onMouseDown,
  onOpenInEditor,
  onEditNota,
  onDeleteNota,
  onStartConnecting,
  onFinishConnecting,
  isConnecting,
  isConnectingSource,
}) => {
  const { data, position, color, isSelected } = node;
  const tema = temas.find((t) => t.id === data.temaId);

  const formatDate = (dateStr?: string | null) => {
    if (!dateStr) return "-";
    try {
      return new Date(dateStr).toLocaleDateString("pt-BR", {
        day: "2-digit",
        month: "short",
      });
    } catch {
      return dateStr;
    }
  };

  const truncateText = (text: string, maxLength: number) => {
    if (!text) return "";
    return text.length > maxLength ? text.slice(0, maxLength) + "..." : text;
  };

  const contentPreview = truncateText(
    toPlainMarkdown(data.conteudo || "Nenhum conteúdo adicionado."),
    80
  );

  return (
    <div
      className="absolute group"
      style={{
        transform: `translate3d(${position.x}px, ${position.y}px, 0)`,
        width: "288px",
        height: "160px",
      }}
      onMouseDown={(e) => onMouseDown(e, node.id)}
      onDoubleClick={(e) => {
        e.stopPropagation();
        onOpenInEditor(node.id);
      }}
    >
      <button
        type="button"
        title="Conectar (Topo)"
        onMouseDown={(e) => e.stopPropagation()}
        onClick={(e) => {
          if (isConnecting) {
            onFinishConnecting(e, node.id, "top");
          } else {
            onStartConnecting(e, node.id, "top");
          }
        }}
        className="absolute -top-2 left-1/2 -translate-x-1/2 w-4 h-4 rounded-full bg-[#8B5CF6] border-2 border-[#0F0E17] opacity-0 group-hover:opacity-100 hover:scale-125 transition-all z-30 cursor-crosshair"
      />
      <button
        type="button"
        title="Conectar (Base)"
        onMouseDown={(e) => e.stopPropagation()}
        onClick={(e) => {
          if (isConnecting) {
            onFinishConnecting(e, node.id, "bottom");
          } else {
            onStartConnecting(e, node.id, "bottom");
          }
        }}
        className="absolute -bottom-2 left-1/2 -translate-x-1/2 w-4 h-4 rounded-full bg-[#8B5CF6] border-2 border-[#0F0E17] opacity-0 group-hover:opacity-100 hover:scale-125 transition-all z-30 cursor-crosshair"
      />
      <button
        type="button"
        title="Conectar (Esquerda)"
        onMouseDown={(e) => e.stopPropagation()}
        onClick={(e) => {
          if (isConnecting) {
            onFinishConnecting(e, node.id, "left");
          } else {
            onStartConnecting(e, node.id, "left");
          }
        }}
        className="absolute -left-2 top-1/2 -translate-y-1/2 w-4 h-4 rounded-full bg-[#8B5CF6] border-2 border-[#0F0E17] opacity-0 group-hover:opacity-100 hover:scale-125 transition-all z-30 cursor-crosshair"
      />
      <button
        type="button"
        title="Conectar (Direita)"
        onMouseDown={(e) => e.stopPropagation()}
        onClick={(e) => {
          if (isConnecting) {
            onFinishConnecting(e, node.id, "right");
          } else {
            onStartConnecting(e, node.id, "right");
          }
        }}
        className="absolute -right-2 top-1/2 -translate-y-1/2 w-4 h-4 rounded-full bg-[#8B5CF6] border-2 border-[#0F0E17] opacity-0 group-hover:opacity-100 hover:scale-125 transition-all z-30 cursor-crosshair"
      />

      <div
        className={`w-full h-full rounded-xl border p-3.5 flex flex-col justify-between transition-all duration-150 cursor-grab active:cursor-grabbing shadow-xl shadow-purple-950/20 relative ${
          isSelected
            ? "bg-[#27374D] border-[#9DB2BF] ring-2 ring-[#9DB2BF]/30 shadow-2xl scale-[1.02]"
            : isConnectingSource
            ? "bg-[#27374D] border-[#9DB2BF] ring-2 ring-emerald-500/50"
            : "bg-[#1C2430]/95 border-[#526D82]/40 hover:border-[#526D82]"
        }`}
        style={{
          borderTopWidth: "4px",
          borderTopColor: color,
        }}
      >
        <div>
          <div className="flex items-start justify-between gap-2 mb-1.5">
            <h3 className="font-semibold text-sm text-[#DDE6ED] leading-snug line-clamp-1 flex-1">
              {data.titulo || "Sem título"}
            </h3>

            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  onOpenInEditor(node.id);
                }}
                className="p-1 rounded text-[#9DB2BF] hover:text-white hover:bg-[#526D82]/50 transition-colors"
                title="Abrir no Editor"
              >
                <ExternalLink className="w-3.5 h-3.5" />
              </button>
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  onEditNota(node);
                }}
                className="p-1 rounded text-[#9DB2BF] hover:text-white hover:bg-[#526D82]/50 transition-colors"
                title="Editar"
              >
                <Edit3 className="w-3.5 h-3.5" />
              </button>
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  onDeleteNota(node);
                }}
                className="p-1 rounded text-[#9DB2BF] hover:text-red-400 hover:bg-red-500/10 transition-colors"
                title="Excluir"
              >
                <Trash2 className="w-3.5 h-3.5" />
              </button>
            </div>
          </div>

          <p className="text-xs text-[#9DB2BF] line-clamp-2 leading-relaxed">
            {contentPreview}
          </p>
        </div>

        <div className="flex items-center justify-between text-[11px] text-[#526D82] pt-2 border-t border-[#526D82]/20 mt-auto">
          <div className="flex items-center gap-1 max-w-[60%]">
            <Tag className="w-3 h-3 shrink-0" style={{ color }} />
            <span className="truncate text-[#9DB2BF]">{tema?.nome || "Geral"}</span>
          </div>

          <div className="flex items-center gap-2">
            {data.resumoIa && (
              <span title="Resumo de IA Gerado">
                <Sparkles className="w-3 h-3 text-amber-400" />
              </span>
            )}
            <div className="flex items-center gap-1">
              <Calendar className="w-3 h-3" />
              <span>{formatDate(data.dataAtualizacao || data.dataCriacao)}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
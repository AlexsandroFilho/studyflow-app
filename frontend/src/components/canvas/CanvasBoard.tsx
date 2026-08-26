import React from "react";
import { CanvasNode, CanvasEdge, CanvasViewport, Position } from "../../types/canvas";
import { Tema } from "../../types/tema";
import { CanvasNodeCard } from "./CanvasNodeCard";
import { CanvasControls } from "./CanvasControls";
import { EmptyState } from "../ui/EmptyState";
import { Network, MousePointer2, Link2, X } from "lucide-react";

interface CanvasBoardProps {
  nodes: CanvasNode[];
  edges: CanvasEdge[];
  temas: Tema[];
  viewport: CanvasViewport;
  connectingSourceId: number | null;
  connectingMousePos: Position | null;
  onCanvasMouseDown: (e: React.MouseEvent) => void;
  onNodeMouseDown: (e: React.MouseEvent, nodeId: number) => void;
  onStartConnecting: (e: React.MouseEvent, sourceNodeId: number) => void;
  onMouseMove: (e: React.MouseEvent) => void;
  onMouseUp: () => void;
  onOpenInEditor: (notaId: number) => void;
  onEditNota: (node: CanvasNode) => void;
  onDeleteNota: (node: CanvasNode) => void;
  onCreateConnectedNote: (sourceNode: CanvasNode) => void;
  onDeleteEdge?: (edge: CanvasEdge) => void;
  onZoomIn: () => void;
  onZoomOut: () => void;
  onResetView: () => void;
  onOpenCreateNota: () => void;
  onDoubleClickCanvas?: (x: number, y: number) => void;
  onCancelConnecting?: () => void;
}

export const CanvasBoard: React.FC<CanvasBoardProps> = ({
  nodes,
  edges,
  temas,
  viewport,
  connectingSourceId,
  connectingMousePos,
  onCanvasMouseDown,
  onNodeMouseDown,
  onStartConnecting,
  onMouseMove,
  onMouseUp,
  onOpenInEditor,
  onEditNota,
  onDeleteNota,
  onCreateConnectedNote,
  onDeleteEdge,
  onZoomIn,
  onZoomOut,
  onResetView,
  onOpenCreateNota,
  onDoubleClickCanvas,
  onCancelConnecting,
}) => {
  const nodeMap = new Map<number, CanvasNode>(nodes.map((n) => [n.id, n]));

  // Curva de Bézier suave entre os nós
  const calculateCurvePath = (x1: number, y1: number, x2: number, y2: number) => {
    const dx = Math.abs(x2 - x1) * 0.45;
    const dy = (y2 - y1) * 0.45;
    return `M ${x1} ${y1} C ${x1 + dx} ${y1 + dy}, ${x2 - dx} ${y2 - dy}, ${x2} ${y2}`;
  };

  const handleDoubleClick = (e: React.MouseEvent) => {
    const target = e.target as HTMLElement;
    if (target === e.currentTarget || target.classList.contains("canvas-bg")) {
      const rect = e.currentTarget.getBoundingClientRect();
      const x = (e.clientX - rect.left - viewport.x) / viewport.zoom;
      const y = (e.clientY - rect.top - viewport.y) / viewport.zoom;
      if (onDoubleClickCanvas) {
        onDoubleClickCanvas(x, y);
      } else {
        onOpenCreateNota();
      }
    }
  };

  const connectingSourceNode = connectingSourceId ? nodeMap.get(connectingSourceId) : null;

  return (
    <div
      className="canvas-container relative flex-1 h-full w-full overflow-hidden bg-[#161B22] select-none cursor-default"
      onMouseDown={onCanvasMouseDown}
      onMouseMove={onMouseMove}
      onMouseUp={onMouseUp}
      onDoubleClick={handleDoubleClick}
    >
      {/* Grade de pontos suave (#2D3748) */}
      <div
        className="absolute inset-0 pointer-events-none canvas-bg"
        style={{
          backgroundImage: "radial-gradient(circle, #2D3748 1.2px, transparent 1.2px)",
          backgroundSize: `${32 * viewport.zoom}px ${32 * viewport.zoom}px`,
          backgroundPosition: `${viewport.x}px ${viewport.y}px`,
        }}
      />

      {/* Camada transformável com Pan & Zoom */}
      <div
        className="absolute inset-0 origin-top-left canvas-bg"
        style={{
          transform: `translate(${viewport.x}px, ${viewport.y}px) scale(${viewport.zoom})`,
        }}
      >
        {/* SVG de Conexões Persistidas */}
        <svg className="absolute inset-0 w-[50000px] h-[50000px] pointer-events-none overflow-visible">
          <defs>
            <marker
              id="arrowhead"
              markerWidth="8"
              markerHeight="6"
              refX="7"
              refY="3"
              orient="auto"
            >
              <polygon points="0 0, 8 3, 0 6" fill="#9DB2BF" />
            </marker>
          </defs>

          {/* Arestas Fixas */}
          {edges.map((edge) => {
            const source = nodeMap.get(edge.sourceId);
            const target = nodeMap.get(edge.targetId);
            if (!source || !target) return null;

            const cw = 288;
            const ch = 150;
            const x1 = source.position.x + cw;
            const y1 = source.position.y + ch / 2;
            const x2 = target.position.x;
            const y2 = target.position.y + ch / 2;

            const path = calculateCurvePath(x1, y1, x2, y2);

            return (
              <g key={edge.id} className="group/edge pointer-events-auto cursor-pointer">
                {/* Linha invisível para clique facilitado */}
                <path
                  d={path}
                  fill="none"
                  stroke="transparent"
                  strokeWidth="20"
                  onClick={() => onDeleteEdge && onDeleteEdge(edge)}
                />
                {/* Linha de conexão visível em #526D82 com ponta em #9DB2BF */}
                <path
                  d={path}
                  fill="none"
                  stroke="#526D82"
                  strokeWidth="2"
                  strokeOpacity="0.85"
                  strokeLinecap="round"
                  markerEnd="url(#arrowhead)"
                  className="hover:stroke-[#9DB2BF] hover:stroke-[3px] transition-all"
                  onClick={() => onDeleteEdge && onDeleteEdge(edge)}
                />
              </g>
            );
          })}

          {/* Linha Elástica durante criação de conexão */}
          {connectingSourceNode && connectingMousePos && (
            <path
              d={calculateCurvePath(
                connectingSourceNode.position.x + 288,
                connectingSourceNode.position.y + 75,
                connectingMousePos.x,
                connectingMousePos.y
              )}
              fill="none"
              stroke="#9DB2BF"
              strokeWidth="2.5"
              strokeDasharray="6 4"
              strokeLinecap="round"
            />
          )}
        </svg>

        {/* Cartões de Nós (#27374D) */}
        {nodes.map((node) => (
          <CanvasNodeCard
            key={node.id}
            node={node}
            temas={temas}
            onMouseDown={onNodeMouseDown}
            onOpenInEditor={onOpenInEditor}
            onEditNota={onEditNota}
            onDeleteNota={onDeleteNota}
            onCreateConnectedNote={onCreateConnectedNote}
            onStartConnecting={onStartConnecting}
            isConnectingSource={connectingSourceId === node.id}
          />
        ))}
      </div>

      {/* Estado Vazio */}
      {nodes.length === 0 && (
        <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
          <div className="pointer-events-auto">
            <EmptyState
              icon={<Network className="w-8 h-8" />}
              title="Seu Mapa Mental está vazio"
              description="Crie sua primeira nota e utilize o botão '+' nos cartões para expandir conexões no grafo de estudos."
              actionLabel="Criar Primeira Nota"
              onAction={onOpenCreateNota}
            />
          </div>
        </div>
      )}

      {/* Banner de Modo de Conexão ou Dica Padrão */}
      <div className="absolute top-4 left-5 z-20">
        {connectingSourceId ? (
          <div className="flex items-center gap-2.5 px-3.5 py-2 rounded-lg bg-[#27374D] border border-[#9DB2BF] text-xs text-[#DDE6ED] shadow-xl animate-pulse">
            <Link2 className="w-4 h-4 text-[#9DB2BF]" />
            <span>Clique na nota de destino para conectá-las</span>
            <button
              onClick={onCancelConnecting}
              className="ml-2 p-1 rounded hover:bg-[#526D82] text-[#9DB2BF] hover:text-white"
              title="Cancelar"
            >
              <X className="w-3.5 h-3.5" />
            </button>
          </div>
        ) : (
          <div className="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-[#1C2430]/90 border border-[#526D82]/50 text-[11px] text-[#9DB2BF] shadow-md backdrop-blur-sm">
            <MousePointer2 className="w-3.5 h-3.5 shrink-0 text-[#9DB2BF]" />
            <span>Arraste o fundo para navegar · Use o '+' no card para criar conectada</span>
          </div>
        )}
      </div>

      {/* Controles de Zoom e Nova Nota */}
      <CanvasControls
        zoom={viewport.zoom}
        onZoomIn={onZoomIn}
        onZoomOut={onZoomOut}
        onResetView={onResetView}
        onAddNoteAtCenter={onOpenCreateNota}
      />
    </div>
  );
};

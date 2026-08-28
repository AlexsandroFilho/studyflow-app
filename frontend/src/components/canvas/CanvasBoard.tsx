import React from "react";
import { CanvasNode, CanvasEdge, CanvasViewport, Position } from "../../types/canvas";
import { AnchorSide } from "../../hooks/useCanvas";
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
  connectingSourceSide: AnchorSide | null;
  connectingMousePos: Position | null;
  onCanvasMouseDown: (e: React.MouseEvent) => void;
  onNodeMouseDown: (e: React.MouseEvent, nodeId: number) => void;
  onStartConnecting: (e: React.MouseEvent, sourceNodeId: number, sourceSide: AnchorSide) => void;
  onMouseMove: (e: React.MouseEvent) => void;
  onMouseUp: () => void;
  onOpenInEditor: (notaId: number) => void;
  onEditNota: (node: CanvasNode) => void;
  onDeleteNota: (node: CanvasNode) => void;
  onFinishConnecting: (e: React.MouseEvent, targetNodeId: number, targetSide: AnchorSide) => void;
  onDeleteEdge?: (edge: CanvasEdge) => void;
  onZoomIn: () => void;
  onZoomOut: () => void;
  onResetView: () => void;
  onOpenCreateNota: () => void;
  onDoubleClickCanvas?: (x: number, y: number) => void;
  onCancelConnecting?: () => void;
}

const CARD_WIDTH = 288;
const CARD_HEIGHT = 160;

const getAnchorPoint = (node: CanvasNode, side: AnchorSide) => {
  if (side === "top") return { x: node.position.x + CARD_WIDTH / 2, y: node.position.y };
  if (side === "right") return { x: node.position.x + CARD_WIDTH, y: node.position.y + CARD_HEIGHT / 2 };
  if (side === "bottom") return { x: node.position.x + CARD_WIDTH / 2, y: node.position.y + CARD_HEIGHT };
  return { x: node.position.x, y: node.position.y + CARD_HEIGHT / 2 };
};

const getClosestAnchor = (source: CanvasNode, target: CanvasNode) => {
  const sCenter = { x: source.position.x + CARD_WIDTH / 2, y: source.position.y + CARD_HEIGHT / 2 };
  const tCenter = { x: target.position.x + CARD_WIDTH / 2, y: target.position.y + CARD_HEIGHT / 2 };

  const dx = tCenter.x - sCenter.x;
  const dy = tCenter.y - sCenter.y;

  let x1 = source.position.x + CARD_WIDTH / 2;
  let y1 = source.position.y + CARD_HEIGHT / 2;
  let x2 = target.position.x + CARD_WIDTH / 2;
  let y2 = target.position.y + CARD_HEIGHT / 2;

  if (Math.abs(dx) > Math.abs(dy)) {
    x1 = dx > 0 ? source.position.x + CARD_WIDTH : source.position.x;
    x2 = dx > 0 ? target.position.x : target.position.x + CARD_WIDTH;
  } else {
    y1 = dy > 0 ? source.position.y + CARD_HEIGHT : source.position.y;
    y2 = dy > 0 ? target.position.y : target.position.y + CARD_HEIGHT;
  }

  return { x1, y1, x2, y2 };
};

export const CanvasBoard: React.FC<CanvasBoardProps> = ({
  nodes,
  edges,
  temas,
  viewport,
  connectingSourceId,
  connectingSourceSide,
  connectingMousePos,
  onCanvasMouseDown,
  onNodeMouseDown,
  onStartConnecting,
  onMouseMove,
  onMouseUp,
  onOpenInEditor,
  onEditNota,
  onDeleteNota,
  onFinishConnecting,
  onDeleteEdge,
  onZoomIn,
  onZoomOut,
  onResetView,
  onOpenCreateNota,
  onDoubleClickCanvas,
  onCancelConnecting,
}) => {
  const nodeMap = new Map<number, CanvasNode>(nodes.map((n) => [n.id, n]));

  const calculateCurvePath = (
    x1: number,
    y1: number,
    x2: number,
    y2: number,
    sourceSide?: AnchorSide,
    targetSide?: AnchorSide
  ) => {
    const distance = Math.max(60, Math.min(180, Math.hypot(x2 - x1, y2 - y1) * 0.45));
    const sourceVector = sourceSide === "top"
      ? { x: 0, y: -1 }
      : sourceSide === "right"
      ? { x: 1, y: 0 }
      : sourceSide === "bottom"
      ? { x: 0, y: 1 }
      : { x: -1, y: 0 };
    const targetVector = targetSide === "top"
      ? { x: 0, y: -1 }
      : targetSide === "right"
      ? { x: 1, y: 0 }
      : targetSide === "bottom"
      ? { x: 0, y: 1 }
      : { x: -1, y: 0 };
    const control1 = { x: x1 + sourceVector.x * distance, y: y1 + sourceVector.y * distance };
    const control2 = { x: x2 + targetVector.x * distance, y: y2 + targetVector.y * distance };
    return `M ${x1} ${y1} C ${control1.x} ${control1.y}, ${control2.x} ${control2.y}, ${x2} ${y2}`;
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
      className="canvas-container relative flex-1 h-full w-full overflow-hidden select-none cursor-default"
      onMouseDown={onCanvasMouseDown}
      onMouseMove={onMouseMove}
      onMouseUp={onMouseUp}
      onDoubleClick={handleDoubleClick}
    >
      <div
        className="absolute inset-0 pointer-events-none canvas-bg bg-[#F8FAFC]"
        style={{
          backgroundImage: "radial-gradient(circle, rgba(148, 163, 184, 0.42) 1.1px, transparent 1.1px)",
          backgroundSize: `${32 * viewport.zoom}px ${32 * viewport.zoom}px`,
          backgroundPosition: `${viewport.x}px ${viewport.y}px`,
        }}
      />

      <div
        className="absolute inset-0 origin-top-left canvas-bg"
        style={{
          transform: `translate(${viewport.x}px, ${viewport.y}px) scale(${viewport.zoom})`,
        }}
      >
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
              <polygon points="0 0, 8 3, 0 6" fill="#2563eb" />
            </marker>
          </defs>

          {/* Conexões com Ancoragem Dinâmica Inteligente */}
          {edges.map((edge) => {
            const source = nodeMap.get(edge.sourceId);
            const target = nodeMap.get(edge.targetId);
            if (!source || !target) return null;

            const closest = getClosestAnchor(source, target);
            const sourcePoint = edge.sourceSide ? getAnchorPoint(source, edge.sourceSide) : { x: closest.x1, y: closest.y1 };
            const targetPoint = edge.targetSide ? getAnchorPoint(target, edge.targetSide) : { x: closest.x2, y: closest.y2 };
            const { x1, y1, x2, y2 } = {
              x1: sourcePoint.x,
              y1: sourcePoint.y,
              x2: targetPoint.x,
              y2: targetPoint.y,
            };
            const path = calculateCurvePath(x1, y1, x2, y2, edge.sourceSide, edge.targetSide);

            return (
              <g key={edge.id} className="group/edge pointer-events-auto cursor-pointer">
                <path
                  d={path}
                  fill="none"
                  stroke="transparent"
                  strokeWidth="20"
                  onClick={() => onDeleteEdge && onDeleteEdge(edge)}
                />
                <path
                  d={path}
                  fill="none"
                  stroke="#2563eb"
                  strokeWidth="2"
                  strokeOpacity="0.9"
                  strokeLinecap="round"
                  markerEnd="url(#arrowhead)"
                  className="hover:stroke-blue-600 hover:stroke-[3px] transition-all"
                  onClick={() => onDeleteEdge && onDeleteEdge(edge)}
                />
              </g>
            );
          })}

          {connectingSourceNode && connectingMousePos && (
            <path
              d={calculateCurvePath(
                getAnchorPoint(connectingSourceNode, connectingSourceSide || "right").x,
                getAnchorPoint(connectingSourceNode, connectingSourceSide || "right").y,
                connectingMousePos.x,
                connectingMousePos.y,
                connectingSourceSide || undefined
              )}
              fill="none"
              stroke="#2563eb"
              strokeWidth="2.5"
              strokeDasharray="6 4"
              strokeLinecap="round"
            />
          )}
        </svg>

        {nodes.map((node) => (
          <CanvasNodeCard
            key={node.id}
            node={node}
            temas={temas}
            onMouseDown={onNodeMouseDown}
            onOpenInEditor={onOpenInEditor}
            onEditNota={onEditNota}
            onDeleteNota={onDeleteNota}
            onStartConnecting={onStartConnecting}
            onFinishConnecting={onFinishConnecting}
            isConnecting={connectingSourceId !== null}
            isConnectingSource={connectingSourceId === node.id}
          />
        ))}
      </div>

      {nodes.length === 0 && (
        <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
          <div className="pointer-events-auto">
            <EmptyState
              icon={<Network className="w-8 h-8" />}
              title="Seu Mapa Mental está vazio"
              description="Crie sua primeira nota e utilize os pontos nos cartões para conectar notas existentes."
              actionLabel="Criar Primeira Nota"
              onAction={onOpenCreateNota}
            />
          </div>
        </div>
      )}

      <div className="absolute top-4 left-5 z-20">
        {connectingSourceId ? (
          <div className="flex items-center gap-2.5 px-3.5 py-2 rounded-lg bg-white border border-blue-200 text-xs text-slate-700 shadow-md animate-pulse">
            <Link2 className="w-4 h-4 text-blue-600" />
            <span>Clique na nota de destino para conectá-las</span>
            <button
              onClick={onCancelConnecting}
              className="ml-2 p-1 rounded hover:bg-slate-100 text-slate-500 hover:text-slate-700"
              title="Cancelar"
            >
              <X className="w-3.5 h-3.5" />
            </button>
          </div>
        ) : (
          <div className="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-white/90 border border-slate-200 text-[11px] text-slate-600 shadow-sm backdrop-blur-sm">
            <MousePointer2 className="w-3.5 h-3.5 shrink-0 text-blue-600" />
            <span>Arraste o fundo para navegar · Use os pontos do card para conectar notas</span>
          </div>
        )}
      </div>

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
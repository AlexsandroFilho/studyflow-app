import React from "react";
import { CanvasNode, CanvasEdge, CanvasViewport, Position } from "../../types/canvas";
import { AnchorSide } from "../../hooks/useCanvas";
import { Tema } from "../../types/tema";
import { CanvasNodeCard } from "./CanvasNodeCard";
import { CanvasControls } from "./CanvasControls";
import { EmptyState } from "../ui/EmptyState";
import { Network, MousePointer2, Link2, X, FileText, Loader2, ListChecks } from "lucide-react";

interface CanvasBoardProps {
  nodes: CanvasNode[];
  edges: CanvasEdge[];
  temas: Tema[];
  temaSelecionado: Tema | null;
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
  onGerarResumoTema?: () => void;
  resumindoTema?: boolean;
  onGerarQuizTema?: () => void;
  gerandoQuizTema?: boolean;
  onDoubleClickCanvas?: (x: number, y: number) => void;
  onCancelConnecting?: () => void;
}

const CARD_WIDTH = 288;
const CARD_HEIGHT = 160;
const CONNECTION_GAP = 18;

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

  let sourceSide: AnchorSide;
  let targetSide: AnchorSide;

  if (Math.abs(dx) > Math.abs(dy)) {
    sourceSide = dx > 0 ? "right" : "left";
    targetSide = dx > 0 ? "left" : "right";
  } else {
    sourceSide = dy > 0 ? "bottom" : "top";
    targetSide = dy > 0 ? "top" : "bottom";
  }

  return { sourceSide, targetSide };
};

const moveFromAnchor = (point: Position, side: AnchorSide, distance: number): Position => {
  if (side === "top") return { x: point.x, y: point.y - distance };
  if (side === "right") return { x: point.x + distance, y: point.y };
  if (side === "bottom") return { x: point.x, y: point.y + distance };
  return { x: point.x - distance, y: point.y };
};

const isHorizontal = (side: AnchorSide) => side === "left" || side === "right";

export const CanvasBoard: React.FC<CanvasBoardProps> = ({
  nodes,
  edges,
  temas,
  temaSelecionado,
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
  onGerarResumoTema,
  resumindoTema = false,
  onGerarQuizTema,
  gerandoQuizTema = false,
  onDoubleClickCanvas,
  onCancelConnecting,
}) => {
  const nodeMap = new Map<number, CanvasNode>(nodes.map((n) => [n.id, n]));

  const calculateOrthogonalPath = (sourcePoint: Position, targetPoint: Position, sourceSide: AnchorSide, targetSide: AnchorSide) => {
    const sourceExit = moveFromAnchor(sourcePoint, sourceSide, CONNECTION_GAP);
    const targetEntry = moveFromAnchor(targetPoint, targetSide, CONNECTION_GAP);
    const sourceHorizontal = isHorizontal(sourceSide);
    const targetHorizontal = isHorizontal(targetSide);

    if (sourceHorizontal && targetHorizontal)
      return `M ${sourcePoint.x} ${sourcePoint.y} H ${sourceExit.x} V ${targetEntry.y} H ${targetPoint.x}`;

    if (!sourceHorizontal && !targetHorizontal)
      return `M ${sourcePoint.x} ${sourcePoint.y} V ${sourceExit.y} H ${targetEntry.x} V ${targetPoint.y}`;

    if (sourceHorizontal) {
      const meioY = (sourceExit.y + targetEntry.y) / 2;
      return `M ${sourcePoint.x} ${sourcePoint.y} H ${sourceExit.x} V ${meioY} H ${targetEntry.x} V ${targetPoint.y}`;
    }

    const meioX = (sourceExit.x + targetEntry.x) / 2;
    return `M ${sourcePoint.x} ${sourcePoint.y} V ${sourceExit.y} H ${meioX} V ${targetEntry.y} H ${targetPoint.x}`;
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
              refX="10"
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

            const ancorasPadrao = getClosestAnchor(source, target);
            const sourceSide = edge.sourceSide ?? ancorasPadrao.sourceSide;
            const targetSide = edge.targetSide ?? ancorasPadrao.targetSide;
            const sourcePoint = getAnchorPoint(source, sourceSide);
            const targetPoint = getAnchorPoint(target, targetSide);
            const path = calculateOrthogonalPath(sourcePoint, targetPoint, sourceSide, targetSide);

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
              d={`M ${getAnchorPoint(connectingSourceNode, connectingSourceSide || "right").x} ${getAnchorPoint(connectingSourceNode, connectingSourceSide || "right").y} L ${connectingMousePos.x} ${connectingMousePos.y}`}
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

      {(onGerarResumoTema || onGerarQuizTema) && (
        <div className="absolute top-4 right-5 z-20 flex items-center gap-2">
          {onGerarQuizTema && <button
            onClick={onGerarQuizTema}
            disabled={!temaSelecionado || gerandoQuizTema || nodes.length === 0}
            className="flex items-center gap-2 rounded-lg border border-blue-200 bg-white px-3.5 py-2 text-xs font-semibold text-blue-700 shadow-sm hover:bg-blue-50 disabled:cursor-not-allowed disabled:opacity-60"
            title={!temaSelecionado ? "Selecione um tema para gerar o quiz" : nodes.length === 0 ? "Crie uma nota neste tema" : "Gerar quiz do tema"}
          >
            {gerandoQuizTema ? <Loader2 className="w-4 h-4 animate-spin" /> : <ListChecks className="w-4 h-4" />}
            {gerandoQuizTema ? "Gerando..." : "Quiz do tema"}
          </button>}
          {onGerarResumoTema &&
          <button
            onClick={onGerarResumoTema}
            disabled={!temaSelecionado || resumindoTema || nodes.length === 0}
            className="flex items-center gap-2 rounded-lg bg-blue-600 px-3.5 py-2 text-xs font-semibold text-white shadow-sm hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
            title={!temaSelecionado
              ? "Selecione um tema na barra lateral para gerar o resumo"
              : nodes.length === 0
                ? "Crie uma nota neste tema antes de gerar um resumo"
                : "Gerar resumo do tema com IA"}
          >
            {resumindoTema ? <Loader2 className="w-4 h-4 animate-spin" /> : <FileText className="w-4 h-4" />}
            {resumindoTema ? "Gerando..." : !temaSelecionado ? "Selecione um tema" : "Resumo do tema"}
          </button>
          }
        </div>
      )}

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

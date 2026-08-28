import { useState, useEffect, useCallback, useRef } from "react";
import { Nota } from "../types/nota";
import { Conexao } from "../types/conexao";
import { CanvasNode, CanvasEdge, CanvasViewport, Position } from "../types/canvas";

export type AnchorSide = "top" | "right" | "bottom" | "left";

export interface ConnectionAnchor {
  nodeId: number;
  side: AnchorSide;
}

const LOCAL_STORAGE_POSITIONS_KEY = "canvas_node_positions";

function getSavedPositions(): Record<number, Position> {
  try {
    const saved = localStorage.getItem(LOCAL_STORAGE_POSITIONS_KEY);
    return saved ? JSON.parse(saved) : {};
  } catch {
    return {};
  }
}

function savePositions(positions: Record<number, Position>) {
  try {
    localStorage.setItem(LOCAL_STORAGE_POSITIONS_KEY, JSON.stringify(positions));
  } catch (err) {
    console.error("Erro ao salvar posições do canvas:", err);
  }
}

const THEME_COLORS = [
  "#6C35D9",
  "#9B72E8",
  "#5120B5",
  "#D9C8F7",
];

export function getThemeColor(temaId: number | null): string {
  const index = temaId === null ? 0 : Math.abs(temaId) % THEME_COLORS.length;
  return THEME_COLORS[index];
}

export function useCanvas(
  notas: Nota[],
  conexoes: Conexao[],
  onConnectNodes?: (source: ConnectionAnchor, target: ConnectionAnchor) => void
) {
  const [nodes, setNodes] = useState<CanvasNode[]>([]);
  const [viewport, setViewport] = useState<CanvasViewport>({ x: 120, y: 100, zoom: 1 });
  const [selectedNodeId, setSelectedNodeId] = useState<number | null>(null);

  const [connectingSourceId, setConnectingSourceId] = useState<number | null>(null);
  const [connectingSourceSide, setConnectingSourceSide] = useState<AnchorSide | null>(null);
  const [connectingMousePos, setConnectingMousePos] = useState<Position | null>(null);
  const [edgeAnchors, setEdgeAnchors] = useState<Record<string, { sourceSide: AnchorSide; targetSide: AnchorSide }>>({});

  const isPanningRef = useRef(false);
  const panStartRef = useRef<Position>({ x: 0, y: 0 });
  const initialViewportRef = useRef<CanvasViewport>({ x: 0, y: 0, zoom: 1 });

  const draggedNodeIdRef = useRef<number | null>(null);
  const dragStartMouseRef = useRef<Position>({ x: 0, y: 0 });
  const dragStartNodePosRef = useRef<Position>({ x: 0, y: 0 });

  useEffect(() => {
    setNodes((prevNodes) => {
      const prevMap = new Map(prevNodes.map((n) => [n.id, n.position]));
      const savedPositions = getSavedPositions();

      const temaGroups = new Map<number | null, Nota[]>();
      notas.forEach((n) => {
        const list = temaGroups.get(n.temaId) || [];
        list.push(n);
        temaGroups.set(n.temaId, list);
      });

      let groupIndex = 0;
      const calculatedNodes: CanvasNode[] = [];

      temaGroups.forEach((groupNotas, temaId) => {
        const groupCenterX = 260 + (groupIndex % 3) * 500;
        const groupCenterY = 220 + Math.floor(groupIndex / 3) * 460;
        const color = getThemeColor(temaId);

        groupNotas.forEach((nota, idx) => {
          let pos = prevMap.get(nota.id) || savedPositions[nota.id];

          if (!pos) {
            const angle = (idx * (2 * Math.PI)) / Math.max(groupNotas.length, 1);
            const radius = groupNotas.length > 1 ? 180 : 0;
            pos = {
              x: groupCenterX + Math.cos(angle) * radius,
              y: groupCenterY + Math.sin(angle) * radius,
            };
          }

          calculatedNodes.push({
            id: nota.id,
            data: nota,
            position: pos,
            color,
            isSelected: nota.id === selectedNodeId,
          });
        });

        groupIndex++;
      });

      return calculatedNodes;
    });
  }, [notas, selectedNodeId]);

  const edges: CanvasEdge[] = conexoes.map((c) => ({
    id: `edge-${c.id}`,
    sourceId: c.notaOrigemId,
    targetId: c.notaDestinoId,
    ...edgeAnchors[`pair-${c.notaOrigemId}-${c.notaDestinoId}`],
    color: "#6C35D9",
  }));

  const handleCanvasMouseDown = useCallback(
    (e: React.MouseEvent) => {
      if (
        e.target !== e.currentTarget &&
        !(e.target as HTMLElement).classList.contains("canvas-bg")
      ) {
        return;
      }
      if (connectingSourceId !== null) {
        setConnectingSourceId(null);
        setConnectingSourceSide(null);
        setConnectingMousePos(null);
        return;
      }
      isPanningRef.current = true;
      panStartRef.current = { x: e.clientX, y: e.clientY };
      initialViewportRef.current = { ...viewport };
    },
    [viewport, connectingSourceId]
  );

  const handleNodeMouseDown = useCallback(
    (e: React.MouseEvent, nodeId: number, targetSide?: AnchorSide) => {
      e.stopPropagation();
      setSelectedNodeId(nodeId);

      if (connectingSourceId !== null) {
        if (connectingSourceId !== nodeId && connectingSourceSide && targetSide && onConnectNodes) {
          setEdgeAnchors((prev) => ({
            ...prev,
            [`pair-${connectingSourceId}-${nodeId}`]: {
              sourceSide: connectingSourceSide,
              targetSide,
            },
          }));
          onConnectNodes(
            { nodeId: connectingSourceId, side: connectingSourceSide },
            { nodeId, side: targetSide }
          );
        }
        setConnectingSourceId(null);
        setConnectingSourceSide(null);
        setConnectingMousePos(null);
        return;
      }

      draggedNodeIdRef.current = nodeId;
      dragStartMouseRef.current = { x: e.clientX, y: e.clientY };

      const node = nodes.find((n) => n.id === nodeId);
      if (node) {
        dragStartNodePosRef.current = { ...node.position };
      }
    },
    [nodes, connectingSourceId, connectingSourceSide, onConnectNodes]
  );

  const handleStartConnecting = useCallback(
    (e: React.MouseEvent, sourceNodeId: number, sourceSide: AnchorSide) => {
      e.stopPropagation();
      setConnectingSourceId(sourceNodeId);
      setConnectingSourceSide(sourceSide);
      const rect = (e.currentTarget.closest(".canvas-container") || document.body).getBoundingClientRect();
      setConnectingMousePos({
        x: (e.clientX - rect.left - viewport.x) / viewport.zoom,
        y: (e.clientY - rect.top - viewport.y) / viewport.zoom,
      });
    },
    [viewport]
  );

  const handleMouseMove = useCallback(
    (e: React.MouseEvent) => {
      if (connectingSourceId !== null) {
        const container = e.currentTarget.getBoundingClientRect();
        setConnectingMousePos({
          x: (e.clientX - container.left - viewport.x) / viewport.zoom,
          y: (e.clientY - container.top - viewport.y) / viewport.zoom,
        });
        return;
      }

      if (draggedNodeIdRef.current !== null) {
        const dx = (e.clientX - dragStartMouseRef.current.x) / viewport.zoom;
        const dy = (e.clientY - dragStartMouseRef.current.y) / viewport.zoom;

        setNodes((prev) =>
          prev.map((n) => {
            if (n.id === draggedNodeIdRef.current) {
              return {
                ...n,
                position: {
                  x: dragStartNodePosRef.current.x + dx,
                  y: dragStartNodePosRef.current.y + dy,
                },
              };
            }
            return n;
          })
        );
      } else if (isPanningRef.current) {
        const dx = e.clientX - panStartRef.current.x;
        const dy = e.clientY - panStartRef.current.y;
        setViewport({
          ...initialViewportRef.current,
          x: initialViewportRef.current.x + dx,
          y: initialViewportRef.current.y + dy,
        });
      }
    },
    [viewport, connectingSourceId]
  );

  const handleMouseUp = useCallback(() => {
    isPanningRef.current = false;

    if (draggedNodeIdRef.current !== null) {
      draggedNodeIdRef.current = null;

      setNodes((currentNodes) => {
        const positionsMap: Record<number, Position> = {};
        currentNodes.forEach((n) => {
          positionsMap[n.id] = n.position;
        });
        savePositions(positionsMap);
        return currentNodes;
      });
    }
  }, []);

  const handleCancelConnecting = useCallback(() => {
    setConnectingSourceId(null);
    setConnectingSourceSide(null);
    setConnectingMousePos(null);
  }, []);

  const zoomIn = () => setViewport((v) => ({ ...v, zoom: Math.min(v.zoom + 0.15, 2.2) }));
  const zoomOut = () => setViewport((v) => ({ ...v, zoom: Math.max(v.zoom - 0.15, 0.4) }));
  const resetView = () => setViewport({ x: 120, y: 100, zoom: 1 });

  return {
    nodes,
    edges,
    viewport,
    selectedNodeId,
    setSelectedNodeId,
    connectingSourceId,
    connectingSourceSide,
    connectingMousePos,
    handleCanvasMouseDown,
    handleNodeMouseDown,
    handleStartConnecting,
    handleMouseMove,
    handleMouseUp,
    handleCancelConnecting,
    zoomIn,
    zoomOut,
    resetView,
    setViewport,
  };
}
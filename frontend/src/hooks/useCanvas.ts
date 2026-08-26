import { useState, useEffect, useCallback, useRef } from "react";
import { Nota } from "../types/nota";
import { Conexao } from "../types/conexao";
import { CanvasNode, CanvasEdge, CanvasViewport, Position } from "../types/canvas";

// Paleta Steel Blue & Slate Gray
const THEME_COLORS = [
  "#526D82", // Azul-aço médio (Principal)
  "#7E9CB3", // Azul-ardósia claro
  "#9DB2BF", // Cinza-azul suave
  "#6C8AA1", // Azul-aço profundo
  "#8FA7BC", // Névoa azulada
  "#5D7A91", // Grafite azulado
  "#ABC0CF", // Azul-gelo suave
  "#7493A8", // Azul-oceano fosco
];

export function getThemeColor(temaId: number): string {
  const index = Math.abs(temaId) % THEME_COLORS.length;
  return THEME_COLORS[index];
}

export function useCanvas(
  notas: Nota[],
  conexoes: Conexao[],
  onConnectNodes?: (sourceId: number, targetId: number) => void
) {
  const [nodes, setNodes] = useState<CanvasNode[]>([]);
  const [viewport, setViewport] = useState<CanvasViewport>({ x: 120, y: 100, zoom: 1 });
  const [selectedNodeId, setSelectedNodeId] = useState<number | null>(null);

  // Linha de conexão interativa (arraste para conectar)
  const [connectingSourceId, setConnectingSourceId] = useState<number | null>(null);
  const [connectingMousePos, setConnectingMousePos] = useState<Position | null>(null);

  const isPanningRef = useRef(false);
  const panStartRef = useRef<Position>({ x: 0, y: 0 });
  const initialViewportRef = useRef<CanvasViewport>({ x: 0, y: 0, zoom: 1 });

  const draggedNodeIdRef = useRef<number | null>(null);
  const dragStartMouseRef = useRef<Position>({ x: 0, y: 0 });
  const dragStartNodePosRef = useRef<Position>({ x: 0, y: 0 });

  // Sincroniza e distribui nós no canvas
  useEffect(() => {
    setNodes((prevNodes) => {
      const prevMap = new Map(prevNodes.map((n) => [n.id, n.position]));

      const temaGroups = new Map<number, Nota[]>();
      notas.forEach((n) => {
        const list = temaGroups.get(n.tema_id) || [];
        list.push(n);
        temaGroups.set(n.tema_id, list);
      });

      let groupIndex = 0;
      const calculatedNodes: CanvasNode[] = [];

      temaGroups.forEach((groupNotas, temaId) => {
        const groupCenterX = 260 + (groupIndex % 3) * 500;
        const groupCenterY = 220 + Math.floor(groupIndex / 3) * 460;
        const color = getThemeColor(temaId);

        groupNotas.forEach((nota, idx) => {
          let pos = prevMap.get(nota.id);
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

  // Converte conexões persistidas em arestas do Canvas
  const edges: CanvasEdge[] = conexoes.map((c) => ({
    id: `edge-${c.id}`,
    sourceId: c.nota_origem_id,
    targetId: c.nota_destino_id,
    color: "#526D82",
  }));

  // Pan no Canvas
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
        setConnectingMousePos(null);
        return;
      }
      isPanningRef.current = true;
      panStartRef.current = { x: e.clientX, y: e.clientY };
      initialViewportRef.current = { ...viewport };
    },
    [viewport, connectingSourceId]
  );

  // Arraste de Nó ou Seleção de Destino
  const handleNodeMouseDown = useCallback(
    (e: React.MouseEvent, nodeId: number) => {
      e.stopPropagation();
      setSelectedNodeId(nodeId);

      if (connectingSourceId !== null) {
        if (connectingSourceId !== nodeId && onConnectNodes) {
          onConnectNodes(connectingSourceId, nodeId);
        }
        setConnectingSourceId(null);
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
    [nodes, connectingSourceId, onConnectNodes]
  );

  // Iniciar criação de conexão a partir do handle
  const handleStartConnecting = useCallback(
    (e: React.MouseEvent, sourceNodeId: number) => {
      e.stopPropagation();
      setConnectingSourceId(sourceNodeId);
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
    draggedNodeIdRef.current = null;
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
    connectingMousePos,
    handleCanvasMouseDown,
    handleNodeMouseDown,
    handleStartConnecting,
    handleMouseMove,
    handleMouseUp,
    zoomIn,
    zoomOut,
    resetView,
    setViewport,
  };
}

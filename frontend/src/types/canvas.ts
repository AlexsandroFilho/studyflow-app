import { Nota } from "./nota";

export interface Position {
  x: number;
  y: number;
}

export interface CanvasNode {
  id: number;
  data: Nota;
  position: Position;
  color: string;
  isSelected?: boolean;
}

export interface CanvasEdge {
  id: string;
  sourceId: number;
  targetId: number;
  sourceSide?: "top" | "right" | "bottom" | "left";
  targetSide?: "top" | "right" | "bottom" | "left";
  color?: string;
}

export interface CanvasViewport {
  x: number;
  y: number;
  zoom: number;
}

export interface DragState {
  isDragging: boolean;
  nodeId: number | null;
  startPos: Position;
  initialNodePos: Position;
}

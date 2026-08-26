import React from "react";
import { ZoomIn, ZoomOut, Maximize2, Plus } from "lucide-react";

interface CanvasControlsProps {
  zoom: number;
  onZoomIn: () => void;
  onZoomOut: () => void;
  onResetView: () => void;
  onAddNoteAtCenter: () => void;
}

export const CanvasControls: React.FC<CanvasControlsProps> = ({
  zoom,
  onZoomIn,
  onZoomOut,
  onResetView,
  onAddNoteAtCenter,
}) => {
  return (
    <div className="absolute bottom-5 right-5 z-20 flex items-center gap-2 select-none">
      {/* Botão de nova nota — Azul-aço com hover em cinza-azul claro */}
      <button
        onClick={onAddNoteAtCenter}
        className="flex items-center gap-2 px-4 py-2 rounded-lg bg-[#526D82] text-[#DDE6ED] hover:bg-[#9DB2BF] hover:text-[#161B22] font-semibold text-xs border border-[#526D82] shadow-lg transition-all"
      >
        <Plus className="w-4 h-4 font-bold" />
        Nova Nota
      </button>

      {/* Controles de Zoom */}
      <div className="flex items-center bg-[#1C2430] border border-[#526D82] rounded-lg p-0.5 shadow-lg">
        <button
          onClick={onZoomOut}
          className="p-2 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#27374D] transition-colors"
          title="Reduzir"
        >
          <ZoomOut className="w-4 h-4" />
        </button>

        <span className="px-2 text-xs font-mono text-[#DDE6ED] min-w-[48px] text-center font-medium">
          {Math.round(zoom * 100)}%
        </span>

        <button
          onClick={onZoomIn}
          className="p-2 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#27374D] transition-colors"
          title="Ampliar"
        >
          <ZoomIn className="w-4 h-4" />
        </button>

        <div className="w-px h-4 bg-[#526D82] mx-0.5" />

        <button
          onClick={onResetView}
          className="p-2 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#27374D] transition-colors"
          title="Centralizar Visualização"
        >
          <Maximize2 className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
};

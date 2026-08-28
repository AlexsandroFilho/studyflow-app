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
      <button
        onClick={onAddNoteAtCenter}
        className="flex items-center gap-2 px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 font-semibold text-xs border border-blue-600 shadow-lg transition-all"
      >
        <Plus className="w-4 h-4 font-bold" />
        Nova Nota
      </button>

      {/* Controles de Zoom */}
      <div className="flex items-center bg-white border border-slate-200 rounded-lg p-0.5 shadow-lg">
        <button
          onClick={onZoomOut}
          className="p-2 rounded-md text-slate-500 hover:text-slate-800 hover:bg-slate-100 transition-colors"
          title="Reduzir"
        >
          <ZoomOut className="w-4 h-4" />
        </button>

        <span className="px-2 text-xs font-mono text-slate-700 min-w-[48px] text-center font-medium">
          {Math.round(zoom * 100)}%
        </span>

        <button
          onClick={onZoomIn}
          className="p-2 rounded-md text-slate-500 hover:text-slate-800 hover:bg-slate-100 transition-colors"
          title="Ampliar"
        >
          <ZoomIn className="w-4 h-4" />
        </button>

        <div className="w-px h-4 bg-slate-200 mx-0.5" />

        <button
          onClick={onResetView}
          className="p-2 rounded-md text-slate-500 hover:text-slate-800 hover:bg-slate-100 transition-colors"
          title="Centralizar Visualização"
        >
          <Maximize2 className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
};

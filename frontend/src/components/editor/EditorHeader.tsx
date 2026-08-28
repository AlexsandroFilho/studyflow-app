import React from "react";
import { ArrowLeft, CheckCircle2, RefreshCw, AlertCircle, Eye, Columns, Edit3, Trash2 } from "lucide-react";
import { Tema } from "../../types/tema";
import { SaveStatus } from "../../hooks/useAutoSave";

interface EditorHeaderProps {
  titulo: string;
  onTituloChange: (titulo: string) => void;
  temaId: number;
  onTemaChange: (temaId: number) => void;
  temas: Tema[];
  saveStatus: SaveStatus;
  editorMode: "split" | "edit" | "preview";
  setEditorMode: (mode: "split" | "edit" | "preview") => void;
  onBackToCanvas: () => void;
  onDeleteNota: () => void;
}

export const EditorHeader: React.FC<EditorHeaderProps> = ({
  titulo,
  onTituloChange,
  temaId,
  onTemaChange,
  temas,
  saveStatus,
  editorMode,
  setEditorMode,
  onBackToCanvas,
  onDeleteNota,
}) => {
  const renderSaveStatus = () => {
    switch (saveStatus) {
      case "saving":
        return (
          <span className="flex items-center gap-1.5 text-[11px] text-slate-500">
            <RefreshCw className="w-3 h-3 animate-spin" />
            Salvando...
          </span>
        );
      case "saved":
        return (
          <span className="flex items-center gap-1.5 text-[11px] text-slate-700">
            <CheckCircle2 className="w-3 h-3 text-blue-600" />
            Salvo
          </span>
        );
      case "error":
        return (
          <span className="flex items-center gap-1.5 text-[11px] text-red-400">
            <AlertCircle className="w-3 h-3" />
            Erro ao salvar
          </span>
        );
      default:
        return (
          <span className="text-[11px] text-slate-500">Modificado</span>
        );
    }
  };

  return (
    <div className="border-b border-slate-200 bg-white px-5 py-3 flex flex-col gap-3 shrink-0 shadow-sm">
      {/* Linha Superior */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <button
            onClick={onBackToCanvas}
            className="flex items-center gap-1.5 text-xs font-semibold text-slate-700 bg-white hover:bg-blue-50 px-3 py-1.5 rounded-lg transition-colors border border-slate-200"
          >
            <ArrowLeft className="w-3.5 h-3.5 text-blue-600" />
            Quadro Canvas
          </button>
          {renderSaveStatus()}
        </div>

        <div className="flex items-center gap-2">
          {/* Alternador de visualização */}
          <div className="flex items-center bg-slate-100 border border-slate-200 rounded-lg p-0.5 shadow-inner">
            {(["edit", "split", "preview"] as const).map((mode) => {
              const icons = {
                edit: <Edit3 className="w-3.5 h-3.5" />,
                split: <Columns className="w-3.5 h-3.5" />,
                preview: <Eye className="w-3.5 h-3.5" />,
              };
              const titles = { edit: "Edição", split: "Dividido", preview: "Leitura" };
              return (
                <button
                  key={mode}
                  onClick={() => setEditorMode(mode)}
                  className={`p-1.5 rounded-md transition-colors ${
                    editorMode === mode
                      ? "bg-white text-blue-700 font-bold shadow-sm"
                      : "text-slate-500 hover:text-slate-800 hover:bg-blue-50"
                  }`}
                  title={titles[mode]}
                >
                  {icons[mode]}
                </button>
              );
            })}
          </div>

          <button
            onClick={onDeleteNota}
            className="p-1.5 rounded-lg text-slate-500 hover:text-red-600 hover:bg-red-50 transition-colors"
            title="Excluir Nota"
          >
            <Trash2 className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      {/* Linha Inferior: Título + Seletor de Tema */}
      <div className="flex items-center gap-4">
        <input
          type="text"
          value={titulo}
          onChange={(e) => onTituloChange(e.target.value)}
          placeholder="Título da nota..."
          className="flex-1 bg-transparent text-lg font-bold text-slate-800 placeholder:text-slate-400 focus:outline-none"
        />

        <select
          value={temaId}
          onChange={(e) => onTemaChange(Number(e.target.value))}
          className="bg-slate-50 border border-slate-200 rounded-lg px-3 py-1.5 text-xs text-slate-800 focus:outline-none focus:border-blue-500 cursor-pointer"
        >
          {temas.map((t) => (
            <option key={t.id} value={t.id}>
              {t.nome}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
};

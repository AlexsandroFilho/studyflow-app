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
          <span className="flex items-center gap-1.5 text-[11px] text-[#9DB2BF]">
            <RefreshCw className="w-3 h-3 animate-spin" />
            Salvando...
          </span>
        );
      case "saved":
        return (
          <span className="flex items-center gap-1.5 text-[11px] text-[#DDE6ED]">
            <CheckCircle2 className="w-3 h-3 text-[#9DB2BF]" />
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
          <span className="text-[11px] text-[#9DB2BF]">Modificado</span>
        );
    }
  };

  return (
    <div className="border-b border-[#526D82]/50 bg-[#161B22] px-5 py-3 flex flex-col gap-3 shrink-0 shadow-sm">
      {/* Linha Superior */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <button
            onClick={onBackToCanvas}
            className="flex items-center gap-1.5 text-xs font-semibold text-[#DDE6ED] bg-[#27374D] hover:bg-[#526D82] px-3 py-1.5 rounded-lg transition-colors border border-[#526D82]"
          >
            <ArrowLeft className="w-3.5 h-3.5 text-[#9DB2BF]" />
            Quadro Canvas
          </button>
          {renderSaveStatus()}
        </div>

        <div className="flex items-center gap-2">
          {/* Alternador de visualização */}
          <div className="flex items-center bg-[#27374D] border border-[#526D82] rounded-lg p-0.5 shadow-inner">
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
                      ? "bg-[#526D82] text-[#DDE6ED] font-bold shadow-sm"
                      : "text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#31435D]"
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
            className="p-1.5 rounded-lg text-[#9DB2BF] hover:text-red-400 hover:bg-red-950/40 transition-colors"
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
          className="flex-1 bg-transparent text-lg font-bold text-[#DDE6ED] placeholder-[#526D82] focus:outline-none"
        />

        <select
          value={temaId}
          onChange={(e) => onTemaChange(Number(e.target.value))}
          className="bg-[#27374D] border border-[#526D82] rounded-lg px-3 py-1.5 text-xs text-[#DDE6ED] focus:outline-none focus:border-[#9DB2BF] cursor-pointer"
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

import React, { useState, useEffect, useCallback, useMemo, useRef } from "react";
import { Nota, NotaUpdateDto } from "../../types/nota";
import { Tema } from "../../types/tema";
import { EditorHeader } from "./EditorHeader";
import { MarkdownPreview } from "./MarkdownPreview";
import { useAutoSave } from "../../hooks/useAutoSave";
import { Bold, Italic, List, Heading, Quote, Code, Sparkles, Loader2 } from "lucide-react";
import { revisaoService } from "../../services/revisaoService";
import { RevisaoNota } from "../../types/revisao";
import { ReviewPanel } from "./ReviewPanel";

function obterMensagemErroRevisao(error: any): string {
  const mensagem = error?.response?.data?.message || error?.message || "";
  const excedeuCota =
    mensagem.includes("429") ||
    mensagem.includes("RESOURCE_EXHAUSTED") ||
    mensagem.toLowerCase().includes("quota exceeded");

  return excedeuCota
    ? "O limite temporário da IA foi atingido. Tente novamente mais tarde."
    : "Não foi possível revisar esta nota agora. Tente novamente em alguns instantes.";
}

interface ObsidianEditorProps {
  nota: Nota;
  temas: Tema[];
  onUpdateNota: (id: number, dto: NotaUpdateDto) => Promise<void>;
  onDeleteNota: (nota: Nota) => void;
  onBackToCanvas: () => void;
}

export const ObsidianEditor: React.FC<ObsidianEditorProps> = ({
  nota,
  temas,
  onUpdateNota,
  onDeleteNota,
  onBackToCanvas,
}) => {
  const [titulo, setTitulo] = useState(nota.titulo);
  const [conteudo, setConteudo] = useState(nota.conteudo);
  const [temaId, setTemaId] = useState(nota.temaId);
  const [editorMode, setEditorMode] = useState<"split" | "edit" | "preview">("split");
  const [revisao, setRevisao] = useState<RevisaoNota | null>(null);
  const [revisando, setRevisando] = useState(false);
  const [erroRevisao, setErroRevisao] = useState<string | null>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    setTitulo(nota.titulo);
    setConteudo(nota.conteudo);
    setTemaId(nota.temaId);
  }, [nota.id]);

  const handleAutoSave = useCallback(
    async (val: { titulo: string; conteudo: string; tema_id: number | null }) => {
      if (!val.titulo.trim() || !val.conteudo.trim()) return;
      await onUpdateNota(nota.id, {
        titulo: val.titulo,
        conteudo: val.conteudo,
        temaId: val.tema_id,
      });
    },
    [nota.id, onUpdateNota]
  );

  const dadosParaSalvar = useMemo(
    () => ({ titulo, conteudo, tema_id: temaId }),
    [titulo, conteudo, temaId]
  );

  const { status: saveStatus } = useAutoSave(dadosParaSalvar, handleAutoSave, 700);

  const insertMarkdown = (prefix: string, suffix = "") => {
    const textarea = textareaRef.current;
    if (!textarea) return;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selected = conteudo.substring(start, end);
    const replacement = `${prefix}${selected || "texto"}${suffix}`;
    setConteudo(conteudo.substring(0, start) + replacement + conteudo.substring(end));
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(start + prefix.length, start + prefix.length + (selected.length || 5));
    }, 10);
  };

  const wordCount = conteudo.trim() ? conteudo.trim().split(/\s+/).length : 0;

  const handleReview = async () => {
    if (!titulo.trim() || !conteudo.trim()) return;
    setRevisando(true);
    setErroRevisao(null);
    try {
      await onUpdateNota(nota.id, { titulo, conteudo, temaId });
      setRevisao(await revisaoService.criar(nota.id));
    } catch (error: any) {
      setErroRevisao(obterMensagemErroRevisao(error));
    } finally {
      setRevisando(false);
    }
  };

  const toolbarItems = [
    { icon: <Heading className="w-3.5 h-3.5" />, action: () => insertMarkdown("## "), title: "Título" },
    { icon: <Bold className="w-3.5 h-3.5" />, action: () => insertMarkdown("**", "**"), title: "Negrito" },
    { icon: <Italic className="w-3.5 h-3.5" />, action: () => insertMarkdown("*", "*"), title: "Itálico" },
    { icon: <List className="w-3.5 h-3.5" />, action: () => insertMarkdown("- "), title: "Lista" },
    { icon: <Quote className="w-3.5 h-3.5" />, action: () => insertMarkdown("> "), title: "Citação" },
    { icon: <Code className="w-3.5 h-3.5" />, action: () => insertMarkdown("`", "`"), title: "Código" },
  ];

  return (
    <div className="flex-1 flex flex-col h-full bg-white overflow-hidden">
      <EditorHeader
        titulo={titulo}
        onTituloChange={setTitulo}
        temaId={temaId}
        onTemaChange={setTemaId}
        temas={temas}
        saveStatus={saveStatus}
        editorMode={editorMode}
        setEditorMode={setEditorMode}
        onBackToCanvas={onBackToCanvas}
        onDeleteNota={() => onDeleteNota(nota)}
      />

      {/* Toolbar de Formatação */}
      <div className="px-5 py-1.5 border-b border-slate-200 bg-slate-50 flex items-center gap-0.5">
        {toolbarItems.map((item, i) => (
          <button
            key={i}
            onClick={item.action}
            className="p-1.5 rounded text-slate-500 hover:text-blue-700 hover:bg-blue-50 transition-colors"
            title={item.title}
          >
            {item.icon}
          </button>
        ))}
        <div className="ml-auto flex items-center gap-2">
          {erroRevisao && <span className="text-xs text-red-600">{erroRevisao}</span>}
          <button onClick={handleReview} disabled={revisando || !conteudo.trim()} className="flex items-center gap-1.5 rounded-md bg-blue-600 px-2.5 py-1.5 text-xs font-semibold text-white hover:bg-blue-700 disabled:opacity-60" title="Revisar nota com IA">
            {revisando ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Sparkles className="w-3.5 h-3.5" />}
            {revisando ? "Revisando..." : "Revisar com IA"}
          </button>
        </div>
      </div>

      {/* Área de Edição / Visualização */}
      <div className="flex-1 flex overflow-hidden">
        {(editorMode === "edit" || editorMode === "split") && (
          <div
            className={`flex-1 flex flex-col overflow-hidden ${
              editorMode === "split" ? "border-r border-slate-200" : ""
            }`}
          >
            <textarea
              ref={textareaRef}
              value={conteudo}
              onChange={(e) => setConteudo(e.target.value)}
              placeholder="Comece a escrever suas anotações em Markdown..."
              className="flex-1 w-full bg-transparent px-6 py-5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none resize-none font-mono leading-relaxed select-text"
            />
          </div>
        )}

        {(editorMode === "preview" || editorMode === "split") && (
          <div className="flex-1 overflow-y-auto px-6 py-5 bg-white">
            <MarkdownPreview content={conteudo} />
          </div>
        )}
        {revisao && <ReviewPanel revisao={revisao} onClose={() => setRevisao(null)} />}
      </div>

      {/* Rodapé */}
      <div className="h-7 border-t border-slate-200 bg-slate-50 px-5 flex items-center justify-between text-[11px] text-slate-500 font-mono shrink-0">
        <span>Nota #{nota.id}</span>
        <span>
          {wordCount} palavras · {conteudo.length} caracteres
        </span>
      </div>
    </div>
  );
};

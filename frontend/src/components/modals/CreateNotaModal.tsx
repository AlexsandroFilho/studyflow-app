import React, { useState, useEffect } from "react";
import { Modal } from "../ui/Modal";
import { Input } from "../ui/Input";
import { Textarea } from "../ui/Textarea";
import { Button } from "../ui/Button";
import { Tema } from "../../types/tema";
import { Nota, NotaRequestDto, NotaUpdateDto } from "../../types/nota";
import { NotaConectadaRequestDto } from "../../types/conexao";
import { Link2 } from "lucide-react";

interface CreateNotaModalProps {
  isOpen: boolean;
  onClose: () => void;
  temas: Tema[];
  selectedTemaId: number | null;
  editingNota?: Nota | null;
  connectedSourceNota?: Nota | null;
  onSubmitCreate: (dto: NotaRequestDto) => Promise<void>;
  onSubmitUpdate: (id: number, dto: NotaUpdateDto) => Promise<void>;
  onSubmitCreateConnected?: (dto: NotaConectadaRequestDto) => Promise<void>;
}

export const CreateNotaModal: React.FC<CreateNotaModalProps> = ({
  isOpen,
  onClose,
  temas,
  selectedTemaId,
  editingNota,
  connectedSourceNota,
  onSubmitCreate,
  onSubmitUpdate,
  onSubmitCreateConnected,
}) => {
  const [titulo, setTitulo] = useState("");
  const [conteudo, setConteudo] = useState("");
  const [temaId, setTemaId] = useState<number>(1);
  const [rotuloConexao, setRotuloConexao] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (editingNota) {
      setTitulo(editingNota.titulo);
      setConteudo(editingNota.conteudo);
      setTemaId(editingNota.temaId);
    } else if (connectedSourceNota) {
      setTitulo("");
      setConteudo("");
      setTemaId(connectedSourceNota.temaId);
      setRotuloConexao("");
    } else {
      setTitulo("");
      setConteudo("");
      setTemaId(selectedTemaId || (temas.length > 0 ? temas[0].id : 1));
      setRotuloConexao("");
    }
    setError(null);
  }, [editingNota, connectedSourceNota, isOpen, selectedTemaId, temas]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!titulo.trim() || titulo.trim().length < 3) {
      setError("O título deve ter pelo menos 3 caracteres.");
      return;
    }
    if (!conteudo.trim()) {
      setError("O conteúdo não pode estar vazio.");
      return;
    }

    setLoading(true);
    setError(null);
    try {
      if (editingNota) {
        await onSubmitUpdate(editingNota.id, {
          titulo: titulo.trim(),
          conteudo: conteudo.trim(),
          
        });
      } else if (connectedSourceNota && onSubmitCreateConnected) {
        await onSubmitCreateConnected({
          notaOrigemId: connectedSourceNota.id,
          titulo: titulo.trim(),
          conteudo: conteudo.trim(),
          temaId: temaId,
          rotulo: rotuloConexao.trim() || null,
        });
      } else {
        await onSubmitCreate({
          titulo: titulo.trim(),
          conteudo: conteudo.trim(),
          temaId: temaId,
        });
      }
      onClose();
    } catch (err: any) {
      setError(err.message || "Erro ao salvar nota.");
    } finally {
      setLoading(false);
    }
  };

  const modalTitle = editingNota
    ? "Editar Nota"
    : connectedSourceNota
    ? `Nova Nota Conectada a "${connectedSourceNota.titulo}"`
    : "Criar Nova Nota";

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={modalTitle}
      subtitle={
        connectedSourceNota
          ? "Esta nota será criada e vinculada automaticamente no seu Mapa Mental."
          : "Defina o título, tema e conteúdo da nota."
      }
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && (
          <div className="p-3 rounded-lg bg-red-950/40 border border-red-800/60 text-xs text-red-300">
            {error}
          </div>
        )}

        {connectedSourceNota && (
          <div className="flex items-center gap-2 p-2.5 rounded-lg bg-[#161B22] border border-[#526D82] text-xs text-[#DDE6ED]">
            <Link2 className="w-4 h-4 text-[#9DB2BF]" />
            <span>
              Origem: <strong>{connectedSourceNota.titulo}</strong>
            </span>
          </div>
        )}

        <Input
          label="Título da Nota"
          value={titulo}
          onChange={(e) => setTitulo(e.target.value)}
          placeholder="Ex: Teoria de Grafos e Algoritmos"
          required
        />

        <div className="w-full space-y-1.5">
          <label className="block text-[11px] font-semibold uppercase tracking-wider text-[#9DB2BF]">
            Tema de Estudo
          </label>
          <select
            value={temaId}
            onChange={(e) => setTemaId(Number(e.target.value))}
            className="w-full rounded-lg bg-[#161B22] border border-[#526D82] px-3.5 py-2 text-sm text-[#DDE6ED] focus:border-[#9DB2BF] focus:outline-none transition-colors cursor-pointer"
          >
            {temas.map((t) => (
              <option key={t.id} value={t.id}>
                {t.nome}
              </option>
            ))}
          </select>
        </div>

        {connectedSourceNota && (
          <Input
            label="Rótulo da Conexão (Opcional)"
            value={rotuloConexao}
            onChange={(e) => setRotuloConexao(e.target.value)}
            placeholder="Ex: é pré-requisito de, deriva de, exemplo prático..."
          />
        )}

        <Textarea
          label="Conteúdo Inicial (Markdown Suportado)"
          value={conteudo}
          onChange={(e) => setConteudo(e.target.value)}
          placeholder="Escreva suas anotações, sínteses ou referências..."
          rows={5}
          required
        />

        <div className="flex items-center justify-end gap-2 pt-3 border-t border-[#526D82]/50">
          <Button variant="ghost" size="sm" type="button" onClick={onClose}>
            Cancelar
          </Button>
          <Button variant="primary" size="sm" type="submit" isLoading={loading}>
            {editingNota
              ? "Salvar Alterações"
              : connectedSourceNota
              ? "Criar e Conectar"
              : "Criar Nota"}
          </Button>
        </div>
      </form>
    </Modal>
  );
};

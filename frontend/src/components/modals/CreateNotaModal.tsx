import React, { useState, useEffect } from "react";
import { Modal } from "../ui/Modal";
import { Input } from "../ui/Input";
import { Textarea } from "../ui/Textarea";
import { Button } from "../ui/Button";
import { Tema } from "../../types/tema";
import { Nota, NotaRequestDto, NotaUpdateDto } from "../../types/nota";
import { getApiErrorMessage } from "../../utils/apiError";

interface CreateNotaModalProps {
  isOpen: boolean;
  onClose: () => void;
  temas: Tema[];
  editingNota?: Nota | null;
  onSubmitCreate: (dto: NotaRequestDto) => Promise<void>;
  onSubmitUpdate: (id: number, dto: NotaUpdateDto) => Promise<void>;
}

export const CreateNotaModal: React.FC<CreateNotaModalProps> = ({
  isOpen,
  onClose,
  temas,
  editingNota,
  onSubmitCreate,
  onSubmitUpdate,
}) => {
  const [titulo, setTitulo] = useState("");
  const [conteudo, setConteudo] = useState("");
  const [temaId, setTemaId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (editingNota) {
      setTitulo(editingNota.titulo);
      setConteudo(editingNota.conteudo);
      setTemaId(editingNota.temaId);
    } else {
      setTitulo("");
      setConteudo("");
      setTemaId(null);
    }
    setError(null);
  }, [editingNota, isOpen]);

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
          temaId,
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
      setError(getApiErrorMessage(err, "Erro ao salvar nota."));
    } finally {
      setLoading(false);
    }
  };

  const modalTitle = editingNota
    ? "Editar Nota"
    : "Criar Nova Nota";

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={modalTitle}
      subtitle={
        "Defina o título, tema e conteúdo da nota."
      }
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && (
          <div className="p-3 rounded-lg bg-red-50 border border-red-200 text-xs text-red-600">
            {error}
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
          <label className="block text-[11px] font-semibold uppercase tracking-wider text-slate-500">
            Tema de Estudo
          </label>
          <select
            value={temaId ?? ""}
            onChange={(e) => setTemaId(e.target.value ? Number(e.target.value) : null)}
            className="w-full rounded-lg bg-slate-50 border border-slate-200 px-3.5 py-2 text-sm text-slate-800 focus:border-blue-500 focus:outline-none transition-colors cursor-pointer"
          >
            <option value="">Sem tema</option>
            {temas.map((t) => (
              <option key={t.id} value={t.id}>
                {t.nome}
              </option>
            ))}
          </select>
        </div>

        <Textarea
          label="Conteúdo Inicial (Markdown Suportado)"
          value={conteudo}
          onChange={(e) => setConteudo(e.target.value)}
          placeholder="Escreva suas anotações, sínteses ou referências..."
          rows={5}
          required
        />

        <div className="flex items-center justify-end gap-2 pt-3 border-t border-slate-200">
          <Button variant="ghost" size="sm" type="button" onClick={onClose}>
            Cancelar
          </Button>
          <Button variant="primary" size="sm" type="submit" isLoading={loading}>
            {editingNota
              ? "Salvar Alterações"
              : "Criar Nota"}
          </Button>
        </div>
      </form>
    </Modal>
  );
};

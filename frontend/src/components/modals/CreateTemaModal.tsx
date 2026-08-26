import React, { useState, useEffect } from "react";
import { Modal } from "../ui/Modal";
import { Input } from "../ui/Input";
import { Textarea } from "../ui/Textarea";
import { Button } from "../ui/Button";
import { Tema, TemaRequestDto } from "../../types/tema";

interface CreateTemaModalProps {
  isOpen: boolean;
  onClose: () => void;
  editingTema?: Tema | null;
  onSubmitCreate: (dto: TemaRequestDto) => Promise<void>;
  onSubmitUpdate: (id: number, dto: TemaRequestDto) => Promise<void>;
}

export const CreateTemaModal: React.FC<CreateTemaModalProps> = ({
  isOpen,
  onClose,
  editingTema,
  onSubmitCreate,
  onSubmitUpdate,
}) => {
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (editingTema) {
      setNome(editingTema.nome);
      setDescricao(editingTema.descricao || "");
    } else {
      setNome("");
      setDescricao("");
    }
    setError(null);
  }, [editingTema, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nome.trim() || nome.trim().length < 3) {
      setError("O nome do tema deve ter pelo menos 3 caracteres.");
      return;
    }

    setLoading(true);
    setError(null);
    try {
      if (editingTema) {
        await onSubmitUpdate(editingTema.id, {
          nome: nome.trim(),
          descricao: descricao.trim() || null,
        });
      } else {
        await onSubmitCreate({
          nome: nome.trim(),
          descricao: descricao.trim() || null,
        });
      }
      onClose();
    } catch (err: any) {
      setError(err.message || "Erro ao salvar tema.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={editingTema ? "Editar Tema de Estudo" : "Criar Novo Tema"}
      subtitle="Temas organizam suas notas e definem as cores no seu Mapa Mental."
      maxWidth="md"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && (
          <div className="p-3 rounded-lg bg-red-950/40 border border-red-800/60 text-xs text-red-300">
            {error}
          </div>
        )}

        <Input
          label="Nome do Tema"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          placeholder="Ex: Engenharia de Software e Grafos"
          required
        />

        <Textarea
          label="Descrição (Opcional)"
          value={descricao}
          onChange={(e) => setDescricao(e.target.value)}
          placeholder="Breve resumo sobre este tópico de estudos..."
          rows={3}
        />

        <div className="flex items-center justify-end gap-2 pt-3 border-t border-[#526D82]/50">
          <Button variant="ghost" size="sm" type="button" onClick={onClose}>
            Cancelar
          </Button>
          <Button variant="primary" size="sm" type="submit" isLoading={loading}>
            {editingTema ? "Salvar Tema" : "Criar Tema"}
          </Button>
        </div>
      </form>
    </Modal>
  );
};

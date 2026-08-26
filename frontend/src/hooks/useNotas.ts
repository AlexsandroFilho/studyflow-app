import { useState, useEffect, useCallback } from "react";
import { Nota, NotaRequestDto, NotaUpdateDto } from "../types/nota";
import { notaService } from "../services/notaService";

export function useNotas(selectedTemaId: number | null) {
  const [notas, setNotas] = useState<Nota[]>([]);
  const [activeNota, setActiveNota] = useState<Nota | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const carregarNotas = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      if (selectedTemaId) {
        const data = await notaService.listarPorTema(selectedTemaId);
        setNotas(data);
      } else {
        const data = await notaService.listarTodas();
        setNotas(data);
      }
    } catch (err: any) {
      setError(err.response?.data?.mensagem || err.message || "Erro ao carregar notas.");
    } finally {
      setLoading(false);
    }
  }, [selectedTemaId]);

  useEffect(() => {
    carregarNotas();
  }, [carregarNotas]);

  const criarNota = async (dto: NotaRequestDto): Promise<Nota> => {
    setError(null);
    try {
      const nova = await notaService.criar(dto);
      setNotas((prev) => [nova, ...prev]);
      return nova;
    } catch (err: any) {
      const msg = err.response?.data?.mensagem || err.message || "Erro ao criar nota.";
      setError(msg);
      throw new Error(msg);
    }
  };

  const atualizarNota = async (id: number, dto: NotaUpdateDto): Promise<void> => {
    setError(null);
    try {
      await notaService.atualizar(id, dto);
      setNotas((prev) =>
        prev.map((n) => (n.id === id ? { ...n, ...dto } : n))
      );
      if (activeNota && activeNota.id === id) {
        setActiveNota((prev) => (prev ? { ...prev, ...dto } : null));
      }
    } catch (err: any) {
      const msg = err.response?.data?.mensagem || err.message || "Erro ao atualizar nota.";
      setError(msg);
      throw new Error(msg);
    }
  };

  const deletarNota = async (id: number): Promise<void> => {
    setError(null);
    try {
      await notaService.deletar(id);
      setNotas((prev) => prev.filter((n) => n.id !== id));
      if (activeNota && activeNota.id === id) {
        setActiveNota(null);
      }
    } catch (err: any) {
      const msg = err.response?.data?.mensagem || err.message || "Erro ao deletar nota.";
      setError(msg);
      throw new Error(msg);
    }
  };

  return {
    notas,
    activeNota,
    setActiveNota,
    loading,
    error,
    carregarNotas,
    criarNota,
    atualizarNota,
    deletarNota,
  };
}
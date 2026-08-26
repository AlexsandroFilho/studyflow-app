import { useState, useEffect, useCallback } from "react";
import { Tema, TemaRequestDto } from "../types/tema";
import { temaService } from "../services/temaService";

export function useTemas() {
  const [temas, setTemas] = useState<Tema[]>([]);
  const [selectedTemaId, setSelectedTemaId] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const carregarTemas = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await temaService.listar();
      setTemas(data);
    } catch (err: any) {
      setError(err.message || "Erro ao carregar temas.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregarTemas();
  }, [carregarTemas]);

  const criarTema = async (dto: TemaRequestDto): Promise<Tema> => {
    setError(null);
    try {
      const novo = await temaService.criar(dto);
      setTemas((prev) => [...prev, novo]);
      return novo;
    } catch (err: any) {
      setError(err.message || "Erro ao criar tema.");
      throw err;
    }
  };

  const atualizarTema = async (id: number, dto: TemaRequestDto): Promise<Tema> => {
    setError(null);
    try {
      const atualizado = await temaService.atualizar(id, dto);
      setTemas((prev) => prev.map((t) => (t.id === id ? atualizado : t)));
      return atualizado;
    } catch (err: any) {
      setError(err.message || "Erro ao atualizar tema.");
      throw err;
    }
  };

  const deletarTema = async (id: number): Promise<void> => {
    setError(null);
    try {
      await temaService.deletar(id);
      setTemas((prev) => prev.filter((t) => t.id !== id));
      if (selectedTemaId === id) {
        setSelectedTemaId(null);
      }
    } catch (err: any) {
      setError(err.message || "Erro ao deletar tema.");
      throw err;
    }
  };

  return {
    temas,
    selectedTemaId,
    setSelectedTemaId,
    loading,
    error,
    carregarTemas,
    criarTema,
    atualizarTema,
    deletarTema,
  };
}

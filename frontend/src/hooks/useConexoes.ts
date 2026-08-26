import { useState, useEffect, useCallback } from "react";
import { Conexao, ConexaoRequestDto } from "../types/conexao";
import { conexaoService } from "../services/conexaoService";

export function useConexoes(selectedTemaId: number | null) {
  const [conexoes, setConexoes] = useState<Conexao[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const carregarConexoes = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await conexaoService.listar(selectedTemaId);
      setConexoes(data);
    } catch (err: any) {
      setError(err.message || "Erro ao carregar conexões.");
    } finally {
      setLoading(false);
    }
  }, [selectedTemaId]);

  useEffect(() => {
    carregarConexoes();
  }, [carregarConexoes]);

  const conectar = async (dto: ConexaoRequestDto): Promise<Conexao> => {
    setError(null);
    try {
      const nova = await conexaoService.conectar(dto);
      setConexoes((prev) => {
        if (prev.some((c) => c.id === nova.id)) return prev;
        return [...prev, nova];
      });
      return nova;
    } catch (err: any) {
      setError(err.message || "Erro ao conectar notas.");
      throw err;
    }
  };

  const desconectarPorId = async (conexaoId: number): Promise<void> => {
    setError(null);
    try {
      await conexaoService.desconectarPorId(conexaoId);
      setConexoes((prev) => prev.filter((c) => c.id !== conexaoId));
    } catch (err: any) {
      setError(err.message || "Erro ao desconectar notas.");
      throw err;
    }
  };

  const desconectarPorPar = async (origemId: number, destinoId: number): Promise<void> => {
    setError(null);
    try {
      await conexaoService.desconectarPorPar(origemId, destinoId);
      setConexoes((prev) =>
        prev.filter(
          (c) =>
            !(
              (c.notaOrigemId === origemId && c.notaDestinoId === destinoId) ||
              (c.notaOrigemId === destinoId && c.notaDestinoId === origemId)
            )
        )
      );
    } catch (err: any) {
      setError(err.message || "Erro ao desconectar notas.");
      throw err;
    }
  };

  return {
    conexoes,
    loading,
    error,
    carregarConexoes,
    conectar,
    desconectarPorId,
    desconectarPorPar,
  };
}

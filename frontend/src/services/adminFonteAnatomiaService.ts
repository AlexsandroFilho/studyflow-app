import { api } from "./api";
import { CriarIngestaoFonteAnatomiaForm, IngestaoFonteAnatomia } from "../types/ingestaoFonteAnatomia";

const endpoint = "/admin/fontes-anatomia/ingestoes";

export const adminFonteAnatomiaService = {
  async listar(): Promise<IngestaoFonteAnatomia[]> {
    const response = await api.get<IngestaoFonteAnatomia[]>(endpoint);
    return response.data;
  },

  async criar(dados: CriarIngestaoFonteAnatomiaForm): Promise<IngestaoFonteAnatomia> {
    const formData = new FormData();
    formData.append("arquivo", dados.arquivo);
    formData.append("titulo", dados.titulo);
    formData.append("autor", dados.autor ?? "");
    formData.append("versao", dados.versao);
    formData.append("assunto", dados.assunto);
    formData.append("subassunto", dados.subassunto ?? "");
    const response = await api.post<IngestaoFonteAnatomia>(endpoint, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },

  async reprocessar(id: string): Promise<IngestaoFonteAnatomia> {
    const response = await api.post<IngestaoFonteAnatomia>(`${endpoint}/${id}/reprocessar`);
    return response.data;
  },
};

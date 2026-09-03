import { ChangeEvent, FormEvent, useCallback, useEffect, useState } from "react";
import { ArrowLeft, FileUp, Loader2, RefreshCw, ShieldCheck } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { adminFonteAnatomiaService } from "../../services/adminFonteAnatomiaService";
import { IngestaoFonteAnatomia } from "../../types/ingestaoFonteAnatomia";
import { getApiErrorMessage } from "../../utils/apiError";

const statusLabels = { pendente: "Na fila", processando: "Processando", concluida: "Concluída", falhou: "Falhou" } as const;
const statusClasses = { pendente: "bg-amber-50 text-amber-700 border-amber-200", processando: "bg-blue-50 text-blue-700 border-blue-200", concluida: "bg-emerald-50 text-emerald-700 border-emerald-200", falhou: "bg-red-50 text-red-700 border-red-200" } as const;

export function AdminFontesPage() {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const [historico, setHistorico] = useState<IngestaoFonteAnatomia[]>([]);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [titulo, setTitulo] = useState("");
  const [autor, setAutor] = useState("");
  const [versao, setVersao] = useState("");
  const [assunto, setAssunto] = useState("Anatomia Humana");
  const [subassunto, setSubassunto] = useState("");
  const [enviando, setEnviando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  const carregar = useCallback(async () => {
    try { setHistorico(await adminFonteAnatomiaService.listar()); }
    catch (error) { setErro(getApiErrorMessage(error, "Não foi possível carregar o histórico.")); }
  }, []);

  useEffect(() => { carregar(); }, [carregar]);
  useEffect(() => {
    if (!historico.some(item => item.status === "pendente" || item.status === "processando")) return;
    const id = window.setInterval(carregar, 8000);
    return () => window.clearInterval(id);
  }, [historico, carregar]);

  const selecionarArquivo = (event: ChangeEvent<HTMLInputElement>) => {
    const selecionado = event.target.files?.[0] ?? null;
    if (selecionado && selecionado.size > 25 * 1024 * 1024) {
      setErro("O PDF deve ter no máximo 25 MB.");
      event.target.value = "";
      return;
    }
    setArquivo(selecionado);
  };

  const enviar = async (event: FormEvent) => {
    event.preventDefault();
    if (!arquivo) { setErro("Selecione um PDF pesquisável."); return; }
    setEnviando(true); setErro(null);
    try {
      const nova = await adminFonteAnatomiaService.criar({ arquivo, titulo, autor, versao, assunto, subassunto });
      setHistorico(anteriores => [nova, ...anteriores]);
      setArquivo(null); setTitulo(""); setAutor(""); setVersao(""); setSubassunto("");
    } catch (error) { setErro(getApiErrorMessage(error, "Não foi possível enviar o PDF.")); }
    finally { setEnviando(false); }
  };

  const reprocessar = async (id: string) => {
    setErro(null);
    try {
      const atualizada = await adminFonteAnatomiaService.reprocessar(id);
      setHistorico(anteriores => anteriores.map(item => item.id === id ? atualizada : item));
    } catch (error) { setErro(getApiErrorMessage(error, "Não foi possível reenfileirar a ingestão.")); }
  };

  return <main className="min-h-screen bg-slate-50 text-slate-800">
    <header className="flex h-14 items-center justify-between border-b border-slate-200 bg-white px-5 shadow-sm">
      <button onClick={() => navigate("/")} className="flex items-center gap-2 text-sm font-semibold text-slate-700 hover:text-blue-700"><ArrowLeft className="h-4 w-4" /> Voltar ao StudyFlow</button>
      <button onClick={logout} className="text-xs font-medium text-slate-500 hover:text-red-600">Sair</button>
    </header>
    <div className="mx-auto max-w-5xl px-5 py-8">
      <div className="mb-7 flex items-start gap-3"><div className="rounded-xl bg-blue-600 p-3 text-white"><ShieldCheck className="h-6 w-6" /></div><div><h1 className="text-2xl font-bold">Administração do acervo</h1><p className="mt-1 text-sm text-slate-500">Envie PDFs licenciados para ampliar o contexto oficial usado pela IA.</p></div></div>
      <section className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm"><h2 className="text-base font-bold">Nova fonte de Anatomia</h2><p className="mt-1 text-xs text-slate-500">A indexação ocorre em segundo plano. O PDF deve ter texto selecionável e até 25 MB.</p>
        <form onSubmit={enviar} className="mt-5 grid gap-4 md:grid-cols-2">
          <label className="md:col-span-2 rounded-lg border border-dashed border-slate-300 bg-slate-50 p-4 text-sm text-slate-600 cursor-pointer hover:border-blue-400"><span className="flex items-center gap-2 font-medium"><FileUp className="h-4 w-4 text-blue-600" />{arquivo ? arquivo.name : "Selecionar PDF"}</span><input className="mt-3 block w-full text-xs" type="file" accept="application/pdf,.pdf" onChange={selecionarArquivo} required /></label>
          <label className="text-xs font-semibold text-slate-600">Título<input value={titulo} onChange={e => setTitulo(e.target.value)} className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm" required /></label>
          <label className="text-xs font-semibold text-slate-600">Versão<input value={versao} onChange={e => setVersao(e.target.value)} className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm" required /></label>
          <label className="text-xs font-semibold text-slate-600">Assunto<input value={assunto} onChange={e => setAssunto(e.target.value)} className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm" required /></label>
          <label className="text-xs font-semibold text-slate-600">Subassunto <span className="font-normal">(opcional)</span><input value={subassunto} onChange={e => setSubassunto(e.target.value)} className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm" /></label>
          <label className="text-xs font-semibold text-slate-600 md:col-span-2">Autor <span className="font-normal">(opcional)</span><input value={autor} onChange={e => setAutor(e.target.value)} className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm" /></label>
          {erro && <p className="md:col-span-2 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{erro}</p>}
          <div className="md:col-span-2 flex justify-end"><button disabled={enviando} className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white disabled:opacity-60">{enviando && <Loader2 className="h-4 w-4 animate-spin" />}{enviando ? "Enviando..." : "Enviar para indexação"}</button></div>
        </form>
      </section>
      <section className="mt-6 rounded-xl border border-slate-200 bg-white shadow-sm"><div className="flex items-center justify-between border-b border-slate-200 px-5 py-4"><div><h2 className="font-bold">Histórico de ingestões</h2><p className="text-xs text-slate-500">Os status em andamento são atualizados automaticamente.</p></div><button onClick={carregar} title="Atualizar" className="rounded-lg p-2 text-slate-500 hover:bg-slate-100"><RefreshCw className="h-4 w-4" /></button></div>
        <div className="divide-y divide-slate-100">{historico.length === 0 ? <p className="p-6 text-sm text-slate-500">Nenhum PDF enviado ainda.</p> : historico.map(item => <div key={item.id} className="p-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h3 className="font-semibold">{item.titulo}</h3><p className="mt-1 text-xs text-slate-500">{item.assunto} · versão {item.versao} · {new Date(item.dataCriacao).toLocaleString("pt-BR")}</p></div><span className={`rounded-full border px-2.5 py-1 text-xs font-semibold ${statusClasses[item.status]}`}>{statusLabels[item.status]}</span></div>{item.status === "concluida" && <p className="mt-3 text-sm text-emerald-700">Fonte publicada com {item.quantidadeChunks} chunks.</p>}{item.mensagemErro && <p className="mt-3 text-sm text-red-700">{item.mensagemErro}</p>}{item.status === "falhou" && <button onClick={() => reprocessar(item.id)} className="mt-3 rounded-lg border border-blue-200 px-3 py-1.5 text-xs font-semibold text-blue-700 hover:bg-blue-50">Tentar novamente</button>}</div>)}</div>
      </section>
    </div>
  </main>;
}

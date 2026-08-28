import React, { FormEvent, useState } from "react";
import { Network, ArrowRight, LoaderCircle } from "lucide-react";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

export const LoginPage: React.FC = () => {
  const [mode, setMode] = useState<"login" | "register">("login");
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { login, register } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const destination = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname || "/";

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);
    try {
      if (mode === "login") await login(email.trim(), senha);
      else await register(nome.trim(), email.trim(), senha);
      navigate(destination, { replace: true });
    } catch (requestError: any) {
      setError(requestError.response?.data?.mensagem || "Não foi possível concluir a autenticação.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <main className="min-h-screen flex items-center justify-center bg-slate-100 px-4 py-8">
      <section className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-7 shadow-xl shadow-slate-300/40">
        <div className="flex flex-col items-center text-center">
          <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-blue-600 shadow-sm">
            <Network className="h-6 w-6 text-white" />
          </div>
          <h1 className="text-2xl font-bold tracking-tight text-slate-800">Study<span className="text-blue-600">Flow</span></h1>
          <p className="mt-1 text-sm text-slate-500">Seu espaço para organizar ideias e aprendizados.</p>
        </div>

        <div className="mt-7 grid grid-cols-2 rounded-lg bg-slate-100 p-1">
          {(["login", "register"] as const).map((tab) => (
            <button key={tab} type="button" onClick={() => { setMode(tab); setError(""); }} className={`rounded-md py-2 text-sm font-semibold transition-colors ${mode === tab ? "bg-white text-blue-700 shadow-sm" : "text-slate-500 hover:text-slate-800"}`}>
              {tab === "login" ? "Entrar" : "Criar conta"}
            </button>
          ))}
        </div>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          {mode === "register" && <label className="block text-sm font-medium text-slate-700">Nome<input value={nome} onChange={(event) => setNome(event.target.value)} className="mt-1.5 w-full rounded-lg border border-slate-200 bg-slate-50 px-3.5 py-2.5 text-slate-800 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100" required minLength={2} /></label>}
          <label className="block text-sm font-medium text-slate-700">E-mail<input type="email" value={email} onChange={(event) => setEmail(event.target.value)} className="mt-1.5 w-full rounded-lg border border-slate-200 bg-slate-50 px-3.5 py-2.5 text-slate-800 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100" required /></label>
          <label className="block text-sm font-medium text-slate-700">Senha<input type="password" value={senha} onChange={(event) => setSenha(event.target.value)} className="mt-1.5 w-full rounded-lg border border-slate-200 bg-slate-50 px-3.5 py-2.5 text-slate-800 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100" required minLength={6} /></label>
          {error && <p role="alert" className="rounded-lg border border-red-200 bg-red-50 px-3 py-2.5 text-sm text-red-600">{error}</p>}
          <button type="submit" disabled={isSubmitting} className="flex w-full items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60">
            {isSubmitting ? <LoaderCircle className="h-4 w-4 animate-spin" /> : <ArrowRight className="h-4 w-4" />}
            {mode === "login" ? "Entrar no StudyFlow" : "Criar minha conta"}
          </button>
        </form>
      </section>
    </main>
  );
};

import React, { useState } from "react";
import { ArrowRight, LoaderCircle } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuth } from "../../contexts/AuthContext";
import { registroSchema, RegistroFormData } from "../../schemas/registroSchema";

interface RegistroFormProps {
  onSuccess: () => void;
}

export const RegistroForm: React.FC<RegistroFormProps> = ({ onSuccess }) => {
  const { register: registerUser } = useAuth();
  const [requestError, setRequestError] = useState("");
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegistroFormData>({
    resolver: zodResolver(registroSchema),
    mode: "onTouched",
  });

  const onSubmit = async (data: RegistroFormData) => {
    setRequestError("");
    try {
      await registerUser(data.nome, data.email, data.senha, data.confirmacaoSenha);
      onSuccess();
    } catch (error: any) {
      setRequestError(error.response?.data?.mensagem || "Não foi possível criar sua conta.");
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="mt-6 space-y-4" noValidate>
      <FormField label="Nome" error={errors.nome?.message}>
        <input {...register("nome")} className={inputClass(Boolean(errors.nome))} autoComplete="name" />
      </FormField>
      <FormField label="E-mail" error={errors.email?.message}>
        <input {...register("email")} type="email" className={inputClass(Boolean(errors.email))} autoComplete="email" />
      </FormField>
      <FormField label="Senha" error={errors.senha?.message}>
        <input {...register("senha")} type="password" className={inputClass(Boolean(errors.senha))} autoComplete="new-password" />
      </FormField>
      <FormField label="Confirmar senha" error={errors.confirmacaoSenha?.message}>
        <input {...register("confirmacaoSenha")} type="password" className={inputClass(Boolean(errors.confirmacaoSenha))} autoComplete="new-password" />
      </FormField>

      {requestError && <p role="alert" className="rounded-lg border border-red-200 bg-red-50 px-3 py-2.5 text-sm text-red-600">{requestError}</p>}
      <button type="submit" disabled={isSubmitting} className="flex w-full items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60">
        {isSubmitting ? <LoaderCircle className="h-4 w-4 animate-spin" /> : <ArrowRight className="h-4 w-4" />}
        Criar minha conta
      </button>
    </form>
  );
};

interface FormFieldProps {
  label: string;
  error?: string;
  children: React.ReactNode;
}

const FormField: React.FC<FormFieldProps> = ({ label, error, children }) => (
  <label className="block text-sm font-medium text-slate-700">
    {label}
    {children}
    {error && <span className="mt-1 block text-xs font-normal text-red-600">{error}</span>}
  </label>
);

const inputClass = (hasError: boolean) =>
  `mt-1.5 w-full rounded-lg border ${hasError ? "border-red-400" : "border-slate-300"} bg-slate-50 px-3.5 py-2.5 text-slate-800 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100`;
import { z } from "zod";

const senhaForte = z
  .string()
  .min(8, "A senha deve ter pelo menos 8 caracteres.")
  .regex(/[A-Z]/, "A senha deve conter pelo menos uma letra maiúscula.")
  .regex(/[a-z]/, "A senha deve conter pelo menos uma letra minúscula.")
  .regex(/[0-9]/, "A senha deve conter pelo menos um número.")
  .regex(/[^a-zA-Z0-9]/, "A senha deve conter pelo menos um caractere especial.");

export const registroSchema = z
  .object({
    nome: z.string().trim().min(2, "O nome deve ter pelo menos 2 caracteres."),
    email: z.string().trim().email("Informe um e-mail válido."),
    senha: senhaForte,
    confirmacaoSenha: z.string().min(1, "Confirme sua senha."),
  })
  .refine((data) => data.senha === data.confirmacaoSenha, {
    path: ["confirmacaoSenha"],
    message: "A confirmação deve ser igual à senha.",
  });

export type RegistroFormData = z.infer<typeof registroSchema>;
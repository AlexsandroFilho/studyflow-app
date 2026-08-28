import React, { createContext, useContext, useEffect, useState } from "react";
import { api } from "../services/api";

export interface AuthUser {
  userId: string;
  nome: string;
  email: string;
}

interface AuthResponse {
  token: string;
  userId: string;
  nome: string;
  email: string;
}

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, senha: string) => Promise<void>;
  register: (nome: string, email: string, senha: string, confirmacaoSenha: string) => Promise<void>;
  logout: () => void;
}

const TOKEN_KEY = "token";
const USER_KEY = "studyflow_user";

function isTokenValid(token: string | null): boolean {
  if (!token) return false;
  try {
    const encodedPayload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const payload = JSON.parse(atob(encodedPayload));
    return typeof payload.exp !== "number" || payload.exp * 1000 > Date.now();
  } catch {
    return false;
  }
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<AuthUser | null>(() => {
    const savedUser = localStorage.getItem(USER_KEY);
    if (!savedUser) return null;
    try {
      return JSON.parse(savedUser) as AuthUser;
    } catch {
      localStorage.removeItem(USER_KEY);
      return null;
    }
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (!isTokenValid(token)) {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
      setToken(null);
      setUser(null);
    }
    setIsLoading(false);
  }, [token]);

  useEffect(() => {
    if (!token) return;
    try {
      const payload = JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
      if (typeof payload.exp !== "number") return;
      const timeout = window.setTimeout(() => {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        setToken(null);
        setUser(null);
        window.location.href = "/login";
      }, Math.max(0, payload.exp * 1000 - Date.now()));
      return () => window.clearTimeout(timeout);
    } catch {
      return;
    }
  }, [token]);

  const saveSession = (response: AuthResponse) => {
    const authenticatedUser = {
      userId: response.userId,
      nome: response.nome,
      email: response.email,
    };
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(authenticatedUser));
    setToken(response.token);
    setUser(authenticatedUser);
  };

  const login = async (email: string, senha: string) => {
    const { data } = await api.post<AuthResponse>("/auth/login", { email, senha });
    saveSession(data);
  };

  const register = async (nome: string, email: string, senha: string, confirmacaoSenha: string) => {
    const { data } = await api.post<AuthResponse>("/auth/register", { nome, email, senha, confirmacaoSenha });
    saveSession(data);
  };

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
    window.location.href = "/login";
  };

  return (
    <AuthContext.Provider value={{ user, token, isAuthenticated: Boolean(token && isTokenValid(token)), isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth deve ser usado dentro de AuthProvider");
  return context;
}

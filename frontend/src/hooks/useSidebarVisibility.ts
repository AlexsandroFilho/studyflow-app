import { useEffect, useState } from "react";

const SIDEBAR_COLLAPSED_KEY = "studyflow_sidebar_collapsed";

function obterEstadoInicial(): boolean {
  try {
    return localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === "true";
  } catch {
    return false;
  }
}

export function useSidebarVisibility() {
  const [isCollapsed, setIsCollapsed] = useState(obterEstadoInicial);

  useEffect(() => {
    localStorage.setItem(SIDEBAR_COLLAPSED_KEY, String(isCollapsed));
  }, [isCollapsed]);

  return { isCollapsed, toggle: () => setIsCollapsed(valor => !valor) };
}

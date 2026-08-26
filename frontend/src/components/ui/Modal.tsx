import React, { useEffect } from "react";
import { X } from "lucide-react";

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  subtitle?: string;
  children: React.ReactNode;
  maxWidth?: "sm" | "md" | "lg" | "xl";
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  title,
  subtitle,
  children,
  maxWidth = "md",
}) => {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) {
        onClose();
      }
    };
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const maxWidthClasses = {
    sm: "max-w-sm",
    md: "max-w-md",
    lg: "max-w-lg",
    xl: "max-w-2xl",
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Backdrop escuro neutro */}
      <div
        className="fixed inset-0 bg-black/80 backdrop-blur-sm transition-opacity"
        onClick={onClose}
      />

      {/* Card do Modal */}
      <div
        className={`relative w-full ${maxWidthClasses[maxWidth]} rounded-xl bg-[#27374D] border border-[#526D82] p-5 shadow-2xl z-10`}
      >
        {/* Header */}
        <div className="flex items-start justify-between pb-3 border-b border-[#526D82]/50">
          <div>
            <h3 className="text-base font-bold text-[#DDE6ED] tracking-tight">{title}</h3>
            {subtitle && <p className="text-xs text-[#9DB2BF] mt-0.5">{subtitle}</p>}
          </div>
          <button
            onClick={onClose}
            className="p-1 rounded-md text-[#9DB2BF] hover:text-[#DDE6ED] hover:bg-[#526D82]/40 transition-colors"
          >
            <X className="w-4 h-4" />
          </button>
        </div>

        {/* Conteúdo */}
        <div className="pt-4">{children}</div>
      </div>
    </div>
  );
};

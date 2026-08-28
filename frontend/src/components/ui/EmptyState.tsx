import React from "react";
import { Button } from "./Button";
import { Plus } from "lucide-react";

interface EmptyStateProps {
  icon?: React.ReactNode;
  title: string;
  description: string;
  actionLabel?: string;
  onAction?: () => void;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  icon,
  title,
  description,
  actionLabel,
  onAction,
}) => {
  return (
    <div className="flex flex-col items-center justify-center p-10 text-center rounded-xl border border-dashed border-slate-300 bg-white max-w-md mx-auto my-8 shadow-lg">
      {icon && (
        <div className="w-12 h-12 rounded-xl bg-blue-50 border border-blue-100 flex items-center justify-center text-blue-600 mb-3 shadow-sm">
          {icon}
        </div>
      )}
      <h3 className="text-sm font-bold text-slate-800 mb-1">{title}</h3>
      <p className="text-xs text-slate-500 max-w-xs mb-5 leading-relaxed">{description}</p>
      {actionLabel && onAction && (
        <Button onClick={onAction} size="sm" icon={<Plus className="w-3.5 h-3.5 font-bold" />}>
          {actionLabel}
        </Button>
      )}
    </div>
  );
};

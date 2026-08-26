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
    <div className="flex flex-col items-center justify-center p-10 text-center rounded-xl border border-dashed border-[#526D82] bg-[#27374D]/90 backdrop-blur-md max-w-md mx-auto my-8 shadow-xl">
      {icon && (
        <div className="w-12 h-12 rounded-xl bg-[#161B22] border border-[#526D82] flex items-center justify-center text-[#9DB2BF] mb-3 shadow-md">
          {icon}
        </div>
      )}
      <h3 className="text-sm font-bold text-[#DDE6ED] mb-1">{title}</h3>
      <p className="text-xs text-[#9DB2BF] max-w-xs mb-5 leading-relaxed">{description}</p>
      {actionLabel && onAction && (
        <Button onClick={onAction} size="sm" icon={<Plus className="w-3.5 h-3.5 font-bold" />}>
          {actionLabel}
        </Button>
      )}
    </div>
  );
};

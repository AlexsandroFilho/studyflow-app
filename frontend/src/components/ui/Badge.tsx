import React from "react";

interface BadgeProps {
  children: React.ReactNode;
  color?: string;
  variant?: "solid" | "outline" | "subtle";
  size?: "sm" | "md";
}

export const Badge: React.FC<BadgeProps> = ({
  children,
  color = "#526D82",
  variant = "subtle",
  size = "sm",
}) => {
  const sizeClasses = size === "sm" ? "px-2 py-0.5 text-xs" : "px-2.5 py-1 text-xs font-medium";

  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-md font-medium tracking-wide ${sizeClasses}`}
      style={{
        backgroundColor: variant === "subtle" ? `${color}33` : variant === "solid" ? color : "transparent",
        color: variant === "solid" ? "#ffffff" : "#DDE6ED",
        border: `1px solid ${color}${variant === "outline" ? "88" : "55"}`,
      }}
    >
      <span
        className="w-1.5 h-1.5 rounded-full shrink-0"
        style={{ backgroundColor: color }}
      />
      {children}
    </span>
  );
};

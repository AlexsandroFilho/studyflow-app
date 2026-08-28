import React from "react";

interface TextareaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
}

export const Textarea: React.FC<TextareaProps> = ({
  label,
  error,
  className = "",
  id,
  rows = 4,
  ...props
}) => {
  const textareaId = id || label?.toLowerCase().replace(/\s+/g, "-");

  return (
    <div className="w-full space-y-1.5">
      {label && (
        <label
          htmlFor={textareaId}
          className="block text-[11px] font-semibold uppercase tracking-wider text-slate-500"
        >
          {label}
        </label>
      )}
      <textarea
        id={textareaId}
        rows={rows}
        className={`w-full rounded-lg bg-slate-50 border ${
          error ? "border-red-500" : "border-slate-200"
        } px-3.5 py-2 text-sm text-slate-800 placeholder:text-slate-400 focus:border-blue-500 focus:outline-none transition-colors resize-none ${className}`}
        {...props}
      />
      {error && <p className="text-[11px] text-red-400">{error}</p>}
    </div>
  );
};

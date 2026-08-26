import React from "react";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input: React.FC<InputProps> = ({ label, error, className = "", id, ...props }) => {
  const inputId = id || label?.toLowerCase().replace(/\s+/g, "-");
  return (
    <div className="w-full space-y-1.5">
      {label && (
        <label htmlFor={inputId} className="block text-[11px] font-semibold uppercase tracking-wider text-[#9DB2BF]">
          {label}
        </label>
      )}
      <input
        id={inputId}
        className={`w-full rounded-lg bg-[#161B22] border ${
          error ? "border-red-500" : "border-[#526D82]"
        } px-3.5 py-2 text-sm text-[#DDE6ED] placeholder-[#526D82] focus:border-[#9DB2BF] focus:outline-none transition-colors ${className}`}
        {...props}
      />
      {error && <p className="text-[11px] text-red-400">{error}</p>}
    </div>
  );
};

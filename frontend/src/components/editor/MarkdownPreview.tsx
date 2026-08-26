import React from "react";

interface MarkdownPreviewProps {
  content: string;
}

export const MarkdownPreview: React.FC<MarkdownPreviewProps> = ({ content }) => {
  if (!content.trim()) {
    return (
      <div className="text-[#9DB2BF] italic text-xs py-4">
        Comece a digitar no editor para visualizar a prévia formatada...
      </div>
    );
  }

  const renderFormattedMarkdown = (text: string) => {
    const lines = text.split("\n");
    return lines.map((line, idx) => {
      // Títulos
      if (line.startsWith("### ")) {
        return (
          <h3 key={idx} className="text-sm font-bold text-[#DDE6ED] mt-3.5 mb-1.5 border-b border-[#526D82]/40 pb-1">
            {line.replace("### ", "")}
          </h3>
        );
      }
      if (line.startsWith("## ")) {
        return (
          <h2 key={idx} className="text-base font-bold text-[#9DB2BF] mt-4 mb-2 border-b border-[#526D82]/40 pb-1">
            {line.replace("## ", "")}
          </h2>
        );
      }
      if (line.startsWith("# ")) {
        return (
          <h1 key={idx} className="text-xl font-bold text-[#DDE6ED] mt-5 mb-2.5 border-b border-[#526D82]/50 pb-1.5 tracking-tight">
            {line.replace("# ", "")}
          </h1>
        );
      }

      // Citações / Callouts
      if (line.startsWith("> ")) {
        return (
          <blockquote
            key={idx}
            className="border-l-2 border-[#9DB2BF] bg-[#27374D] px-3.5 py-1.5 rounded-r-md my-2 text-xs text-[#DDE6ED] italic"
          >
            {line.replace("> ", "")}
          </blockquote>
        );
      }

      // Listas
      if (line.startsWith("- ") || line.startsWith("* ")) {
        return (
          <li key={idx} className="ml-5 list-disc text-xs text-[#DDE6ED] my-0.5 leading-relaxed">
            {line.replace(/^[-*]\s/, "")}
          </li>
        );
      }

      // Linhas em branco
      if (!line.trim()) {
        return <div key={idx} className="h-2.5" />;
      }

      // Parágrafo padrão
      return (
        <p key={idx} className="text-xs text-[#DDE6ED] leading-relaxed my-1">
          {line}
        </p>
      );
    });
  };

  return <div className="space-y-1 select-text">{renderFormattedMarkdown(content)}</div>;
};

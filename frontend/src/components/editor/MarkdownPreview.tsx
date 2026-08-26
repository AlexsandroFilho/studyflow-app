import React from "react";
import { parseMarkdownBlocks, renderInlineMarkdown } from "../../utils/markdown";

interface MarkdownPreviewProps {
  content: string;
  variant?: "full" | "snippet";
}

const FULL_CLASSES = {
  h1: "text-xl font-bold text-[#DDE6ED] mt-5 mb-2.5 border-b border-[#526D82]/50 pb-1.5 tracking-tight",
  h2: "text-base font-bold text-[#9DB2BF] mt-4 mb-2 border-b border-[#526D82]/40 pb-1",
  h3: "text-sm font-bold text-[#DDE6ED] mt-3.5 mb-1.5 border-b border-[#526D82]/40 pb-1",
  quote:
    "border-l-2 border-[#9DB2BF] bg-[#27374D] px-3.5 py-1.5 rounded-r-md my-2 text-xs text-[#DDE6ED] italic",
  li: "ml-5 list-disc text-xs text-[#DDE6ED] my-0.5 leading-relaxed",
  p: "text-xs text-[#DDE6ED] leading-relaxed my-1",
  blank: "h-2.5",
} as const;

const SNIPPET_CLASSES = {
  h1: "text-xs font-semibold text-[#DDE6ED] leading-relaxed",
  h2: "text-xs font-semibold text-[#DDE6ED] leading-relaxed",
  h3: "text-xs font-semibold text-[#DDE6ED] leading-relaxed",
  quote: "text-xs text-[#9DB2BF] italic leading-relaxed border-l-2 border-[#526D82] pl-2",
  li: "ml-4 list-disc text-xs text-[#9DB2BF] leading-relaxed",
  p: "text-xs text-[#9DB2BF] leading-relaxed",
  blank: "h-1",
} as const;

export const MarkdownPreview: React.FC<MarkdownPreviewProps> = ({
  content,
  variant = "full",
}) => {
  const classes = variant === "snippet" ? SNIPPET_CLASSES : FULL_CLASSES;

  if (!content.trim()) {
    if (variant === "snippet") return null;
    return (
      <div className="text-[#9DB2BF] italic text-xs py-4">
        Comece a digitar no editor para visualizar a prévia formatada...
      </div>
    );
  }

  const blocks = parseMarkdownBlocks(content);

  return (
    <div className={variant === "full" ? "space-y-1 select-text" : "space-y-0.5"}>
      {blocks.map((block) => {
        const inline = renderInlineMarkdown(block.content, `md-${block.key}`);

        if (block.type === "h1") {
          return (
            <h1 key={block.key} className={classes.h1}>
              {inline}
            </h1>
          );
        }
        if (block.type === "h2") {
          return (
            <h2 key={block.key} className={classes.h2}>
              {inline}
            </h2>
          );
        }
        if (block.type === "h3") {
          return (
            <h3 key={block.key} className={classes.h3}>
              {inline}
            </h3>
          );
        }
        if (block.type === "quote") {
          return (
            <blockquote key={block.key} className={classes.quote}>
              {inline}
            </blockquote>
          );
        }
        if (block.type === "li") {
          return (
            <li key={block.key} className={classes.li}>
              {inline}
            </li>
          );
        }
        if (block.type === "blank") {
          return <div key={block.key} className={classes.blank} />;
        }
        return (
          <p key={block.key} className={classes.p}>
            {inline}
          </p>
        );
      })}
    </div>
  );
};

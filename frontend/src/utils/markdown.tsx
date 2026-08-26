import React from "react";

export type MarkdownBlockType = "h1" | "h2" | "h3" | "quote" | "li" | "blank" | "p";

export interface MarkdownBlock {
  type: MarkdownBlockType;
  content: string;
  key: number;
}

export function toPlainMarkdown(text: string): string {
  return text
    .replace(/^#{1,6}\s*/gm, "")
    .replace(/^>\s?/gm, "")
    .replace(/^\s*[-*+]\s+/gm, "")
    .replace(/^\s*\d+\.\s+/gm, "")
    .replace(/!\[([^\]]*)\]\([^)]*\)/g, "$1")
    .replace(/\[([^\]]+)\]\([^)]*\)/g, "$1")
    .replace(/(```|~~~)[\s\S]*?\1/g, "")
    .replace(/[*_~`]/g, "")
    .replace(/\s+/g, " ")
    .trim();
}

const INLINE_PATTERN =
  /`([^`]+)`|\*\*\*([^*]+)\*\*\*|\*\*([^*]+)\*\*|\*([^*]+)\*/g;

export function renderInlineMarkdown(text: string, keyPrefix: string): React.ReactNode {
  const nodes: React.ReactNode[] = [];
  let lastIndex = 0;
  let match: RegExpExecArray | null;
  let i = 0;

  INLINE_PATTERN.lastIndex = 0;
  while ((match = INLINE_PATTERN.exec(text)) !== null) {
    if (match.index > lastIndex) {
      nodes.push(text.slice(lastIndex, match.index));
    }

    const key = `${keyPrefix}-${i}`;
    if (match[1] !== undefined) {
      nodes.push(
        <code
          key={key}
          className="font-mono text-[0.9em] bg-[#161B22] text-[#9DB2BF] px-1 py-0.5 rounded"
        >
          {match[1]}
        </code>
      );
    } else if (match[2] !== undefined) {
      nodes.push(
        <strong key={key}>
          <em>{match[2]}</em>
        </strong>
      );
    } else if (match[3] !== undefined) {
      nodes.push(<strong key={key}>{match[3]}</strong>);
    } else if (match[4] !== undefined) {
      nodes.push(<em key={key}>{match[4]}</em>);
    }

    lastIndex = match.index + match[0].length;
    i += 1;
  }

  if (lastIndex < text.length) {
    nodes.push(text.slice(lastIndex));
  }

  return nodes.length ? nodes : text;
}

export function parseMarkdownBlocks(text: string): MarkdownBlock[] {
  return text.split("\n").map((line, key) => {
    const heading = line.match(/^(#{1,3})(?!#)(\s?)(.*)$/);
    if (heading) {
      const level = heading[1].length as 1 | 2 | 3;
      return { type: `h${level}` as "h1" | "h2" | "h3", content: heading[3], key };
    }

    if (line.startsWith("> ")) {
      return { type: "quote", content: line.slice(2), key };
    }

    if (line.startsWith("- ") || line.startsWith("* ")) {
      return { type: "li", content: line.slice(2), key };
    }

    if (!line.trim()) {
      return { type: "blank", content: "", key };
    }

    return { type: "p", content: line, key };
  });
}

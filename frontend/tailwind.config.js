/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Paleta Steel Blue & Graphite Grey
        background: "#161B22",
        surface: "#1C2430",
        "surface-raised": "#27374D",
        "surface-hover": "#31435D",
        border: "#526D82",
        "border-subtle": "#374A5E",

        // Textos
        text: {
          primary: "#DDE6ED",
          secondary: "#9DB2BF",
          muted: "#7B93A4",
        },

        // Acentos e Botões
        accent: {
          DEFAULT: "#526D82",
          hover: "#9DB2BF",
          light: "#DDE6ED",
          dark: "#27374D",
          highlight: "#9DB2BF",
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
        mono: ['JetBrains Mono', 'Fira Code', 'monospace'],
      },
    },
  },
  plugins: [],
}

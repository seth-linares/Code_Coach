import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./src/**/*.{js,ts,jsx,tsx,mdx}", // Covers all subdirectories
  ],
  theme: {
    extend: {
      backgroundImage: {
        "gradient-radial": "radial-gradient(var(--tw-gradient-stops))",
        "gradient-conic": "conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))",
      },
    },
  },
  plugins: [
    require('daisyui'),
    require('@tailwindcss/typography'),
  ],
  daisyui: {
    themes: [
      {
        mytheme: {
          "primary": "#0071ff",
          "primary-content": "#000416",
          "secondary": "#00e2ff",
          "secondary-content": "#001216",
          "accent": "#fb7185",
          "accent-content": "#150406",
          "neutral": "#374151",
          "neutral-content": "#d3d6da",
          "base-100": "#1d2735",
          "base-200": "#18202d",
          "base-300": "#131a25",
          "base-content": "#cccfd3",
          "info": "#c084fc",
          "info-content": "#0e0616",
          "success": "#16a34a",
          "success-content": "#000a02",
          "warning": "#facc15",
          "warning-content": "#150f00",
          "error": "#df1e39",
          "error-content": "#ffd8d6",
        },
      },
    ],
    darkTheme: "mytheme",
    base: true,
    styled: true,
    utils: true,
    prefix: "",
    logs: true,
    themeRoot: ":root",
  },
};

export default config;

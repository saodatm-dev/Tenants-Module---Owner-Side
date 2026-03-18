/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#FFF5F3',
          100: '#FFEBE6',
          200: '#FFD1C7',
          300: '#FFB8A8',
          400: '#FF8A73',
          500: '#FF5B3C',
          600: '#E04A2E',
          700: '#C03A22',
          800: '#9F2D18',
          900: '#7D2110',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}

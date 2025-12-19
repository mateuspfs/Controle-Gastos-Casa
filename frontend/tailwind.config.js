/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        display: ['Inter', 'system-ui', 'sans-serif'],
      },
      colors: {
        brand: {
          50: '#f2f7ff',
          100: '#e6f0ff',
          200: '#c6dbff',
          300: '#9dbdff',
          400: '#6f98ff',
          500: '#4a78ff',
          600: '#2f5cf5',
          700: '#2348d2',
          800: '#1f3ba8',
          900: '#1e3486',
        },
      },
    },
  },
  plugins: [],
}


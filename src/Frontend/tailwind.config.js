/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        seb: {
          green: '#60cd18',
          dark: '#1a1a1a',
          grey: '#f5f5f5',
          text: '#333333'
        }
      }
    },
  },
  plugins: [],
}

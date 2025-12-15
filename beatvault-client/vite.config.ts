import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  // === NEW SECTION: SERVER CONFIG ===
  server: {
    host: true,  // This is the magic line. It exposes the app to your network.
    port: 5173,  // Keeps the port consistent.
    watch: {
      usePolling: true // Helps Docker see your file changes on Windows.
    }
  }
})
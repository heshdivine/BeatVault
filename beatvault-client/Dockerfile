# Use the official Node.js image (The "Kitchen" for JavaScript)
# We use 'alpine' because it is very small and lightweight
FROM node:22-alpine

WORKDIR /app

# Copy the dependency list first (for faster builds)
COPY package*.json ./

# Install the libraries
RUN npm install

# Copy the rest of your code
COPY . .

# Expose the standard React port (3000 for CRA, 5173 for Vite)
# We will assume 5173 (Vite) as it's modern, but we can change it later.
EXPOSE 5173

# Start the development server
CMD ["npm", "run", "dev"]
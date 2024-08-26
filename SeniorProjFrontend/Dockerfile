# Use Node.js LTS version
FROM node:18-alpine

# Install pnpm
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy package.json and pnpm-lock.yaml
COPY package.json pnpm-lock.yaml* ./

# Install dependencies
RUN pnpm install

# Copy the rest of your app's source code
COPY . .

# Expose the port the app runs on
EXPOSE 3000

# Start the development server
CMD ["pnpm", "dev"]
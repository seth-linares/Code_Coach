# Use Node.js LTS version for building the application
FROM node:18-alpine AS builder

# Install pnpm and clean up npm cache
RUN npm install -g pnpm

# Set working directory
WORKDIR /app

# Copy only necessary files to take advantage of Docker's layer caching
COPY package.json pnpm-lock.yaml* ./


# Copy the rest of your app's source code
COPY . .

# Production image
FROM node:18-alpine AS runner

WORKDIR /app

# Install pnpm and clean up npm cache
RUN npm install -g pnpm

# Set a non-root user (optional but recommended)
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Set environment variables
ENV NODE_ENV production
ENV NEXT_TELEMETRY_DISABLED 1

# Copy production dependencies and built assets from builder stage
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/pnpm-lock.yaml ./pnpm-lock.yaml
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/public ./public
COPY --from=builder /app/next.config.mjs ./next.config.mjs

# Expose the port the app runs on
EXPOSE 3000

# Start the application
CMD ["pnpm", "start"]

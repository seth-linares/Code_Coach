# Production stage
FROM node:18-alpine AS runner

WORKDIR /app

# Lock pnpm version and create non-root user
RUN npm install -g pnpm@8.6.2 && \
    addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

# Copy necessary files from builder stage
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/public ./public

# Install production dependencies
RUN pnpm install --prod --frozen-lockfile --no-cache --prefer-offline

# Use non-root user for better security
USER nextjs

# Set environment variables
ENV NODE_ENV production
ENV NEXT_TELEMETRY_DISABLED 1

# Expose the port the app runs on
EXPOSE 3000

# Add a health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:3000/_next/health || exit 1

# Start the application
CMD ["pnpm", "start"]

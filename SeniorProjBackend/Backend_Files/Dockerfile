# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Remove the USER app line as it may cause permission issues
WORKDIR /app
EXPOSE 8080
# Remove EXPOSE 8081 as we're only using 8080 now

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SeniorProjBackend.csproj", "."]
RUN dotnet restore "./SeniorProjBackend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./SeniorProjBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SeniorProjBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy appsettings.json
COPY ["appsettings.json", "."]
# Set ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=Development
# Change the ENTRYPOINT to use the new ASPNETCORE_URLS
ENTRYPOINT ["dotnet", "SeniorProjBackend.dll", "--urls", "http://+:8080"]
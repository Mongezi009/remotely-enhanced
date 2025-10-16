# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Server/Server.csproj", "Server/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Agent/Agent.csproj", "Agent/"]
COPY ["Desktop.Core/Desktop.Core.csproj", "Desktop.Core/"]
COPY ["Desktop.Shared/Desktop.Shared.csproj", "Desktop.Shared/"]

RUN dotnet restore "Server/Server.csproj" -p:EnableLibraryManagerRestore=false

# Copy source code
COPY . .

# Build server (disable LibraryManager)
RUN dotnet publish "Server/Server.csproj" -c Release -o /app/publish --no-restore -p:EnableLibraryManagerRestore=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install required packages
RUN apt-get update && apt-get install -y \
    curl \
    libc6-dev \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

# Create app data directory
RUN mkdir -p /app/AppData

# Copy published app
COPY --from=build /app/publish .

# Create non-root user
RUN groupadd -r remotely && useradd -r -g remotely remotely && \
    chown -R remotely:remotely /app
USER remotely

# Expose ports
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:5000/api/HealthCheck || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "Server.dll"]

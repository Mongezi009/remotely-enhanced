# Test Docker Build Script for Remotely Enhanced
# This script tests the Docker build process

Write-Host "=== Remotely Enhanced - Build Test Script ===" -ForegroundColor Green
Write-Host ""

# Check if Docker is available
try {
    docker --version | Out-Null
    Write-Host "✅ Docker is available" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Docker Desktop and try again" -ForegroundColor Yellow
    exit 1
}

# Check if docker-compose is available
try {
    docker-compose --version | Out-Null
    Write-Host "✅ Docker Compose is available" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Compose is not available" -ForegroundColor Red
    Write-Host "Trying 'docker compose' command instead..." -ForegroundColor Yellow
    try {
        docker compose version | Out-Null
        Write-Host "✅ Docker Compose (v2) is available" -ForegroundColor Green
        $composeCommand = "docker compose"
    } catch {
        Write-Host "❌ Neither 'docker-compose' nor 'docker compose' work" -ForegroundColor Red
        exit 1
    }
} 

if (-not $composeCommand) {
    $composeCommand = "docker-compose"
}

Write-Host ""
Write-Host "Building Remotely Enhanced Server..." -ForegroundColor Cyan
Write-Host ""

# Ask user which compose file to use
Write-Host "Which Docker Compose configuration would you like to use?" -ForegroundColor Yellow
Write-Host "1. Simple (Server only, HTTP, SQLite) - Recommended for testing" -ForegroundColor White
Write-Host "2. Full Stack (Server + Database + Redis + Proxy)" -ForegroundColor White
$choice = Read-Host "Enter choice (1 or 2, default: 1)"

if ([string]::IsNullOrEmpty($choice) -or $choice -eq "1") {
    $composeFile = "docker-compose.simple.yml"
    $serviceName = "remotely-server"
    Write-Host "Using simple configuration..." -ForegroundColor Green
} else {
    $composeFile = "docker-compose.yml"
    $serviceName = "remotely-server"
    Write-Host "Using full stack configuration..." -ForegroundColor Green
}
Write-Host ""

# Verify libman.json status
Write-Host "Checking libman.json status..." -ForegroundColor Cyan
if (Test-Path "Server\libman.json") {
    Write-Host "✅ libman.json exists (empty version)" -ForegroundColor Green
    Get-Content "Server\libman.json" | Write-Host -ForegroundColor Gray
} else {
    Write-Host "⚠️  libman.json does not exist" -ForegroundColor Yellow
}
Write-Host ""

# Check Server.csproj for LibraryManager references
Write-Host "Checking Server.csproj for LibraryManager references..." -ForegroundColor Cyan
$csprojContent = Get-Content "Server\Server.csproj" -Raw
if ($csprojContent -match "LibraryManager") {
    Write-Host "⚠️  Found LibraryManager references in Server.csproj" -ForegroundColor Yellow
} else {
    Write-Host "✅ No LibraryManager references found" -ForegroundColor Green
}

# Check assembly name
if ($csprojContent -match "<AssemblyName>(.*?)</AssemblyName>") {
    $assemblyName = $matches[1]
    Write-Host "✅ Assembly name: $assemblyName.dll" -ForegroundColor Green
} else {
    Write-Host "ℹ️  Using default assembly name: Server.dll" -ForegroundColor Cyan
}
Write-Host ""

# Build the server image
try {
    Write-Host "Starting Docker build..." -ForegroundColor Cyan
    Write-Host "Command: $composeCommand -f $composeFile build $serviceName --no-cache" -ForegroundColor Gray
    Invoke-Expression "$composeCommand -f $composeFile build $serviceName --no-cache"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Build completed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "To start the application, run:" -ForegroundColor Yellow
        Write-Host "  $composeCommand -f $composeFile up -d" -ForegroundColor White
        Write-Host ""
        Write-Host "The application will be available at:" -ForegroundColor Yellow
        if ($composeFile -eq "docker-compose.simple.yml") {
            Write-Host "  HTTP: http://localhost:5000" -ForegroundColor White
        } else {
            Write-Host "  HTTP:  http://localhost:5000" -ForegroundColor White
            Write-Host "  HTTPS: https://localhost:5001" -ForegroundColor White
            Write-Host "  Proxy: http://localhost:80 (redirects to HTTPS)" -ForegroundColor White
        }
    } else {
        Write-Host ""
        Write-Host "❌ Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Try running with --no-cache flag or check Docker logs" -ForegroundColor Yellow
    }
} catch {
    Write-Host ""
    Write-Host "❌ Build failed with error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Build Test Complete ===" -ForegroundColor Green
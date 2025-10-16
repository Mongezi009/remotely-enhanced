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
Write-Host ""

# Build the server image
try {
    Write-Host "Starting Docker build..." -ForegroundColor Cyan
    Invoke-Expression "$composeCommand build remotely-server --no-cache"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Build completed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "To start the full stack, run:" -ForegroundColor Yellow
        Write-Host "  $composeCommand up -d" -ForegroundColor White
        Write-Host ""
        Write-Host "The application will be available at:" -ForegroundColor Yellow
        Write-Host "  HTTP:  http://localhost:5000" -ForegroundColor White
        Write-Host "  HTTPS: https://localhost:5001" -ForegroundColor White
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
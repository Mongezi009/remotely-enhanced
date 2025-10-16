#!/usr/bin/env pwsh
# Remotely Enhanced - Quick Start Script

param(
    [Parameter()]
    [ValidateSet("dev", "prod")]
    [string]$Mode = "dev",
    
    [Parameter()]
    [string]$Domain = "remotely.localhost",
    
    [Parameter()]
    [switch]$SkipBuild,
    
    [Parameter()]
    [switch]$Clean,
    
    [Parameter()]
    [switch]$Help
)

function Show-Help {
    Write-Host @"
Remotely Enhanced - Quick Start Script

Usage: ./quick-start.ps1 [OPTIONS]

OPTIONS:
    -Mode <dev|prod>    Deployment mode (default: dev)
    -Domain <domain>    Domain name (default: remotely.localhost)
    -SkipBuild         Skip building containers
    -Clean             Remove all containers and volumes
    -Help              Show this help message

EXAMPLES:
    # Start in development mode
    ./quick-start.ps1 -Mode dev

    # Start in production mode with custom domain
    ./quick-start.ps1 -Mode prod -Domain remotely.mycompany.com

    # Clean up all containers and volumes
    ./quick-start.ps1 -Clean

"@
}

function Write-Status {
    param([string]$Message, [string]$Color = "Green")
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] $Message" -ForegroundColor $Color
}

function Write-Error-Status {
    param([string]$Message)
    Write-Status $Message -Color "Red"
}

function Write-Warning-Status {
    param([string]$Message)
    Write-Status $Message -Color "Yellow"
}

function Test-Prerequisites {
    Write-Status "Checking prerequisites..."
    
    # Check Docker
    try {
        $dockerVersion = docker --version
        Write-Status "Docker found: $dockerVersion"
    }
    catch {
        Write-Error-Status "Docker is not installed or not in PATH"
        exit 1
    }
    
    # Check Docker Compose
    try {
        $composeVersion = docker compose version
        Write-Status "Docker Compose found: $composeVersion"
    }
    catch {
        Write-Error-Status "Docker Compose is not available"
        exit 1
    }
    
    # Check if Docker daemon is running
    try {
        docker info | Out-Null
        Write-Status "Docker daemon is running"
    }
    catch {
        Write-Error-Status "Docker daemon is not running. Please start Docker."
        exit 1
    }
}

function Initialize-Environment {
    Write-Status "Initializing environment..."
    
    # Create .env file from template if it doesn't exist
    if (-not (Test-Path ".env")) {
        if (Test-Path ".env.example") {
            Copy-Item ".env.example" ".env"
            Write-Status "Created .env file from template"
            Write-Warning-Status "Please review and update .env file with your settings"
        }
        else {
            Write-Warning-Status ".env.example not found. Creating minimal .env file"
            @"
DOMAIN=$Domain
DB_TYPE=SQLite
SQLITE_PATH=/app/AppData/Remotely.db
"@ | Out-File -FilePath ".env" -Encoding utf8
        }
    }
    
    # Update domain in .env file
    (Get-Content ".env") -replace "DOMAIN=.*", "DOMAIN=$Domain" | Set-Content ".env"
    Write-Status "Updated domain to: $Domain"
    
    # Create necessary directories
    $directories = @("logs", "certs", "caddy")
    foreach ($dir in $directories) {
        if (-not (Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-Status "Created directory: $dir"
        }
    }
}

function Start-Services {
    Write-Status "Starting Remotely Enhanced services..."
    
    Set-Location "docker-compose"
    
    if (-not $SkipBuild) {
        Write-Status "Building containers..."
        docker compose build
        if ($LASTEXITCODE -ne 0) {
            Write-Error-Status "Failed to build containers"
            exit 1
        }
    }
    
    Write-Status "Starting containers..."
    docker compose up -d
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to start containers"
        exit 1
    }
    
    Set-Location ".."
    Write-Status "Services started successfully!"
}

function Show-Services-Status {
    Write-Status "Checking services status..."
    Set-Location "docker-compose"
    docker compose ps
    Set-Location ".."
    
    Write-Host ""
    Write-Status "Service URLs:"
    Write-Host "🌐 Web Interface (HTTP): http://localhost:5000" -ForegroundColor Cyan
    Write-Host "🔒 Web Interface (HTTPS): https://localhost:5001" -ForegroundColor Cyan
    Write-Host "📊 API Documentation: http://localhost:5000/swagger" -ForegroundColor Cyan
    
    Write-Host ""
    Write-Host "📝 View logs: docker compose -f docker-compose/docker-compose.yml logs -f" -ForegroundColor Gray
    Write-Host "🛑 Stop services: docker compose -f docker-compose/docker-compose.yml down" -ForegroundColor Gray
}

function Clean-Services {
    Write-Status "Cleaning up services..." -Color "Yellow"
    
    Set-Location "docker-compose"
    
    Write-Status "Stopping containers..."
    docker compose down 2>$null
    
    Write-Status "Removing volumes..."
    docker volume prune -f
    
    Write-Status "Removing unused images..."
    docker image prune -f
    
    Set-Location ".."
    Write-Status "Cleanup completed!"
}

function Wait-For-Services {
    Write-Status "Waiting for services to be ready..."
    
    $maxAttempts = 30
    $attempt = 0
    
    do {
        $attempt++
        Start-Sleep -Seconds 2
        
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-Status "Services are ready!"
                return
            }
        }
        catch {
            Write-Host "." -NoNewline -ForegroundColor Gray
        }
    } while ($attempt -lt $maxAttempts)
    
    Write-Warning-Status "Services may still be starting up. Check logs if needed."
}

# Main execution
if ($Help) {
    Show-Help
    exit 0
}

if ($Clean) {
    Clean-Services
    exit 0
}

Write-Host @"
🚀 Remotely Enhanced - Quick Start
═══════════════════════════════════════════════════════════
Enhanced features:
✅ Comprehensive device information
✅ Modern React-based interface  
✅ Advanced remote control features
✅ Enhanced security monitoring
✅ Performance analytics
"@ -ForegroundColor Blue

Test-Prerequisites
Start-Services
Wait-For-Services
Show-Services-Status

Write-Host ""
Write-Host "🎉 Remotely Enhanced is now running!" -ForegroundColor Green
Write-Host "📚 Check the README.md for more information and usage instructions." -ForegroundColor Gray
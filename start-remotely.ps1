# Start Remotely Enhanced with Original Configuration
# This script provides a simple way to run Remotely Enhanced like the original

Write-Host "=== Remotely Enhanced - Original Style Startup ===" -ForegroundColor Green
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

# Check for docker-compose command
$composeCommand = ""
try {
    docker-compose --version | Out-Null
    $composeCommand = "docker-compose"
    Write-Host "✅ Docker Compose (v1) is available" -ForegroundColor Green
} catch {
    try {
        docker compose version | Out-Null
        $composeCommand = "docker compose"
        Write-Host "✅ Docker Compose (v2) is available" -ForegroundColor Green
    } catch {
        Write-Host "❌ Neither 'docker-compose' nor 'docker compose' work" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Building Remotely Enhanced (Original Style)..." -ForegroundColor Cyan

# Build the image
try {
    Write-Host "Command: $composeCommand -f docker-compose.original.yml build" -ForegroundColor Gray
    Invoke-Expression "$composeCommand -f docker-compose.original.yml build"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "❌ Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} catch {
    Write-Host "❌ Build failed with error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Starting Remotely Enhanced..." -ForegroundColor Cyan

# Start the container
try {
    Invoke-Expression "$composeCommand -f docker-compose.original.yml up -d"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Container started successfully!" -ForegroundColor Green
        
        # Wait a moment for startup
        Write-Host ""
        Write-Host "Waiting for application to start..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        # Get the container IP
        $containerIP = docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' remotely-enhanced 2>$null
        
        Write-Host ""
        Write-Host "🚀 Remotely Enhanced is now running!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Access URLs:" -ForegroundColor Yellow
        Write-Host "  Local:     http://localhost:5000" -ForegroundColor White
        
        if ($containerIP) {
            Write-Host "  Container: http://$containerIP`:5000" -ForegroundColor White
        }
        
        # Get host IP addresses
        $hostIPs = @()
        try {
            $networkAdapters = Get-NetIPAddress -AddressFamily IPv4 -PrefixOrigin Dhcp,Manual | Where-Object {$_.IPAddress -ne "127.0.0.1"}
            foreach ($adapter in $networkAdapters) {
                $hostIPs += $adapter.IPAddress
            }
        } catch {
            # Fallback method
            $hostIPs = @((Test-NetConnection -ComputerName www.google.com -Port 80).SourceAddress.IPAddress)
        }
        
        if ($hostIPs.Count -gt 0) {
            Write-Host "  Host IPs:" -ForegroundColor White
            foreach ($ip in $hostIPs) {
                Write-Host "             http://$ip`:5000" -ForegroundColor White
            }
        }
        
        Write-Host ""
        Write-Host "📋 Management Commands:" -ForegroundColor Yellow
        Write-Host "  Stop:      $composeCommand -f docker-compose.original.yml down" -ForegroundColor White
        Write-Host "  Logs:      $composeCommand -f docker-compose.original.yml logs -f" -ForegroundColor White
        Write-Host "  Restart:   $composeCommand -f docker-compose.original.yml restart" -ForegroundColor White
        Write-Host ""
        Write-Host "The application uses SQLite database stored in Docker volume 'remotely-data'" -ForegroundColor Cyan
        
    } else {
        Write-Host "❌ Failed to start container with exit code: $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} catch {
    Write-Host "❌ Failed to start container: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Startup Complete ===" -ForegroundColor Green
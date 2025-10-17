# Remotely Enhanced - Troubleshooting Script
# This script helps diagnose Docker networking and connectivity issues

Write-Host "=== Remotely Enhanced - Troubleshooting ===" -ForegroundColor Green
Write-Host ""

# Check if container is running
Write-Host "1. Checking container status..." -ForegroundColor Cyan
$container = docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" --filter "name=remotely-enhanced" 2>$null

if ($container) {
    Write-Host "✅ Container is running:" -ForegroundColor Green
    Write-Host $container -ForegroundColor White
} else {
    Write-Host "❌ Container is not running" -ForegroundColor Red
    Write-Host "Try starting it with: .\start-remotely.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Check container logs for errors
Write-Host "2. Checking recent container logs..." -ForegroundColor Cyan
$logs = docker logs --tail 20 remotely-enhanced 2>$null

if ($logs) {
    Write-Host "Recent logs:" -ForegroundColor Yellow
    $logs | ForEach-Object {
        if ($_ -match "error|exception|fail") {
            Write-Host $_ -ForegroundColor Red
        } elseif ($_ -match "warn") {
            Write-Host $_ -ForegroundColor Yellow
        } else {
            Write-Host $_ -ForegroundColor Gray
        }
    }
} else {
    Write-Host "No logs available" -ForegroundColor Yellow
}

Write-Host ""

# Get container network information
Write-Host "3. Container network information..." -ForegroundColor Cyan
$containerIP = docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' remotely-enhanced 2>$null
$containerGateway = docker inspect -f '{{range.NetworkSettings.Networks}}{{.Gateway}}{{end}}' remotely-enhanced 2>$null

if ($containerIP) {
    Write-Host "Container IP: $containerIP" -ForegroundColor White
    Write-Host "Gateway IP: $containerGateway" -ForegroundColor White
} else {
    Write-Host "❌ Could not get container IP" -ForegroundColor Red
}

Write-Host ""

# Test connectivity to container
Write-Host "4. Testing connectivity..." -ForegroundColor Cyan

# Test localhost
Write-Host "Testing localhost:5000..." -ForegroundColor Gray
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000" -Method Head -TimeoutSec 10 -ErrorAction Stop
    Write-Host "✅ Localhost access: WORKING ($($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "❌ Localhost access: FAILED ($($_.Exception.Message))" -ForegroundColor Red
}

# Test container IP if available
if ($containerIP) {
    Write-Host "Testing container IP ($containerIP`:5000)..." -ForegroundColor Gray
    try {
        $response = Invoke-WebRequest -Uri "http://$containerIP:5000" -Method Head -TimeoutSec 10 -ErrorAction Stop
        Write-Host "✅ Container IP access: WORKING ($($response.StatusCode))" -ForegroundColor Green
    } catch {
        Write-Host "❌ Container IP access: FAILED ($($_.Exception.Message))" -ForegroundColor Red
    }
}

# Test health endpoint
Write-Host "Testing health endpoint..." -ForegroundColor Gray
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/healthcheck" -Method Get -TimeoutSec 10 -ErrorAction Stop
    Write-Host "✅ Health endpoint: WORKING ($($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "❌ Health endpoint: FAILED ($($_.Exception.Message))" -ForegroundColor Red
}

Write-Host ""

# Get host network information
Write-Host "5. Host network information..." -ForegroundColor Cyan
try {
    $hostIPs = @()
    $networkAdapters = Get-NetIPAddress -AddressFamily IPv4 -PrefixOrigin Dhcp,Manual | Where-Object {$_.IPAddress -ne "127.0.0.1"}
    
    Write-Host "Available host IP addresses:" -ForegroundColor White
    foreach ($adapter in $networkAdapters) {
        $hostIPs += $adapter.IPAddress
        Write-Host "  $($adapter.IPAddress) (Interface: $($adapter.InterfaceAlias))" -ForegroundColor Gray
        
        # Test each host IP
        try {
            $response = Invoke-WebRequest -Uri "http://$($adapter.IPAddress):5000" -Method Head -TimeoutSec 5 -ErrorAction Stop
            Write-Host "    ✅ Access via $($adapter.IPAddress):5000 - WORKING" -ForegroundColor Green
        } catch {
            Write-Host "    ❌ Access via $($adapter.IPAddress):5000 - FAILED" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "Could not retrieve host network information" -ForegroundColor Yellow
}

Write-Host ""

# Docker port mapping check
Write-Host "6. Docker port mapping..." -ForegroundColor Cyan
$portInfo = docker port remotely-enhanced 2>$null
if ($portInfo) {
    Write-Host "Port mappings:" -ForegroundColor White
    Write-Host $portInfo -ForegroundColor Gray
} else {
    Write-Host "No port mapping information available" -ForegroundColor Yellow
}

Write-Host ""

# Check firewall (Windows)
Write-Host "7. Windows Firewall check..." -ForegroundColor Cyan
try {
    $firewallRule = Get-NetFirewallRule -DisplayName "*Docker*" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($firewallRule) {
        Write-Host "✅ Docker firewall rules found" -ForegroundColor Green
    } else {
        Write-Host "⚠️  No Docker firewall rules found - this might block external access" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Could not check Windows Firewall" -ForegroundColor Yellow
}

Write-Host ""

# Recommendations
Write-Host "8. Recommendations:" -ForegroundColor Cyan

if ($containerIP -and $hostIPs.Count -gt 0) {
    Write-Host "✅ Network setup appears correct" -ForegroundColor Green
    Write-Host ""
    Write-Host "Try accessing Remotely Enhanced at:" -ForegroundColor Yellow
    Write-Host "  Local:     http://localhost:5000" -ForegroundColor White
    Write-Host "  Container: http://$containerIP`:5000" -ForegroundColor White
    foreach ($ip in $hostIPs) {
        Write-Host "  Host IP:   http://$ip`:5000" -ForegroundColor White
    }
} else {
    Write-Host "❌ Network setup issues detected" -ForegroundColor Red
    Write-Host ""
    Write-Host "Try these steps:" -ForegroundColor Yellow
    Write-Host "1. Restart the container: docker-compose -f docker-compose.original.yml restart" -ForegroundColor White
    Write-Host "2. Check Docker Desktop is running and configured properly" -ForegroundColor White
    Write-Host "3. Ensure Windows Firewall allows Docker Desktop" -ForegroundColor White
    Write-Host "4. Try rebuilding: docker-compose -f docker-compose.original.yml build --no-cache" -ForegroundColor White
}

Write-Host ""
Write-Host "=== Troubleshooting Complete ===" -ForegroundColor Green
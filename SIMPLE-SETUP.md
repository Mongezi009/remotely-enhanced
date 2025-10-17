# Remotely Enhanced - Simplified Docker Setup

This setup provides a simple, single-container deployment that works exactly like the original Remotely Docker configuration, but with all the enhanced features included.

## 🚀 Quick Start

### Option 1: Automated Startup (Recommended)
```powershell
.\start-remotely.ps1
```

### Option 2: Manual Commands
```bash
# Build and start
docker-compose -f docker-compose.original.yml up -d

# Stop
docker-compose -f docker-compose.original.yml down

# View logs
docker-compose -f docker-compose.original.yml logs -f
```

## 🌐 Access Your Installation

Once running, you can access Remotely Enhanced at:

- **Local Access**: http://localhost:5000
- **Network Access**: http://YOUR-IP-ADDRESS:5000
- **Container Access**: http://CONTAINER-IP:5000

The startup script will show you all available IP addresses automatically.

## 📋 What's Included

### Original Remotely Compatibility
- ✅ Same Docker network configuration (172.28.0.0/16)
- ✅ Same environment variable format (`Remotely_` prefix)
- ✅ Same port mapping (5000:5000)
- ✅ Same SQLite database setup
- ✅ Same health check endpoint

### Enhanced Features Added
- 🔧 **Comprehensive Device Information**: Hardware specs, memory details, storage health
- 🎨 **Modern UI Components**: Enhanced interface with better device management
- 🔒 **Advanced Security Monitoring**: Windows Defender, firewall, BitLocker status
- 📊 **Performance Metrics**: Real-time CPU, memory, disk, and network monitoring
- 🖥️ **Multi-Monitor Support**: Enhanced remote desktop capabilities
- 📁 **Improved File Transfer**: Better file management and streaming
- 🔄 **Real-time Updates**: Live performance data and device status

## 🔧 Configuration

### Environment Variables (all optional)

The container uses the same environment variables as original Remotely:

```yaml
# Database (SQLite by default)
- Remotely_ApplicationOptions__DbProvider=SQLite
- Remotely_ConnectionStrings__SQLite=Data Source=/app/AppData/Remotely.db

# Docker Gateway (for forwarded headers)
- Remotely_ApplicationOptions__DockerGateway=172.28.0.1

# Logging
- Serilog__MinimumLevel__Override__Microsoft.AspNetCore=Warning
```

### Data Persistence

Your data is stored in Docker volume `remotely-data`:
```bash
# View volume contents
docker run --rm -v remotely-data:/app/AppData alpine ls -la /app/AppData

# Backup volume
docker run --rm -v remotely-data:/source -v $(pwd):/backup alpine tar czf /backup/remotely-backup.tar.gz -C /source .

# Restore volume
docker run --rm -v remotely-data:/target -v $(pwd):/backup alpine tar xzf /backup/remotely-backup.tar.gz -C /target
```

## 🔍 Troubleshooting

### Network Access Issues

If you can't access the application from other machines:

1. **Run the troubleshooting script**:
   ```powershell
   .\troubleshoot.ps1
   ```

2. **Check Windows Firewall**:
   - Ensure Docker Desktop is allowed through Windows Firewall
   - Or temporarily disable Windows Firewall for testing

3. **Verify Docker is binding to all interfaces**:
   ```bash
   docker port remotely-enhanced
   # Should show: 5000/tcp -> 0.0.0.0:5000
   ```

### Container Won't Start

1. **Check the logs**:
   ```bash
   docker-compose -f docker-compose.original.yml logs
   ```

2. **Rebuild from scratch**:
   ```bash
   docker-compose -f docker-compose.original.yml down
   docker-compose -f docker-compose.original.yml build --no-cache
   docker-compose -f docker-compose.original.yml up -d
   ```

3. **Verify all files are in place**:
   ```bash
   docker run --rm -it remotely-enhanced ls -la /app/
   ```

### Health Check Failures

The health check endpoint is: `/api/healthcheck`

Test manually:
```bash
curl http://localhost:5000/api/healthcheck
```

Should return HTTP 204 (No Content) if healthy.

## 🔄 Migration from Original Remotely

### Database Migration

If you have an existing Remotely database:

1. **Stop both containers**
2. **Copy the database file**:
   ```bash
   docker cp old-remotely-container:/app/AppData/Remotely.db ./Remotely.db
   docker cp ./Remotely.db remotely-enhanced:/app/AppData/Remotely.db
   ```
3. **Restart enhanced container**

### Agent Configuration

Your existing agents will work without changes. The enhanced version is fully backward compatible with existing Remotely agents.

## 📚 Additional Resources

- **Original Remotely Documentation**: https://github.com/immense/Remotely
- **Enhanced Features Documentation**: See README.md in this directory
- **Docker Documentation**: https://docs.docker.com/

## 🆘 Getting Help

If you encounter issues:

1. Run `.\troubleshoot.ps1` for automated diagnostics
2. Check container logs: `docker-compose -f docker-compose.original.yml logs`
3. Verify network connectivity from other machines
4. Ensure Docker Desktop is properly configured

The enhanced version maintains 100% compatibility with original Remotely while adding powerful new features for better device management and monitoring.
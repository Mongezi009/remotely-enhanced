# 🚀 Remotely Enhanced - Quick Deployment Guide

This guide will help you deploy Remotely Enhanced in just a few minutes!

## 📋 Prerequisites

- **Docker Desktop** (Windows/Mac) or **Docker Engine** (Linux)
- **Git** (for cloning the repository)
- At least 4GB RAM and 10GB free disk space

## 🚀 Option 1: Quick Start (Recommended)

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/remotely-enhanced.git
cd remotely-enhanced
```

### 2. Run the Quick Start Script
```powershell
# Windows PowerShell
./quick-start-fixed.ps1

# Or with custom options
./quick-start-fixed.ps1 -Mode prod -Domain remotely.yourdomain.com
```

### 3. Access the Application
- **Web Interface**: http://localhost:5000
- **HTTPS Interface**: https://localhost:5001  
- **API Documentation**: http://localhost:5000/swagger

### 4. Create Your First Account
1. Go to http://localhost:5000
2. Click "Register" to create the admin account
3. This account becomes both server admin and organization admin
4. The register button disappears after the first account is created

## 🐳 Option 2: Manual Docker Compose

### 1. Navigate to Docker Compose Directory
```bash
cd docker-compose
```

### 2. Build and Start Services
```bash
# Build the containers
docker compose build

# Start the services
docker compose up -d

# Check status
docker compose ps
```

### 3. View Logs (if needed)
```bash
docker compose logs -f
```

### 4. Stop Services
```bash
docker compose down
```

## 📊 What You Get

### Enhanced Device Information
- **Hardware Details**: CPU, motherboard, BIOS, temperatures
- **Memory Information**: Individual RAM modules, speeds, manufacturers
- **Storage Analytics**: Physical drives, health monitoring, SSD detection
- **Network Intelligence**: All adapters, speeds, connectivity status
- **Security Status**: Windows Defender, firewall, UAC, BitLocker
- **Software Inventory**: Installed programs, services, startup items
- **User Management**: Active sessions, accounts, privileges

### Advanced Remote Features
- **Multi-Monitor Support**: Control across multiple displays
- **Clipboard Synchronization**: Seamless copy/paste between devices
- **File Streaming**: Real-time file transfer with progress
- **Enhanced Performance**: Adaptive quality based on network

### Modern Interface
- **React + Material-UI**: Professional, responsive design
- **Mobile Support**: Works on tablets and phones
- **Real-time Updates**: Live performance metrics
- **Tabbed Organization**: Hardware, Network, Security, etc.

## 🔧 Configuration

### Environment Variables
Edit the docker-compose.yml file to customize:

```yaml
environment:
  # Database type
  - Remotely_ApplicationOptions__DbProvider=SQLite
  
  # Allow multiple organizations
  - Remotely_ApplicationOptions__MaxOrganizationCount=-1
  
  # Enable API access
  - Remotely_ApplicationOptions__AllowApiLogin=true
  
  # Enhanced features
  - Remotely_EnhancedFeatures__EnableComprehensiveScanning=true
  - Remotely_EnhancedFeatures__ScanIntervalMinutes=60
```

### SSL/HTTPS Setup
For production deployment:
1. Obtain SSL certificates
2. Place them in the `certs/` directory  
3. Update the docker-compose.yml file
4. Use a reverse proxy like Caddy or Nginx

## 🔒 Security Considerations

### Production Deployment
- Change default passwords
- Use proper SSL certificates
- Configure firewall rules
- Set up proper authentication
- Regular security updates

### Network Access
- By default, runs on localhost only
- For remote access, configure reverse proxy
- Use VPN for secure remote administration
- Monitor access logs regularly

## 📱 Agent Installation

### Windows Agent
1. Go to http://your-server:5000
2. Click "Downloads"
3. Download and run the Windows installer
4. Follow the setup wizard

### Linux Agent
```bash
# Ubuntu/Debian
curl -s http://your-server:5000/Content/Install-Ubuntu-x64.sh | sudo bash

# Other distributions - check the Downloads page
```

## 🔧 Troubleshooting

### Common Issues

**Container won't start:**
```bash
# Check logs
docker compose logs remotely-enhanced

# Restart services
docker compose down
docker compose up -d
```

**Can't access web interface:**
- Check if port 5000 is available
- Verify Docker is running
- Check firewall settings

**Agent won't connect:**
- Verify server URL is accessible from agent machine
- Check network connectivity
- Review agent logs in `%ProgramData%\Remotely\Logs`

### Getting Help
- Check the logs: `docker compose logs -f`
- Verify Docker status: `docker compose ps`
- Restart services: `docker compose restart`

## 🚀 Scaling for Production

### Performance Optimization
- Use SQL Server or PostgreSQL instead of SQLite
- Add Redis for caching and session management
- Configure load balancing for multiple server instances
- Set up dedicated database server

### Monitoring
- Enable comprehensive logging
- Set up health checks
- Monitor resource usage
- Configure alerts for critical issues

## 📚 Additional Resources

- **Original Remotely**: https://github.com/immense/Remotely
- **Docker Documentation**: https://docs.docker.com/
- **ASP.NET Core Configuration**: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/

## 🎉 Success!

You now have a powerful, enterprise-grade remote access and device management solution running!

Key URLs to bookmark:
- **Main Interface**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **Agent Downloads**: http://localhost:5000/Downloads

Start by registering your first account and then download the agent for your devices.
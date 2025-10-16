#!/usr/bin/env pwsh
# GitHub Setup Script for Remotely Enhanced

param(
    [Parameter(Mandatory)]
    [string]$GitHubUsername,
    
    [Parameter()]
    [string]$RepositoryName = "remotely-enhanced",
    
    [Parameter()]
    [switch]$Help
)

function Show-Help {
    Write-Host @"
GitHub Setup Script for Remotely Enhanced

Usage: ./setup-github.ps1 -GitHubUsername <username> [OPTIONS]

PARAMETERS:
    -GitHubUsername <username>    Your GitHub username (required)
    -RepositoryName <name>       Repository name (default: remotely-enhanced)
    -Help                        Show this help message

EXAMPLES:
    # Set up with GitHub username
    ./setup-github.ps1 -GitHubUsername myusername

    # Set up with custom repository name
    ./setup-github.ps1 -GitHubUsername myusername -RepositoryName my-remotely

PREREQUISITES:
    1. Create a repository on GitHub first:
       - Go to https://github.com/new
       - Repository name: $RepositoryName
       - Make it public
       - Don't initialize with README (we have one)
       - Click "Create repository"

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
    
    # Check Git
    try {
        $gitVersion = git --version
        Write-Status "Git found: $gitVersion"
    }
    catch {
        Write-Error-Status "Git is not installed or not in PATH"
        Write-Host "Please install Git: https://git-scm.com/downloads" -ForegroundColor Yellow
        exit 1
    }
}

function Initialize-Repository {
    Write-Status "Initializing Git repository..."
    
    # Remove existing git if it exists
    if (Test-Path ".git") {
        Write-Warning-Status "Removing existing .git directory..."
        Remove-Item -Recurse -Force ".git"
    }
    
    # Initialize new git repo
    git init
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to initialize Git repository"
        exit 1
    }
    
    # Set up git config if not set
    try {
        $userName = git config --global user.name
        $userEmail = git config --global user.email
        
        if ([string]::IsNullOrEmpty($userName) -or [string]::IsNullOrEmpty($userEmail)) {
            Write-Warning-Status "Git user not configured. Please set up your Git identity:"
            Write-Host "  git config --global user.name `"Your Name`"" -ForegroundColor Yellow
            Write-Host "  git config --global user.email `"your.email@example.com`"" -ForegroundColor Yellow
            Write-Host ""
            $continue = Read-Host "Continue anyway? (y/N)"
            if ($continue -ne "y" -and $continue -ne "Y") {
                exit 1
            }
        }
    }
    catch {
        Write-Warning-Status "Could not check Git configuration"
    }
    
    Write-Status "Git repository initialized"
}

function Add-Files {
    Write-Status "Adding files to repository..."
    
    # Add all files
    git add .
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to add files to Git"
        exit 1
    }
    
    # Create initial commit
    git commit -m "Initial commit: Remotely Enhanced with comprehensive device management

- Enhanced device information collection with 50+ data points
- Modern React-based dashboard with Material-UI
- Advanced remote control features (multi-monitor, clipboard sync, file streaming)
- Comprehensive security monitoring and compliance tracking
- Performance analytics and real-time monitoring
- Docker-based deployment with production-ready configuration
- API enhancements for enterprise integration
- Mobile-responsive interface

Based on the original Remotely project by Jared Goodwin
Enhanced for enterprise-grade device management and remote access"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to create initial commit"
        exit 1
    }
    
    Write-Status "Files committed to local repository"
}

function Setup-Remote {
    $repoUrl = "https://github.com/$GitHubUsername/$RepositoryName.git"
    
    Write-Status "Setting up remote repository: $repoUrl"
    
    # Add remote origin
    git remote add origin $repoUrl
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to add remote origin"
        exit 1
    }
    
    # Rename branch to main
    git branch -M main
    if ($LASTEXITCODE -ne 0) {
        Write-Warning-Status "Could not rename branch to main (this is ok)"
    }
    
    Write-Status "Remote repository configured"
}

function Push-Repository {
    Write-Status "Pushing to GitHub..."
    
    # Push to GitHub
    git push -u origin main
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Status "Failed to push to GitHub"
        Write-Host ""
        Write-Host "This might happen if:" -ForegroundColor Yellow
        Write-Host "1. The repository doesn't exist on GitHub" -ForegroundColor Yellow
        Write-Host "2. You don't have write access to the repository" -ForegroundColor Yellow
        Write-Host "3. You need to authenticate with GitHub" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "To fix authentication issues:" -ForegroundColor Cyan
        Write-Host "1. Set up SSH keys: https://docs.github.com/en/authentication/connecting-to-github-with-ssh" -ForegroundColor Cyan
        Write-Host "2. Or use Personal Access Token: https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token" -ForegroundColor Cyan
        exit 1
    }
    
    Write-Status "Successfully pushed to GitHub!"
}

function Create-Release {
    Write-Status "Creating release tag..."
    
    # Create and push version tag
    git tag -a v1.0.0 -m "Version 1.0.0: Initial release of Remotely Enhanced

Features:
- Comprehensive device information collection
- Modern React-based interface with Material-UI
- Advanced remote control capabilities
- Enhanced security monitoring
- Performance analytics and monitoring
- Docker-based deployment
- Enterprise-grade API enhancements"

    git push origin v1.0.0
    if ($LASTEXITCODE -ne 0) {
        Write-Warning-Status "Could not push release tag (this is ok)"
    } else {
        Write-Status "Release tag v1.0.0 created"
    }
}

function Show-Next-Steps {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCCESS! Your repository is now on GitHub" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    
    $repoUrl = "https://github.com/$GitHubUsername/$RepositoryName"
    Write-Host "Repository URL: $repoUrl" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Visit your repository: $repoUrl" -ForegroundColor White
    Write-Host "2. Update the repository description" -ForegroundColor White
    Write-Host "3. Add topics/tags: docker, remote-desktop, device-management" -ForegroundColor White
    Write-Host "4. Create a release from the v1.0.0 tag" -ForegroundColor White
    Write-Host "5. Share with others!" -ForegroundColor White
    Write-Host ""
    
    Write-Host "To deploy on any machine:" -ForegroundColor Yellow
    Write-Host "git clone $repoUrl.git" -ForegroundColor White
    Write-Host "cd $RepositoryName" -ForegroundColor White
    Write-Host "./quick-start-fixed.ps1" -ForegroundColor White
    Write-Host ""
}

# Main execution
if ($Help) {
    Show-Help
    exit 0
}

if ([string]::IsNullOrEmpty($GitHubUsername)) {
    Write-Error-Status "GitHub username is required"
    Show-Help
    exit 1
}

Write-Host @"
GitHub Setup for Remotely Enhanced
===================================
Username: $GitHubUsername
Repository: $RepositoryName
"@ -ForegroundColor Blue

Test-Prerequisites
Initialize-Repository
Add-Files
Setup-Remote
Push-Repository
Create-Release
Show-Next-Steps

Write-Host "Setup completed successfully!" -ForegroundColor Green
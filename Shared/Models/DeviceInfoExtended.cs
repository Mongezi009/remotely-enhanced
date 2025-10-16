using System.Runtime.InteropServices;

namespace Remotely.Shared.Models;

public class DeviceInfoExtended
{
    // Basic Info (existing)
    public string DeviceName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string OSDescription { get; set; } = string.Empty;
    public Architecture OSArchitecture { get; set; }
    public bool Is64Bit { get; set; }
    public int ProcessorCount { get; set; }
    
    // Enhanced Hardware Info
    public string? ProcessorName { get; set; }
    public string? ProcessorManufacturer { get; set; }
    public double ProcessorSpeedGHz { get; set; }
    public string? MotherboardManufacturer { get; set; }
    public string? MotherboardModel { get; set; }
    public string? BIOSVersion { get; set; }
    public DateTime? BIOSDate { get; set; }
    
    // Memory Details
    public double TotalPhysicalMemoryGB { get; set; }
    public double AvailableMemoryGB { get; set; }
    public double UsedMemoryGB { get; set; }
    public double VirtualMemoryTotalGB { get; set; }
    public double VirtualMemoryAvailableGB { get; set; }
    public List<MemoryModule> MemoryModules { get; set; } = new();
    
    // Storage Details
    public List<StorageDevice> StorageDevices { get; set; } = new();
    public List<Drive> LogicalDrives { get; set; } = new();
    
    // Network Information
    public List<NetworkAdapter> NetworkAdapters { get; set; } = new();
    public string? PublicIPAddress { get; set; }
    public string? LocalIPAddress { get; set; }
    public List<string> DNSServers { get; set; } = new();
    public string? DefaultGateway { get; set; }
    public bool IsConnectedToInternet { get; set; }
    public double NetworkLatencyMs { get; set; }
    
    // Graphics Information
    public List<GraphicsAdapter> GraphicsAdapters { get; set; } = new();
    public List<Monitor> Monitors { get; set; } = new();
    public string? PrimaryDisplayResolution { get; set; }
    
    // System Performance
    public double CPUTemperature { get; set; }
    public double CPUUtilizationPercent { get; set; }
    public double GPUUtilizationPercent { get; set; }
    public double GPUTemperature { get; set; }
    public TimeSpan SystemUptime { get; set; }
    public DateTime LastBootTime { get; set; }
    
    // Software Information
    public List<InstalledSoftware> InstalledPrograms { get; set; } = new();
    public List<RunningProcess> RunningProcesses { get; set; } = new();
    public List<SystemService> SystemServices { get; set; } = new();
    public List<StartupProgram> StartupPrograms { get; set; } = new();
    public string? AntivirusStatus { get; set; }
    public bool WindowsDefenderEnabled { get; set; }
    public DateTime? LastWindowsUpdate { get; set; }
    public List<string> PendingUpdates { get; set; } = new();
    
    // User Information
    public List<UserAccount> UserAccounts { get; set; } = new();
    public string? CurrentlyLoggedInUser { get; set; }
    public List<ActiveSession> ActiveSessions { get; set; } = new();
    
    // Security Information
    public bool FirewallEnabled { get; set; }
    public string? DomainStatus { get; set; }
    public bool BitLockerEnabled { get; set; }
    public List<SecurityEvent> RecentSecurityEvents { get; set; } = new();
    public bool UACEnabled { get; set; }
    
    // Remote Access Info
    public bool RDPEnabled { get; set; }
    public int RDPPort { get; set; }
    public bool TeamViewerInstalled { get; set; }
    public bool AnyDeskInstalled { get; set; }
    public List<string> RemoteAccessSoftware { get; set; } = new();
    
    // Environmental
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? AssetTag { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyExpiration { get; set; }
    
    // Timestamps
    public DateTime LastFullScan { get; set; }
    public DateTime LastQuickUpdate { get; set; }
    public DateTime LastSeen { get; set; }
}

public class MemoryModule
{
    public string? Manufacturer { get; set; }
    public double CapacityGB { get; set; }
    public int SpeedMHz { get; set; }
    public string? FormFactor { get; set; }
    public string? PartNumber { get; set; }
    public string? Slot { get; set; }
}

public class StorageDevice
{
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public double CapacityGB { get; set; }
    public string? Interface { get; set; }
    public bool IsSSD { get; set; }
    public string? SerialNumber { get; set; }
    public double TemperatureCelsius { get; set; }
    public int HealthPercentage { get; set; }
}

public class NetworkAdapter
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? MACAddress { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsConnected { get; set; }
    public string? ConnectionType { get; set; }
    public double SpeedMbps { get; set; }
    public string? IPAddress { get; set; }
    public string? SubnetMask { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
}

public class GraphicsAdapter
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public double MemoryGB { get; set; }
    public string? DriverVersion { get; set; }
    public DateTime? DriverDate { get; set; }
}

public class Monitor
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? Resolution { get; set; }
    public double DiagonalInches { get; set; }
    public bool IsPrimary { get; set; }
    public string? SerialNumber { get; set; }
}

public class InstalledSoftware
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Publisher { get; set; }
    public DateTime? InstallDate { get; set; }
    public double SizeMB { get; set; }
    public string? InstallLocation { get; set; }
}

public class RunningProcess
{
    public string? Name { get; set; }
    public int ProcessId { get; set; }
    public double CPUPercent { get; set; }
    public double MemoryMB { get; set; }
    public string? UserName { get; set; }
    public DateTime StartTime { get; set; }
}

public class SystemService
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Status { get; set; }
    public string? StartType { get; set; }
    public string? Description { get; set; }
}

public class StartupProgram
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Command { get; set; }
    public bool IsEnabled { get; set; }
    public string? Impact { get; set; }
}

public class UserAccount
{
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? PasswordLastSet { get; set; }
    public bool PasswordExpires { get; set; }
}

public class ActiveSession
{
    public string? Username { get; set; }
    public string? SessionType { get; set; }
    public DateTime LoginTime { get; set; }
    public string? ClientName { get; set; }
    public string? ClientIP { get; set; }
}

public class SecurityEvent
{
    public DateTime Timestamp { get; set; }
    public string? EventType { get; set; }
    public string? Description { get; set; }
    public string? Severity { get; set; }
    public string? Source { get; set; }
}
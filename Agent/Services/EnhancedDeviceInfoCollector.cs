using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Remotely.Shared.Models;

namespace Remotely.Agent.Services;

public class EnhancedDeviceInfoCollector
{
    private readonly ILogger<EnhancedDeviceInfoCollector> _logger;

    public EnhancedDeviceInfoCollector(ILogger<EnhancedDeviceInfoCollector> logger)
    {
        _logger = logger;
    }

    public async Task<DeviceInfoExtended> CollectFullSystemInfoAsync()
    {
        var deviceInfo = new DeviceInfoExtended
        {
            DeviceName = Environment.MachineName,
            Platform = GetPlatformString(),
            OSDescription = RuntimeInformation.OSDescription,
            OSArchitecture = RuntimeInformation.OSArchitecture,
            Is64Bit = Environment.Is64BitOperatingSystem,
            ProcessorCount = Environment.ProcessorCount,
            LastFullScan = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        try
        {
            // Collect hardware information
            await CollectHardwareInfoAsync(deviceInfo);
            
            // Collect memory information
            await CollectMemoryInfoAsync(deviceInfo);
            
            // Collect storage information
            await CollectStorageInfoAsync(deviceInfo);
            
            // Collect network information
            await CollectNetworkInfoAsync(deviceInfo);
            
            // Collect graphics information
            await CollectGraphicsInfoAsync(deviceInfo);
            
            // Collect performance information
            await CollectPerformanceInfoAsync(deviceInfo);
            
            // Collect software information
            await CollectSoftwareInfoAsync(deviceInfo);
            
            // Collect user information
            await CollectUserInfoAsync(deviceInfo);
            
            // Collect security information
            await CollectSecurityInfoAsync(deviceInfo);
            
            // Collect remote access information
            await CollectRemoteAccessInfoAsync(deviceInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting comprehensive device information");
        }

        return deviceInfo;
    }

    private async Task CollectHardwareInfoAsync(DeviceInfoExtended deviceInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                deviceInfo.ProcessorName = obj["Name"]?.ToString();
                deviceInfo.ProcessorManufacturer = obj["Manufacturer"]?.ToString();
                if (uint.TryParse(obj["MaxClockSpeed"]?.ToString(), out uint clockSpeed))
                {
                    deviceInfo.ProcessorSpeedGHz = clockSpeed / 1000.0;
                }
                break; // Take first processor
            }

            using var motherboardSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject obj in motherboardSearcher.Get())
            {
                deviceInfo.MotherboardManufacturer = obj["Manufacturer"]?.ToString();
                deviceInfo.MotherboardModel = obj["Product"]?.ToString();
                break;
            }

            using var biosSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (ManagementObject obj in biosSearcher.Get())
            {
                deviceInfo.BIOSVersion = obj["SMBIOSBIOSVersion"]?.ToString();
                if (DateTime.TryParse(obj["ReleaseDate"]?.ToString(), out DateTime biosDate))
                {
                    deviceInfo.BIOSDate = biosDate;
                }
                break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting hardware information");
        }
    }

    private async Task CollectMemoryInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            var memoryStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memoryStatus))
            {
                deviceInfo.TotalPhysicalMemoryGB = Math.Round(memoryStatus.ullTotalPhys / (1024.0 * 1024.0 * 1024.0), 2);
                deviceInfo.AvailableMemoryGB = Math.Round(memoryStatus.ullAvailPhys / (1024.0 * 1024.0 * 1024.0), 2);
                deviceInfo.UsedMemoryGB = deviceInfo.TotalPhysicalMemoryGB - deviceInfo.AvailableMemoryGB;
                deviceInfo.VirtualMemoryTotalGB = Math.Round(memoryStatus.ullTotalVirtual / (1024.0 * 1024.0 * 1024.0), 2);
                deviceInfo.VirtualMemoryAvailableGB = Math.Round(memoryStatus.ullAvailVirtual / (1024.0 * 1024.0 * 1024.0), 2);
            }

            // Collect memory modules information
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var module = new MemoryModule
                    {
                        Manufacturer = obj["Manufacturer"]?.ToString(),
                        CapacityGB = Math.Round((ulong)(obj["Capacity"] ?? 0UL) / (1024.0 * 1024.0 * 1024.0), 2),
                        SpeedMHz = int.TryParse(obj["Speed"]?.ToString(), out int speed) ? speed : 0,
                        FormFactor = obj["FormFactor"]?.ToString(),
                        PartNumber = obj["PartNumber"]?.ToString()?.Trim(),
                        Slot = obj["DeviceLocator"]?.ToString()
                    };
                    deviceInfo.MemoryModules.Add(module);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting memory information");
        }
    }

    private async Task CollectStorageInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            // Collect logical drives
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives.Where(d => d.IsReady))
            {
                deviceInfo.LogicalDrives.Add(new Drive
                {
                    Name = drive.Name,
                    DriveType = drive.DriveType,
                    DriveFormat = drive.DriveFormat,
                    TotalSize = Math.Round(drive.TotalSize / (1024.0 * 1024.0 * 1024.0), 2),
                    FreeSpace = Math.Round(drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 2),
                    VolumeLabel = drive.VolumeLabel,
                    RootDirectory = drive.RootDirectory.FullName
                });
            }

            // Collect physical storage devices (Windows only)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var storageDevice = new StorageDevice
                    {
                        Model = obj["Model"]?.ToString(),
                        Manufacturer = obj["Manufacturer"]?.ToString(),
                        CapacityGB = Math.Round((ulong)(obj["Size"] ?? 0UL) / (1024.0 * 1024.0 * 1024.0), 2),
                        Interface = obj["InterfaceType"]?.ToString(),
                        SerialNumber = obj["SerialNumber"]?.ToString()?.Trim(),
                        IsSSD = obj["MediaType"]?.ToString()?.Contains("SSD") == true
                    };
                    deviceInfo.StorageDevices.Add(storageDevice);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting storage information");
        }
    }

    private async Task CollectNetworkInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in networkInterfaces)
            {
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    var adapter = new NetworkAdapter
                    {
                        Name = ni.Name,
                        MACAddress = ni.GetPhysicalAddress().ToString(),
                        IsEnabled = ni.OperationalStatus == OperationalStatus.Up,
                        IsConnected = ni.OperationalStatus == OperationalStatus.Up,
                        ConnectionType = ni.NetworkInterfaceType.ToString(),
                        SpeedMbps = ni.Speed > 0 ? ni.Speed / 1_000_000.0 : 0
                    };

                    var ipProps = ni.GetIPProperties();
                    var unicastAddresses = ipProps.UnicastAddresses;
                    var ipv4Address = unicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipv4Address != null)
                    {
                        adapter.IPAddress = ipv4Address.Address.ToString();
                        adapter.SubnetMask = ipv4Address.IPv4Mask?.ToString();
                    }

                    var stats = ni.GetIPv4Statistics();
                    adapter.BytesSent = stats.BytesSent;
                    adapter.BytesReceived = stats.BytesReceived;

                    deviceInfo.NetworkAdapters.Add(adapter);
                }
            }

            // Get public IP address
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                deviceInfo.PublicIPAddress = await client.GetStringAsync("https://api.ipify.org");
                deviceInfo.IsConnectedToInternet = !string.IsNullOrEmpty(deviceInfo.PublicIPAddress);
            }
            catch
            {
                deviceInfo.IsConnectedToInternet = false;
            }

            // Get local IP and gateway
            var activeAdapter = networkInterfaces
                .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up && 
                                     ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
            
            if (activeAdapter != null)
            {
                var ipProps = activeAdapter.GetIPProperties();
                var unicastAddresses = ipProps.UnicastAddresses;
                var ipv4Address = unicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                
                if (ipv4Address != null)
                {
                    deviceInfo.LocalIPAddress = ipv4Address.Address.ToString();
                }

                var gateway = ipProps.GatewayAddresses.FirstOrDefault();
                if (gateway != null)
                {
                    deviceInfo.DefaultGateway = gateway.Address.ToString();
                }

                deviceInfo.DNSServers = ipProps.DnsAddresses
                    .Where(dns => dns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(dns => dns.ToString())
                    .ToList();
            }

            // Test network latency to Google DNS
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync("8.8.8.8", 3000);
                if (reply.Status == IPStatus.Success)
                {
                    deviceInfo.NetworkLatencyMs = reply.RoundtripTime;
                }
            }
            catch
            {
                deviceInfo.NetworkLatencyMs = -1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting network information");
        }
    }

    private async Task CollectGraphicsInfoAsync(DeviceInfoExtended deviceInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                var adapter = new GraphicsAdapter
                {
                    Name = obj["Name"]?.ToString(),
                    Manufacturer = obj["AdapterCompatibility"]?.ToString(),
                    MemoryGB = Math.Round((uint)(obj["AdapterRAM"] ?? 0U) / (1024.0 * 1024.0 * 1024.0), 2),
                    DriverVersion = obj["DriverVersion"]?.ToString()
                };

                if (DateTime.TryParse(obj["DriverDate"]?.ToString(), out DateTime driverDate))
                {
                    adapter.DriverDate = driverDate;
                }

                deviceInfo.GraphicsAdapters.Add(adapter);
            }

            // Get monitor information
            using var monitorSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
            foreach (ManagementObject obj in monitorSearcher.Get())
            {
                var monitor = new Monitor
                {
                    Name = obj["Name"]?.ToString(),
                    Manufacturer = obj["MonitorManufacturer"]?.ToString(),
                    SerialNumber = obj["SerialNumber"]?.ToString()
                };

                if (uint.TryParse(obj["ScreenWidth"]?.ToString(), out uint width) &&
                    uint.TryParse(obj["ScreenHeight"]?.ToString(), out uint height))
                {
                    monitor.Resolution = $"{width}x{height}";
                }

                deviceInfo.Monitors.Add(monitor);
            }

            // Get primary display resolution
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var width = GetSystemMetrics(0); // SM_CXSCREEN
                var height = GetSystemMetrics(1); // SM_CYSCREEN
                if (width > 0 && height > 0)
                {
                    deviceInfo.PrimaryDisplayResolution = $"{width}x{height}";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting graphics information");
        }
    }

    private async Task CollectPerformanceInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            // Get CPU utilization
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // First call returns 0
            await Task.Delay(1000);
            deviceInfo.CPUUtilizationPercent = Math.Round(cpuCounter.NextValue(), 2);

            // Get system uptime
            deviceInfo.SystemUptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            deviceInfo.LastBootTime = DateTime.Now - deviceInfo.SystemUptime;

            // Try to get CPU temperature (Windows only, requires WMI access)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var temp = Convert.ToDouble(obj["CurrentTemperature"]);
                        deviceInfo.CPUTemperature = Math.Round((temp / 10.0) - 273.15, 1); // Convert from Kelvin to Celsius
                        break;
                    }
                }
                catch
                {
                    // Temperature monitoring may not be available on all systems
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting performance information");
        }
    }

    private async Task CollectSoftwareInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            // Get running processes (top 20 by memory usage)
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                .OrderByDescending(p => p.WorkingSet64)
                .Take(20);

            foreach (var process in processes)
            {
                try
                {
                    var runningProcess = new RunningProcess
                    {
                        Name = process.ProcessName,
                        ProcessId = process.Id,
                        MemoryMB = Math.Round(process.WorkingSet64 / (1024.0 * 1024.0), 2),
                        StartTime = process.StartTime,
                        UserName = GetProcessOwner(process.Id)
                    };

                    deviceInfo.RunningProcesses.Add(runningProcess);
                }
                catch
                {
                    // Some processes may not be accessible
                }
            }

            // Get installed software (Windows only)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CollectInstalledSoftware(deviceInfo);
                CollectSystemServices(deviceInfo);
                CollectStartupPrograms(deviceInfo);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting software information");
        }
    }

    private void CollectInstalledSoftware(DeviceInfoExtended deviceInfo)
    {
        try
        {
            var uninstallKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var keyPath in uninstallKeys)
            {
                using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                if (key == null) continue;

                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using var subkey = key.OpenSubKey(subkeyName);
                    if (subkey == null) continue;

                    var displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName)) continue;

                    var software = new InstalledSoftware
                    {
                        Name = displayName,
                        Version = subkey.GetValue("DisplayVersion") as string,
                        Publisher = subkey.GetValue("Publisher") as string,
                        InstallLocation = subkey.GetValue("InstallLocation") as string
                    };

                    if (DateTime.TryParse(subkey.GetValue("InstallDate") as string, out DateTime installDate))
                    {
                        software.InstallDate = installDate;
                    }

                    if (double.TryParse(subkey.GetValue("EstimatedSize") as string, out double sizeKB))
                    {
                        software.SizeMB = Math.Round(sizeKB / 1024.0, 2);
                    }

                    deviceInfo.InstalledPrograms.Add(software);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting installed software information");
        }
    }

    private async Task CollectUserInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            deviceInfo.CurrentlyLoggedInUser = Environment.UserName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserAccount WHERE LocalAccount = True");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var userAccount = new UserAccount
                    {
                        Username = obj["Name"]?.ToString(),
                        FullName = obj["FullName"]?.ToString(),
                        IsEnabled = !bool.Parse(obj["Disabled"]?.ToString() ?? "false"),
                        IsAdmin = bool.Parse(obj["LocalAccount"]?.ToString() ?? "false")
                    };

                    deviceInfo.UserAccounts.Add(userAccount);
                }

                // Get active sessions
                using var sessionSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogonSession");
                foreach (ManagementObject obj in sessionSearcher.Get())
                {
                    var logonType = obj["LogonType"]?.ToString();
                    if (logonType == "2" || logonType == "10") // Interactive or RemoteInteractive
                    {
                        var session = new ActiveSession
                        {
                            SessionType = logonType == "2" ? "Local" : "Remote",
                            LoginTime = DateTime.FromFileTime(long.Parse(obj["StartTime"]?.ToString() ?? "0"))
                        };

                        deviceInfo.ActiveSessions.Add(session);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting user information");
        }
    }

    private async Task CollectSecurityInfoAsync(DeviceInfoExtended deviceInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        try
        {
            // Check Windows Defender status
            using var defenderSearcher = new ManagementObjectSearcher(@"root\Microsoft\Windows\Defender", "SELECT * FROM MSFT_MpComputerStatus");
            foreach (ManagementObject obj in defenderSearcher.Get())
            {
                deviceInfo.WindowsDefenderEnabled = bool.Parse(obj["AntivirusEnabled"]?.ToString() ?? "false");
                deviceInfo.AntivirusStatus = obj["QuickScanAge"]?.ToString();
                break;
            }

            // Check firewall status
            using var firewallSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Name = 'MpsSvc'");
            foreach (ManagementObject obj in firewallSearcher.Get())
            {
                deviceInfo.FirewallEnabled = obj["State"]?.ToString() == "Running";
                break;
            }

            // Check UAC status
            using var uacKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
            if (uacKey != null)
            {
                var enableLUA = uacKey.GetValue("EnableLUA");
                deviceInfo.UACEnabled = enableLUA?.ToString() == "1";
            }

            // Check BitLocker status
            try
            {
                using var bitlockerSearcher = new ManagementObjectSearcher(@"root\CIMV2\Security\MicrosoftVolumeEncryption", "SELECT * FROM Win32_EncryptableVolume");
                foreach (ManagementObject obj in bitlockerSearcher.Get())
                {
                    var protectionStatus = obj["ProtectionStatus"]?.ToString();
                    deviceInfo.BitLockerEnabled = protectionStatus == "1";
                    break; // Check only the first volume (typically C:)
                }
            }
            catch
            {
                // BitLocker WMI may not be available on all Windows versions
            }

            // Get domain status
            try
            {
                using var computerSystem = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in computerSystem.Get())
                {
                    var domain = obj["Domain"]?.ToString();
                    var workgroup = obj["Workgroup"]?.ToString();
                    deviceInfo.DomainStatus = !string.IsNullOrEmpty(domain) ? $"Domain: {domain}" : $"Workgroup: {workgroup}";
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting domain status");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting security information");
        }
    }

    private async Task CollectRemoteAccessInfoAsync(DeviceInfoExtended deviceInfo)
    {
        try
        {
            // Check RDP status
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var rdpKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server");
                if (rdpKey != null)
                {
                    var rdpEnabled = rdpKey.GetValue("fDenyTSConnections");
                    deviceInfo.RDPEnabled = rdpEnabled?.ToString() == "0";
                }

                using var portKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp");
                if (portKey != null)
                {
                    var port = portKey.GetValue("PortNumber");
                    if (int.TryParse(port?.ToString(), out int rdpPort))
                    {
                        deviceInfo.RDPPort = rdpPort;
                    }
                }

                // Check for common remote access software
                var remoteAccessSoftware = new List<string>();
                
                // Check TeamViewer
                if (Directory.Exists(@"C:\Program Files\TeamViewer") || Directory.Exists(@"C:\Program Files (x86)\TeamViewer"))
                {
                    deviceInfo.TeamViewerInstalled = true;
                    remoteAccessSoftware.Add("TeamViewer");
                }

                // Check AnyDesk
                if (Directory.Exists(@"C:\Program Files\AnyDesk") || Directory.Exists(@"C:\Program Files (x86)\AnyDesk") || 
                    Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnyDesk")))
                {
                    deviceInfo.AnyDeskInstalled = true;
                    remoteAccessSoftware.Add("AnyDesk");
                }

                // Check for other remote access tools
                var commonRemoteTools = new[]
                {
                    "Chrome Remote Desktop",
                    "VNC",
                    "LogMeIn",
                    "Splashtop",
                    "RemotePC",
                    "GoToMyPC"
                };

                // This is a simplified check - in reality, you'd want more sophisticated detection
                foreach (var tool in commonRemoteTools)
                {
                    // Check in installed programs
                    if (deviceInfo.InstalledPrograms.Any(p => p.Name?.Contains(tool, StringComparison.OrdinalIgnoreCase) == true))
                    {
                        remoteAccessSoftware.Add(tool);
                    }
                }

                deviceInfo.RemoteAccessSoftware = remoteAccessSoftware;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting remote access information");
        }
    }

    private void CollectSystemServices(DeviceInfoExtended deviceInfo)
    {
        try
        {
            var services = ServiceController.GetServices()
                .Take(50) // Limit to top 50 services
                .Select(service => new SystemService
                {
                    Name = service.ServiceName,
                    DisplayName = service.DisplayName,
                    Status = service.Status.ToString(),
                    StartType = service.StartType.ToString()
                });

            deviceInfo.SystemServices.AddRange(services);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting system services information");
        }
    }

    private void CollectStartupPrograms(DeviceInfoExtended deviceInfo)
    {
        try
        {
            var startupKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
            };

            foreach (var keyPath in startupKeys)
            {
                using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                if (key == null) continue;

                foreach (var valueName in key.GetValueNames())
                {
                    var command = key.GetValue(valueName) as string;
                    if (string.IsNullOrEmpty(command)) continue;

                    var startupProgram = new StartupProgram
                    {
                        Name = valueName,
                        Command = command,
                        Location = "Registry (Machine)",
                        IsEnabled = true
                    };

                    deviceInfo.StartupPrograms.Add(startupProgram);
                }
            }

            // Also check user startup programs
            using var userKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (userKey != null)
            {
                foreach (var valueName in userKey.GetValueNames())
                {
                    var command = userKey.GetValue(valueName) as string;
                    if (string.IsNullOrEmpty(command)) continue;

                    var startupProgram = new StartupProgram
                    {
                        Name = valueName,
                        Command = command,
                        Location = "Registry (User)",
                        IsEnabled = true
                    };

                    deviceInfo.StartupPrograms.Add(startupProgram);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error collecting startup programs information");
        }
    }

    private string GetPlatformString()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "macOS";
        return "Unknown";
    }

    private string GetProcessOwner(int processId)
    {
        try
        {
            var query = $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}";
            using var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj in searcher.Get())
            {
                var ownerInfo = new string[2];
                obj.InvokeMethod("GetOwner", ownerInfo);
                return $"{ownerInfo[1]}\\{ownerInfo[0]}";
            }
        }
        catch
        {
            // Ignore errors
        }
        return "Unknown";
    }

    // Windows API declarations
    [StructLayout(LayoutKind.Sequential)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(MEMORYSTATUSEX lpBuffer);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}
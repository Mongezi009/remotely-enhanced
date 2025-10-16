import React, { useState, useEffect } from 'react';
import {
  Card,
  CardHeader,
  CardContent,
  CardActions,
  Grid,
  Typography,
  Chip,
  Button,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Tabs,
  Tab,
  Box,
  LinearProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Tooltip,
  Badge,
  Alert,
  Snackbar
} from '@mui/material';
import {
  Computer,
  Memory,
  Storage,
  NetworkWifi,
  Security,
  Person,
  Settings,
  Refresh,
  PowerSettingsNew,
  RemoteControlCamera,
  Chat,
  Upload,
  Download,
  ExpandMore,
  Info,
  Warning,
  CheckCircle,
  Error as ErrorIcon,
  Speed,
  Thermostat,
  MonitorHeart
} from '@mui/icons-material';

interface DeviceInfoExtended {
  // Basic Info
  deviceName: string;
  platform: string;
  osDescription: string;
  isOnline: boolean;
  lastSeen: string;
  agentVersion: string;
  
  // Hardware Info
  processorName: string;
  processorManufacturer: string;
  processorSpeedGHz: number;
  processorCount: number;
  motherboardManufacturer: string;
  motherboardModel: string;
  biosVersion: string;
  
  // Memory Info
  totalPhysicalMemoryGB: number;
  usedMemoryGB: number;
  availableMemoryGB: number;
  memoryModules: MemoryModule[];
  
  // Storage Info
  storageDevices: StorageDevice[];
  logicalDrives: Drive[];
  
  // Network Info
  networkAdapters: NetworkAdapter[];
  publicIPAddress: string;
  localIPAddress: string;
  isConnectedToInternet: boolean;
  networkLatencyMs: number;
  
  // Graphics Info
  graphicsAdapters: GraphicsAdapter[];
  monitors: Monitor[];
  primaryDisplayResolution: string;
  
  // Performance
  cpuUtilizationPercent: number;
  gpuUtilizationPercent: number;
  cpuTemperature: number;
  systemUptime: string;
  
  // Software Info
  installedPrograms: InstalledSoftware[];
  runningProcesses: RunningProcess[];
  systemServices: SystemService[];
  
  // Security Info
  firewallEnabled: boolean;
  windowsDefenderEnabled: boolean;
  uacEnabled: boolean;
  bitLockerEnabled: boolean;
  
  // Remote Access Info
  rdpEnabled: boolean;
  rdpPort: number;
  remoteAccessSoftware: string[];
  
  // User Info
  currentlyLoggedInUser: string;
  userAccounts: UserAccount[];
  activeSessions: ActiveSession[];
}

interface MemoryModule {
  manufacturer: string;
  capacityGB: number;
  speedMHz: number;
  slot: string;
}

interface StorageDevice {
  model: string;
  capacityGB: number;
  isSSD: boolean;
  healthPercentage: number;
  temperatureCelsius: number;
}

interface NetworkAdapter {
  name: string;
  macAddress: string;
  isConnected: boolean;
  speedMbps: number;
  ipAddress: string;
}

interface GraphicsAdapter {
  name: string;
  memoryGB: number;
  driverVersion: string;
}

interface Monitor {
  name: string;
  resolution: string;
  isPrimary: boolean;
}

interface InstalledSoftware {
  name: string;
  version: string;
  publisher: string;
  sizeMB: number;
}

interface RunningProcess {
  name: string;
  processId: number;
  memoryMB: number;
  cpuPercent: number;
  userName: string;
}

interface SystemService {
  name: string;
  displayName: string;
  status: string;
}

interface UserAccount {
  username: string;
  isEnabled: boolean;
  isAdmin: boolean;
  lastLogin: string;
}

interface ActiveSession {
  username: string;
  sessionType: string;
  loginTime: string;
}

interface Drive {
  name: string;
  totalSize: number;
  freeSpace: number;
  driveType: string;
}

const DeviceDashboard: React.FC<{ device: DeviceInfoExtended }> = ({ device }) => {
  const [selectedTab, setSelectedTab] = useState(0);
  const [detailsDialog, setDetailsDialog] = useState<string | null>(null);
  const [refreshing, setRefreshing] = useState(false);
  const [notification, setNotification] = useState<{ message: string; severity: 'success' | 'error' | 'warning' | 'info' } | null>(null);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedTab(newValue);
  };

  const handleRefresh = async () => {
    setRefreshing(true);
    try {
      // Call refresh API
      await new Promise(resolve => setTimeout(resolve, 2000)); // Simulate API call
      setNotification({ message: 'Device information updated', severity: 'success' });
    } catch (error) {
      setNotification({ message: 'Failed to refresh device information', severity: 'error' });
    } finally {
      setRefreshing(false);
    }
  };

  const handleRemoteControl = () => {
    // Implement remote control logic
    console.log('Starting remote control...');
  };

  const handleFileTransfer = () => {
    // Implement file transfer logic
    console.log('Starting file transfer...');
  };

  const getStatusColor = (isOnline: boolean) => {
    return isOnline ? 'success' : 'error';
  };

  const getPercentageColor = (percentage: number) => {
    if (percentage < 50) return 'success';
    if (percentage < 80) return 'warning';
    return 'error';
  };

  const formatBytes = (bytes: number) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  const formatUptime = (uptime: string) => {
    // Format uptime string to be more readable
    return uptime;
  };

  const TabPanel: React.FC<{ children: React.ReactNode; value: number; index: number }> = ({ children, value, index }) => (
    <div hidden={value !== index}>
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );

  return (
    <Box sx={{ width: '100%', bgcolor: 'background.paper' }}>
      {/* Header Card */}
      <Card sx={{ mb: 3, background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' }}>
        <CardContent sx={{ color: 'white' }}>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} md={8}>
              <Typography variant="h4" component="h1" gutterBottom>
                <Computer sx={{ mr: 2, verticalAlign: 'middle' }} />
                {device.deviceName}
              </Typography>
              <Grid container spacing={2}>
                <Grid item>
                  <Chip
                    icon={<PowerSettingsNew />}
                    label={device.isOnline ? 'Online' : 'Offline'}
                    color={getStatusColor(device.isOnline)}
                    variant="outlined"
                    sx={{ color: 'white', borderColor: 'white' }}
                  />
                </Grid>
                <Grid item>
                  <Chip
                    label={device.platform}
                    variant="outlined"
                    sx={{ color: 'white', borderColor: 'white' }}
                  />
                </Grid>
                <Grid item>
                  <Chip
                    label={`Agent v${device.agentVersion}`}
                    variant="outlined"
                    sx={{ color: 'white', borderColor: 'white' }}
                  />
                </Grid>
              </Grid>
              <Typography variant="body2" sx={{ mt: 1, opacity: 0.9 }}>
                Last seen: {new Date(device.lastSeen).toLocaleString()}
              </Typography>
            </Grid>
            <Grid item xs={12} md={4}>
              <Box sx={{ textAlign: 'right' }}>
                <Button
                  variant="contained"
                  color="primary"
                  startIcon={<RemoteControlCamera />}
                  sx={{ mr: 1, mb: 1 }}
                  onClick={handleRemoteControl}
                  disabled={!device.isOnline}
                >
                  Remote Control
                </Button>
                <Button
                  variant="contained"
                  color="secondary"
                  startIcon={<Upload />}
                  sx={{ mr: 1, mb: 1 }}
                  onClick={handleFileTransfer}
                  disabled={!device.isOnline}
                >
                  File Transfer
                </Button>
                <IconButton
                  color="inherit"
                  onClick={handleRefresh}
                  disabled={refreshing}
                  sx={{ mb: 1 }}
                >
                  <Refresh />
                </IconButton>
              </Box>
            </Grid>
          </Grid>
        </CardContent>
        {refreshing && <LinearProgress />}
      </Card>

      {/* Quick Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Speed color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">CPU</Typography>
              </Box>
              <Typography variant="h4" color={getPercentageColor(device.cpuUtilizationPercent)}>
                {device.cpuUtilizationPercent.toFixed(1)}%
              </Typography>
              <LinearProgress
                variant="determinate"
                value={device.cpuUtilizationPercent}
                color={getPercentageColor(device.cpuUtilizationPercent)}
                sx={{ mt: 1 }}
              />
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                {device.processorName}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Memory color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">Memory</Typography>
              </Box>
              <Typography variant="h4" color={getPercentageColor((device.usedMemoryGB / device.totalPhysicalMemoryGB) * 100)}>
                {((device.usedMemoryGB / device.totalPhysicalMemoryGB) * 100).toFixed(1)}%
              </Typography>
              <LinearProgress
                variant="determinate"
                value={(device.usedMemoryGB / device.totalPhysicalMemoryGB) * 100}
                color={getPercentageColor((device.usedMemoryGB / device.totalPhysicalMemoryGB) * 100)}
                sx={{ mt: 1 }}
              />
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                {device.usedMemoryGB.toFixed(1)} / {device.totalPhysicalMemoryGB.toFixed(1)} GB
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Storage color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">Storage</Typography>
              </Box>
              {device.logicalDrives.length > 0 && (
                <>
                  <Typography variant="h4" color={getPercentageColor(((device.logicalDrives[0].totalSize - device.logicalDrives[0].freeSpace) / device.logicalDrives[0].totalSize) * 100)}>
                    {(((device.logicalDrives[0].totalSize - device.logicalDrives[0].freeSpace) / device.logicalDrives[0].totalSize) * 100).toFixed(1)}%
                  </Typography>
                  <LinearProgress
                    variant="determinate"
                    value={((device.logicalDrives[0].totalSize - device.logicalDrives[0].freeSpace) / device.logicalDrives[0].totalSize) * 100}
                    color={getPercentageColor(((device.logicalDrives[0].totalSize - device.logicalDrives[0].freeSpace) / device.logicalDrives[0].totalSize) * 100)}
                    sx={{ mt: 1 }}
                  />
                  <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                    {(device.logicalDrives[0].totalSize - device.logicalDrives[0].freeSpace).toFixed(1)} / {device.logicalDrives[0].totalSize.toFixed(1)} GB
                  </Typography>
                </>
              )}
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <NetworkWifi color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">Network</Typography>
              </Box>
              <Typography variant="h4" color={device.isConnectedToInternet ? 'success.main' : 'error.main'}>
                {device.isConnectedToInternet ? 'Connected' : 'Offline'}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                {device.networkLatencyMs > 0 ? `${device.networkLatencyMs}ms latency` : 'No latency data'}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {device.publicIPAddress || 'No public IP'}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Detailed Information Tabs */}
      <Card>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={selectedTab} onChange={handleTabChange} variant="scrollable" scrollButtons="auto">
            <Tab icon={<Computer />} label="Hardware" />
            <Tab icon={<Memory />} label="Memory" />
            <Tab icon={<Storage />} label="Storage" />
            <Tab icon={<NetworkWifi />} label="Network" />
            <Tab icon={<MonitorHeart />} label="Performance" />
            <Tab icon={<Settings />} label="Software" />
            <Tab icon={<Person />} label="Users" />
            <Tab icon={<Security />} label="Security" />
          </Tabs>
        </Box>

        {/* Hardware Tab */}
        <TabPanel value={selectedTab} index={0}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Processor Information" />
                <CardContent>
                  <Typography><strong>Name:</strong> {device.processorName}</Typography>
                  <Typography><strong>Manufacturer:</strong> {device.processorManufacturer}</Typography>
                  <Typography><strong>Speed:</strong> {device.processorSpeedGHz.toFixed(2)} GHz</Typography>
                  <Typography><strong>Cores:</strong> {device.processorCount}</Typography>
                  {device.cpuTemperature > 0 && (
                    <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
                      <Thermostat color={device.cpuTemperature > 70 ? 'error' : 'primary'} sx={{ mr: 1 }} />
                      <Typography>Temperature: {device.cpuTemperature}°C</Typography>
                    </Box>
                  )}
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="System Information" />
                <CardContent>
                  <Typography><strong>Motherboard:</strong> {device.motherboardManufacturer} {device.motherboardModel}</Typography>
                  <Typography><strong>BIOS:</strong> {device.biosVersion}</Typography>
                  <Typography><strong>OS:</strong> {device.osDescription}</Typography>
                  <Typography><strong>Uptime:</strong> {formatUptime(device.systemUptime)}</Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="Graphics Information" />
                <CardContent>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Graphics Card</TableCell>
                          <TableCell>Memory</TableCell>
                          <TableCell>Driver Version</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.graphicsAdapters.map((adapter, index) => (
                          <TableRow key={index}>
                            <TableCell>{adapter.name}</TableCell>
                            <TableCell>{adapter.memoryGB.toFixed(1)} GB</TableCell>
                            <TableCell>{adapter.driverVersion}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Memory Tab */}
        <TabPanel value={selectedTab} index={1}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={8}>
              <Card variant="outlined">
                <CardHeader title="Memory Modules" />
                <CardContent>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Slot</TableCell>
                          <TableCell>Manufacturer</TableCell>
                          <TableCell>Capacity</TableCell>
                          <TableCell>Speed</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.memoryModules.map((module, index) => (
                          <TableRow key={index}>
                            <TableCell>{module.slot}</TableCell>
                            <TableCell>{module.manufacturer}</TableCell>
                            <TableCell>{module.capacityGB} GB</TableCell>
                            <TableCell>{module.speedMHz} MHz</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={4}>
              <Card variant="outlined">
                <CardHeader title="Memory Usage" />
                <CardContent>
                  <Box sx={{ mb: 2 }}>
                    <Typography>Physical Memory</Typography>
                    <LinearProgress
                      variant="determinate"
                      value={(device.usedMemoryGB / device.totalPhysicalMemoryGB) * 100}
                      sx={{ mb: 1 }}
                    />
                    <Typography variant="body2">
                      {device.usedMemoryGB.toFixed(1)} / {device.totalPhysicalMemoryGB.toFixed(1)} GB
                    </Typography>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Storage Tab */}
        <TabPanel value={selectedTab} index={2}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="Physical Storage Devices" />
                <CardContent>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Device</TableCell>
                          <TableCell>Type</TableCell>
                          <TableCell>Capacity</TableCell>
                          <TableCell>Health</TableCell>
                          <TableCell>Temperature</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.storageDevices.map((storage, index) => (
                          <TableRow key={index}>
                            <TableCell>{storage.model}</TableCell>
                            <TableCell>
                              <Chip
                                label={storage.isSSD ? 'SSD' : 'HDD'}
                                color={storage.isSSD ? 'primary' : 'default'}
                                size="small"
                              />
                            </TableCell>
                            <TableCell>{storage.capacityGB.toFixed(0)} GB</TableCell>
                            <TableCell>
                              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                {storage.healthPercentage >= 90 ? (
                                  <CheckCircle color="success" sx={{ mr: 1 }} />
                                ) : storage.healthPercentage >= 70 ? (
                                  <Warning color="warning" sx={{ mr: 1 }} />
                                ) : (
                                  <ErrorIcon color="error" sx={{ mr: 1 }} />
                                )}
                                {storage.healthPercentage}%
                              </Box>
                            </TableCell>
                            <TableCell>
                              {storage.temperatureCelsius > 0 ? `${storage.temperatureCelsius}°C` : 'N/A'}
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="Logical Drives" />
                <CardContent>
                  <Grid container spacing={2}>
                    {device.logicalDrives.map((drive, index) => (
                      <Grid item xs={12} sm={6} md={4} key={index}>
                        <Card variant="outlined">
                          <CardContent>
                            <Typography variant="h6">{drive.name}</Typography>
                            <Typography color="text.secondary">{drive.driveType}</Typography>
                            <LinearProgress
                              variant="determinate"
                              value={((drive.totalSize - drive.freeSpace) / drive.totalSize) * 100}
                              sx={{ my: 1 }}
                            />
                            <Typography variant="body2">
                              {(drive.totalSize - drive.freeSpace).toFixed(1)} / {drive.totalSize.toFixed(1)} GB
                            </Typography>
                          </CardContent>
                        </Card>
                      </Grid>
                    ))}
                  </Grid>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Network Tab */}
        <TabPanel value={selectedTab} index={3}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Network Status" />
                <CardContent>
                  <Grid container spacing={2}>
                    <Grid item xs={12}>
                      <Alert severity={device.isConnectedToInternet ? "success" : "error"}>
                        Internet Connection: {device.isConnectedToInternet ? "Connected" : "Disconnected"}
                      </Alert>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography><strong>Public IP:</strong></Typography>
                      <Typography>{device.publicIPAddress || 'N/A'}</Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography><strong>Local IP:</strong></Typography>
                      <Typography>{device.localIPAddress || 'N/A'}</Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography><strong>Latency:</strong></Typography>
                      <Typography>{device.networkLatencyMs > 0 ? `${device.networkLatencyMs}ms` : 'N/A'}</Typography>
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="Network Adapters" />
                <CardContent>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Name</TableCell>
                          <TableCell>Status</TableCell>
                          <TableCell>IP Address</TableCell>
                          <TableCell>Speed</TableCell>
                          <TableCell>MAC Address</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.networkAdapters.map((adapter, index) => (
                          <TableRow key={index}>
                            <TableCell>{adapter.name}</TableCell>
                            <TableCell>
                              <Chip
                                label={adapter.isConnected ? 'Connected' : 'Disconnected'}
                                color={adapter.isConnected ? 'success' : 'default'}
                                size="small"
                              />
                            </TableCell>
                            <TableCell>{adapter.ipAddress || 'N/A'}</TableCell>
                            <TableCell>{adapter.speedMbps > 0 ? `${adapter.speedMbps} Mbps` : 'N/A'}</TableCell>
                            <TableCell><code>{adapter.macAddress}</code></TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Performance Tab */}
        <TabPanel value={selectedTab} index={4}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="System Performance" />
                <CardContent>
                  <Box sx={{ mb: 3 }}>
                    <Typography>CPU Utilization</Typography>
                    <LinearProgress
                      variant="determinate"
                      value={device.cpuUtilizationPercent}
                      color={getPercentageColor(device.cpuUtilizationPercent)}
                      sx={{ mb: 1 }}
                    />
                    <Typography>{device.cpuUtilizationPercent.toFixed(1)}%</Typography>
                  </Box>

                  {device.gpuUtilizationPercent > 0 && (
                    <Box sx={{ mb: 3 }}>
                      <Typography>GPU Utilization</Typography>
                      <LinearProgress
                        variant="determinate"
                        value={device.gpuUtilizationPercent}
                        color={getPercentageColor(device.gpuUtilizationPercent)}
                        sx={{ mb: 1 }}
                      />
                      <Typography>{device.gpuUtilizationPercent.toFixed(1)}%</Typography>
                    </Box>
                  )}

                  <Typography><strong>System Uptime:</strong> {formatUptime(device.systemUptime)}</Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Top Processes" />
                <CardContent>
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Process</TableCell>
                          <TableCell>Memory</TableCell>
                          <TableCell>CPU</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.runningProcesses.slice(0, 10).map((process, index) => (
                          <TableRow key={index}>
                            <TableCell>{process.name}</TableCell>
                            <TableCell>{process.memoryMB.toFixed(1)} MB</TableCell>
                            <TableCell>{process.cpuPercent.toFixed(1)}%</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Software Tab */}
        <TabPanel value={selectedTab} index={5}>
          <Accordion>
            <AccordionSummary expandIcon={<ExpandMore />}>
              <Typography variant="h6">Installed Software ({device.installedPrograms.length})</Typography>
            </AccordionSummary>
            <AccordionDetails>
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Name</TableCell>
                      <TableCell>Version</TableCell>
                      <TableCell>Publisher</TableCell>
                      <TableCell>Size</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {device.installedPrograms.slice(0, 50).map((software, index) => (
                      <TableRow key={index}>
                        <TableCell>{software.name}</TableCell>
                        <TableCell>{software.version}</TableCell>
                        <TableCell>{software.publisher}</TableCell>
                        <TableCell>{software.sizeMB > 0 ? `${software.sizeMB.toFixed(1)} MB` : 'N/A'}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </AccordionDetails>
          </Accordion>

          <Accordion sx={{ mt: 2 }}>
            <AccordionSummary expandIcon={<ExpandMore />}>
              <Typography variant="h6">System Services ({device.systemServices.length})</Typography>
            </AccordionSummary>
            <AccordionDetails>
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Name</TableCell>
                      <TableCell>Display Name</TableCell>
                      <TableCell>Status</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {device.systemServices.map((service, index) => (
                      <TableRow key={index}>
                        <TableCell>{service.name}</TableCell>
                        <TableCell>{service.displayName}</TableCell>
                        <TableCell>
                          <Chip
                            label={service.status}
                            color={service.status === 'Running' ? 'success' : 'default'}
                            size="small"
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </AccordionDetails>
          </Accordion>
        </TabPanel>

        {/* Users Tab */}
        <TabPanel value={selectedTab} index={6}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Current User" />
                <CardContent>
                  <Typography variant="h6">{device.currentlyLoggedInUser}</Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="User Accounts" />
                <CardContent>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Username</TableCell>
                          <TableCell>Status</TableCell>
                          <TableCell>Admin</TableCell>
                          <TableCell>Last Login</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {device.userAccounts.map((user, index) => (
                          <TableRow key={index}>
                            <TableCell>{user.username}</TableCell>
                            <TableCell>
                              <Chip
                                label={user.isEnabled ? 'Enabled' : 'Disabled'}
                                color={user.isEnabled ? 'success' : 'default'}
                                size="small"
                              />
                            </TableCell>
                            <TableCell>
                              {user.isAdmin && <CheckCircle color="primary" />}
                            </TableCell>
                            <TableCell>{user.lastLogin ? new Date(user.lastLogin).toLocaleDateString() : 'Never'}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card variant="outlined">
                <CardHeader title="Active Sessions" />
                <CardContent>
                  {device.activeSessions.length === 0 ? (
                    <Typography color="text.secondary">No active sessions</Typography>
                  ) : (
                    <TableContainer>
                      <Table>
                        <TableHead>
                          <TableRow>
                            <TableCell>Username</TableCell>
                            <TableCell>Session Type</TableCell>
                            <TableCell>Login Time</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {device.activeSessions.map((session, index) => (
                            <TableRow key={index}>
                              <TableCell>{session.username}</TableCell>
                              <TableCell>
                                <Chip
                                  label={session.sessionType}
                                  color={session.sessionType === 'Local' ? 'primary' : 'secondary'}
                                  size="small"
                                />
                              </TableCell>
                              <TableCell>{new Date(session.loginTime).toLocaleString()}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  )}
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Security Tab */}
        <TabPanel value={selectedTab} index={7}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Security Status" />
                <CardContent>
                  <Grid container spacing={2}>
                    <Grid item xs={12}>
                      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                        {device.firewallEnabled ? (
                          <CheckCircle color="success" sx={{ mr: 1 }} />
                        ) : (
                          <ErrorIcon color="error" sx={{ mr: 1 }} />
                        )}
                        <Typography>Windows Firewall: {device.firewallEnabled ? 'Enabled' : 'Disabled'}</Typography>
                      </Box>
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                        {device.windowsDefenderEnabled ? (
                          <CheckCircle color="success" sx={{ mr: 1 }} />
                        ) : (
                          <ErrorIcon color="error" sx={{ mr: 1 }} />
                        )}
                        <Typography>Windows Defender: {device.windowsDefenderEnabled ? 'Enabled' : 'Disabled'}</Typography>
                      </Box>
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                        {device.uacEnabled ? (
                          <CheckCircle color="success" sx={{ mr: 1 }} />
                        ) : (
                          <Warning color="warning" sx={{ mr: 1 }} />
                        )}
                        <Typography>UAC: {device.uacEnabled ? 'Enabled' : 'Disabled'}</Typography>
                      </Box>
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                        {device.bitLockerEnabled ? (
                          <CheckCircle color="success" sx={{ mr: 1 }} />
                        ) : (
                          <Info color="info" sx={{ mr: 1 }} />
                        )}
                        <Typography>BitLocker: {device.bitLockerEnabled ? 'Enabled' : 'Disabled'}</Typography>
                      </Box>
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardHeader title="Remote Access" />
                <CardContent>
                  <Box sx={{ mb: 2 }}>
                    <Typography><strong>RDP Status:</strong> {device.rdpEnabled ? 'Enabled' : 'Disabled'}</Typography>
                    {device.rdpEnabled && (
                      <Typography><strong>RDP Port:</strong> {device.rdpPort}</Typography>
                    )}
                  </Box>
                  
                  {device.remoteAccessSoftware.length > 0 && (
                    <Box>
                      <Typography><strong>Remote Access Software:</strong></Typography>
                      <Box sx={{ mt: 1 }}>
                        {device.remoteAccessSoftware.map((software, index) => (
                          <Chip key={index} label={software} sx={{ mr: 1, mb: 1 }} />
                        ))}
                      </Box>
                    </Box>
                  )}
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>
      </Card>

      {/* Notification Snackbar */}
      <Snackbar
        open={notification !== null}
        autoHideDuration={4000}
        onClose={() => setNotification(null)}
      >
        {notification && (
          <Alert severity={notification.severity} onClose={() => setNotification(null)}>
            {notification.message}
          </Alert>
        )}
      </Snackbar>
    </Box>
  );
};

export default DeviceDashboard;
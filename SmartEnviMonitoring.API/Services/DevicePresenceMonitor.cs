using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.API.Hubs;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.Common.Clients;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.API.Services;

public class DevicePresenceOptions
{
    public int OfflineTimeoutSeconds { get; set; } = 60;
    public int CheckIntervalSeconds { get; set; } = 10;
}

public class DevicePresenceMonitor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILoginDevicesService _loginDevicesService;
    private readonly IHubContext<DeviceHub> _deviceHubContext;
    private readonly IMapper _mapper;
    private readonly DevicePresenceOptions _options;

    public DevicePresenceMonitor(
        IServiceScopeFactory scopeFactory,
        ILoginDevicesService loginDevicesService,
        IHubContext<DeviceHub> deviceHubContext,
        IMapper mapper,
        IOptions<DevicePresenceOptions> options)
    {
        _scopeFactory = scopeFactory;
        _loginDevicesService = loginDevicesService;
        _deviceHubContext = deviceHubContext;
        _mapper = mapper;
        _options = options.Value ?? new DevicePresenceOptions();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan offlineTimeout = NormalizeSeconds(_options.OfflineTimeoutSeconds, 60);
        TimeSpan checkInterval = NormalizeSeconds(_options.CheckIntervalSeconds, 10);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckOfflineDevicesAsync(offlineTimeout, stoppingToken);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Device presence monitor failed.");
            }

            await Task.Delay(checkInterval, stoppingToken);
        }
    }

    private async Task CheckOfflineDevicesAsync(TimeSpan offlineTimeout, CancellationToken token)
    {
        if (_loginDevicesService.Devices.IsEmpty)
        {
            return;
        }

        DateTime cutoff = DateTime.Now.Subtract(offlineTimeout);
        List<string> offlineIds = new List<string>();

        foreach (var entry in _loginDevicesService.Devices)
        {
            MonitoringDevice device = entry.Value;
            if (device.LastLoginTimestamp <= cutoff)
            {
                offlineIds.Add(entry.Key);
            }
        }

        if (offlineIds.Count == 0)
        {
            return;
        }

        using IServiceScope scope = _scopeFactory.CreateScope();
        IDeviceRepository deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        foreach (string deviceUID in offlineIds)
        {
            if (_loginDevicesService.Devices.TryRemove(deviceUID, out _))
            {
                MonitoringDevice device = deviceRepository.FindDevice(deviceUID);
                if (device != null)
                {
                    device.State = DeviceState.Offline;
                    await deviceRepository.UpdateAsync(device);
                }
            }
        }

        await _deviceHubContext.Clients.All.SendAsync(
            SignalEvents.DevicesUpdated.ToString(),
            _loginDevicesService.Devices.Values.Select(d => _mapper.Map<DeviceDto>(d)).ToArray(),
            token);
    }

    private static TimeSpan NormalizeSeconds(int value, int fallbackSeconds)
    {
        if (value <= 0)
        {
            return TimeSpan.FromSeconds(fallbackSeconds);
        }
        return TimeSpan.FromSeconds(value);
    }
}

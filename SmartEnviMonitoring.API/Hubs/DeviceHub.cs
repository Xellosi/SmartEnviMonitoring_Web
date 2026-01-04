using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SmartEnviMonitoring.API.Services;
using SmartEnviMonitoring.Common.Clients;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.API.Hubs;

public class DeviceHub : Hub
{
    public static HashSet<string> ConnectedIds = new HashSet<string>();
    private readonly ILoginDevicesService _loginDevicesService;
    private readonly IMapper _mapper;

    public DeviceHub(ILoginDevicesService loginDevicesService, IMapper mapper)
    {
        _loginDevicesService = loginDevicesService;
        _mapper = mapper;
    }
    public async Task LoginDevicesChanged(List<DeviceDto> devices)
    {
        await Clients.All.SendAsync(SignalEvents.DevicesUpdated.ToString(), devices);
    }

    public async Task NewMeasurementArrived(List<WeatherReportDto> reports)
    {
        await Clients.All.SendAsync(SignalEvents.MeasurementArrival.ToString(), reports);
    }

    public override async Task OnConnectedAsync()
    {
        lock(ConnectedIds){
            ConnectedIds.Add(Context.ConnectionId);
        }
        await Clients.Caller.SendAsync(
            SignalEvents.DevicesUpdated.ToString(),
            _loginDevicesService.Devices.Values.Select(d => _mapper.Map<DeviceDto>(d)).ToArray());
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        lock(ConnectedIds){
            ConnectedIds.Remove(Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
}

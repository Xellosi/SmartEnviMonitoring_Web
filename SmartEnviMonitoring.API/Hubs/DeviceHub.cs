using Microsoft.AspNetCore.SignalR;
using SmartEnviMonitoring.Common.Model;
using SmartEnviMonitoring.Common.Clients;

namespace SmartEnviMonitoring.API.Hubs;

public class DeviceHub : Hub
{
    public static HashSet<string> ConnectedIds = new HashSet<string>();
    public async Task LoginDevicesChanged(List<DeviceDto> devices)
    {
        await Clients.All.SendAsync(SignalEvents.DevicesUpdated.ToString(), devices);
    }

    public async Task NewMeasurementArrived(List<WeatherReportDto> reports)
    {
        await Clients.All.SendAsync(SignalEvents.MeasurementArrival.ToString(), reports);
    }

    public override Task OnConnectedAsync()
    {
        lock(ConnectedIds){
            ConnectedIds.Add(Context.ConnectionId);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        lock(ConnectedIds){
            ConnectedIds.Remove(Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
}

using Microsoft.AspNetCore.SignalR.Client;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.Common.Clients;

public class DeviceHubClient : IAsyncDisposable
{
    public const string HUBURL = "/DeviceHub";
    public EventHandler<LoginDevicesChangedEventArgs> LoginDevicesChanged;
    public EventHandler<ReportArrivalEventArgs> WeatherReportArrival;
    private readonly string _hubUrl;
    private HubConnection _hubConnection;
    public bool Started { get; private set; } = false;

    public DeviceHubClient(string siteUrl)
    {
        if (string.IsNullOrWhiteSpace(siteUrl))
            throw new ArgumentNullException(nameof(siteUrl));
        _hubUrl = siteUrl.TrimEnd('/') + HUBURL;
    }

    /// <summary>
    /// Start the SignalR client 
    /// </summary>
    public async Task StartAsync()
    {
        if (Started){
            return;
        }
        
        _hubConnection = new HubConnectionBuilder()
        .WithUrl(_hubUrl)
        .Build();

        _hubConnection.On<List<DeviceDto>>(
            SignalEvents.DevicesUpdated.ToString(), HandleLoginDevicesChanged);

        _hubConnection.On<WeatherReportDto>(
            SignalEvents.MeasurementArrival.ToString(), HandleWeatherReportArrival);

        await _hubConnection.StartAsync();

        Console.WriteLine("Client Started.");
        Started = true;
    }

    /// <summary>
    /// Stop the client (if started)
    /// </summary>
    public async Task StopAsync()
    {
        if (!Started){
            return;
        }

        if (_hubConnection != null){
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();   
        }

        _hubConnection = null;
        Started = false;
        Console.WriteLine("Client stopped.");
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }

    private void HandleLoginDevicesChanged(List<DeviceDto> dtos)
    {
        LoginDevicesChanged?.Invoke(this, new LoginDevicesChangedEventArgs(dtos.ToArray()));
    }

    private void HandleWeatherReportArrival(WeatherReportDto dto)
    {
        WeatherReportArrival?.Invoke(this, new ReportArrivalEventArgs(dto));
    }
}

public class LoginDevicesChangedEventArgs : EventArgs
{
    public DeviceDto[] Devices { get; private set; }
    public LoginDevicesChangedEventArgs(DeviceDto[] devices)
    {
        Devices = devices;
    }
}

public class ReportArrivalEventArgs : EventArgs
{
    public WeatherReportDto Report { get; private set; }
    public ReportArrivalEventArgs(WeatherReportDto dto)
    {
        Report = dto;
    }
}
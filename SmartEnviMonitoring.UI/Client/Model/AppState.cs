using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using SmartEnviMonitoring.Common.Clients;

namespace SmartEnviMonitoring.UI.Client.Model;

public class AppState
{
    public int ReportMaxinum { get; set; } = 10;
    public int ReportCount { 
        get{
            return _reportCount;
        } 
    }
    private int _reportCount = 0;
    public ConcurrentQueue<WeatherReportDetail> Reports { get; private set; } = 
    new ConcurrentQueue<WeatherReportDetail>();
    public List<UI.DeviceDto> Devices = new List<UI.DeviceDto>();
    
    public DeviceHubClient? HubClient = null;
    public string ApiBaseUrl { get; }
    public string HubBaseUrl { get; }

    public AppState(IConfiguration configuration)
    {
        ApiBaseUrl = configuration["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Api:BaseUrl is not configured.");
        HubBaseUrl = configuration["Hub:BaseUrl"]
            ?? throw new InvalidOperationException("Hub:BaseUrl is not configured.");
    }

    public async Task StartHubAsync(){
        if (HubClient == null){
            HubClient = new DeviceHubClient(HubBaseUrl);
        }
        await HubClient.StartAsync();
    }

    public void AddWeatherReport(Common.Model.WeatherReportDto dto){
        if (dto == null){
            return;
        }
        WeatherReportDetail wrd = new WeatherReportDetail(dto, 
                Interlocked.Increment(ref _reportCount));

        Reports.Enqueue(wrd);
        lock (Reports){
            if (wrd.Index > ReportMaxinum){
                Reports.TryDequeue(out _);
            }
        }
    }
}

using System.Collections.Concurrent;
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

    public AppState()
    {

    }

    public async Task StartHubAsync(){
        if (HubClient == null){
            HubClient = new DeviceHubClient("http://127.0.0.1");
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
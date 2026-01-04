
namespace SmartEnviMonitoring.UI.Client.Model;
public class WeatherReportDetail : WeatherReportDto
    {
        public int Index { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;

        public WeatherReportDetail(){}

        public WeatherReportDetail(Common.Model.WeatherReportDto dto, int count){
            if (dto != null){
                TemperatureC = dto.TemperatureC;
                Humidity = dto.Humidity;
                DeviceUID = dto.DeviceUID;
                Index = count;
            }
        }
    }
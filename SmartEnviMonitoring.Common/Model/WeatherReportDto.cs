using System.Text;
using SmartEnviMonitoring.Common.Configurations;

namespace SmartEnviMonitoring.Common.Model;

public class WeatherReportDto
{
    public const char Separator = ',';
    public double TemperatureC { get; set; }
    public double Humidity { get; set; }
    public string DeviceUID { get; set; } = string.Empty;

    public WeatherReportDto() : this(string.Empty){

    }

    public WeatherReportDto(string s){
        WeatherReportDto dto = TryParse(s);
        if (dto != null){

        }
    }

    public static WeatherReportDto TryParse(string s){
        try {
            string[] sections = s.Split(',');
            DateTime t = DateTime.ParseExact(sections[0], CommonConfig.TimeFormat, null);
            double temp = double.Parse(sections[1]);
            double humidity = double.Parse(sections[2]);
            return new WeatherReportDto{
                TemperatureC = temp,
                Humidity = humidity,
            };
        }
        catch {
            return null;
        }
    }

    public string Serialize(){

        StringBuilder builder = new StringBuilder();
        builder.Append(TemperatureC.ToString("F3"));
        builder.Append(Separator);
        builder.Append(Humidity.ToString("F3"));
        return builder.ToString();
    }
}
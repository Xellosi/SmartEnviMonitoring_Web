using SmartEnviMonitoring.API.Data.Monitoring;

namespace SmartEnviMonitoring.API.Repositories;
public interface IWeatherRepository : IGenericRepository<MeasurementRecord>
{
    IEnumerable<MeasurementRecord> GetRecordsByDevice(string deviceUID);
}
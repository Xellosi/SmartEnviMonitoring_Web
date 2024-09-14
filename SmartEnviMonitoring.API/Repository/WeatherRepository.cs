using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.Data;

namespace SmartEnviMonitoring.API.Repositories;


public class WeatherRepository : GenericRepository<MeasurementRecord>, IWeatherRepository
{
    public WeatherRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        
    }

    public IEnumerable<MeasurementRecord> GetRecordsByDevice(string deviceUID)
    {
        return _context.MeasRecords.Where(r => r.Source.DeviceUID == deviceUID);
    }
}
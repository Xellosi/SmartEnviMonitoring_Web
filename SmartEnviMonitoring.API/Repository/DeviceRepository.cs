using AutoMapper;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.Data;

public class DeviceRepository : GenericRepository<MonitoringDevice>, IDeviceRepository
{
    public DeviceRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        
    }

    public MonitoringDevice FindDevice(string uid)
    {
        return _context.Devices.FirstOrDefault(d => d.DeviceUID == uid);
    }
}
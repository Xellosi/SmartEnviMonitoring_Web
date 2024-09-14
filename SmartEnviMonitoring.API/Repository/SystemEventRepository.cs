using AutoMapper;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.Data;

public class SystemEventRepository : GenericRepository<SystemEvent>, ISystemEventRepository
{
    public SystemEventRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        
    }
}
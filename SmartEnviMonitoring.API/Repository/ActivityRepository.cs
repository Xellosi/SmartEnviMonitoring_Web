using AutoMapper;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.Data;

namespace SmartEnviMonitoring.API.Repositories;

public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
{
    public ActivityRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        
    }
}
using AutoMapper;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.Data;

namespace SmartEnviMonitoring.API.Repositories;
public class AudioRecordRepository : GenericRepository<AudioRecord>, IAudioRecordRepository
{
    public AudioRecordRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
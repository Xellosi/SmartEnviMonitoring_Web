using AutoMapper;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.API.Configurations;
public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<WeatherReportDto, MeasurementRecord>().ReverseMap();
        CreateMap<MonitoringDevice, DeviceDto>().ReverseMap();
    }
}
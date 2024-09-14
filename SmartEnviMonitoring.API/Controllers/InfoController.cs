using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using SmartEnviMonitoring.API.Configurations;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.API.Services;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.API.Controllers;

[Route("api/info")]
[ApiController]
[AllowAnonymous]
public class InfoController : ControllerBase
{
    private readonly ISystemEventRepository _eventRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IWeatherRepository _weatherRepository;
    private readonly ILoginDevicesService _loginDevicesService;
    private readonly IMapper _mapper;
    public InfoController(ISystemEventRepository eventRepository,
    IActivityRepository activityRepository, IWeatherRepository weatherRepository,
    ILoginDevicesService loginDevicesService,
    IMapper mapper)
    {
        _eventRepository = eventRepository;
        _activityRepository = activityRepository;
        _weatherRepository = weatherRepository;
        _loginDevicesService = loginDevicesService;
        _mapper = mapper;
    }

    [HttpGet("logindevices")]
    public async Task<ActionResult<List<DeviceDto>>> LoginDevicesAsync(){
        return await Task.Run(()=> {
            return _loginDevicesService.Devices.Values.
            Select(d => _mapper.Map<DeviceDto>(d)).ToList();
        });
    }

    [HttpGet("lastmeasurements")]
    public async Task<List<WeatherReportDto>> LastMeasurementsAsync([FromQuery]int num = 10)
    {
        List<MeasurementRecord> records = await _weatherRepository.GetLastNRecordsAsync(num);
        List<WeatherReportDto> dtos = records.Select(
            r => _mapper.Map<MeasurementRecord, WeatherReportDto>(r)).ToList();
        return dtos;
    }
}
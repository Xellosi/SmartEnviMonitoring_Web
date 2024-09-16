using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartEnviMonitoring.API.Data.Communication;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.API.Hubs;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.API.Services;
using SmartEnviMonitoring.Common.Clients;
using SmartEnviMonitoring.Common.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartEnviMonitoring.API.Controllers;

[Route("api/weather")]
[ApiController]
[AllowAnonymous]
public class WeatherController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILoginDevicesService _loginDevicesService;
    private IHubContext<DeviceHub> _deviceHubContext;
    private readonly HttpResBuilder _commandBuilder;

    public WeatherController(IWeatherRepository weatherRepository,
    IMapper mapper, IWebHostEnvironment webHostEnvironment, 
    IHubContext<DeviceHub> deviceHubContext,
    HttpResBuilder commandBuilder, IDeviceRepository deviceRepository,
    ILoginDevicesService loginDevicesService)
    {
        this._weatherRepository = weatherRepository;
        this._deviceRepository = deviceRepository;

        this._mapper = mapper;
        this._webHostEnvironment = webHostEnvironment;
        this._commandBuilder = commandBuilder;

        this._deviceHubContext = deviceHubContext;
        this._loginDevicesService = loginDevicesService;
    }

    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<string>> Report([FromQuery]WeatherReportDto dto)
    {
        string key = "report";
        MeasurementRecord record = _mapper.Map<MeasurementRecord>(dto);

        if (string.IsNullOrWhiteSpace(dto.DeviceUID)){
            Log.Error("device id empty");
            return _commandBuilder.PostResponse(key, CommandResult.Error);
        }

        MonitoringDevice device = _deviceRepository.FindDevice(dto.DeviceUID);
        if (device == null){
            Log.Error($"Unknown device {dto.DeviceUID}");   
            return _commandBuilder.PostResponse(key, CommandResult.Error);
        }

        _loginDevicesService.Devices.TryAdd(dto.DeviceUID, device);
        
        record.Source = device;
        record.Timestamp = DateTime.Now;
        record = await _weatherRepository.AddAsync(record);

        Console.WriteLine(DeviceHub.ConnectedIds.Count());
        await _deviceHubContext.Clients.All.SendAsync(
            SignalEvents.MeasurementArrival.ToString(), dto);

        Log.Information($"{dto.DeviceUID} report tempc: {dto.TemperatureC}, humidity: {dto.Humidity}.");
        return _commandBuilder.PostResponse(key, CommandResult.Successful);
    }

    [HttpGet("{deviceUID}")]
    public Task<IEnumerable<MeasurementRecord>> GetRecordByDeviceAsync(string deviceUID)
    {
        return Task.Run(() => {
            IEnumerable<MeasurementRecord> records = _weatherRepository.GetRecordsByDevice(deviceUID);
            return records;
        });
    }
}
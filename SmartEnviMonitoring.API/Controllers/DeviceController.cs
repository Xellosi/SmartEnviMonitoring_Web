using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartEnviMonitoring.API.Configurations;
using SmartEnviMonitoring.API;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.API.Data.System;
using System.Collections.Concurrent;
using SmartEnviMonitoring.API.Data;
using SmartEnviMonitoring.API.Data.Communication;
using System.Diagnostics.Tracing;
using SmartEnviMonitoring.API.Services;
using SmartEnviMonitoring.Common.Model;
using SmartEnviMonitoring.Common.Configurations;
using Microsoft.AspNetCore.SignalR;
using SmartEnviMonitoring.API.Hubs;
using SmartEnviMonitoring.Common.Clients;
using AutoMapper;


namespace SmartEnviMonitoring.API.Controllers;

[Route("api/device")]
[ApiController]
[AllowAnonymous]
public class DeviceController : ControllerBase
{
    private readonly ISystemEventRepository _eventRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILoginDevicesService _loginDevicesService;

    private IHubContext<DeviceHub> _deviceHubContext;

    private IMapper _mapper;
    private readonly MqttManager _mqttManager;
    private readonly HttpResBuilder _commandBuilder;

    public DeviceController(ISystemEventRepository eventRepository, 
    IActivityRepository activityRepository, IDeviceRepository deviceRepository,
    ILoginDevicesService loginDevicesService, IHubContext<DeviceHub> deviceHubContext,
    IMapper mapper, MqttManager mqttManager, HttpResBuilder commandBuilder)
    {
        _eventRepository = eventRepository;
        _activityRepository = activityRepository;
        _deviceRepository = deviceRepository;

        _loginDevicesService = loginDevicesService;

        _deviceHubContext = deviceHubContext;

        _mapper = mapper;
        _mqttManager = mqttManager;
        _commandBuilder = commandBuilder;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromQuery]string deviceUID)
    {
        Log.Information($"{nameof(LoginAsync)}");
        string responseKey = "login";
        if (string.IsNullOrWhiteSpace(deviceUID)){
            Log.Error($"arg {nameof(deviceUID)} null.");
            return _commandBuilder.PostResponse(responseKey, CommandResult.Error);
        }

        MonitoringDevice device = _deviceRepository.FindDevice(deviceUID);

        if (device == null){
            Log.Error($"Unknown Device {deviceUID}");
            return _commandBuilder.PostResponse(responseKey, CommandResult.Error);
        }

        device.LastLoginTimestamp = DateTime.Now;
        device.State = DeviceState.Online;
        if (_loginDevicesService.Devices.TryAdd(deviceUID, device)){
            try{
                string msg = $"device {deviceUID} login.";
                await _eventRepository.AddAsync(new SystemEvent(EventLevel.Informational, msg));
                await _deviceRepository.UpdateAsync(device);
                msg += "success.";
                Log.Information(msg);
            }
            catch(Exception exc){
                Log.Error(exc, $"device {deviceUID} Db update error.");
                return _commandBuilder.PostResponse(responseKey, CommandResult.Error);
            }
            await _deviceHubContext.Clients.All.SendAsync(SignalEvents.DevicesUpdated.ToString(), 
            _loginDevicesService.Devices.Values.Select(d => _mapper.Map<DeviceDto>(d)).ToArray());
        }
        else {
            string msg = $"device {deviceUID} login repeat.";
            await _eventRepository.AddAsync(new SystemEvent(EventLevel.Informational, msg));
            await _deviceRepository.UpdateAsync(device);
            msg += "success.";
            Log.Information(msg);
        }

        return _commandBuilder.PostResponse(responseKey, CommandResult.Successful);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<string>> LogoutAsync([FromQuery]string deviceUID)
    {
        Log.Information($"{nameof(LogoutAsync)}");
        string responseKey = "logout";
        if (string.IsNullOrWhiteSpace(deviceUID)){
            Log.Error($"arg {nameof(deviceUID)} null.");
            return $"arg {nameof(deviceUID)} null.";
        }

        MonitoringDevice device;

        if (_loginDevicesService.Devices.TryRemove(deviceUID, out device)){
            try{
                string msg = $"device {deviceUID} logout";
                await _eventRepository.AddAsync(new SystemEvent(EventLevel.Informational, msg));
                device.LastLoginTimestamp = DateTime.Now;
                device.State = DeviceState.Offline;
                await _deviceRepository.UpdateAsync(device);
                msg += "success.";
                Log.Information(msg);
            }
            catch(Exception exc){
                Log.Error(exc, $"device {deviceUID} Db update error.");
                return _commandBuilder.PostResponse(responseKey, CommandResult.Error);
            }
            await _deviceHubContext.Clients.All.SendAsync(SignalEvents.DevicesUpdated.ToString(), 
            _loginDevicesService.Devices.Values.Select(d => _mapper.Map<DeviceDto>(d)).ToArray());
        }
        else {
            string msg = $"device {deviceUID} not login.";
            Log.Error(msg);
            await _eventRepository.AddAsync(new SystemEvent(EventLevel.Informational, msg));
            return _commandBuilder.PostResponse(responseKey, CommandResult.Error);
        }

        return $"{DateTime.Now.ToString(CommonConfig.TimeFormat)}";
    }
    
    [HttpGet("timecurrent")]
    public Task<ActionResult<string>> GetCurrentTimeAsync([FromQuery]string deviceUID){
        return Task.Run(() => {
            Log.Information($"device {deviceUID} {nameof(GetCurrentTimeAsync)}");
            return (ActionResult<string>)_commandBuilder.PostResponseCurrentTime(DateTime.Now);
        });
    }
}
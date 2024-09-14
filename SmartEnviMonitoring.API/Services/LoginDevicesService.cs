using System.Collections.Concurrent;
using SmartEnviMonitoring.API.Data.System;

namespace SmartEnviMonitoring.API.Services;

public interface ILoginDevicesService{
    ConcurrentDictionary<string, MonitoringDevice> Devices { get;}
}

public class LoginDevicesService : ILoginDevicesService{
    public ConcurrentDictionary<string, MonitoringDevice> Devices { get; protected set;} = 
    new ConcurrentDictionary<string, MonitoringDevice>();
}
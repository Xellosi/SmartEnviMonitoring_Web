using SmartEnviMonitoring.API.Data.System;

namespace SmartEnviMonitoring.API.Repositories;
public interface IDeviceRepository: IGenericRepository<MonitoringDevice>{
    MonitoringDevice FindDevice(string uid);
}
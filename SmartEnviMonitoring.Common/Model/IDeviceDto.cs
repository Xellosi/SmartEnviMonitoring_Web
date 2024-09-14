namespace SmartEnviMonitoring.Common.Model;

public interface IDeviceDto
{
    string DeviceUID { get; }
    DeviceState State { get; }
    DateTime LastLoginTimestamp { get; }
}
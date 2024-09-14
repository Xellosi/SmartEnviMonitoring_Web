using System.ComponentModel;

namespace SmartEnviMonitoring.Common.Model;


public class DeviceDto : IDeviceDto
{
    public string DeviceUID { get; set; } = string.Empty;
    [DefaultValue(DeviceState.Offline)]
    public DeviceState State { get; set; }
    public DateTime LastLoginTimestamp { get; set; }
}
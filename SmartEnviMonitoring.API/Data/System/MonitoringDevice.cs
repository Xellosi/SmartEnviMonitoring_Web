using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartEnviMonitoring.Common.Model;

namespace SmartEnviMonitoring.API.Data.System;

[Index(nameof(DeviceUID), IsUnique = true)]
public class MonitoringDevice : IDeviceDto
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string DeviceUID { get; set; } = string.Empty;
    [DefaultValue(DeviceState.Offline)]
    public DeviceState State { get; set; }
    public DateTime LastLoginTimestamp { get; set; }
}
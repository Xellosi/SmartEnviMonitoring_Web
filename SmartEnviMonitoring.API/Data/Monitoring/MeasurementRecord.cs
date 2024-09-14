using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SmartEnviMonitoring.API.Data.System;

namespace SmartEnviMonitoring.API.Data.Monitoring;

public class MeasurementRecord
{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double TemperatureC { get; set; }
    public double Humidity { get; set; }
    public virtual MonitoringDevice? Source { get; set; }
}
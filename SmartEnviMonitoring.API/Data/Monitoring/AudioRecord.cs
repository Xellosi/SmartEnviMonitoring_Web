using System.ComponentModel.DataAnnotations;
using SmartEnviMonitoring.API.Data.System;

namespace SmartEnviMonitoring.API.Data.Monitoring;

public class AudioRecord{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string File { get; set; }
    public virtual MonitoringDevice? Source { get; set; }
    public string Result { get; set; }
}
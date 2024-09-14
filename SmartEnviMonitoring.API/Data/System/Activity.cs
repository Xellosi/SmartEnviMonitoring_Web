namespace SmartEnviMonitoring.API.Data.System;
public class Activity
{
    public int Id { get; set; }
    public DateTime Timestamp {get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
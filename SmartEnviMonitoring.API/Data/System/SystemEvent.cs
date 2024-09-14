using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace SmartEnviMonitoring.API.Data.System;

public class SystemEvent{
    [Key]
    public int Id { get; private set; }
    public EventLevel EventSeverity { get; private set; } = EventLevel.Informational;
    public DateTime Timestamp {get; private set; } = DateTime.Now;
    public string Source { get; private set;} = string.Empty;
    public string Message { get; private set; } = string.Empty;

    public SystemEvent(){

    }

    public SystemEvent(EventLevel severity,  string message, [CallerMemberName]string source = ""){
        EventSeverity = severity;
        this.Source = source;
        this.Message = message;
    }
}
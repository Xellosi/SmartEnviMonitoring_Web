using Microsoft.OpenApi.Extensions;

namespace SmartEnviMonitoring.API.Data.Communication;

public enum CommandResult{
    [NameInMessage("succ")]
    Successful,
    [NameInMessage("error")]
    Error,
    [NameInMessage("timeout")]
    Timeout,


    [NameInMessage("unknown")]
    Unknown = 999,
}

public enum CommandType{
    [NameInMessage("get")]
    GetConfig,
    [NameInMessage("set")]
    SetConfig,
    [NameInMessage("rec")]
    StartRecording,
    [NameInMessage("ping")]
    Ping,

    [NameInMessage("unknown")]
    Unknown = 999,
}

public enum MQTTMessageType{
    Request,
    Result,
    
    Unknown,
}

public class CommSetting{
    public const char MsgStart = '#';
    public const char FieldSeperater = '_';
    public const char KeyValueSeperater = '@';
    public const char MsgEnd = ';';

    public static string GetNameInMessage(CommandResult c){
        return c.GetAttributeOfType<NameInMessageAttribute>().Name;
    }

    public static string GetNameInMessage(CommandType c){
        return c.GetAttributeOfType<NameInMessageAttribute>().Name;
    }

    public static CommandResult ParseResult(string m){
        foreach(CommandResult r in Enum.GetValues(typeof(CommandResult))){
            if(r.GetAttributeOfType<NameInMessageAttribute>()?.Name == m){
                return r;
            }
        }
        return CommandResult.Unknown;
    }

    public static CommandType ParseType(string m){
        foreach(CommandType r in Enum.GetValues(typeof(CommandType))){
            if(r.GetAttributeOfType<NameInMessageAttribute>()?.Name == m){
                return r;
            }
        }
        return CommandType.Unknown;
    }
}

public class MQTTCommSetting{
    public const string ReqPostfix = "_req";
    public const string ResPostfix = "_res";
    public static MQTTMessageType AnalysisTopic(string topic, out string device)
    {
        if (topic.EndsWith(ReqPostfix)){
            device = topic.Replace(ReqPostfix, string.Empty);
            return MQTTMessageType.Request;
        }
        else if(topic.EndsWith(ResPostfix)){
            device = topic.Replace(ResPostfix, string.Empty);
            return MQTTMessageType.Result;
        }
        device = string.Empty;
        return MQTTMessageType.Unknown;
    }
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class NameInMessageAttribute : Attribute
{
    public string Name
    {
        get { return _name; }
    }
    protected readonly string _name;
    
    // This is a positional argument
    public NameInMessageAttribute(string name)
    {
        this._name = name;
    }
}
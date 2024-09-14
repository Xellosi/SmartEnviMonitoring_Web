using System.Text;

namespace SmartEnviMonitoring.API.Data.Communication;

public interface IMQTTCommandBase{
    string DeviceUID { get; }
    string CommandUID { get; }
    MQTTMessageType MessageType { get; }
    CommandType Command { get; }
    string Payload { get;}
    string ToMessage();
}

public class MQTTCommandBase : IMQTTCommandBase
{
    public string DeviceUID { get; set;}
    public string CommandUID { get; set;}
    public MQTTMessageType MessageType { get; set;}
    public CommandType Command { get; set;}
    public string Payload { get; set;}
    public MQTTCommandBase(){}

    public static MQTTCommandBase TryParse(string topic, string msg){
        string deviceUID;
        MQTTMessageType msgType = MQTTCommSetting.AnalysisTopic(topic, out deviceUID);
        if (msgType == MQTTMessageType.Unknown){
            return null;
        }
        string temp = msg;
        if (temp[0] != CommSetting.MsgStart || 
            temp[temp.Length - 1] != CommSetting.MsgEnd){
            return null;
        }
        temp.Replace($"{CommSetting.MsgStart}", string.Empty);
        temp.Replace($"{CommSetting.MsgStart}", string.Empty);
        string[] fields = temp.Split(CommSetting.FieldSeperater);
        string cmduid = fields[0];
        CommandType CommandType = CommSetting.ParseType(fields[1]);
        string payload = fields[2];

        return new MQTTCommandBase(){
            DeviceUID = deviceUID,
            CommandUID = cmduid,
            MessageType = msgType,
            Command = CommandType,
            Payload = payload,
        };
    }

    public string ToMessage()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(CommSetting.MsgStart);
        builder.Append(DeviceUID);
        builder.Append(CommSetting.FieldSeperater);
        builder.Append(CommSetting.GetNameInMessage(Command));
        builder.Append(CommSetting.FieldSeperater);
        builder.Append(Payload);
        builder.Append(CommSetting.MsgEnd);
        return builder.ToString();
    }
    public override string ToString() => ToMessage();
}
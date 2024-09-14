namespace SmartEnviMonitoring.API.Data.Communication;

//format: #{key}@{value};
public class HttpResMessage{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public HttpResMessage(){}

    public HttpResMessage(string key, string value){
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return $"{CommSetting.MsgStart}{Key}{CommSetting.KeyValueSeperater}{Value}{CommSetting.MsgEnd}";
    }

    public void Parse(string msg){
        if (!msg.StartsWith(CommSetting.MsgStart) ||
            !msg.EndsWith(CommSetting.MsgEnd)){
                throw new FormatException($"{msg} format error.");
        }
        msg = msg.Remove(0, 1);
        msg = msg.Remove(msg.Length - 1, 1);
        string[] fields = msg.Split(new char[] { CommSetting.KeyValueSeperater });
        Key = fields[0];
        Value = fields[1];
    }

    public bool TryParse(string msg, out Exception exc){
        try {
            Parse(msg);
            exc = null;
            return true;
        }catch (Exception e){
            exc = e;
            return false;
        }
    }
}
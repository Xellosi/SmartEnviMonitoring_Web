using SmartEnviMonitoring.API.Data.Communication;

namespace SmartEnviMonitoring.API.Data.Communication;

public class RequestGetConfig : MQTTCommandBase, IMQTTCommand
{
    public string BuildPayLoad()
    {
        throw new NotImplementedException();
    }

    public void HandleResponse(IMQTTCommand command)
    {
        throw new NotImplementedException();
    }
}
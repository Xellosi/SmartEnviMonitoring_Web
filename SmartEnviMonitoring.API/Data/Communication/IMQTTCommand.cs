namespace SmartEnviMonitoring.API.Data.Communication;

public interface IMQTTCommand : IMQTTCommandBase
{
    string BuildPayLoad();
    void HandleResponse(IMQTTCommand command);
}
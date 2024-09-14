

using Serilog;

namespace SmartEnviMonitoring.API;
//  1. Add the interface `IHostedService` to the class you would like
//     to be called during an application event.
internal class LifetimeEventsHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private MqttManager _mqttManager = null;
    // 2. Inject `IHostApplicationLifetime` through dependency injection in the constructor.
    public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime, 
                                       MqttManager mqttManager)
    {
        _appLifetime = appLifetime;
        _mqttManager = mqttManager;
    }

    // 3. Implemented by `IHostedService`, setup here your event registration. 
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    // 4. Implemented by `IHostedService`, setup here your shutdown registration.
    //    If you have nothing to stop, then just return `Task.CompletedTask`
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        Log.Information("Server started.");
        try{
            _mqttManager?.StartAsync(CancellationToken.None).Wait();
        }catch(Exception exc){

        }
        // Perform post-startup activities here
    }

    private void OnStopping()
    {
        Log.Information("OnStopping has been called.");
        try{
            _mqttManager?.StopAsync(CancellationToken.None).Wait();
        }catch(Exception exc){

        }
        // Perform on-stopping activities here
    }

    private void OnStopped()
    {
        Log.Information("Server stopped.");

        // Perform post-stopped activities here
    }
}
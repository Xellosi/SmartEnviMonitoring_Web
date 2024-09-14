using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Windows.Input;
using Microsoft.AspNetCore.Identity;
using MQTTnet.Server;
using Serilog;
using SmartEnviMonitoring.API.Data.Communication;
using SmartEnviMonitoring.API.Data.System;

namespace SmartEnviMonitoring.API;

public class MqttManager : IHostedService, IDisposable{
    public enum MessageType{
        Request,
        Result,

        Unknown = 99,
    }

    private readonly MqttServer _mqttServer;
    private readonly HttpResBuilder _commandBuilder;
    private ConcurrentDictionary<string, List<IMQTTCommand>> SentCommandsById = 
    new ConcurrentDictionary<string, List<IMQTTCommand>>();
    
    public MqttManager(MqttServer server){
        _mqttServer = server;
        _commandBuilder = new HttpResBuilder();
        ServerSetup(server);
    }

    public bool SendRequest(MonitoringDevice device, IMQTTCommand command){
        return true;
    }


    private void ServerSetup(MqttServer server){
        if (server == null){
            Log.Error($"arg {nameof(server)} null");
            return;
        }
        
        server.ClientConnectedAsync += ClientConnectedAsync;
        server.ValidatingConnectionAsync += ValidatingConnectionAsync;
        server.ClientDisconnectedAsync += ClientDisconnectedAsync;
        server.ClientSubscribedTopicAsync += ClientSubscribedTopicAsync;
        server.ClientUnsubscribedTopicAsync += ClientUnsubscribedTopicAsync;
        server.InterceptingPublishAsync += InterceptoringPublishAsync;
    }
    private Task ClientConnectedAsync(ClientConnectedEventArgs args){
        Log.Information($"MQTT {args.ClientId} connected.");
        return Task.CompletedTask;
    }
    private Task ValidatingConnectionAsync(ValidatingConnectionEventArgs args){
        Log.Information($"MQTT {nameof(ValidatingConnectionAsync)} {args.ClientId}");
        return Task.CompletedTask;
    }
    private Task ClientDisconnectedAsync(ClientDisconnectedEventArgs args){
        Log.Information($"MQTT {args.ClientId} disconnected.");
        return Task.CompletedTask;
    }
    private Task ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs args){
        Log.Information($"MQTT {args.ClientId} subscribed to topic {args.TopicFilter.Topic}.");
        return Task.CompletedTask;
    }
    private Task ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs args){
        return Task.CompletedTask;
    }

    private Task InterceptoringPublishAsync(InterceptingPublishEventArgs args){
        return Task.Run(() => {
            string payloadMsg = System.Text.Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
            Log.Information($"MQTT {args.ClientId} publush {payloadMsg} to topic {args.ApplicationMessage.Topic}.");
            try {
                MessageType type = TryAnalysisData(args.ClientId, 
                args.ApplicationMessage.Topic, payloadMsg);
                if (type == MessageType.Unknown){
                    Log.Error($"{args.ClientId} {nameof(TryAnalysisData)} failed.");
                    return;
                }

                switch(type){
                    case MessageType.Request:
                    break;
                    case MessageType.Result:
                    HandleResultMessage(args.ClientId, type, payloadMsg);
                    break;
                    case MessageType.Unknown:
                    break;
                }
            }
            catch(Exception exc){
                Log.Error(exc, $"MQTT prcocessing {args.ClientId} publish failed.");
            }
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttServer != null){
            try {
                Log.Information($"MQTT Stopping MQTT server start.");
                await _mqttServer.StopAsync(new MqttServerStopOptions());
                Log.Information($"MQTT Stopping MQTT server end.");
            }
            catch(Exception exc){
                Log.Error(exc, $"MQTT Stopping MQTT server failed.");
            }
        }
    }

    private MessageType TryAnalysisData(string clientId, string topic, string payloadMsg)
    {
        if (topic.Contains(MQTTCommSetting.ReqPostfix)){
            return MessageType.Request;
        }
        else if(topic.Contains(MQTTCommSetting.ResPostfix)){
            return MessageType.Result;
        }
        return MessageType.Unknown;
    }

    private void HandleResultMessage(string clientId, MessageType type, string payloadMsg)
    {

    }

    public void Dispose()
    {

    }
}
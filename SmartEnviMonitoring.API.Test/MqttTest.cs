using System.Diagnostics;
using System.Net;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

namespace SmartEnviMonitoring.API.Test;
public class MqttTests{
    public void SimpleSubPubTest() {
        string topic = "topic_req";

        // Create a new MQTT client.
        var factory = new MqttFactory();
        IMqttClient client = factory.CreateMqttClient();
        MqttClientOptions options = new MqttClientOptionsBuilder()
        {
            
        }.
        WithTcpServer("localhost").Build();

        MqttClientConnectResult connectResult = client.ConnectAsync(options).Result;

        if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
        {
            return;
        }

        client.SubscribeAsync(topic).Wait();
        // Callback function when a message is received
        client.ApplicationMessageReceivedAsync += e =>
        {
            Debug.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };
        
        // Publish a message 10 times
        for (int i = 0; i < 10; i++)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload($"#2fc_ping_;")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            client.PublishAsync(message).Wait();
            Task.Delay(1000).Wait();
        }
        
        Thread.Sleep(1000000);
        // Unsubscribe and disconnect
        client.UnsubscribeAsync(topic).Wait();
        client.DisconnectAsync().Wait();
    }
}
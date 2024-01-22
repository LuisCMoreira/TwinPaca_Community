using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MqttService
{
    private readonly MqttClient mqttClient;
    private string latestJsonData;

    public MqttService(string brokerIpAddress, int brokerPort, string clientId, string topic)
    {
        mqttClient = new MqttClient(brokerIpAddress, brokerPort, false, null, null, MqttSslProtocols.None);

        try
        {
            mqttClient.Connect(clientId);

            mqttClient.MqttMsgPublishReceived += (sender, e) =>
            {
                string message = System.Text.Encoding.UTF8.GetString(e.Message);
                string receivedTopic = e.Topic;

                if (receivedTopic == topic)
                {
                    latestJsonData = message;
                    Console.WriteLine($"Received MQTT Message on topic '{receivedTopic}': {message}");
                }
            };

            mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            // Handle the exception as needed
        }
    }

    public string GetLatestJsonData()
    {
        return latestJsonData;
    }

    public void Dispose()
    {
        if (mqttClient.IsConnected)
        {
            mqttClient.Disconnect();
        }
    }
}

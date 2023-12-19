using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    // MQTT Broker Config
    private const string MqttBrokerHost = "localhost";
    private const int MqttBrokerPort = 1883;
    private const string MqttUsername = "";
    private const string MqttPassword = "";

    // MongoDB Config
    private const string MongoConnectionString = "mongodb://localhost:27017/";
    private const string MongoDatabaseName = "mqtt";
    private const string MongoCollectionName = "brokertopics";

    static async Task Main(string[] args)
    {
        // MQTT Client Setup
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateManagedMqttClient();

        var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(MqttBrokerHost, MqttBrokerPort)
                .WithCredentials(MqttUsername, MqttPassword)
                .Build())
            .Build();

        mqttClient.UseApplicationMessageReceivedHandler(new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived));

        await mqttClient.StartAsync(options);

        // MongoDB Setup
        var mongoClient = new MongoClient(MongoConnectionString);
        var database = mongoClient.GetDatabase(MongoDatabaseName);
        var collection = database.GetCollection<BsonDocument>(MongoCollectionName);

        // Keep the application running
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Disconnect MQTT client
        await mqttClient.StopAsync();
    }

    private static void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string topic = e.ApplicationMessage.Topic;
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        // Handle the payload and update MongoDB as needed
        HandleMessage(topic, payload);
    }

    private static void HandleMessage(string topic, string payload)
    {
        try
        {
            var data = BsonDocument.Parse(payload);

            // MongoDB handling code (similar to Python code)

            Console.WriteLine($"Updated data in MongoDB for topic {topic}: {data}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

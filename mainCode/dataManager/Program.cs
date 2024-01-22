using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    // MQTT Broker Config
    private const string MqttBrokerHost = "host.docker.internal";
    private const int MqttBrokerPort = 1884;
    private const string MqttUsername = null;
    private const string MqttPassword = null;

    // MongoDB Config
    private const string MongoConnectionString = "mongodb://host.docker.internal:27016/";
    private const string MongoDatabaseName = "mqtt";
    private const string MongoCollectionName = "brokertopics";

    private const string MongoStorageCollection = "telemetry";

    static async Task Main(string[] args)
    {
        // MQTT Client Setup
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(MqttBrokerHost, MqttBrokerPort)
            .WithCredentials(MqttUsername, MqttPassword)
            .WithCleanSession()
            .Build();


        mqttClient.UseConnectedHandler(e => SubscribeToTopicsAsync(mqttClient));
        mqttClient.UseApplicationMessageReceivedHandler(OnMessageReceived);

        await mqttClient.ConnectAsync(options);

        // MongoDB Setup
        var mongoClient = new MongoClient(MongoConnectionString);
        var database = mongoClient.GetDatabase(MongoDatabaseName);
        var collection = database.GetCollection<BsonDocument>(MongoCollectionName);

        while (true)
        {
            // Keep the application running
            //Console.WriteLine("Press Enter to exit.");
            //Console.ReadLine();
            Thread.Sleep(100);
        }

    }

    private static async Task SubscribeToTopicsAsync(IMqttClient mqttClient)
    {
        await mqttClient.SubscribeAsync("#");
        Console.WriteLine("Subscribed to all topics (#).");
    }

    private static void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string topic = e.ApplicationMessage.Topic;
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        Console.WriteLine($"Received message on topic {topic}: {payload}");

        // Handle the payload and update MongoDB as needed
        HandleMessage(topic, payload);
    }

    private static void HandleMessage(string topic, string payload)
    {
        try
        {
            var data = BsonDocument.Parse(payload);

            // MongoDB handling code (similar to Python code)
            UpdateMongoDB(topic, data);

            Console.WriteLine($"Updated data in MongoDB for topic {topic}: {data}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void UpdateMongoDB(string topic, BsonDocument data)
    {
        try
        {
            // MongoDB Setup
            var mongoClient = new MongoClient(MongoConnectionString);
            var database = mongoClient.GetDatabase(MongoDatabaseName);
            var collection = database.GetCollection<BsonDocument>(MongoCollectionName);

            // Find the existing document or create a new one
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "topic_values");
            var existingDocument = collection.Find(filter).FirstOrDefault() ?? new BsonDocument();

            // Update the existing JSON structure with the new data and updatedAt timestamp
            UpdateJsonStructure(topic, data, existingDocument);

            // Update the updatedAt timestamp
            UpdateTimestamp(topic, existingDocument);

            // Update the MongoDB document with the updated JSON structure
            collection.ReplaceOne(filter, existingDocument, new ReplaceOptions { IsUpsert = true });

            Console.WriteLine($"Updated data in MongoDB for topic {topic}: {data}");

            Console.WriteLine($"test topic {topic}");
            AppendValuesToCollection(MongoDatabaseName, "Telemetry", topic, data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MongoDB Error: {ex.Message}");
        }
    }

    private static void UpdateJsonStructure(string topic, BsonDocument data, BsonDocument jsonStructure)
    {
        var topicParts = topic.Split("/");
        var current = jsonStructure;

        foreach (var part in topicParts)
        {
            if (!current.Contains(part))
            {
                current.Add(part, new BsonDocument());
            }

            current = current[part].AsBsonDocument;

            if (part == topicParts[^1]) // Check if it's the last part of the topic
            {
                current["value"] = data;
            }
        }
    }

    private static void UpdateTimestamp(string topic, BsonDocument jsonStructure)
    {
        var current = jsonStructure;

        foreach (var part in topic.Split("/"))
        {
            current = current[part].AsBsonDocument;
        }

        current["updated_at"] = DateTime.UtcNow;
    }


    private static void AppendValuesToCollection(string dbName, string collectionName, string key1, BsonDocument jsonValue1)
    {
        try
        {
            // MongoDB Setup
            var mongoClient = new MongoClient(MongoConnectionString);
            var database = mongoClient.GetDatabase(dbName);
            var collection = database.GetCollection<BsonDocument>(collectionName);


            var bsonValue1 = (jsonValue1);

            // Create a document with the specified key-value pairs
            var documentToAppend = new BsonDocument
            {
                { key1, bsonValue1 },
                { "timestamp", DateTime.UtcNow }
                // Add other fields as needed
            };

            // Insert the document into the collection
            collection.InsertOne(documentToAppend);

            Console.WriteLine($"Appended values to MongoDB collection: {bsonValue1}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error appending values to MongoDB collection: {ex.Message}");
        }
    }

}

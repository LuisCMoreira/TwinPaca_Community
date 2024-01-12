using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using System.Security.Cryptography.X509Certificates;


namespace Magent
{
    class mqttPUB
    {
        public mqttPUB(string name)
        {
            this.name=name;

            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            options = new ManagedMqttClientOptions();

            lastmsg = new Dictionary<object, object>();

           
        }
    
        public void startMQTTclient( string IP, int Port, string clientID, bool mqttUseCert, List<X509Certificate>? cert)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            if (IP == null || Port == 0)
            {
                Console.WriteLine("Missing MQTT config!!!");
                return;
            }

            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                .WithClientId(clientID)
                .WithTcpServer(IP, Port)
                ;


            if (mqttUseCert)
            {
                MqttClientOptionsBuilderTlsParameters tlsOptions = new MqttClientOptionsBuilderTlsParameters();
                tlsOptions.Certificates = cert;
                tlsOptions.SslProtocol = System.Security.Authentication.SslProtocols.Tls12;
                tlsOptions.UseTls = true;
                tlsOptions.AllowUntrustedCertificates = true;
                tlsOptions.CertificateValidationHandler = delegate { return true; };

                builder = new MqttClientOptionsBuilder()
                                            .WithClientId(clientID)
                                            .WithTcpServer(IP, Port)
                                            .WithTls(tlsOptions)
                                            ;
            };
            
            
            options = new ManagedMqttClientOptionsBuilder()
                                                .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                                .WithClientOptions(builder.Build())
                                                .Build();



            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a =>
            {
                Log.Logger.Information("Message recieved: {payload}", a.ApplicationMessage);
            });


        }



        public void startMQTTclient( string IP, int Port, string clientID)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            if (IP == null || Port == 0)
            {
                Console.WriteLine("Missing MQTT config!!!");
                return;
            }

            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                .WithClientId(clientID)
                .WithTcpServer(IP, Port)
                ;
            
            
            options = new ManagedMqttClientOptionsBuilder()
                                                .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                                .WithClientOptions(builder.Build())
                                                .Build();



            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a =>
            {
                Log.Logger.Information("Message recieved: {payload}", a.ApplicationMessage);
            });


        }




        public void pubTopic(List<Object[]> msg, string deviceType, string  DeviceID,string  manufacturerName,string  serialNo, string  beaconMsg)
        {

            if (!_mqttClient.IsStarted)
            {
                _mqttClient.StartAsync(options).GetAwaiter().GetResult();

                Console.WriteLine("MQTT Client Connected!!!");

                
                string jsonBeacon = (@"{ ""Connection Message"": """ + beaconMsg + " This is " + _mqttClient.Options.ClientOptions.ClientId + " connecting at " + DateTimeOffset.UtcNow ) + @"""," ;
                
                string jsonInfo = jsonBeacon + @" ""AgentVersion"": " + 4 + @" }"; 
                
                _mqttClient.PublishAsync(deviceType +"/" + DeviceID +"/devicesetup", jsonInfo);

                string jsonAtrib = @"{ ""manufacturer"" : """ + manufacturerName + @""",  ""serialNo"" : """ + serialNo + @""" , ""lastConnectionTime"" : " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}";
                                Console.WriteLine(jsonAtrib);
                _mqttClient.PublishAsync(deviceType +"/" + DeviceID +"/atributes", jsonAtrib);
            }

            
        
            var json = @"{ ";

             foreach (var singmsg in msg)
            {
                bool checker = false;

                try
                {
                    if  (Convert.ToBoolean(lastmsg[singmsg[0]]) == Convert.ToBoolean(singmsg[1]))
                    {checker=true;}
                }
                catch
                {}

                try
                {
                    if  (Convert.ToDouble(lastmsg[singmsg[0]]) == Convert.ToDouble(singmsg[1]))
                    {checker=true;}
                }
                catch
                {}

                if  (checker)
                {
                    Console.WriteLine(Convert.ToString(singmsg[0]) +  " did not changed since last message sent!");
                    //return;
                }
                else
                {
                try
                {

                try
                {
                json = json + @"""" + singmsg[0] + @""" :";
                if (Convert.ToString(singmsg[1])=="False"){singmsg[1]="false";};
                if (Convert.ToString(singmsg[1])=="True"){singmsg[1]="true";};
                json = json + singmsg[1] + "," ;
                }
                catch
                {
                json = json + @"""" + (singmsg[0]) + @""" :";
                json = json + Convert.ToString(singmsg[1]) + "," ;
                }


                Console.WriteLine(singmsg[0] + " value is " + singmsg[1]);



                lastmsg[singmsg[0]] = singmsg[1];   
                }
                catch
                {
                lastmsg.Add(singmsg[0],singmsg[1]);
                }
                

                }
                
                

            }
            
            
            
            
            
            json = json +@"""MQTTtimeStamp"": " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() +  "}";

    

            _mqttClient.PublishAsync(deviceType +"/" + DeviceID +"/features", json);
            
            Console.WriteLine(" \n Topic:" + deviceType +"/" + DeviceID +"/features");
            Console.WriteLine(" \n Message" + json);
            Console.WriteLine(" at " + DateTimeOffset.UtcNow +" \n ");

        }

        public void OnConnected(MqttClientConnectedEventArgs obj)
        {
            //Log.Logger.Information("Successfully connected.");
        }

        public void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            //Log.Logger.Warning("Couldn't connect to broker.");
        }

        public void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            //Log.Logger.Information("Successfully disconnected.");
        }

        public static long getUTCTimeInMilliseconds(DateTime utcDT)
        {
            DateTime convertedDate = DateTime.SpecifyKind(utcDT, DateTimeKind.Utc);
            return new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();
        }

        public string name;

        public Dictionary<object, object> lastmsg;

        public IManagedMqttClient _mqttClient;

        public ManagedMqttClientOptions options;

        

    }
}
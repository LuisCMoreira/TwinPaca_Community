var DAgetter = new DAgent.opcDAget("OPCDA");
var mPUB = new Magent.mqttPUB("MQTT");
//var certt = new MQTTNet_AWS.awsCertConvertion("certs");
var config = new jsonReadSpace.JConfig("agentConfig.json");

var deviceType = "unconfig";
var deviceID = "unconfig";
var manufacturerName = "unconfig";
var serialNo = "unconfig";
var taskRate = 1000;
var mqttIP = "localhost";
var mqttPort = 1883;
var mqttClientID = "unconfig";
var mqttBeaconMsg = "";
var mqttUseCert = false;

List<string> opcdaVars = new List<string>() { };
var opcdaIP = "localhost";
var opcdaServer = "Matrikon.OPC.Simulation.1";

taskRate = setupFromJSON(config, ref deviceType, ref deviceID, ref manufacturerName, ref serialNo, ref mqttIP, ref mqttPort, ref mqttClientID,ref mqttBeaconMsg, ref mqttUseCert, ref opcdaVars, ref opcdaIP, ref opcdaServer);

//var mqttcert = certt.certTask();

//mPUB.startMQTTclient(mqttIP, mqttPort, mqttClientID, mqttUseCert, mqttcert);
mPUB.startMQTTclient(mqttIP, mqttPort, mqttClientID);

var daClient = DAgetter.DAclient(opcdaIP, opcdaServer, opcdaVars);



while (true)
{
    DAgetter.opcdaGet(daClient);

    mPUB.pubTopic(DAgetter.outget, deviceType, deviceID, manufacturerName, serialNo,  mqttBeaconMsg);

    Thread.Sleep(taskRate);
}


static int setupFromJSON(jsonReadSpace.JConfig config, ref string deviceType,ref string deviceID,ref string manufacturerName,ref string serialNo, ref string mqttIP, ref int mqttPort, ref string mqttClientID, ref string mqttBeaconMsg, ref bool mqttUseCert, ref List<string> opcdaVars, ref string opcdaIP, ref string opcdaServer)
{
    int taskRate;
    Console.Clear();

    config.Getinfo("agentConfig.json");

    deviceType = config.AgentconfigVars.DeviceType;

    deviceID = config.AgentconfigVars.DeviceID;

    manufacturerName = config.AgentconfigVars.ManufacturerName;

    serialNo  = config.AgentconfigVars.serialNo;

    taskRate = config.AgentconfigVars.TimeInMillis;

    if (config.AgentconfigVars.mqtt != null)
    {
        mqttIP = config.AgentconfigVars.mqtt.IP_address;
        mqttPort = config.AgentconfigVars.mqtt.port;
        mqttBeaconMsg = config.AgentconfigVars.mqtt.beaconMsg;
        mqttUseCert = config.AgentconfigVars.mqtt.useCerts;
    }


    if (config.AgentconfigVars.opc_da != null)
    {
        opcdaIP = config.AgentconfigVars.opc_da.IP_address;
        opcdaServer = config.AgentconfigVars.opc_da.server;
        if (config.AgentconfigVars.opc_da.variablesToGet != null)
        {
            opcdaVars = config.AgentconfigVars.opc_da.variablesToGet;
        }
    }

    Console.WriteLine(" \n press any key to start \n ");
    Console.ReadKey();
    Console.Clear();
    return taskRate;
}
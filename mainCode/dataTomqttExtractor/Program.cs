using dataTomqttExtractor;

var modbusGet =new  modbusTCPget.ModbusHandler("modbusTCP");

var DAgetter = new DAgent.opcDAget("OPCDA");
var mPUB = new Magent.mqttPUB("MQTT");
//var certt = new MQTTNet_AWS.awsCertConvertion("certs");
var config = new jsonReadSpace.JConfig("agentConfig.json");



var taskRate = 1000;

taskRate = jsonReadSpace.JConfig.setupFromJSON(config, ref jsonImport.deviceType, ref jsonImport.deviceID, ref jsonImport.manufacturerName, ref jsonImport.serialNo, ref jsonImport.mqttIP, ref jsonImport.mqttPort, ref jsonImport.mqttClientID,ref jsonImport.mqttBeaconMsg, ref jsonImport.mqttUseCert, ref jsonImport.opcdaVars, ref jsonImport.opcdaIP, ref jsonImport.opcdaServer, ref jsonImport.modbusIP, ref jsonImport.modbusPort, ref jsonImport.modbusVars);

//var certt = new MQTTNet_AWS.awsCertConvertion("certs");

//var mqttcert = certt.certTask();

//mPUB.startMQTTclient(mqttIP, mqttPort, mqttClientID, mqttUseCert, mqttcert);

mPUB.startMQTTclient(jsonImport.mqttIP, jsonImport.mqttPort, jsonImport.mqttClientID);

var daClient = DAgetter.DAclient(jsonImport.opcdaIP, jsonImport.opcdaServer, jsonImport.opcdaVars);

var mbClient= modbusGet.MBclient("localhost", 502);





while (true)
{
    DAgetter.opcdaGet(daClient);

    modbusGet.ModbusTCPget( mbClient, jsonImport.modbusVars);


    mPUB.pubTopic(DAgetter.outget, jsonImport.deviceType, jsonImport.deviceID, jsonImport.manufacturerName, jsonImport.serialNo,  jsonImport.mqttBeaconMsg);


   Thread.Sleep(taskRate);

    mPUB.pubTopic(modbusGet.outgetModbus, jsonImport.deviceType, jsonImport.deviceID, jsonImport.manufacturerName, jsonImport.serialNo,  jsonImport.mqttBeaconMsg);

}


using System.Text.Json;

namespace jsonReadSpace
{
    public class JConfig
    {

        public JConfig(string jsonName)
        {
            AgentconfigVars = new AgentConfig();
            this.jsonName = jsonName;
        }

        public void Getinfo(string jsonName)
        {
            string fileName = jsonName;//"agentConfig.json";
            string jsonString = File.ReadAllText(fileName);
            AgentConfig agentConfig = JsonSerializer.Deserialize<AgentConfig>(jsonString)!;

            Console.WriteLine(" \n Importing from Configuration File \n ");

            Console.WriteLine(" \n General Information: \n ");

            AgentconfigVars = agentConfig;

            Console.WriteLine($"    Device Type: {AgentconfigVars.DeviceType}");

            Console.WriteLine($"    Device ID: {AgentconfigVars.DeviceID}");

            Console.WriteLine($"    Device Manufacturer: {AgentconfigVars.ManufacturerName}");

            Console.WriteLine($"    Device Serial Number: {AgentconfigVars.serialNo}");

            Console.WriteLine(" \n Basic Configuration: \n ");

            Console.WriteLine($"    Rate of Execution: {AgentconfigVars.TimeInMillis}");

            if (AgentconfigVars.mqtt != null)
            {
                Console.WriteLine(" \n MQTT Configuration: \n ");

                Console.WriteLine($"    MQTT Enabled: {AgentconfigVars.mqtt.mqttON}");

                Console.WriteLine($"    MQTT Broker Address: {AgentconfigVars.mqtt.IP_address}");

                Console.WriteLine($"    MQTT Broker Port: {AgentconfigVars.mqtt.port}");

                Console.WriteLine($"    ClientID: {AgentconfigVars.mqtt.clientID}");

                Console.WriteLine($"    Beacon Message: {AgentconfigVars.mqtt.beaconMsg}");

                Console.WriteLine($"    Using Certificate Files: {AgentconfigVars.mqtt.useCerts}");
            }

            if (AgentconfigVars.opc_da != null)
            {
                Console.WriteLine(" \n OPC-DA Configuration: \n ");

                Console.WriteLine($"    OPCDA Enabled: {AgentconfigVars.opc_da.opcdaON}");

                Console.WriteLine($"    OPCDA Server Address: {AgentconfigVars.opc_da.IP_address}");

                Console.WriteLine($"    OPCDA Server Name: {AgentconfigVars.opc_da.server}");

                Console.WriteLine(" ");

                var varToGet = AgentconfigVars.opc_da.variablesToGet;
                if (varToGet != null)
                {
                    foreach (string opcdavariable in (varToGet))
                    {
                        Console.WriteLine($"        Reading: {opcdavariable}");
                    }
                }
            }

            if (AgentconfigVars.modbus != null)
            {
                Console.WriteLine(" \n Modbus Configuration: \n ");

                Console.WriteLine($"    ModbusEnabled: {AgentconfigVars.modbus.modbusON}");

                Console.WriteLine($"    Modbus Server Address: {AgentconfigVars.modbus.IP_address}");

                Console.WriteLine($"    Modbus Server Port: {AgentconfigVars.modbus.port}");

                Console.WriteLine(" ");

                var varToGet = AgentconfigVars.modbus.variablesToGet;
                if (varToGet != null)
                {
                    foreach (var modbusvariable in varToGet)
                    {
                        Console.WriteLine($"        Reading: {modbusvariable.TypeOf} : {modbusvariable.Address} | Qty : {modbusvariable.Qty} | Variable Name : {modbusvariable.varName}  ");
                    }
                }
            }



            Console.WriteLine(" \n Configuration Ready \n ");

        }

        public static int setupFromJSON(JConfig config, ref string deviceType, ref string deviceID, ref string manufacturerName, ref string serialNo, ref string mqttIP, ref int mqttPort, ref string mqttClientID, ref string mqttBeaconMsg, ref bool mqttUseCert, ref List<string> opcdaVars, ref string opcdaIP, ref string opcdaServer, ref string modbusIP, ref int modbusPort, ref List<object>? modbusVars)
        {
            int taskRate;
            Console.Clear();

            config.Getinfo("agentConfig.json");

            deviceType = config.AgentconfigVars.DeviceType;

            deviceID = config.AgentconfigVars.DeviceID;

            manufacturerName = config.AgentconfigVars.ManufacturerName;

            serialNo = config.AgentconfigVars.serialNo;

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


            if (config.AgentconfigVars.modbus != null)
            {
                modbusIP = config.AgentconfigVars.modbus.IP_address;
                modbusPort = config.AgentconfigVars.modbus.port;
                if (config.AgentconfigVars.modbus.variablesToGet != null)
                {

                    modbusVars = new List<object>
                    {

                    };


                    foreach (var modbusVTG in config.AgentconfigVars.modbus.variablesToGet)
                    {
                        modbusVars.Add(new
                        {
                            modbusVTG.TypeOf,
                            modbusVTG.Address,
                            modbusVTG.Qty,
                            modbusVTG.varName
                        });

                    };







                }
            }

            //Uncoment to Start after key press
            //Console.WriteLine(" \n press any key to start \n ");
            //Console.ReadKey();

            //Start after time sleep
            Console.WriteLine(" \n Starting in 5 seconds\n ");
            Thread.Sleep(5000);

            Console.Clear();


            Console.WriteLine(" \n Starting in \n ");
            Thread.Sleep(1000);
           
            return taskRate;
        }

        public string jsonName;

        public AgentConfig AgentconfigVars;

    }

    public class AgentConfig
    {
        public int TimeInMillis { get; set; } = 1000;
        public string DeviceType { get; set; } = "";
        public string DeviceID { get; set; } = "";
        public string ManufacturerName { get; set; } = "";
        public string serialNo { get; set; } = "";
        public mqttConfig? mqtt { get; set; } = null;
        public opc_daConfig? opc_da { get; set; } = null;

        public modbusConfig? modbus { get; set; } = null;
    }

    public class opc_daConfig
    {
        public bool opcdaON { get; set; } = false;
        public string IP_address { get; set; } = "";
        public string server { get; set; } = "";
        public List<string>? variablesToGet { get; set; }
    }


    public class mqttConfig
    {
        public bool mqttON { get; set; } = false;
        public string IP_address { get; set; } = "";
        public int port { get; set; } = 1883;
        public string clientID { get; set; } = "";
        public string beaconMsg { get; set; } = "";
        public bool useCerts { get; set; } = false;
    }

    public class modbusConfig
    {
        public bool modbusON { get; set; } = false;
        public string IP_address { get; set; } = "";
        public int port { get; set; } = 502;
        public List<modbusVariablesToGet>? variablesToGet { get; set; } = null;
    }

    public class modbusVariablesToGet
    {
        public string TypeOf { get; set; } = "";
        public int Address { get; set; } = 1;
        public int Qty { get; set; } = 1;
        public string varName { get; set; } = "";
    }


}
namespace dataTomqttExtractor
{
    public class jsonImport
    {
        public static string deviceType = "unconfig";
        public static string deviceID = "unconfig";
        public static string manufacturerName = "unconfig";
        public static string serialNo = "unconfig";
        //public static int taskRate = 1000;
        public static string mqttIP = "localhost";
        public static int mqttPort = 1883;
        public static string mqttClientID = "unconfig";
        public static string mqttBeaconMsg = "";
        public static bool mqttUseCert = false;

        public static List<string> opcdaVars = new List<string>() { };
        public static string opcdaIP = "localhost";
        public static string opcdaServer = "Matrikon.OPC.Simulation.1";

        public static string modbusIP = "localhost";
        public static int modbusPort = 502;
        public static List<object>? modbusVars = null;
    }
}
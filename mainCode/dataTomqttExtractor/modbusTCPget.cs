using NModbus;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace modbusTCPget
{
    class ModbusHandler
    {
        private bool isConnected;
        
        

        public ModbusHandler(string name)
        {
            this.name = name;
            outgetModbus = new List<Object[]>();
            
            isConnected = false;
            
    
   

        }

public IModbusMaster MBclient(string ipAddress, int port)
{
TcpClient tcpClient = new TcpClient(ipAddress, port);

ModbusFactory modbusFactory = new ModbusFactory();
IModbusMaster modbusMaster=modbusFactory.CreateMaster(tcpClient);       

return modbusMaster;
}


        public List<Object[]>? ModbusTCPget(IModbusMaster modbusMaster, List<object> modbusList)
        {
        


            if (modbusList == null || modbusMaster == null )
            {
                return null;
            }


outgetModbus.Clear();


            // Replace these values with the actual Modbus address and number of coils to read
            object? TypeOfObject = modbusList[0].GetType().GetProperty("TypeOf")?.GetValue(modbusList[0]);
            object? startObject = modbusList[0].GetType().GetProperty("Start")?.GetValue(modbusList[0]);
            object? numObject = modbusList[0].GetType().GetProperty("Address")?.GetValue(modbusList[0]);
            if (startObject!=null && numObject!=null && TypeOfObject!=null)
            {
            string TypeOf = (string)(object)TypeOfObject;
            ushort startCoil = (ushort)(int)startObject;
            ushort numCoils = (ushort)(int)numObject;
      
            try
            {


 
                // Read coils from the Modbus server
                bool[] data = modbusMaster.ReadCoils(0, startCoil, numCoils);

                // Display the received data
                for (int i = 0; i < data.Length; i++)
                {
                    Console.WriteLine($"{TypeOf} {startCoil + i}: {data[i]}");

                    // Store the data in the output list as an array
                    outgetModbus.Add(new Object[] { (TypeOf).ToString(),(startCoil + i).ToString(), (data[i]).ToString() });
                }

                // Return the data
                return outgetModbus;



                  

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return an empty list in case of an error
                return new List<Object[]>();
            }
            }
            else
            {
                return null;
            }

        }

        public string name;
        public List<Object[]> outgetModbus;
    }
}


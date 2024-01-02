using NModbus;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace modbusTCPget
{
    class ModbusHandler
    {
        public ModbusHandler(string name)
        {
            this.name = name;
            outget = new List<Object[]>();
        }


        public List<Object[]>? ModbusTCPget(string ipAddress, int port, List<object> modbusList)
        {
        


            if (modbusList == null || ipAddress == null || port == 0 )
            {
                return null;
            }





            // Replace these values with the actual Modbus address and number of coils to read
            object? startObject = modbusList[0].GetType().GetProperty("Start")?.GetValue(modbusList[0]);
            object? numObject = modbusList[0].GetType().GetProperty("Address")?.GetValue(modbusList[0]);
            if (startObject!=null && numObject!=null)
            {
            ushort startCoil = (ushort)(int)startObject;
            ushort numCoils = (ushort)(int)numObject;
      
            
            
            try
            {



                // Create a Modbus TCP master using TcpClient
                using (TcpClient tcpClient = new TcpClient(ipAddress, port))
                {
                    ModbusFactory modbusFactory = new ModbusFactory();
                    Console.WriteLine("Connecting");
                    IModbusMaster modbusMaster=modbusFactory.CreateMaster(tcpClient);
             
               

                // Read coils from the Modbus server
                bool[] data = modbusMaster.ReadCoils(1, 1, 1);
                

                // Display the received data
                for (int i = 0; i < data.Length; i++)
                {
                    Console.WriteLine($"Coil {startCoil + i}: {data[i]}");

                    // Store the data in the output list as an array
                    outget.Add(new Object[] { (startCoil + i).ToString(), (data[i]).ToString() });
                }

                // Return the data
                return outget;



                }   

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
        public List<Object[]> outget;
    }
}


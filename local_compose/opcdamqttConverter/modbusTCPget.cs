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

        public List<Object[]> ModbusTCPget(string ipAddress, int port)
        {
            // Create a Modbus TCP master
            TcpClient tcpClient = new TcpClient(ipAddress, port);

            ModbusFactory modbusFactory = new ModbusFactory();
            IModbusMaster modbusMaster = modbusFactory.CreateMaster(tcpClient);

            // Replace these values with the actual Modbus address and number of coils to read
            ushort startCoil = 1;
            ushort numCoils = 1;  // Replace with the actual number of coils to read

            try
            {
                // Read coils from the Modbus server
                bool[] data = modbusMaster.ReadCoils(1, startCoil,numCoils);// .ReadCoils(startCoil, numCoils);

                // Display the received data
                for (int i = 0; i < data.Length; i++)
                {
                    Console.WriteLine($"Coil {startCoil + i}: {data[i]}");

                    // Store the data in the output list as an array
                    outget.Add(new Object[] { (startCoil + i).ToString(),( data[i]).ToString() });
                }

                // Return the data
                return outget;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return an empty list in case of an error
                return new List<Object[]>();
            }
            finally
            {
                // Dispose of the Modbus master (if it implements IDisposable)
                if (modbusMaster is IDisposable disposableMaster)
                {
                    disposableMaster.Dispose();
                }
            }
        }

        public string name;
        public List<Object[]> outget;
    }
}

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
            outgetModbus = new List<Object[]>();
        }

        public IModbusMaster MBclient(string ipAddress, int port)
        {
            TcpClient tcpClient = new TcpClient(ipAddress, port);

            ModbusFactory modbusFactory = new ModbusFactory();
            IModbusMaster modbusMaster = modbusFactory.CreateMaster(tcpClient);

            return modbusMaster;
        }

        public List<Object[]> ModbusTCPget(IModbusMaster modbusMaster, List<object>? modbusList)
        {

            outgetModbus.Clear();
            if (modbusList == null || modbusMaster == null)
            {
                outgetModbus = new List<Object[]>();
            }
            else
            {

                foreach (var row in modbusList)
                {
                    // Replace these values with the actual Modbus address and number of coils to read
                    object? TypeOfObject = row.GetType().GetProperty("TypeOf")?.GetValue(row);
                    object? startObject = row.GetType().GetProperty("Address")?.GetValue(row);
                    object? numObject = row.GetType().GetProperty("Qty")?.GetValue(row);
                    object? varName = row.GetType().GetProperty("varName")?.GetValue(row);

                    if (startObject != null && numObject != null && TypeOfObject != null && varName != null)
                    {
                        string TypeOf = (string)(object)TypeOfObject;
                        ushort startCoil = (ushort)(int)startObject;
                        ushort numCoils = (ushort)(int)numObject;
                        string vName = (string)(object)varName;

                        try
                        {

                            if (TypeOf == "Coil")
                            {
                                // Read coils from the Modbus server
                                bool[] data = modbusMaster.ReadCoils(0, startCoil, numCoils);

                                // Store the data in the output list as an array
                                outgetModbus.Add(new Object[] { vName + "_" + (TypeOf)  + (startCoil).ToString(), (data[0]).ToString() });
                            }


                            if (TypeOf == "Register")
                            {
                                // Read coils from the Modbus server
                                ushort[] data = modbusMaster.ReadHoldingRegisters(0, startCoil, numCoils);

                                // Store the data in the output list as an array
                                outgetModbus.Add(new Object[] { vName + "_" +(TypeOf) + (startCoil).ToString(), (data[0]).ToString() });
                            }

                        }


                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");

                            // Return an empty list in case of an error
                            outgetModbus = new List<Object[]>();
                        }
                    }
                    else
                    {
                        // Return the data
                        outgetModbus = new List<Object[]>();
                    }
                }

            }

            // Return the data
            return outgetModbus;
        }

        public string name;
        public List<Object[]> outgetModbus;
    }
}
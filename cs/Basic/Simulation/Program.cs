// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Simulation
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to develop and test code without a PLC by using the
    /// simulation of the low-level API.
    /// </summary>
    /// <remarks>
    /// Without an argument, the sample uses the simulation. Pass the IP address of your PLC as
    /// the first argument to run the same code against the PLC: it writes DB1.DBD 0 and 4.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 1. PLC or simulation
            // SimaticDevice and VirtualDevice are both derived from PlcDevice: the code that
            // follows is the same for both.
            PlcDevice device = Program.CreateDevice(args);
            #endregion

            using (PlcDeviceConnection connection = device.CreateConnection()) {
                connection.Open();

                #region 2. Read and write
                // The simulation returns what the application has written before.
                connection.WriteInt32("DB1.DBD 0", 1200);
                int speed = connection.ReadInt32("DB1.DBD 0");
                Console.WriteLine("Speed: {0}", speed);
                #endregion

                #region 3. Test code without a PLC
                // The simulation keeps its data per connection and discards it with Close. A
                // test therefore writes its data on the connection the tested code reads from.
                connection.WriteInt32("DB1.DBD 4", 42);
                int pieces = Program.ReadPieces(connection);
                string result = pieces == 42 ? "passed" : "failed";
                Console.WriteLine("Test {0}: pieces = {1}", result, pieces);
                #endregion

                #region 4. Data of another connection
                using (PlcDeviceConnection other = device.CreateConnection()) {
                    other.Open();

                    // In the simulation, a second connection does not see the data of the first.
                    int otherPieces = Program.ReadPieces(other);
                    Console.WriteLine("Pieces on another connection: {0}", otherPieces);
                }
                #endregion
            }
        }

        private static PlcDevice CreateDevice(string[] args)
        {
            if (args.Length == 0)
                return new VirtualDevice();

            // Pass an IPDeviceEndPoint with rack and slot and the device type of your PLC if they
            // differ from the defaults (see the sample DeviceTypes).
            return new SimaticDevice(args[0]);
        }

        // The tested code reads through the connection it receives instead of creating one.
        private static int ReadPieces(PlcDeviceConnection connection)
        {
            return connection.ReadInt32("DB1.DBD 4");
        }
    }
}

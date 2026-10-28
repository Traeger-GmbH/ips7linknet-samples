// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace SharedConnections
{
    using System;
    using System.Threading.Tasks;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how several threads share one SimaticClient and how several
    /// PlcDeviceConnection objects share one connection to the PLC.
    /// </summary>
    /// <remarks>
    /// The sample reads from an S7-300/400 only. Pass the IP address of your PLC as the first
    /// argument; for another device type, change the scheme and SimaticDeviceType.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            string host = args.Length > 0 ? args[0] : "192.168.0.80";

            #region 1. One client in several threads
            using (var client = new SimaticClient("s7300://" + host)) {
                client.Connect();

                // The client locks its connection for each access and runs simultaneous accesses
                // one after the other, without another connection to the PLC.
                Task[] tasks = new Task[3];

                for (int index = 0; index < tasks.Length; index++) {
                    int number = index;
                    tasks[index] = Task.Run(() => Program.ReadMarker(client, number));
                }

                Task.WaitAll(tasks);
                client.Disconnect();
            }
            #endregion

            #region 2. Several connection objects share one connection
            // UsePool is static and applies to the whole application.
            PlcDeviceConnection.UsePool = true;

            var device = new SimaticDevice(host, SimaticDeviceType.S7300_400);
            PlcDeviceConnection readConnection = device.CreateConnection(createNew: true);
            PlcDeviceConnection monitorConnection = device.CreateConnection(createNew: true);

            readConnection.Open();
            monitorConnection.Open();

            Console.WriteLine("MB 0 = {0}", readConnection.ReadByte("MB 0"));
            Console.WriteLine("MB 1 = {0}", monitorConnection.ReadByte("MB 1"));

            // The SDK closes the shared connection to the PLC with the last connection object.
            readConnection.Close();
            monitorConnection.Close();
            PlcDeviceConnection.ClearAllPools();
            #endregion
        }

        private static void ReadMarker(SimaticClient client, int number)
        {
            byte value = client.ReadValue<byte>("MB 0");
            Console.WriteLine("Task {0}: MB 0 = {1}", number, value);
        }
    }
}

// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace DeviceConnection
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates the states of a PlcDeviceConnection from opening to closing and
    /// how to monitor the state of all connections of an application.
    /// </summary>
    /// <remarks>
    /// Without an argument, the sample uses the simulation. Pass the IP address of your PLC as
    /// the first argument to connect to the PLC; the sample reads MB 0 only.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 1. Monitor all connections
            // ConnectionCreated is raised for each new connection of the application. Here, the
            // handler monitors the state of each connection.
            PlcNotifications.ConnectionCreated += Program.HandleConnectionCreated;
            #endregion

            PlcDevice device = Program.CreateDevice(args);

            using (PlcDeviceConnection connection = device.CreateConnection()) {
                #region 2. Break detection and watchdog
                // The break detection only keeps the TCP connection alive. The watchdog reads
                // MB 0 if the application has not accessed the PLC during WatchDogInterval and
                // reports a broken connection with the state Disconnected.
                connection.UseBreakDetection = true;
                connection.UseWatchDog = true;
                #endregion

                #region 3. Open, connect and close
                // Open checks the configuration, Connect establishes the connection to the PLC.
                connection.Open();
                connection.Connect();

                // After a connection break, the next access connects again.
                byte value = connection.ReadByte("MB 0");
                Console.WriteLine("MB 0 = {0}", value);

                // Close ends the connection to the PLC and releases its resources.
                connection.Close();
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

        private static void HandleConnectionCreated(
                object sender,
                PlcNotifications.PlcDeviceConnectionEventArgs e)
        {
            e.Connection.StateChanged += Program.HandleConnectionStateChanged;
        }

        private static void HandleConnectionStateChanged(
                object sender,
                PlcDeviceConnectionStateChangedEventArgs e)
        {
            Console.WriteLine("Connection state changed from {0} to {1}.", e.OldState, e.NewState);
        }
    }
}

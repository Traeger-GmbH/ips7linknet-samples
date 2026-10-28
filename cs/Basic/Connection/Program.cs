// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Connection
{
    using System;
    using System.Threading;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to configure, establish and monitor the connection of a
    /// SimaticClient, including timeouts, break detection and the watchdog.
    /// </summary>
    /// <remarks>
    /// The sample reads MB 0 once per second until Ctrl+C is pressed. Pass the address of your
    /// PLC as the first argument, for example "s71500://192.168.0.80".
    /// </remarks>
    public class Program
    {
        private static readonly ManualResetEvent stop = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            #region 1. Address of the PLC
            // The scheme selects the device type, the query sets rack and slot if they differ
            // from the defaults, for example "s7300://192.168.0.80?rack=0&slot=2".
            string address = args.Length > 0 ? args[0] : "s7300://192.168.0.80";
            #endregion

            using (var client = new SimaticClient(address)) {
                #region 2. Timeouts
                client.ConnectTimeout = 5000;
                client.OperationTimeout = 5000;
                #endregion

                #region 3. Break detection and watchdog
                // The break detection only keeps the TCP connection alive. The watchdog reads
                // MB 0 if the application has not accessed the PLC during WatchDogInterval and
                // reports a broken connection with the state Disconnected.
                client.UseBreakDetection = true;
                client.UseWatchDog = true;
                client.WatchDogInterval = 5000;
                #endregion

                #region 4. Connection events
                client.StateChanged += Program.HandleClientStateChanged;
                client.Disconnected += Program.HandleClientDisconnected;
                #endregion

                #region 5. Connect
                try {
                    client.Connect();
                }
                catch (PlcException ex) when (ex.Code == PlcStatusCode.Timeout) {
                    Console.WriteLine("The PLC is not reachable: {0}", ex.Message);
                    return;
                }
                #endregion

                #region 6. Access and connection break
                Console.CancelKeyPress += Program.HandleCancelKeyPress;
                Console.WriteLine("Reading MB 0 once per second. Press Ctrl+C to exit.");

                while (!Program.stop.WaitOne(1000)) {
                    try {
                        byte value = client.ReadValue<byte>("MB 0");
                        Console.WriteLine("MB 0 = {0}", value);
                    }
                    catch (PlcException ex) {
                        // The client does not reconnect in the background: the next access
                        // establishes the connection again.
                        Console.WriteLine("Access failed: {0}", ex.Code);
                    }
                }

                client.Disconnect();
                #endregion
            }
        }

        private static void HandleCancelKeyPress(
                object sender,
                ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program.stop.Set();
        }

        private static void HandleClientDisconnected(object sender, EventArgs e)
        {
            // Raised when the watchdog detects a broken connection or after Disconnect.
            Console.WriteLine("The connection to the PLC is closed or broken.");
        }

        private static void HandleClientStateChanged(
                object sender,
                SimaticClientStateChangedEventArgs e)
        {
            Console.WriteLine("Client state changed from {0} to {1}.", e.OldState, e.NewState);
        }
    }
}

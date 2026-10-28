// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Binding
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how a PlcObject reports changed values through PropertyChanged,
    /// the event a user interface binds to.
    /// </summary>
    /// <remarks>
    /// Without an argument, the sample uses the simulation and changes the speed itself. Pass the
    /// IP address of your PLC as the first argument to read DB1.DBD 0 and 4 from the PLC.
    /// </remarks>
    public class Program
    {
        private static readonly ManualResetEvent stop = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            PlcDevice device = Program.CreateDevice(args);
            var data = new MachineData();

            #region 1. Handle PropertyChanged
            // A user interface binds to the properties of the object in the same way, for example
            // with a WPF binding to the path Speed.
            data.PropertyChanged += Program.HandleDataPropertyChanged;
            #endregion

            using (PlcDeviceConnection connection = device.CreateConnection()) {
                connection.Open();

                Console.CancelKeyPress += Program.HandleCancelKeyPress;
                Console.WriteLine("Reading once per second. Press Ctrl+C to exit.");

                #region 2. Read cyclically
                int cycle = 0;

                while (!Program.stop.WaitOne(1000)) {
                    if (device is VirtualDevice) {
                        // In the simulation, the sample changes the values itself.
                        connection.WriteInt32("DB1.DBD 0", 1000 + (cycle % 3) * 100);
                        connection.WriteReal("DB1.DBD 4", 21.5f);
                        cycle++;
                    }

                    // ReadObject raises PropertyChanged for each property whose value has changed.
                    connection.ReadObject(data);
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

        private static void HandleCancelKeyPress(
                object sender,
                ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program.stop.Set();
        }

        private static void HandleDataPropertyChanged(
                object sender,
                PropertyChangedEventArgs e)
        {
            var data = (MachineData)sender;
            Console.WriteLine("{0} changed to {1}", e.PropertyName, data[e.PropertyName]);
        }
    }
}

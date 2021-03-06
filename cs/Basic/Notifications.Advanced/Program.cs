﻿// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Notifications
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to work with notifications in an advanced manner.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            //// As already demonstrated in the basic sample 'Notifications' it is possible to
            //// handle the state changes of a connection using the events provided by the static
            //// PlcNotifications class or using the events provided by the instance of a
            //// connection itself.
            //// Additionally taking the basic sample 'Exceptions' into account it is obvious that
            //// developers using the framework are able to fully control and monitor the creation
            //// of connections, the state of these connections and they can influence the status
            //// evaluation of every operation performed by the low level driver used within the
            //// framework.

            //// This sample does therefore demonstrate how a developer can get the most out of
            //// the features represented by the basic samples 'Notifications' and 'Exceptions'.

            // Assign an event handler to the ConnectionCreated event to get notified when ever
            // a new connection has been created within the framework. This handler does then
            // subscribe further events on the connection.
            PlcNotifications.ConnectionCreated += Program.HandleConnectionCreated;

            // Assign a callback method to evaluate the operational status in a custom way to
            // ensure that only in specific conditions failed operations will lead to an exception.
            PlcNotifications.EvaluateStatus = Program.EvaluateStatus;

            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();

            Console.WriteLine();
            Console.WriteLine("= Open =");
            connection.Open();

            //// An explicit call to Connect() is not required. But it will be done here to
            //// demonstrate the appropriate state changes which occur in case of the
            //// first attempt to acccess the PLC device.
            Console.WriteLine();
            Console.WriteLine("= Connect =");
            connection.Connect();

            Program.ReadBoolean(connection);
            Program.ReadBooleanArray(connection);

            Console.WriteLine();
            Console.WriteLine("= Close =");
            connection.Close();

            Console.ReadKey();
        }

        private static bool EvaluateStatus(IPlcStatusProvider provider)
        {
            // By default any PlcStatusCode unequal to NoError will lead to a PlcException.
            // In this scenario we ignore the case that a PLC device can be temporary offline
            // and therefore unavailable. Additionally we do also ignore Timeouts in cases of
            // a bad network connection and access to missing data areas (NoData).
            PlcStatus status = provider.Status;

            Console.WriteLine();
            Console.WriteLine("--> {0}.Status.Evaluate: {1}", provider.GetType().Name, status.Code);
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"));
            Console.WriteLine("----> Text: {0}", status.Text);

            if (status.Type != null)
                Console.WriteLine("----> Type: {0}", status.Type);

            PlcStatusCode statusCode = status.Code;

            return statusCode == PlcStatusCode.NoError
                    || statusCode == PlcStatusCode.CpuNotFound
                    || statusCode == PlcStatusCode.Timeout
                    || statusCode == PlcStatusCode.NoData;

            // Instead of the whole code above is also possible to clear out all types of error and
            // information codes by just always returning the value true. The value true does
            // indicate that each status of any provider has been evaluated by custom code and no
            // further framework evaluation is required.
            //// return true;
        }

        private static void HandleConnectionCreated(object sender, PlcNotifications.PlcDeviceConnectionEventArgs e)
        {
            e.Connection.StateChanged += Program.HandleStateChanged;
            e.Connection.Status.Changed += Program.HandleConnectionStatusChanged;
        }

        private static void HandleStateChanged(object sender, PlcDeviceConnectionStateChangedEventArgs e)
        {
            // The StateChanged event occurs when ever the PlcDeviceConnection.State property gets
            // changed. This property does provide general low level driver independent state
            // information of a connection. This state information does indicate whether a
            // a connection is in a useful state and whether read/write operations can be
            // performed.
            PlcDeviceConnection connection = (PlcDeviceConnection)sender;

            Console.WriteLine();
            Console.WriteLine("Connection to '{0}'", connection.Device.EndPoint);
            Console.WriteLine("-> State.Changed from '{0}' to '{1}'.", e.OldState, e.NewState);
        }

        private static void HandleConnectionStatusChanged(object sender, EventArgs e)
        {
            // The Status.Changed event does provide low level operational status information and
            // does therefore reflect the result of the most recent operation performed on the low
            // level driver used by the framework.
            PlcStatus status = (PlcStatus)sender;

            Console.WriteLine();
            Console.WriteLine("--> Connection.Status.Change: {0}", status.Code);
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"));
            Console.WriteLine("----> Text: {0}", status.Text);

            if (status.Type != null)
                Console.WriteLine("----> Type: {0}", status.Type);
        }

        private static void HandleValueStatusChanged(object sender, EventArgs e)
        {
            // The Status.Changed event does provide low level operational status information and
            // does therefore reflect the result of the most recent operation performed on the low
            // level driver used by the framework.
            PlcStatus status = (PlcStatus)sender;

            Console.WriteLine();
            Console.WriteLine("--> Value.Status.Change: {0}", status.Code);
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"));
            Console.WriteLine("----> Text: {0}", status.Text);

            if (status.Type != null)
                Console.WriteLine("----> Type: {0}", status.Type);
        }

        private static void ReadBoolean(PlcDeviceConnection connection)
        {
            Console.WriteLine();
            Console.WriteLine("= Read Boolean =");
            bool value1 = connection.ReadBoolean("DB1.DBX 1.0");

            Console.WriteLine();
            Console.WriteLine("= Read Boolean (NoData) =");
            bool value2 = connection.ReadBoolean("DB10000.DBX 1.0");

            Console.WriteLine();
            Console.WriteLine("= Read PlcBoolean =");
            PlcBoolean plcValue1 = new PlcBoolean("DB1.DBX 1.0");
            plcValue1.Status.Changed += Program.HandleValueStatusChanged;
            bool value3 = connection.ReadValue(plcValue1);

            Console.WriteLine();
            Console.WriteLine("= Read PlcBoolean (NoData) =");
            PlcBoolean plcValue2 = new PlcBoolean("DB10000.DBX 1.0");
            plcValue2.Status.Changed += Program.HandleValueStatusChanged;
            bool value4 = connection.ReadValue(plcValue2);
        }

        private static void ReadBooleanArray(PlcDeviceConnection connection)
        {
            Console.WriteLine();
            Console.WriteLine("= Read Boolean Array =");
            bool[] values1 = connection.ReadBoolean("DB1.DBX 1.0", 2);

            Console.WriteLine();
            Console.WriteLine("= Read Boolean Array (NoData) =");
            bool[] values2 = connection.ReadBoolean("DB10000.DBX 1.0", 2);

            Console.WriteLine();
            Console.WriteLine("= Read PlcBooleanArray =");
            PlcBooleanArray plcValueArray1 = new PlcBooleanArray("DB1.DBX 1.0", 2);
            plcValueArray1.Status.Changed += Program.HandleValueStatusChanged;
            bool[] values3 = connection.ReadValue(plcValueArray1);

            Console.WriteLine();
            Console.WriteLine("= Read PlcBooleanArray (NoData) =");
            PlcBooleanArray plcValueArray2 = new PlcBooleanArray("DB10000.DBX 1.0", 2);
            plcValueArray2.Status.Changed += Program.HandleValueStatusChanged;
            bool[] values4 = connection.ReadValue(plcValueArray2);
        }
    }
}

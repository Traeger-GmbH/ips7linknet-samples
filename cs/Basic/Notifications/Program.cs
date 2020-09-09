// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Notifications
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to work with notifications.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            #region 1. Way: Global events.
            {
                //// In case there it is necessary of being notified when ever a new connection
                //// has been created or the state of a connection has been changed there does
                //// the PlcNotifications class provide different events that can be used for that
                //// requirement.
                //// Simply assign custom event handlers to the appropriate events like the
                //// following lines demonstrate.
                PlcNotifications.ConnectionCreated += Program.HandleAnyEvent;
                PlcNotifications.ConnectionOpening += Program.HandleAnyEvent;
                PlcNotifications.ConnectionOpened += Program.HandleAnyEvent;
                PlcNotifications.ConnectionConnecting += Program.HandleAnyEvent;
                PlcNotifications.ConnectionConnected += Program.HandleAnyEvent;
                PlcNotifications.ConnectionClosing += Program.HandleAnyEvent;
                PlcNotifications.ConnectionClosed += Program.HandleAnyEvent;

                PlcDeviceConnection connection = device.CreateConnection();
                connection.Open();

                //// An explicit call to Connect() is not required. But it will be done here to
                //// demonstrate the appropriate state changes which occur in case of the
                //// first attempt to acccess the PLC device.
                connection.Connect();

                connection.Close();

                //// In case there is no longer any need of being notified simply detach the
                //// associated custom event handlers like the following lines demonstrate.
                PlcNotifications.ConnectionCreated -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionOpening -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionOpened -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionConnecting -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionConnected -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionClosing -= Program.HandleAnyEvent;
                PlcNotifications.ConnectionClosed -= Program.HandleAnyEvent;
            }
            #endregion

            Console.WriteLine();

            #region 2. Way: Global state event.
            {
                //// Instead of attachning/detaching a custom event handler to specific events it
                //// is also possible to handle all state events using code like the following one.
                PlcNotifications.ConnectionStateChanged += Program.HandleStateEvent;

                PlcDeviceConnection connection = device.CreateConnection();
                connection.Open();

                //// An explicit call to Connect() is not required. But it will be done here to
                //// demonstrate the appropriate state changes which occur in case of the
                //// first attempt to acccess the PLC device.
                connection.Connect();

                connection.Close();

                //// In case there is no longer any need of being notified simply detach the
                //// associated custom event handler like the following line demonstrate.
                PlcNotifications.ConnectionStateChanged -= Program.HandleStateEvent;
            }
            #endregion

            Console.WriteLine();

            #region 3. Way: Instance events.
            {
                //// If there is no need of being notified when ever any connection changes its
                //// state, it is also possible to restrict the event handlers to one or more
                //// specific connection instances.
                //// Simply assign custom event handlers to the appropriate events like the
                //// following lines demonstrate.
                PlcDeviceConnection connection = device.CreateConnection();

                connection.Opening += Program.HandleAnyInstanceEvent;
                connection.Opened += Program.HandleAnyInstanceEvent;
                connection.Connecting += Program.HandleAnyInstanceEvent;
                connection.Connected += Program.HandleAnyInstanceEvent;
                connection.Closing += Program.HandleAnyInstanceEvent;
                connection.Closed += Program.HandleAnyInstanceEvent;

                connection.Open();

                //// An explicit call to Connect() is not required. But it will be done here to
                //// demonstrate the appropriate state changes which occur in case of the
                //// first attempt to acccess the PLC device.
                connection.Connect();

                connection.Close();

                //// In case there is no longer any need of being notified simply detach the
                //// associated custom event handlers like the following lines demonstrate.
                connection.Opening -= Program.HandleAnyInstanceEvent;
                connection.Opened -= Program.HandleAnyInstanceEvent;
                connection.Connecting -= Program.HandleAnyInstanceEvent;
                connection.Connected -= Program.HandleAnyInstanceEvent;
                connection.Closing -= Program.HandleAnyInstanceEvent;
                connection.Closed -= Program.HandleAnyInstanceEvent;
            }
            #endregion

            Console.WriteLine();

            #region 4. Way: Instance state event.
            {
                //// Instead of attachning/detaching a custom event handler to specific events it
                //// is also possible to handle all state events using code like the following one.
                PlcDeviceConnection connection = device.CreateConnection();
                connection.StateChanged += Program.HandleInstanceStateEvent;

                connection.Open();

                //// An explicit call to Connect() is not required. But it will be done here to
                //// demonstrate the appropriate state changes which occur in case of the
                //// first attempt to acccess the PLC device.
                connection.Connect();

                connection.Close();

                //// In case there is no longer any need of being notified simply detach the
                //// associated custom event handler like the following line demonstrate.
                connection.StateChanged -= Program.HandleInstanceStateEvent;
            }
            #endregion

            Console.ReadKey();
        }

        private static void HandleAnyEvent(object sender, PlcNotifications.PlcDeviceConnectionEventArgs e)
        {
            // The passed event data (e) does provide the instance affected by the event.
            PlcDeviceConnection connection = e.Connection;

            Console.WriteLine(
                    "1. Way: State of connection to '{0}': {1}",
                    connection.Device.EndPoint,
                    connection.State);
        }

        private static void HandleAnyInstanceEvent(object sender, EventArgs e)
        {
            // The passed sender does provide the instance affected by the event.
            PlcDeviceConnection connection = (PlcDeviceConnection)sender;

            Console.WriteLine(
                    "3. Way: State of connection to '{0}': {1}",
                    connection.Device.EndPoint,
                    connection.State);
        }

        private static void HandleInstanceStateEvent(object sender, PlcDeviceConnectionStateChangedEventArgs e)
        {
            // The passed sender does provide the instance affected by the event.
            PlcDeviceConnection connection = (PlcDeviceConnection)sender;

            Console.WriteLine("4. Way: State of connection to '{0}':", connection.Device.EndPoint);
            Console.WriteLine("-> Changed from {0} to {1}.", e.OldState, e.NewState);
        }

        private static void HandleStateEvent(object sender, PlcNotifications.PlcDeviceConnectionStateChangedEventArgs e)
        {
            // The passed event data (e) does provide the instance affected by the event.
            PlcDeviceConnection connection = e.Connection;

            Console.WriteLine("2. Way: State of connection to '{0}':", connection.Device.EndPoint);
            Console.WriteLine("-> Changed from {0} to {1}.", e.OldState, e.NewState);
        }
    }
}

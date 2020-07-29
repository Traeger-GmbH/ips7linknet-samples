' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace Notifications
    ''' <summary>
    ''' This sample demonstrates how to work with notifications.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SiemensDevice = New SiemensDevice( _
                    New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

            ' 1. Way: Global events.
            If True Then
                ' In case there it is necessary of being notified when ever a new connection
                ' has been created or the state of a connection has been changed there does
                ' the PlcNotifications class provide different events that can be used for that
                ' requirement.
                ' Simply assign custom event handlers to the appropriate events like the
                ' following lines demonstrate.
                AddHandler PlcNotifications.ConnectionCreated, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionOpening, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionOpened, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionConnecting, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionConnected, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionClosing, AddressOf Program.HandleAnyEvent
                AddHandler PlcNotifications.ConnectionClosed, AddressOf Program.HandleAnyEvent

                Dim connection As PlcDeviceConnection = device.CreateConnection()
                connection.Open()

                ' An explicit call to Connect() is not required. But it will be done here to
                ' demonstrate the appropriate state changes which occur in case of the
                ' first attempt to acccess the PLC device.
                connection.Connect()

                connection.Close()

                ' In case there is no longer any need of being notified simply detach the
                ' associated custom event handlers like the following lines demonstrate.
                RemoveHandler PlcNotifications.ConnectionCreated, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionOpening, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionOpened, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionConnecting, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionConnected, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionClosing, AddressOf Program.HandleAnyEvent
                RemoveHandler PlcNotifications.ConnectionClosed, AddressOf Program.HandleAnyEvent
            End If

            Console.WriteLine()

            ' 2. Way: Global state event.
            If True Then
                ' Instead of attachning/detaching a custom event handler to specific events it
                ' is also possible to handle all state events using code like the following one.
                AddHandler PlcNotifications.ConnectionStateChanged, AddressOf Program.HandleStateEvent

                Dim connection As PlcDeviceConnection = device.CreateConnection()
                connection.Open()

                ' An explicit call to Connect() is not required. But it will be done here to
                ' demonstrate the appropriate state changes which occur in case of the
                ' first attempt to acccess the PLC device.
                connection.Connect()

                connection.Close()

                ' In case there is no longer any need of being notified simply detach the
                ' associated custom event handler like the following line demonstrate.
                RemoveHandler PlcNotifications.ConnectionStateChanged, AddressOf Program.HandleStateEvent
            End If

            Console.WriteLine()

            ' 3. Way: Instance events.
            If True Then
                ' If there is no need of being notified when ever any connection changes its
                ' state, it is also possible to restrict the event handlers to one or more
                ' specific connection instances.
                ' Simply assign custom event handlers to the appropriate events like the
                ' following lines demonstrate.
                Dim connection As PlcDeviceConnection = device.CreateConnection()

                AddHandler connection.Opening, AddressOf Program.HandleAnyInstanceEvent
                AddHandler connection.Opened, AddressOf Program.HandleAnyInstanceEvent
                AddHandler connection.Connecting, AddressOf Program.HandleAnyInstanceEvent
                AddHandler connection.Connected, AddressOf Program.HandleAnyInstanceEvent
                AddHandler connection.Closing, AddressOf Program.HandleAnyInstanceEvent
                AddHandler connection.Closed, AddressOf Program.HandleAnyInstanceEvent

                connection.Open()

                ' An explicit call to Connect() is not required. But it will be done here to
                ' demonstrate the appropriate state changes which occur in case of the
                ' first attempt to acccess the PLC device.
                connection.Connect()

                connection.Close()

                ' In case there is no longer any need of being notified simply detach the
                ' associated custom event handlers like the following lines demonstrate.
                RemoveHandler connection.Opening, AddressOf Program.HandleAnyInstanceEvent
                RemoveHandler connection.Opened, AddressOf Program.HandleAnyInstanceEvent
                RemoveHandler connection.Connecting, AddressOf Program.HandleAnyInstanceEvent
                RemoveHandler connection.Connected, AddressOf Program.HandleAnyInstanceEvent
                RemoveHandler connection.Closing, AddressOf Program.HandleAnyInstanceEvent
                RemoveHandler connection.Closed, AddressOf Program.HandleAnyInstanceEvent
            End If

            Console.WriteLine()

            ' 4. Way: Instance state event.
            If True Then
                ' Instead of attachning/detaching a custom event handler to specific events it
                ' is also possible to handle all state events using code like the following one.
                Dim connection As PlcDeviceConnection = device.CreateConnection()
                AddHandler connection.StateChanged, AddressOf Program.HandleInstanceStateEvent

                connection.Open()

                ' An explicit call to Connect() is not required. But it will be done here to
                ' demonstrate the appropriate state changes which occur in case of the
                ' first attempt to acccess the PLC device.
                connection.Connect()

                connection.Close()

                ' In case there is no longer any need of being notified simply detach the
                ' associated custom event handler like the following line demonstrate.
                RemoveHandler connection.StateChanged, AddressOf Program.HandleInstanceStateEvent
            End If

            Console.ReadKey()
        End Sub

        Private Shared Sub HandleAnyEvent(ByVal sender As Object, ByVal e As PlcNotifications.PlcDeviceConnectionEventArgs)
            ' The passed event data (e) does provide the instance affected by the event.
            Dim connection As PlcDeviceConnection = e.Connection

            Console.WriteLine( _
                    "1. Way: State of connection to '{0}': {1}", _
                    connection.Device.EndPoint, _
                    connection.State)
        End Sub

        Private Shared Sub HandleAnyInstanceEvent(ByVal sender As Object, ByVal e As EventArgs)
            ' The passed sender does provide the instance affected by the event.
            Dim connection As PlcDeviceConnection = CType(sender, PlcDeviceConnection)

            Console.WriteLine( _
                    "3. Way: State of connection to '{0}': {1}", _
                    connection.Device.EndPoint, _
                    connection.State)
        End Sub

        Private Shared Sub HandleInstanceStateEvent(ByVal sender As Object, ByVal e As PlcDeviceConnectionStateChangedEventArgs)
            ' The passed sender does provide the instance affected by the event.
            Dim connection As PlcDeviceConnection = CType(sender, PlcDeviceConnection)

            Console.WriteLine("4. Way: State of connection to '{0}':", connection.Device.EndPoint)
            Console.WriteLine("-> Changed from {0} to {1}.", e.OldState, e.NewState)
        End Sub

        Private Shared Sub HandleStateEvent(ByVal sender As Object, ByVal e As PlcNotifications.PlcDeviceConnectionStateChangedEventArgs)
            ' The passed event data (e) does provide the instance affected by the event.
            Dim connection As PlcDeviceConnection = e.Connection
            
            Console.WriteLine("2. Way: State of connection to '{0}':", connection.Device.EndPoint)
            Console.WriteLine("-> Changed from {0} to {1}.", e.OldState, e.NewState)
        End Sub
    End Class
End Namespace

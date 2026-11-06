' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports IPS7Lnk.Advanced

Namespace DeviceConnection
    ''' <summary>
    ''' This sample demonstrates the states of a PlcDeviceConnection from opening to closing and
    ''' how to monitor the state of all connections of an application.
    ''' </summary>
    ''' <remarks>
    ''' Without an argument, the sample uses the simulation. Pass the IP address of your PLC as
    ''' the first argument to connect to the PLC; the sample reads MB 0 only.
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

#Region "1. Monitor all connections"
            ' ConnectionCreated is raised for each new connection of the application. Here, the
            ' handler monitors the state of each connection.
            AddHandler PlcNotifications.ConnectionCreated, AddressOf Program.HandleConnectionCreated
#End Region

            Dim device As PlcDevice = Program.CreateDevice(args)

            Using connection As PlcDeviceConnection = device.CreateConnection()
#Region "2. Break detection and watchdog"
                ' The break detection only keeps the TCP connection alive. The watchdog reads
                ' MB 0 if the application has not accessed the PLC during WatchDogInterval and
                ' reports a broken connection with the state Disconnected.
                connection.UseBreakDetection = True
                connection.UseWatchDog = True
#End Region

#Region "3. Open, connect and close"
                ' Open checks the configuration, Connect establishes the connection to the PLC.
                connection.Open()
                connection.Connect()

                ' After a connection break, the next access connects again.
                Dim value As Byte = connection.ReadByte("MB 0")
                Console.WriteLine("MB 0 = {0}", value)

                ' Close ends the connection to the PLC and releases its resources.
                connection.Close()
#End Region
            End Using
        End Sub

        Private Shared Function CreateDevice(ByVal args As String()) As PlcDevice

            If args.Length = 0 Then
                Return New VirtualDevice()
            End If
            ' Pass an IPDeviceEndPoint with rack and slot and the device type of your PLC if they
            ' differ from the defaults (see the sample DeviceTypes).
            Return New SimaticDevice(args(0))
        End Function

        Private Shared Sub HandleConnectionCreated(
                ByVal sender As Object,
                ByVal e As PlcNotifications.PlcDeviceConnectionEventArgs)

            AddHandler e.Connection.StateChanged, AddressOf Program.HandleConnectionStateChanged
        End Sub

        Private Shared Sub HandleConnectionStateChanged(
                ByVal sender As Object,
                ByVal e As PlcDeviceConnectionStateChangedEventArgs)

            Console.WriteLine("Connection state changed from {0} to {1}.", e.OldState, e.NewState)
        End Sub
    End Class
End Namespace

' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports System.Threading.Tasks
Imports IPS7Lnk.Advanced

Namespace SharedConnections
    ''' <summary>
    ''' This sample demonstrates how several threads share one SimaticClient and how several
    ''' PlcDeviceConnection objects share one connection to the PLC.
    ''' </summary>
    ''' <remarks>
    ''' The sample reads from an S7-300/400 only. Pass the IP address of your PLC as the first
    ''' argument; for another device type, change the scheme and SimaticDeviceType.
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

            Dim host As String = If(args.Length > 0, args(0), "192.168.0.80")

#Region "1. One client in several threads"
            Using client = New SimaticClient("s7300://" & host)
                client.Connect()

                ' The client locks its connection for each access and runs simultaneous accesses
                ' one after the other, without another connection to the PLC.
                Dim tasks As Task() = New Task(2) {}

                For index As Integer = 0 To tasks.Length - 1
                    Dim number As Integer = index
                    tasks(index) = Task.Run(Sub() Program.ReadMarker(client, number))
                Next

                Task.WaitAll(tasks)
                client.Disconnect()
            End Using
#End Region

#Region "2. Several connection objects share one connection"
            ' UsePool is static and applies to the whole application.
            PlcDeviceConnection.UsePool = True

            Dim device = New SimaticDevice(host, SimaticDeviceType.S7300_400)
            Dim readConnection As PlcDeviceConnection = device.CreateConnection(createNew:=True)
            Dim monitorConnection As PlcDeviceConnection = device.CreateConnection(createNew:=True)

            readConnection.Open()
            monitorConnection.Open()

            Console.WriteLine("MB 0 = {0}", readConnection.ReadByte("MB 0"))
            Console.WriteLine("MB 1 = {0}", monitorConnection.ReadByte("MB 1"))

            ' The SDK closes the shared connection to the PLC with the last connection object.
            readConnection.Close()
            monitorConnection.Close()
            PlcDeviceConnection.ClearAllPools()
#End Region
        End Sub

        Private Shared Sub ReadMarker(ByVal client As SimaticClient, ByVal number As Integer)

            Dim value As Byte = client.ReadValue(Of Byte)("MB 0")
            Console.WriteLine("Task {0}: MB 0 = {1}", number, value)
        End Sub
    End Class
End Namespace

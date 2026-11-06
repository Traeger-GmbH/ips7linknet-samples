' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports IPS7Lnk.Advanced

Namespace Simulation
    ''' <summary>
    ''' This sample demonstrates how to develop and test code without a PLC by using the
    ''' simulation of the low-level API.
    ''' </summary>
    ''' <remarks>
    ''' Without an argument, the sample uses the simulation. Pass the IP address of your PLC as
    ''' the first argument to run the same code against the PLC: it writes DB1.DBD 0 and 4.
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

#Region "1. PLC or simulation"
            ' SimaticDevice and VirtualDevice are both derived from PlcDevice: the code that
            ' follows is the same for both.
            Dim device As PlcDevice = Program.CreateDevice(args)
#End Region

            Using connection As PlcDeviceConnection = device.CreateConnection()
                connection.Open()

#Region "2. Read and write"
                ' The simulation returns what the application has written before.
                connection.WriteInt32("DB1.DBD 0", 1200)
                Dim speed As Integer = connection.ReadInt32("DB1.DBD 0")
                Console.WriteLine("Speed: {0}", speed)
#End Region

#Region "3. Test code without a PLC"
                ' The simulation keeps its data per connection and discards it with Close. A
                ' test therefore writes its data on the connection the tested code reads from.
                connection.WriteInt32("DB1.DBD 4", 42)
                Dim pieces As Integer = Program.ReadPieces(connection)
                Dim result As String = If(pieces = 42, "passed", "failed")
                Console.WriteLine("Test {0}: pieces = {1}", result, pieces)
#End Region

#Region "4. Data of another connection"
                Using other As PlcDeviceConnection = device.CreateConnection()
                    other.Open()

                    ' In the simulation, a second connection does not see the data of the first.
                    Dim otherPieces As Integer = Program.ReadPieces(other)
                    Console.WriteLine("Pieces on another connection: {0}", otherPieces)
                End Using
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

        ' The tested code reads through the connection it receives instead of creating one.
        Private Shared Function ReadPieces(ByVal connection As PlcDeviceConnection) As Integer

            Return connection.ReadInt32("DB1.DBD 4")
        End Function
    End Class
End Namespace

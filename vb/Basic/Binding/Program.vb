' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports System.ComponentModel
Imports System.Threading
Imports IPS7Lnk.Advanced

Namespace Binding
    ''' <summary>
    ''' This sample demonstrates how a PlcObject reports changed values through PropertyChanged,
    ''' the event a user interface binds to.
    ''' </summary>
    ''' <remarks>
    ''' Without an argument, the sample uses the simulation and changes the speed itself. Pass the
    ''' IP address of your PLC as the first argument to read DB1.DBD 0 and 4 from the PLC.
    ''' </remarks>
    Public Class Program

        Private Shared ReadOnly [stop] As ManualResetEvent = New ManualResetEvent(False)

        Public Shared Sub Main(ByVal args As String())

            Dim device As PlcDevice = Program.CreateDevice(args)
            Dim data = New MachineData()

#Region "1. Handle PropertyChanged"
            ' A user interface binds to the properties of the object in the same way, for example
            ' with a WPF binding to the path Speed.
            AddHandler data.PropertyChanged, AddressOf Program.HandleDataPropertyChanged
#End Region

            Using connection As PlcDeviceConnection = device.CreateConnection()
                connection.Open()

                AddHandler Console.CancelKeyPress, AddressOf Program.HandleCancelKeyPress
                Console.WriteLine("Reading once per second. Press Ctrl+C to exit.")

#Region "2. Read cyclically"
                Dim cycle As Integer = 0

                Do While Not Program.stop.WaitOne(1000)
                    If TypeOf device Is VirtualDevice Then
                        ' In the simulation, the sample changes the values itself.
                        connection.WriteInt32("DB1.DBD 0", 1000 + (cycle Mod 3) * 100)
                        connection.WriteReal("DB1.DBD 4", 21.5F)
                        cycle += 1
                    End If

                    ' ReadObject raises PropertyChanged for each property whose value has changed.
                    connection.ReadObject(data)
                Loop
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

        Private Shared Sub HandleCancelKeyPress(
                ByVal sender As Object,
                ByVal e As ConsoleCancelEventArgs)

            e.Cancel = True
            Program.stop.Set()
        End Sub

        Private Shared Sub HandleDataPropertyChanged(
                ByVal sender As Object,
                ByVal e As PropertyChangedEventArgs)

            Dim data = CType(sender, MachineData)
            Console.WriteLine("{0} changed to {1}", e.PropertyName, data(e.PropertyName))
        End Sub
    End Class
End Namespace

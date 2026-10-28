' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports System.Threading
Imports IPS7Lnk.Advanced

Namespace Connection
    ''' <summary>
    ''' This sample demonstrates how to configure, establish and monitor the connection of a
    ''' SimaticClient, including timeouts, break detection and the watchdog.
    ''' </summary>
    ''' <remarks>
    ''' The sample reads MB 0 once per second until Ctrl+C is pressed. Pass the address of your
    ''' PLC as the first argument, for example "s71500://192.168.0.80".
    ''' </remarks>
    Public Class Program

        Private Shared ReadOnly [stop] As ManualResetEvent = New ManualResetEvent(False)

        Public Shared Sub Main(ByVal args As String())

#Region "1. Address of the PLC"
            ' The scheme selects the device type, the query sets rack and slot if they differ
            ' from the defaults, for example "s7300://192.168.0.80?rack=0&slot=2".
            Dim address As String = If(args.Length > 0, args(0), "s7300://192.168.0.80")
#End Region

            Using client = New SimaticClient(address)
#Region "2. Timeouts"
                client.ConnectTimeout = 5000
                client.OperationTimeout = 5000
#End Region

#Region "3. Break detection and watchdog"
                ' The break detection only keeps the TCP connection alive. The watchdog reads
                ' MB 0 if the application has not accessed the PLC during WatchDogInterval and
                ' reports a broken connection with the state Disconnected.
                client.UseBreakDetection = True
                client.UseWatchDog = True
                client.WatchDogInterval = 5000
#End Region

#Region "4. Connection events"
                AddHandler client.StateChanged, AddressOf Program.HandleClientStateChanged
                AddHandler client.Disconnected, AddressOf Program.HandleClientDisconnected
#End Region

#Region "5. Connect"
                Try
                    client.Connect()

                Catch ex As PlcException When ex.Code = PlcStatusCode.Timeout
                    Console.WriteLine("The PLC is not reachable: {0}", ex.Message)
                    Return
                End Try
#End Region

#Region "6. Access and connection break"
                AddHandler Console.CancelKeyPress, AddressOf Program.HandleCancelKeyPress
                Console.WriteLine("Reading MB 0 once per second. Press Ctrl+C to exit.")

                Do While Not Program.stop.WaitOne(1000)
                    Try
                        Dim value As Byte = client.ReadValue(Of Byte)("MB 0")
                        Console.WriteLine("MB 0 = {0}", value)

                    Catch ex As PlcException
                        ' The client does not reconnect in the background: the next access
                        ' establishes the connection again.
                        Console.WriteLine("Access failed: {0}", ex.Code)
                    End Try
                Loop

                client.Disconnect()
#End Region
            End Using
        End Sub

        Private Shared Sub HandleCancelKeyPress(
                ByVal sender As Object,
                ByVal e As ConsoleCancelEventArgs)

            e.Cancel = True
            Program.stop.Set()
        End Sub

        Private Shared Sub HandleClientDisconnected(ByVal sender As Object, ByVal e As EventArgs)

            ' Raised when the watchdog detects a broken connection or after Disconnect.
            Console.WriteLine("The connection to the PLC is closed or broken.")
        End Sub

        Private Shared Sub HandleClientStateChanged(
                ByVal sender As Object,
                ByVal e As SimaticClientStateChangedEventArgs)

            Console.WriteLine("Client state changed from {0} to {1}.", e.OldState, e.NewState)
        End Sub
    End Class
End Namespace

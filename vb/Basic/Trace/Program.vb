' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports System.Collections.Concurrent
Imports System.Threading
Imports IPS7Lnk.Advanced

Namespace Trace
    ''' <summary>
    ''' This sample demonstrates how to enable the trace of the SDK with combined levels and how
    ''' to write its entries through a queue on a separate thread.
    ''' </summary>
    ''' <remarks>
    ''' The sample reads from the PLC only. Pass the address of your PLC as the first argument,
    ''' for example "s71500://192.168.0.80".
    ''' </remarks>
    Public Class Program

        Private Shared ReadOnly entries As BlockingCollection(Of String) =
                New BlockingCollection(Of String)()

        Public Shared Sub Main(ByVal args As String())

#Region "1. Write the entries on a separate thread"
            ' The SDK calls WriteEntry on the thread of the SDK call. A slow output would delay
            ' each access to the PLC: queue the entries there and write them elsewhere.
            Dim writer = New Thread(AddressOf Program.WriteEntries) With {.IsBackground = True}
            writer.Start()
#End Region

#Region "2. Enable the trace"
            ' The levels are flags, not a threshold: an entry is written if its level is part of
            ' Level. Verbose entries contain addresses and values; use them for debugging only.
            PlcTrace.Level = PlcTraceLevel.Error _
                    Or PlcTraceLevel.Warning _
                    Or PlcTraceLevel.Information
            PlcTrace.WriteEntry = AddressOf Program.HandleTraceEntry
            PlcTrace.IsEnabled = True
#End Region

#Region "3. Accesses that write entries"
            Dim address As String = If(args.Length > 0, args(0), "s7300://192.168.0.80")

            Using client = New SimaticClient(address)
                ' Information entries report connecting to and disconnecting from the PLC.
                client.Connect()
                Console.WriteLine("MB 0 = {0}", client.ReadValue(Of Byte)("MB 0"))

                Try
                    ' The following assumes that DB65535 does not exist in the PLC.
                    client.ReadValue(Of Integer)("DB65535.DBD 0")

                Catch ex As PlcException
                    ' Error entries contain the exception with its stack trace.
                    Console.WriteLine("Access failed: {0}", ex.Code)
                End Try

                client.Disconnect()
            End Using
#End Region

            PlcTrace.IsEnabled = False
            Program.entries.CompleteAdding()
            writer.Join()
        End Sub

        Private Shared Sub HandleTraceEntry(ByVal level As PlcTraceLevel, ByVal message As String)

            Program.entries.Add(String.Format("Trace {0}: {1}", level, message))
        End Sub

        Private Shared Sub WriteEntries()

            For Each entry As String In Program.entries.GetConsumingEnumerable()
                Console.WriteLine(entry)
            Next
        End Sub
    End Class
End Namespace

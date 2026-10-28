' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports IPS7Lnk.Advanced

Namespace BulkAccess
    ''' <summary>
    ''' This sample demonstrates how to read many values in one call and how to handle a missing
    ''' data area among them.
    ''' </summary>
    ''' <remarks>
    ''' The sample reads from the PLC only. Pass the address of your PLC as the first argument,
    ''' for example "s71500://192.168.0.80".
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

            Dim address As String = If(args.Length > 0, args(0), "s7300://192.168.0.80")

            Using client = New SimaticClient(address)
#Region "1. Unreachable PLC"
                Try
                    client.Connect()

                Catch ex As PlcException When ex.Code = PlcStatusCode.Timeout
                    Console.WriteLine("The PLC is not reachable: {0}", ex.Message)
                    Return
                End Try
#End Region

#Region "2. Many values in one call"
                Dim markers As PlcByte() = New PlcByte(99) {}

                For index As Integer = 0 To markers.Length - 1
                    markers(index) = New PlcByte("MB " & index)
                Next

                ' The SDK packs the values into as few telegrams as the PDU size allows.
                client.ReadValues(markers)
                Console.WriteLine("MB 0 = {0}, MB 99 = {1}", markers(0).Value, markers(99).Value)
#End Region

#Region "3. Missing data area"
                Dim word = New PlcInt16("MW 0")
                Dim doubleWord = New PlcInt32("MD 4")

                ' The following assumes that DB65535 does not exist in the PLC.
                Dim missing = New PlcInt32("DB65535.DBD 0")

                Try
                    client.ReadValues(word, doubleWord, missing)

                Catch ex As PlcException
                    ' A missing data area is reported with the status code NoData.
                    Console.WriteLine("Access failed: {0} ({1})", ex.Code, ex.Type?.Address)
                End Try
#End Region

#Region "4. Status of each value"
                For Each value As IPlcValue In New IPlcValue() {word, doubleWord, missing}
                    Console.WriteLine("{0}: {1}", value.Type.Address, value.Status.Code)
                Next
#End Region
            End Using
        End Sub
    End Class
End Namespace

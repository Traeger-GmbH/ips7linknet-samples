' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace QueryDynamicData
    ''' <summary>
    ''' This sample demonstrates how to query dynamic data within an application.
    ''' </summary>
    ''' <remarks>
    ''' This application does query data there its source address can vary from read to read.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            ' The following lines of code demonstrate how to read and write data
            ' using a custom PLC Object which defines its members dynamically.
            '
            ' Note that each instance of the class Data initialized does refer to
            ' different DataBlocks upon changing the static DataBlockNumber property
            ' of the class.
            '
            ' This scenario is not a general way how to use dynamic PlcObject members.
            ' It just demonstrates one possible way to refer to dynamic PLC data in an
            ' independent way.

            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            Data.DataBlockNumber = 1
            Dim data1 As Data = connection.ReadObject(Of Data)()

            Data.DataBlockNumber = 10
            Dim data10 As Data = connection.ReadObject(Of Data)()

            Data.DataBlockNumber = 15
            Dim data15 As Data = connection.ReadObject(Of Data)()

            Data.DataBlockNumber = 20

            Dim data20 As Data = New Data()
            data20.ByteValue = data1.ByteValue
            data20.Int16Value = data10.Int16Value
            data20.Int32Value = data15.Int32Value
            data20.RealValue = data10.RealValue / data1.RealValue
            data20.StringValue = data15.StringValue

            connection.WriteObject(data20)
            connection.Close()
        End Sub
    End Class
End Namespace

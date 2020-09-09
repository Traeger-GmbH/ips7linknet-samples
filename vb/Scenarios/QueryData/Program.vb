' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace QueryData
    ''' <summary>
    ''' This sample demonstrates how to query scalar values within an application.
    ''' </summary>
    ''' <remarks>
    ''' This application does write/read scalar values to/from the PLC.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            ' 1. Way: Sequential Write/Read.
            If True Then
                ' Either use the primitive low level methods of the PLC device connection to
                ' sequential write the data or read the desired data areas.

                connection.WriteByte("DB111.DBB 2", 15)
                Console.WriteLine("DB111.DBB 2: {0}", connection.ReadByte("DB111.DBB 2"))

                connection.WriteInt16("DB111.DBW 4", 600)
                Console.WriteLine("DB111.DBW 4: {0}", connection.ReadInt16("DB111.DBW 4"))

                connection.WriteInt32("DB111.DBD 6", 280)
                Console.WriteLine("DB111.DBD 6: {0}", connection.ReadInt32("DB111.DBD 6"))

                connection.WriteReal("DB111.DBD 10", 2.46F)
                Console.WriteLine("DB111.DBD 10: {0}", connection.ReadReal("DB111.DBD 10"))

                connection.WriteString("DB111.DBB 20", "4-036300-076816")
                Console.WriteLine("DB111.DBB 20: {0}", connection.ReadString("DB111.DBB 20", 16))
            End If

            ' 2. Way: Bulk write/read (with variables).
            If True Then
                ' Or use the higher level methods of the PLC device connection to write/read
                ' a whole set of variables at once. While this way would be much faster than the
                ' previous one, because the values are write/read within one transaction
                ' instead of processing each value within a transaction for each action.

                connection.WriteValues( _
                        New PlcByte("DB111.DBB 2", 15), _
                        New PlcInt16("DB111.DBW 4", 600), _
                        New PlcInt32("DB111.DBD 6", 280), _
                        New PlcReal("DB111.DBD 10", 2.46F), _
                        New PlcString("DB111.DBB 20", "4-036300-076816", 16))

                Dim values As Object() = connection.ReadValues( _
                        New PlcByte("DB111.DBB 2"), _
                        New PlcInt16("DB111.DBW 4"), _
                        New PlcInt32("DB111.DBD 6"), _
                        New PlcReal("DB111.DBD 10"), _
                        New PlcString("DB111.DBB 20", 16))

                Console.WriteLine("DB111.DBB 2: {0}", values(0))
                Console.WriteLine("DB111.DBW 4: {0}", values(1))
                Console.WriteLine("DB111.DBD 6: {0}", values(2))
                Console.WriteLine("DB111.DBD 10: {0}", values(3))
                Console.WriteLine("DB111.DBB 20: {0}", values(4))
            End If

            ' 3. Way: Bulk write/read (with object).
            If True Then
                ' Or use the methods of the PLC device connection at the highest abstraction
                ' layer to write/read the whole PLC data at once from a user defined PLC object.

                Dim data As Data = New Data()
                data.ByteValue = 15
                data.Int16Value = 600
                data.Int32Value = 280
                data.RealValue = 2.46F
                data.StringValue = "4-036300-076816"

                connection.WriteObject(data)
                data = connection.ReadObject(Of Data)()
                
                Console.WriteLine("DB111.DBB 2: {0}", data.ByteValue)
                Console.WriteLine("DB111.DBW 4: {0}", data.Int16Value)
                Console.WriteLine("DB111.DBD 6: {0}", data.Int32Value)
                Console.WriteLine("DB111.DBD 10: {0}", data.RealValue)
                Console.WriteLine("DB111.DBB 20: {0}", data.StringValue)
            End If

            Console.ReadKey()
        End Sub
    End Class
End Namespace

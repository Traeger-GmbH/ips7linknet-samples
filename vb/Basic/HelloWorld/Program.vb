' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace HelloWorld
    ''' <summary>
    '' This sample demonstrates a Hello World! application.
    ''' </summary>
    ''' <remarks>
    ''' This application does write/read the 'Hello World!' message to/from the PLC and when
    ''' prints the message on the standard output.
    '' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            connection.WriteString("DB111.DBB 100", "Hello World!")

            Dim message As String = connection.ReadString("DB111.DBB 100", 16)
            Console.WriteLine(message)

            connection.Close()
            Console.ReadKey()
        End Sub
    End Class
End Namespace

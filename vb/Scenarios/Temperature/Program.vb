Imports System
Imports System.Threading

Imports IPS7Lnk.Advanced

Namespace App
    ''' <summary>
    ''' This sample demonstrates a temperature monitor which reads the temperature from the PLC
    ''' once per second.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main()
            Dim device = New SimaticDevice("192.168.0.80")

            Using connection = device.CreateConnection()
                connection.Open()

                While True
                    Dim temperature = connection.ReadDouble("DB10.DBD 20")
                    Console.WriteLine("Current Temperature is {0} °C", temperature)

                    Thread.Sleep(1000)
                End While
            End Using
        End Sub
    End Class
End Namespace

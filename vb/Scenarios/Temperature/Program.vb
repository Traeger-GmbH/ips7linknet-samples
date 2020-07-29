Imports System
Imports System.Threading

Imports IPS7Lnk.Advanced

Namespace App
    Public Class Program
        Public Shared Sub Main()
            Dim device = New SiemensDevice("192.168.0.80")

            Using connection = device.CreateConnection()
                connection.Open()

                While True
                    Dim temperature = connection.ReadDouble("DB10.DBD 20")
                    Console.WriteLine($"Current Temperature is {0} °C", temperature)

                    Thread.Sleep(1000)
                End While
            End Using
        End Sub
    End Class
End Namespace

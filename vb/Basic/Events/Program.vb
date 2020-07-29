' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports System.Threading

Imports IPS7Lnk.Advanced

Namespace Events
    ''' <summary>
    ''' This sample demonstrates a weather station logger application which prints PLC weather
    ''' data to the default output stream.
    ''' </summary>
    ''' <remarks>
    ''' This application does read the weather data from the PLC and when writes the data to the
    ''' default output stream. The logger does poll the PLC every 10 seconds for new weather data.
    ''' Only if the temperature changes the latest weather data is written to the default
    ''' output stream.
    ''' </remarks>
    Public Class Program
        Private Shared temperature As PlcReal = New PlcReal("DB111.DBD 10")

        Public Shared Sub Main(ByVal args As String())
            Dim device As SiemensDevice = New SiemensDevice( _
                    New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            AddHandler Program.temperature.Changed, AddressOf Program.HandleTemperatureChanged
            Dim pollTimer As Timer = New Timer(AddressOf Program.PollWeatherStation, connection, 0, 10000)
            
            Console.ReadKey()
            connection.Close()
        End Sub

        Private Shared Sub HandleTemperatureChanged(ByVal sender As Object, ByVal e As ValueChangedEventArgs(Of Single))
            Console.WriteLine("Temperature changed from {0} °C to {1} °C", e.OldValue, e.NewValue)
        End Sub

        Private Shared Sub PollWeatherStation(ByVal state As Object)
            CType(state, PlcDeviceConnection).ReadValues(Program.temperature)
        End Sub
    End Class
End Namespace

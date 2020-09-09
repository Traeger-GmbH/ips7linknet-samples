' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports System.IO
Imports System.Threading
Imports IPS7Lnk.Advanced

Namespace PlcToCsv
    ''' <summary>
    ''' This sample demonstrates a weather station logger application which writes PLC weather
    ''' data to a CSV file.
    ''' </summary>
    ''' <remarks>
    ''' This application does read the weather data from the PLC and when writes the data to a CSV
    ''' file. After a weather record has been written by the logger, the logger does wait for 30
    ''' minutes before the next record will be read from the PLC.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            Dim values As String() = New String(4) {}
            Dim writer As StreamWriter = File.AppendText(".\Data.csv")

            ' 1. Way: Sequential Read.
            If True Then
                ' Either use the primitive low level methods of the PLC device connection to
                ' sequential read the desired data areas.

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    values(0) = connection.ReadByte("DB111.DBB 2").ToString()
                    values(1) = connection.ReadInt16("DB111.DBW 4").ToString()
                    values(2) = connection.ReadInt32("DB111.DBD 6").ToString()
                    values(3) = connection.ReadReal("DB111.DBD 10").ToString()
                    values(4) = connection.ReadString("DB111.DBB 20", 16)

                    writer.WriteLine(String.Join(";", values))
                    writer.Flush()

                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            ' 2. Way: Bulk read (with variables).
            If True Then
                ' Or use the higher level methods of the PLC device connection to read a whole
                ' set of variables at once. While this way would be much faster than the
                ' previous one, because the values are read within one transaction instead of
                ' querying each value within a transaction for each request.

                Dim chanceOfRain As PlcByte = New PlcByte("DB111.DBB 2")
                Dim windSpeed As PlcInt16 = New PlcInt16("DB111.DBW 4")
                Dim pressure As PlcInt32 = New PlcInt32("DB111.DBD 6")
                Dim temperature As PlcReal = New PlcReal("DB111.DBD 10")
                Dim forecast As PlcString = New PlcString("DB111.DBB 20", 16)

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    connection.ReadValues(chanceOfRain, windSpeed, pressure, temperature, forecast)
                    
                    values(0) = chanceOfRain.Value.ToString()
                    values(1) = windSpeed.Value.ToString()
                    values(2) = pressure.Value.ToString()
                    values(3) = temperature.Value.ToString()
                    values(4) = forecast.Value.ToString()

                    writer.WriteLine(String.Join(";", values))
                    writer.Flush()

                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            ' 3. Way: Bulk read (with object).
            If True Then
                ' Or use the methods of the PLC device connection at the highest abstraction
                ' layer to read the whole PLC data at once into a user defined PLC object.

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    Dim data As WeatherData = connection.ReadObject(Of WeatherData)()

                    values(0) = data.ChanceOfRain.ToString()
                    values(1) = data.WindSpeed.ToString()
                    values(2) = data.Pressure.ToString()
                    values(3) = data.Temperature.ToString()
                    values(4) = data.Forecast.ToString()

                    writer.WriteLine(String.Join(";", values))
                    writer.Flush()

                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            writer.Close()
            connection.Close()
        End Sub
    End Class
End Namespace

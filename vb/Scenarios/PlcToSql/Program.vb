' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports System.Data
Imports System.Data.SQLite
Imports System.Threading

Imports IPS7Lnk.Advanced

Namespace PlcToSql
    ''' <summary>
    ''' This sample demonstrates a weather station logger application which writes PLC weather
    ''' data to a Microsoft SQL Server database.
    ''' </summary>
    ''' <remarks>
    ''' This application does read the weather data from the PLC and when writes the data to a
    ''' Microsoft SQL Server database. After a weather record has been written by the logger,
    ''' the logger does wait for 30 minutes before the next record will be read from the PLC.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            Dim sqlConnection As SQLiteConnection = New SQLiteConnection("Data Source=.\Database.db")
            sqlConnection.Open()

            Dim command As SQLiteCommand = sqlConnection.CreateCommand()
            command.CommandText = "insert into Data (ChanceOfRain, WindSpeed, Pressure, Temperature, Forecast) " _
                    & "values (@chanceOfRain, @windSpeed, @pressure, @temperature, @forecast)"

            Dim sqlChanceOfRain As SQLiteParameter = command.Parameters.Add("@chanceOfRain", DbType.Int16)
            Dim sqlWindSpeed As SQLiteParameter = command.Parameters.Add("@windSpeed", DbType.Int16)
            Dim sqlPressure As SQLiteParameter = command.Parameters.Add("@pressure", DbType.Int16)
            Dim sqlTemperature As SQLiteParameter = command.Parameters.Add("@temperature", DbType.Single)
            Dim sqlForecast As SQLiteParameter = command.Parameters.Add("@forecast", DbType.String)

            ' 1. Way: Sequential Read.
            If True Then
                ' Either use the primitive low level methods of the PLC device connection to
                ' sequential read the desired data areas.

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    sqlChanceOfRain.Value = connection.ReadByte("DB111.DBB 2")    ' Chance of rain.
                    sqlWindSpeed.Value = connection.ReadInt16("DB111.DBW 4")      ' Wind speed.
                    sqlPressure.Value = connection.ReadInt32("DB111.DBD 6")       ' Pressure.
                    sqlTemperature.Value = connection.ReadReal("DB111.DBD 10")    ' Temperature.
                    sqlForecast.Value = connection.ReadString("DB111.DBB 20", 16) ' Forecast.

                    command.ExecuteNonQuery()
                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            ' 2. Way: Bulk read (with variables).
            If True Then
                ' Or use the higher level methods of the PLC device connection to read a whole
                ' set of variables at once. While this way would be much faster than the previous
                ' one, because the values are read within one transaction instead of querying
                ' each value within a transaction for each request.

                Dim chanceOfRain As PlcByte = New PlcByte("DB111.DBB 2")
                Dim windSpeed As PlcInt16 = New PlcInt16("DB111.DBW 4")
                Dim pressure As PlcInt32 = New PlcInt32("DB111.DBD 6")
                Dim temperature As PlcReal = New PlcReal("DB111.DBD 10")
                Dim forecast As PlcString = New PlcString("DB111.DBB 20", 16)

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    connection.ReadValues(chanceOfRain, windSpeed, pressure, temperature, forecast)

                    sqlChanceOfRain.Value = chanceOfRain.Value ' Chance of rain.
                    sqlWindSpeed.Value = windSpeed.Value       ' Wind speed.
                    sqlPressure.Value = pressure.Value         ' Pressure.
                    sqlTemperature.Value = temperature.Value   ' Temperature.
                    sqlForecast.Value = forecast.Value         ' Forecast.

                    command.ExecuteNonQuery()
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

                    sqlChanceOfRain.Value = data.ChanceOfRain ' Chance of rain.
                    sqlWindSpeed.Value = data.WindSpeed       ' Wind speed.
                    sqlPressure.Value = data.Pressure         ' Pressure.
                    sqlTemperature.Value = data.Temperature   ' Temperature.
                    sqlForecast.Value = data.Forecast         ' Forecast.

                    command.ExecuteNonQuery()
                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            sqlConnection.Close()
            connection.Close()
        End Sub
    End Class
End Namespace

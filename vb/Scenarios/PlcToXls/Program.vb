' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.


Imports System
Imports System.Data.OleDb
Imports System.Threading

Imports IPS7Lnk.Advanced

Namespace PlcToXls
    ''' <summary>
    ''' This sample demonstrates a weather station logger application which writes PLC weather
    ''' data to a XLS file.
    ''' </summary>
    ''' <remarks>
    ''' This application does read the weather data from the PLC and when writes the data to a XLS
    ''' file. After a weather record has been written by the logger, the logger does wait for 30
    ''' minutes before the next record will be read from the PLC.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SimaticDevice = New SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            ' NOTE
            ' To access XLS files using OLEDB you will need to install
            ' "Microsoft Access Database Engine 2010 Redistributable"
            ' https://www.microsoft.com/en-us/download/details.aspx?id=13255
            Dim excelConnection As OleDbConnection = New OleDbConnection( _
                    "Provider=Microsoft.ACE.OLEDB.12.0;" _
                    & "Data Source=.\Data.xls;" & "Extended Properties=Excel 12.0")

            excelConnection.Open()

            ' 'Data' represents the Excel Worksheet to write.
            Dim command As OleDbCommand = excelConnection.CreateCommand()
            command.CommandText = "insert into [Data$] values (?, ?, ?, ?, ?)"

            Dim excelChanceOfRain As OleDbParameter = command.Parameters.Add("@chanceOfRain", OleDbType.UnsignedTinyInt)
            Dim excelWindSpeed As OleDbParameter = command.Parameters.Add("@windSpeed", OleDbType.SmallInt)
            Dim excelPressure As OleDbParameter = command.Parameters.Add("@pressure", OleDbType.Integer)
            Dim excelTemperature As OleDbParameter = command.Parameters.Add("@temperature", OleDbType.Single)
            Dim excelForecast As OleDbParameter = command.Parameters.Add("@forecast", OleDbType.BSTR)

            ' 1. Way: Sequential Read.
            If True Then
                ' Either use the primitive low level methods of the PLC device connection to
                ' sequential read the desired data areas.

                ' Is the weather station online.
                While connection.ReadBoolean("E 1.0")
                    excelChanceOfRain.Value = connection.ReadByte("DB111.DBB 2")    ' Chance of rain.
                    excelWindSpeed.Value = connection.ReadInt16("DB111.DBW 4")      ' Wind speed.
                    excelPressure.Value = connection.ReadInt32("DB111.DBD 6")       ' Pressure.
                    excelTemperature.Value = connection.ReadReal("DB111.DBD 10")    ' Temperature.
                    excelForecast.Value = connection.ReadString("DB111.DBB 20", 16) ' Forecast.

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

                    excelChanceOfRain.Value = chanceOfRain.Value ' Chance of rain.
                    excelWindSpeed.Value = windSpeed.Value       ' Wind speed.
                    excelPressure.Value = pressure.Value         ' Pressure.
                    excelTemperature.Value = temperature.Value   ' Temperature.
                    excelForecast.Value = forecast.Value         ' Forecast.

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

                    excelChanceOfRain.Value = data.ChanceOfRain ' Chance of rain.
                    excelWindSpeed.Value = data.WindSpeed       ' Wind speed.
                    excelPressure.Value = data.Pressure         ' Pressure.
                    excelTemperature.Value = data.Temperature   ' Temperature.
                    excelForecast.Value = data.Forecast         ' Forecast.

                    command.ExecuteNonQuery()
                    Thread.Sleep(TimeSpan.FromMinutes(30))
                End While
            End If

            excelConnection.Close()
            connection.Close()
        End Sub
    End Class
End Namespace

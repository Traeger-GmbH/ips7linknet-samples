// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace PlcToXls
{
    using System;
    using System.Data.OleDb;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a weather station logger application which writes PLC weather
    /// data to a XLS file.
    /// </summary>
    /// <remarks>
    /// This application does read the weather data from the PLC and when writes the data to a XLS
    /// file. After a weather record has been written by the logger, the logger does wait for 30
    /// minutes before the next record will be read from the PLC.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            SiemensDevice device = new SiemensDevice(
                    new IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            // NOTE
            // To access XLS files using OLEDB you will need to install
            // "Microsoft Access Database Engine 2010 Redistributable"
            // https://www.microsoft.com/en-us/download/details.aspx?id=13255
            OleDbConnection excelConnection = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;"
                    + @"Data Source=.\Data.xls;"
                    + "Extended Properties=Excel 12.0");

            excelConnection.Open();

            // 'Data' represents the Excel Worksheet to write.
            OleDbCommand command = excelConnection.CreateCommand();
            command.CommandText = "insert into [Data$] values (?, ?, ?, ?, ?)";

            OleDbParameter excelChanceOfRain = command.Parameters.Add("@chanceOfRain", OleDbType.UnsignedTinyInt);
            OleDbParameter excelWindSpeed = command.Parameters.Add("@windSpeed", OleDbType.SmallInt);
            OleDbParameter excelPressure = command.Parameters.Add("@pressure", OleDbType.Integer);
            OleDbParameter excelTemperature = command.Parameters.Add("@temperature", OleDbType.Single);
            OleDbParameter excelForecast = command.Parameters.Add("@forecast", OleDbType.BSTR);

            #region 1. Way: Sequential Read.
            {
                //// Either use the primitive low level methods of the PLC device connection to
                //// sequential read the desired data areas.

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    excelChanceOfRain.Value = connection.ReadByte("DB111.DBB 2");    // Chance of rain.
                    excelWindSpeed.Value = connection.ReadInt16("DB111.DBW 4");      // Wind speed.
                    excelPressure.Value = connection.ReadInt32("DB111.DBD 6");       // Pressure.
                    excelTemperature.Value = connection.ReadReal("DB111.DBD 10");    // Temperature.
                    excelForecast.Value = connection.ReadString("DB111.DBB 20", 16); // Forecast.

                    command.ExecuteNonQuery();
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            #region 2. Way: Bulk read (with variables).
            {
                //// Or use the higher level methods of the PLC device connection to read a whole
                //// set of variables at once. While this way would be much faster than the previous
                //// one, because the values are read within one transaction instead of querying
                //// each value within a transaction for each request.

                PlcByte chanceOfRain = new PlcByte("DB111.DBB 2");
                PlcInt16 windSpeed = new PlcInt16("DB111.DBW 4");
                PlcInt32 pressure = new PlcInt32("DB111.DBD 6");
                PlcReal temperature = new PlcReal("DB111.DBD 10");
                PlcString forecast = new PlcString("DB111.DBB 20", 16);

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    connection.ReadValues(chanceOfRain, windSpeed, pressure, temperature, forecast);

                    excelChanceOfRain.Value = chanceOfRain.Value; // Chance of rain.
                    excelWindSpeed.Value = windSpeed.Value;       // Wind speed.
                    excelPressure.Value = pressure.Value;         // Pressure.
                    excelTemperature.Value = temperature.Value;   // Temperature.
                    excelForecast.Value = forecast.Value;         // Forecast.

                    command.ExecuteNonQuery();
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            #region 3. Way: Bulk read (with object).
            {
                //// Or use the methods of the PLC device connection at the highest abstraction
                //// layer to read the whole PLC data at once into a user defined PLC object.

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    WeatherData data = connection.ReadObject<WeatherData>();

                    excelChanceOfRain.Value = data.ChanceOfRain; // Chance of rain.
                    excelWindSpeed.Value = data.WindSpeed;       // Wind speed.
                    excelPressure.Value = data.Pressure;         // Pressure.
                    excelTemperature.Value = data.Temperature;   // Temperature.
                    excelForecast.Value = data.Forecast;         // Forecast.

                    command.ExecuteNonQuery();
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            excelConnection.Close();
            connection.Close();
        }
    }
}

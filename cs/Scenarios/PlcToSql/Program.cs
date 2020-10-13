// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace PlcToSql
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a weather station logger application which writes PLC weather
    /// data to a Microsoft SQL Server database.
    /// </summary>
    /// <remarks>
    /// This application does read the weather data from the PLC and when writes the data to a
    /// Microsoft SQL Server database. After a weather record has been written by the logger,
    /// the logger does wait for 30 minutes before the next record will be read from the PLC.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = @".\Database.db";

            SQLiteConnection sqlConnection = new SQLiteConnection(
                    builder.ToString(),
                    parseViaFramework: true);

            sqlConnection.Open();

            SQLiteCommand command = sqlConnection.CreateCommand();
            command.CommandText
                    = "insert into Data (ChanceOfRain, WindSpeed, Pressure, Temperature, Forecast) "
                    + "values (@chanceOfRain, @windSpeed, @pressure, @temperature, @forecast)";

            SQLiteParameter sqlChanceOfRain = command.Parameters.Add("@chanceOfRain", DbType.Int16);
            SQLiteParameter sqlWindSpeed = command.Parameters.Add("@windSpeed", DbType.Int16);
            SQLiteParameter sqlPressure = command.Parameters.Add("@pressure", DbType.Int16);
            SQLiteParameter sqlTemperature = command.Parameters.Add("@temperature", DbType.Single);
            SQLiteParameter sqlForecast = command.Parameters.Add("@forecast", DbType.String);

            #region 1. Way: Sequential Read.
            {
                //// Either use the primitive low level methods of the PLC device connection to
                //// sequential read the desired data areas.

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    sqlChanceOfRain.Value = connection.ReadByte("DB111.DBB 2");    // Chance of rain.
                    sqlWindSpeed.Value = connection.ReadInt16("DB111.DBW 4");      // Wind speed.
                    sqlPressure.Value = connection.ReadInt32("DB111.DBD 6");       // Pressure.
                    sqlTemperature.Value = connection.ReadReal("DB111.DBD 10");    // Temperature.
                    sqlForecast.Value = connection.ReadString("DB111.DBB 20", 16); // Forecast.

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

                    sqlChanceOfRain.Value = chanceOfRain.Value; // Chance of rain.
                    sqlWindSpeed.Value = windSpeed.Value;       // Wind speed.
                    sqlPressure.Value = pressure.Value;         // Pressure.
                    sqlTemperature.Value = temperature.Value;   // Temperature.
                    sqlForecast.Value = forecast.Value;         // Forecast.

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

                    sqlChanceOfRain.Value = data.ChanceOfRain; // Chance of rain.
                    sqlWindSpeed.Value = data.WindSpeed;       // Wind speed.
                    sqlPressure.Value = data.Pressure;         // Pressure.
                    sqlTemperature.Value = data.Temperature;   // Temperature.
                    sqlForecast.Value = data.Forecast;         // Forecast.

                    command.ExecuteNonQuery();
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            sqlConnection.Close();
            connection.Close();
        }
    }
}

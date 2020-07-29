// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace PlcToCsv
{
    using System;
    using System.IO;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a weather station logger application which writes PLC weather
    /// data to a CSV file.
    /// </summary>
    /// <remarks>
    /// This application does read the weather data from the PLC and when writes the data to a CSV
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

            string[] values = new string[5];
            StreamWriter writer = File.AppendText(@".\Data.csv");

            #region 1. Way: Sequential Read.
            {
                //// Either use the primitive low level methods of the PLC device connection to
                //// sequential read the desired data areas.

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    values[0] = connection.ReadByte("DB111.DBB 2").ToString();  // Chance of rain.
                    values[1] = connection.ReadInt16("DB111.DBW 4").ToString(); // Wind speed.
                    values[2] = connection.ReadInt32("DB111.DBD 6").ToString(); // Pressure.
                    values[3] = connection.ReadReal("DB111.DBD 10").ToString(); // Temperature.
                    values[4] = connection.ReadString("DB111.DBB 20", 16);      // Forecast.

                    writer.WriteLine(string.Join(";", values));
                    writer.Flush();

                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            #region 2. Way: Bulk read (with variables).
            {
                //// Or use the higher level methods of the PLC device connection to read a whole
                //// set of variables at once. While this way would be much faster than the
                //// previous one, because the values are read within one transaction instead of
                //// querying each value within a transaction for each request.

                PlcByte chanceOfRain = new PlcByte("DB111.DBB 2");
                PlcInt16 windSpeed = new PlcInt16("DB111.DBW 4");
                PlcInt32 pressure = new PlcInt32("DB111.DBD 6");
                PlcReal temperature = new PlcReal("DB111.DBD 10");
                PlcString forecast = new PlcString("DB111.DBB 20", 16);

                // Is the weather station online.
                while (connection.ReadBoolean("E 1.0")) {
                    connection.ReadValues(chanceOfRain, windSpeed, pressure, temperature, forecast);

                    values[0] = chanceOfRain.Value.ToString();  // Chance of rain.
                    values[1] = windSpeed.Value.ToString();     // Wind speed.
                    values[2] = pressure.Value.ToString();      // Pressure.
                    values[3] = temperature.Value.ToString();   // Temperature.
                    values[4] = forecast.Value.ToString();      // Forecast.

                    writer.WriteLine(string.Join(";", values));
                    writer.Flush();

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

                    values[0] = data.ChanceOfRain.ToString();  // Chance of rain.
                    values[1] = data.WindSpeed.ToString();     // Wind speed.
                    values[2] = data.Pressure.ToString();      // Pressure.
                    values[3] = data.Temperature.ToString();   // Temperature.
                    values[4] = data.Forecast.ToString();      // Forecast.

                    writer.WriteLine(string.Join(";", values));
                    writer.Flush();

                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }
            #endregion

            writer.Close();
            connection.Close();
        }
    }
}

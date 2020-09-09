// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Events
{
    using System;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a weather station logger application which prints PLC weather
    /// data to the default output stream.
    /// </summary>
    /// <remarks>
    /// This application does read the weather data from the PLC and when writes the data to the
    /// default output stream. The logger does poll the PLC every 10 seconds for new weather data.
    /// Only if the temperature changes the latest weather data is written to the default
    /// output stream.
    /// </remarks>
    public class Program
    {
        private static PlcReal temperature = new PlcReal("DB111.DBD 10");

        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            Program.temperature.Changed += Program.HandleTemperatureChanged;
            Timer pollTimer = new Timer(Program.PollWeatherStation, connection, 0, 10000);

            Console.ReadKey();
            connection.Close();
        }

        private static void HandleTemperatureChanged(object sender, ValueChangedEventArgs<float> e)
        {
            Console.WriteLine("Temperature changed from {0} °C to {1} °C", e.OldValue, e.NewValue);
        }

        private static void PollWeatherStation(object state)
        {
            ((PlcDeviceConnection)state).ReadValues(Program.temperature);
        }
    }
}

namespace App
{
    using System;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a temperature monitor which reads the temperature from the PLC
    /// once per second.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            var device = new SimaticDevice("192.168.0.80");

            using (var connection = device.CreateConnection()) {
                connection.Open();

                while (true) {                    
                    var temperature = connection.ReadDouble("DB10.DBD 20");
                    Console.WriteLine("Current Temperature is {0} °C", temperature);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}

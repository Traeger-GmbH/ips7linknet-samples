namespace App
{
    using System;
    using System.Threading;

    using IPS7LnkNet.Advanced;

    public class Program
    {
        public static void Main()
        {
            var device = new SiemensDevice("192.168.0.80");

            using (var connection = device.CreateConnection()) {
                connection.Open();

                while (true) {                    
                    var temperature = connection.ReadDouble("DB10.DBD 20");
                    Console.WriteLine($"Current Temperature is {0} °C", temperature);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}

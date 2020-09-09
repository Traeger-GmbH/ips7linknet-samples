// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace HelloWorld
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a Hello World! application.
    /// </summary>
    /// <remarks>
    /// This application does write/read the 'Hello World!' message to/from the PLC and when
    /// prints the message on the standard output.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            connection.WriteString("DB111.DBB 100", "Hello World!");

            string message = connection.ReadString("DB111.DBB 100", 16);
            Console.WriteLine(message);

            connection.Close();
            Console.ReadKey();
        }
    }
}

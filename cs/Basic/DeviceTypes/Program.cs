﻿// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace DeviceTypes
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to work with the different device types supported by the
    /// framework.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            //// Overall there is not really any special knowledge required to establish a
            //// connection to a different Siemens device type. As you will see in the following
            //// snippets there does only differ one argument when initializing a new device
            //// instance or a single property set call to change the device type.

            #region 1. Way: Default device type
            {
                //// The simplest and the most general way is to just initialize a device using the
                //// constructor accepting the end point information. Using this constructor will
                //// result into a device object which can be used to access S7-300 and S7-400 PLC
                //// devices.

                SimaticDevice device = new SimaticDevice("192.168.0.80");
                Console.WriteLine("Default Device.Type={0}", device.Type);
            }
            #endregion

            #region 2. Way: Explicit device type
            {
                //// The advanced way would be to initialize a new device object using the
                //// constructor which besides of an end point does also accept device type
                //// information.

                SimaticDevice device1 = new SimaticDevice("192.168.0.80", SimaticDeviceType.Logo);
                Console.WriteLine("Explicit Device1.Type={0}", device1.Type);

                SimaticDevice device2 = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);
                Console.WriteLine("Explicit Device2.Type={0}", device2.Type);

                SimaticDevice device3 = new SimaticDevice("192.168.0.80", SimaticDeviceType.S71200);
                Console.WriteLine("Explicit Device3.Type={0}", device3.Type);

                SimaticDevice device4 = new SimaticDevice("192.168.0.80", SimaticDeviceType.S71500);
                Console.WriteLine("Explicit Device4.Type={0}", device4.Type);
            }
            #endregion

            #region 3. Way: Late device type (re-)definition
            {
                //// Independent from the way how you decide to initialize your device object you
                //// are always able to change the device type at runtime.

                SimaticDevice device1 = new SimaticDevice("192.168.0.80");
                device1.Type = SimaticDeviceType.S71500;

                Console.WriteLine("Late Device1.Type={0}", device1.Type);

                SimaticDevice device2 = new SimaticDevice("192.168.0.80", SimaticDeviceType.S71500);
                device2.Type = SimaticDeviceType.S7300_400;

                Console.WriteLine("Late Device2.Type={0}", device2.Type);
            }
            #endregion

            Console.ReadKey();
        }
    }
}

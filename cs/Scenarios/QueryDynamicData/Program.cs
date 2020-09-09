// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace QueryDynamicData
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to query dynamic data within an application.
    /// </summary>
    /// <remarks>
    /// This application does query data there its source address can vary from read to read.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            //// The following lines of code demonstrate how to read and write data
            //// using a custom PLC Object which defines its members dynamically.
            ////
            //// Note that each instance of the class Data initialized does refer to
            //// different DataBlocks upon changing the static DataBlockNumber property
            //// of the class.
            ////
            //// This scenario is not a general way how to use dynamic PlcObject members.
            //// It just demonstrates one possible way to refer to dynamic PLC data in an
            //// independent way.

            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            Data.DataBlockNumber = 1;
            Data data1 = connection.ReadObject<Data>();

            Data.DataBlockNumber = 10;
            Data data10 = connection.ReadObject<Data>();

            Data.DataBlockNumber = 15;
            Data data15 = connection.ReadObject<Data>();

            Data.DataBlockNumber = 20;

            Data data20 = new Data();
            data20.ByteValue = data1.ByteValue;
            data20.Int16Value = data10.Int16Value;
            data20.Int32Value = data15.Int32Value;
            data20.RealValue = data10.RealValue / data1.RealValue;
            data20.StringValue = data15.StringValue;

            connection.WriteObject(data20);
            connection.Close();
        }
    }
}

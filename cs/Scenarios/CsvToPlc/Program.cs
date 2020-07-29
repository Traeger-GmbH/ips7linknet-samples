// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace CsvToPlc
{
    using System;
    using System.IO;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a price label printing application which uses a CSV file as data
    /// source of the information required to print the price labels.
    /// </summary>
    /// <remarks>
    /// This application does read the print job information from a CSV file, writes the job
    /// configuration to the PLC and when waits until the job has been processed by the PLC.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            SiemensDevice device = new SiemensDevice(
                    new IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            #region 1. Way: Sequential Write.
            {
                //// Either use the primitive low level methods of the PLC device connection to
                //// sequential write the data.

                foreach (string line in File.ReadAllLines(@".\Data.csv")) {
                    string[] values = line.Split(';');

                    connection.WriteByte("DB111.DBB 2", Convert.ToByte(values[0]));     // Number of pages.
                    connection.WriteInt16("DB111.DBW 4", Convert.ToInt16(values[1]));   // Resolution in dpi.
                    connection.WriteInt32("DB111.DBD 6", Convert.ToInt32(values[2]));   // Line Height in pixels.
                    connection.WriteReal("DB111.DBD 10", Convert.ToSingle(values[3]));  // Price.
                    connection.WriteString("DB111.DBB 20", values[4]);                  // Article Number.

                    connection.WriteBoolean("DB111.DBX 1.0", true);

                    // Wait while printing.
                    while (connection.ReadBoolean("E 1.0"))
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
            #endregion

            #region 2. Way: Bulk write (with variables).
            {
                //// Or use the higher level methods of the PLC device connection to write a whole
                //// set of variables at once. While this way would be much faster than the
                //// previous one, because the values are write within one transaction instead of
                //// processing each value within a transaction for each action.

                PlcByte numberOfPages = new PlcByte("DB111.DBB 2");
                PlcInt16 resolution = new PlcInt16("DB111.DBW 4");
                PlcInt32 lineHeight = new PlcInt32("DB111.DBD 6");
                PlcReal price = new PlcReal("DB111.DBD 10");
                PlcString articleNumber = new PlcString("DB111.DBB 20", 16);

                PlcBoolean startPrint = new PlcBoolean("DB111.DBX 1.0", true);

                foreach (string line in File.ReadAllLines(@".\Data.csv")) {
                    string[] values = line.Split(';');

                    numberOfPages.Value = Convert.ToByte(values[0]);     // Number of pages.
                    resolution.Value = Convert.ToInt16(values[1]);       // Resolution in dpi.
                    lineHeight.Value = Convert.ToInt32(values[2]);       // Line Height in pixels.
                    price.Value = Convert.ToSingle(values[3]);           // Price.
                    articleNumber.Value = Convert.ToString(values[4]);   // Article Number.

                    connection.WriteValues(numberOfPages, resolution, lineHeight, price, articleNumber, startPrint);

                    // Wait while printing.
                    while (connection.ReadBoolean("E 1.0"))
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
            #endregion

            #region 3. Way: Bulk write (with object).
            {
                //// Or use the methods of the PLC device connection at the highest abstraction
                //// layer to write the whole PLC data at once from a user defined PLC object.

                foreach (string line in File.ReadAllLines(@".\Data.csv")) {
                    string[] values = line.Split(';');
                    PrintJobData data = new PrintJobData();

                    data.NumberOfPages = Convert.ToByte(values[0]);     // Number of pages.
                    data.Resolution = Convert.ToInt16(values[1]);       // Resolution in dpi.
                    data.LineHeight = Convert.ToInt32(values[2]);       // Line Height in pixels.
                    data.Price = Convert.ToSingle(values[3]);           // Price.
                    data.ArticleNumber = Convert.ToString(values[4]);   // Article Number.

                    connection.WriteObject(data);

                    // Wait while printing.
                    while (connection.ReadBoolean("E 1.0"))
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
            #endregion

            connection.Close();
        }
    }
}

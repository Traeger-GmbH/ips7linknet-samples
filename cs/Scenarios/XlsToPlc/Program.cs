// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace XlsToPlc
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using System.Threading;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates a price label printing application which uses a XLS file as data
    /// source of the information required to print the price labels.
    /// </summary>
    /// <remarks>
    /// This application does read the print job information from a XLS file, writes the job
    /// configuration to the PLC and when waits until the job has been processed by the PLC.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

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

            // 'Data' represents the Excel Worksheet to read.
            OleDbCommand command = excelConnection.CreateCommand();
            command.CommandText = "select * from [Data$]";

            OleDbDataReader dataReader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(dataReader);

            #region 1. Way: Sequential Write.
            {
                //// Either use the primitive low level methods of the PLC device connection to
                //// sequential write the data.

                foreach (DataRow row in table.Rows) {
                    connection.WriteByte("DB111.DBB 2", Convert.ToByte(row[0]));        // Number of pages.
                    connection.WriteInt16("DB111.DBW 4", Convert.ToInt16(row[1]));      // Resolution in dpi.
                    connection.WriteInt32("DB111.DBD 6", Convert.ToInt32(row[2]));      // Line Height in pixels.
                    connection.WriteReal("DB111.DBD 10", Convert.ToSingle(row[3]));     // Price.
                    connection.WriteString("DB111.DBB 20", Convert.ToString(row[4]));   // Article Number.

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

                foreach (DataRow row in table.Rows) {
                    numberOfPages.Value = Convert.ToByte(row["NumberOfPages"]);     // Number of pages.
                    resolution.Value = Convert.ToInt16(row["Resolution"]);          // Resolution in dpi.
                    lineHeight.Value = Convert.ToInt32(row["LineHeight"]);          // Line Height in pixels.
                    price.Value = Convert.ToSingle(row["Price"]);                   // Price.
                    articleNumber.Value = Convert.ToString(row["ArticleNumber"]);   // Article Number.

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

                foreach (DataRow row in table.Rows) {
                    PrintJobData data = new PrintJobData();

                    data.NumberOfPages = Convert.ToByte(row["NumberOfPages"]);     // Number of pages.
                    data.Resolution = Convert.ToInt16(row["Resolution"]);          // Resolution in dpi.
                    data.LineHeight = Convert.ToInt32(row["LineHeight"]);          // Line Height in pixels.
                    data.Price = Convert.ToSingle(row["Price"]);                   // Price.
                    data.ArticleNumber = Convert.ToString(row["ArticleNumber"]);   // Article Number.

                    connection.WriteObject(data);

                    // Wait while printing.
                    while (connection.ReadBoolean("E 1.0"))
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
            #endregion

            excelConnection.Close();
            connection.Close();
        }
    }
}

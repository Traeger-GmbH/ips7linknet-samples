// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace BulkAccess
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to read many values in one call and how to handle a missing
    /// data area among them.
    /// </summary>
    /// <remarks>
    /// The sample reads from the PLC only. Pass the address of your PLC as the first argument,
    /// for example "s71500://192.168.0.80".
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            string address = args.Length > 0 ? args[0] : "s7300://192.168.0.80";

            using (var client = new SimaticClient(address)) {
                #region 1. Unreachable PLC
                try {
                    client.Connect();
                }
                catch (PlcException ex) when (ex.Code == PlcStatusCode.Timeout) {
                    Console.WriteLine("The PLC is not reachable: {0}", ex.Message);
                    return;
                }
                #endregion

                #region 2. Many values in one call
                PlcByte[] markers = new PlcByte[100];

                for (int index = 0; index < markers.Length; index++) {
                    markers[index] = new PlcByte("MB " + index);
                }

                // The SDK packs the values into as few telegrams as the PDU size allows.
                client.ReadValues(markers);
                Console.WriteLine("MB 0 = {0}, MB 99 = {1}", markers[0].Value, markers[99].Value);
                #endregion

                #region 3. Missing data area
                var word = new PlcInt16("MW 0");
                var doubleWord = new PlcInt32("MD 4");

                // The following assumes that DB65535 does not exist in the PLC.
                var missing = new PlcInt32("DB65535.DBD 0");

                try {
                    client.ReadValues(word, doubleWord, missing);
                }
                catch (PlcException ex) {
                    // A missing data area is reported with the status code NoData.
                    Console.WriteLine("Access failed: {0} ({1})", ex.Code, ex.Type?.Address);
                }
                #endregion

                #region 4. Status of each value
                foreach (IPlcValue value in new IPlcValue[] { word, doubleWord, missing }) {
                    Console.WriteLine("{0}: {1}", value.Type.Address, value.Status.Code);
                }
                #endregion
            }
        }
    }
}

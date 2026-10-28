// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Trace
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to enable the trace of the SDK with combined levels and how
    /// to write its entries through a queue on a separate thread.
    /// </summary>
    /// <remarks>
    /// The sample reads from the PLC only. Pass the address of your PLC as the first argument,
    /// for example "s71500://192.168.0.80".
    /// </remarks>
    public class Program
    {
        private static readonly BlockingCollection<string> entries =
                new BlockingCollection<string>();

        public static void Main(string[] args)
        {
            #region 1. Write the entries on a separate thread
            // The SDK calls WriteEntry on the thread of the SDK call. A slow output would delay
            // each access to the PLC: queue the entries there and write them elsewhere.
            var writer = new Thread(Program.WriteEntries) { IsBackground = true };
            writer.Start();
            #endregion

            #region 2. Enable the trace
            // The levels are flags, not a threshold: an entry is written if its level is part of
            // Level. Verbose entries contain addresses and values; use them for debugging only.
            PlcTrace.Level = PlcTraceLevel.Error
                    | PlcTraceLevel.Warning
                    | PlcTraceLevel.Information;
            PlcTrace.WriteEntry = Program.HandleTraceEntry;
            PlcTrace.IsEnabled = true;
            #endregion

            #region 3. Accesses that write entries
            string address = args.Length > 0 ? args[0] : "s7300://192.168.0.80";

            using (var client = new SimaticClient(address)) {
                // Information entries report connecting to and disconnecting from the PLC.
                client.Connect();
                Console.WriteLine("MB 0 = {0}", client.ReadValue<byte>("MB 0"));

                try {
                    // The following assumes that DB65535 does not exist in the PLC.
                    client.ReadValue<int>("DB65535.DBD 0");
                }
                catch (PlcException ex) {
                    // Error entries contain the exception with its stack trace.
                    Console.WriteLine("Access failed: {0}", ex.Code);
                }

                client.Disconnect();
            }
            #endregion

            PlcTrace.IsEnabled = false;
            Program.entries.CompleteAdding();
            writer.Join();
        }

        private static void HandleTraceEntry(PlcTraceLevel level, string message)
        {
            Program.entries.Add(string.Format("Trace {0}: {1}", level, message));
        }

        private static void WriteEntries()
        {
            foreach (string entry in Program.entries.GetConsumingEnumerable()) {
                Console.WriteLine(entry);
            }
        }
    }
}

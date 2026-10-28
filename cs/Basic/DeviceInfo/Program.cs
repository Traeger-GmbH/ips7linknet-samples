// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace DeviceInfo
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to retrieve the device data of a PLC and information about
    /// its data blocks.
    /// </summary>
    /// <remarks>
    /// Without an argument, the sample uses the simulation, which generates its own device data
    /// and reports no data blocks. Pass the IP address of your PLC as the first argument to
    /// query the PLC.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            PlcDevice device = Program.CreateDevice(args);

            #region 1. Device data
            // Not every PLC provides all device data: the Has... properties tell which it does.
            IPlcDeviceInfo info = device.GetInfo();

            Program.Print("Name", info.HasName, info.Name);
            Program.Print("Location", info.HasLocation, info.Location);
            Program.Print("Plant ID", info.HasPlantId, info.PlantId);
            Program.Print("Module name", info.HasModuleName, info.ModuleName);
            Program.Print("Module type", info.HasModuleType, info.ModuleType);
            Program.Print("Module serial", info.HasModuleSerial, info.ModuleSerial);
            Program.Print("Memory card serial", info.HasMemoryCardSerial, info.MemoryCardSerial);
            Program.Print("Time", info.HasTime, info.Time);
            Program.Print("Copyright", info.HasCopyright, info.Copyright);
            #endregion

            #region 2. Data blocks
            try {
                // Blocks with optimized access of an S7-1200/1500 can be missing in the list.
                foreach (PlcBlockInfo dataBlock in device.GetBlocks()) {
                    Console.WriteLine("{0}: {1} bytes", dataBlock.Operand, dataBlock.Size);
                }
            }
            catch (NotSupportedException ex) {
                // The PLC does not provide the list, for example because of its protection level.
                Console.WriteLine(ex.Message);
            }

            PlcBlockInfo block = device.GetBlock(PlcOperand.DataBlock(1));
            Console.WriteLine("DB1 exists: {0}, size: {1} bytes", block.Exists, block.Size);
            #endregion
        }

        private static PlcDevice CreateDevice(string[] args)
        {
            if (args.Length == 0)
                return new VirtualDevice();

            // Pass an IPDeviceEndPoint with rack and slot and the device type of your PLC if they
            // differ from the defaults (see the sample DeviceTypes).
            return new SimaticDevice(args[0]);
        }

        private static void Print(
                string name,
                bool hasValue,
                object value)
        {
            Console.WriteLine("{0}: {1}", name, hasValue ? value : "(not provided)");
        }
    }
}

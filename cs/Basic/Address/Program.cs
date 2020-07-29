// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Address
{
    using System;
    using System.Collections.Generic;

    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to work with the 'PlcAddress' class.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            //// The PlcAddress class itself does represent an immutable type which describes the
            //// metadata stored within a typical compound string value used to access the
            //// different data areas defined within a PLC device.

            #region 1. Way: Pure coded ctor use.
            {
                //// The use of the ctor is most useful in scenarios there the address needs to be
                //// generated during the runtime of your application/library. So you are not
                //// forced to generate the compound string value of the address manually.

                // The following example results into the address: 'DB100.DBX 1.0'.
                PlcAddress address1 = new PlcAddress(PlcOperandType.DataBlock, 100, PlcRawType.Bit, 1, 0);

                // If you want to address a different data area there you do not need to specify
                // the number of the area like the PERIPHERY INOUT block you just have to leave
                // the second parameter in the ctor.
                // The following example results into the address: 'E 1.0'.
                PlcAddress address2 = new PlcAddress(PlcOperandType.Input, PlcRawType.Bit, 1, 0);
            }
            #endregion

            #region 2. Way: Parse existing compound string values to an address.
            {
                //// The use of the parse methods is useful in scenarios there you may ensure that
                //// a custom address string is valid and does maybe refer to the
                //// expected metadata.

                // You can either use the parse method (which throws an exception if the format of
                // the address string is invalid).
                PlcAddress address1 = PlcAddress.Parse("DB100.DBX 1.0");

                // Or you use the try parse method (which tries to parse the address string). On
                // success the resulting address is stored within the out parameter.
                PlcAddress address2 = null;
                bool success = PlcAddress.TryParse("E 1.0", out address2);
            }
            #endregion

            #region 3. Way: Implicit cast operator.
            {
                //// To reduce the partially cumbersome configuration of an address for e.g. via
                //// ctor you can just create a new PlcAddress using the implicit cast operator
                //// provided by the class. Note: The operator itself does internally use the
                //// parse method, which means initializing a new instance with an invalid
                //// formatted address using the implicit cast operator will end into a format
                //// exception. In general does this method provide the most comfortably way
                //// (especially for those used to the PLC address format).

                PlcAddress address1 = "DB100.DBX 1.0";
                PlcAddress address2 = "E 1.0";
            }
            #endregion

            //// The mechanism used by the PlcAddress class to interpret address strings does
            //// provide a wide range of flexibility.

            #region 1. Way: Chaos addresses.
            {
                //// The following 'chaos' addresses are correctly interpreted by the PlcAddress
                //// class without to miss any information within the malformed address strings.

                // The following example results into the address: 'DB100.DBX 1.0'.
                PlcAddress address1 = " dB 100  . Db 1    . 0  ";

                // The following example results into the address: 'E 1.0'.
                PlcAddress address2 = " E x 1  ";
            }
            #endregion

            #region 2. Way: Siemens and IEC conform address support.
            {
                //// While Siemes and the IEC do not completly match within the address formats
                //// specified in their specifications, the use of the PlcAddress does support
                //// both specifications.

                // You can define an address either in Siemens notation.
                PlcAddress address1 = "E 1.0";

                // Or in IEC notation.
                PlcAddress address2 = "I 1.0";

                //// By default does the PlcAddress class format the address metadata into
                //// the Siemens format but using a different ToString overload you can
                //// specify the desired standard to format the address.

                // The following example results into the address: 'I 1.0'.
                string addressStringAsIec = address1.ToString(PlcOperandStandard.IEC);

                // The following example results into the address: 'E 1.0'.
                string addressStringAsSiemens = address2.ToString(PlcOperandStandard.Siemens);
            }
            #endregion

            //// Additional operators/methods provided by the PlcAddress class support the use of
            //// PLC addresses like numerical values.

            #region 1. Way: Comparison operators.
            {
                //// As soon both addresses are within the same data area they can be easly
                //// compared with each other. Otherwise an address of a different data area
                //// will be always greater or lower than an address refering to a different
                //// data area.
                //// In general addresses from different data areas are compared/sorted within
                //// the following order:
                //// Input, PeripheryInput, Output, PeripheryOutput, Flag, DataBlock,
                //// Counter, Timer.

                PlcAddress address1 = "DB100.DBB 10";
                PlcAddress address2 = "DB100.DBB 20";

                // You can check whether one of them is lower.
                if (address1 < address2)
                    address2 = address1;

                // You can check whether both are equals.
                if (address1 == address2)
                    address2 = "DB100.DBB 4";

                // You can check whether one of them is greater.
                if (address1 > address2)
                    address2 = address1;

                //// Additionally to the represented operators there are also implemented <= and >=
                //// to compare two addresses with each other.
            }
            #endregion

            #region 2. Way: Sorting support.
            {
                //// Instances of the PlcAddress class do also support sorting mechanism through
                //// implementing the required default framework interfaces.

                List<PlcAddress> addresses = new List<PlcAddress>();
                addresses.Add("DB100.DBB 10");
                addresses.Add("DB100.DBB 1");
                addresses.Add("DB100.DBB 12");
                addresses.Add("DB100.DBB 11");
                addresses.Add("DB100.DBB 50");
                addresses.Add("DB100.DBB 8");
                addresses.Add("DB100.DBB 5");
                addresses.Add("DB100.DBB 6");
                addresses.Add("DB100.DBB 100");
                addresses.Add("DB100.DBB 99");

                Console.WriteLine("Before sort...");
                addresses.ForEach((address) => Console.WriteLine(address));

                addresses.Sort();

                Console.WriteLine("After sort...");
                addresses.ForEach((address) => Console.WriteLine(address));
            }
            #endregion

            Console.ReadKey();
        }
    }
}

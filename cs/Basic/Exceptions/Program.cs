// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Exceptions
{
    using System;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates the different mechanisms provided to handle exceptions.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            SimaticDevice device = new SimaticDevice("192.168.0.80", SimaticDeviceType.S7300_400);

            PlcDeviceConnection connection = device.CreateConnection();
            connection.Open();

            #region 1. Way: Default Try-Catch-(Finally) block.
            {
                //// An exception can occur upon different reasons, for e.g. either the IP address
                //// of the PLC device is wrong, the PLC itself is not available or the PLC address
                //// specified (like in the following example) is invalid. In such cases will then
                //// an exception of the type 'PlcException' thrown.

                try {
                    connection.ReadInt16("DB111.DBD 6");
                }
                catch (ArgumentException ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            #region 2. Way: Global status evaluation (via connection).
            {
                //// The default status evaluation which will always throw an exception in cases
                //// there the status does indicate a PlcStatusCode unequal to
                //// PlcStatusCode.NoError can be overridden by a custom callback. As soon a custom
                //// callback has been registered (like in the following example) there will be no
                //// longer an exception thrown. The callback itself is always invoked when a
                //// status of a driver based object has been changed and needs therefore to be
                //// evaluated. A status change does not implicity indicate an internal error.
                ////
                //// This way does demonstrate that the connection will be passed to the global
                //// evaluation callback.

                // The following assumes that 'DB65535' does not exists in the PLC.
                PlcNotifications.EvaluateStatus = Program.EvaluateStatus;
                connection.ReadInt16("DB65535.DBW 6");
            }
            #endregion

            #region 3. Way: Global status evaluation (via value).
            {
                //// The default status evaluation which will always throw an exception in cases
                //// there the status does indicate a PlcStatusCode unequal to
                //// PlcStatusCode.NoError can be overridden by a custom callback. As soon a custom
                //// callback has been registered (like in the following example) there will be no
                //// longer an exception thrown. The callback itself is always invoked when a
                //// status of a driver based object has been changed and needs therefore to be
                //// evaluated. A status change does not implicity indicate an internal error.
                ////
                //// This way does demonstrate that the value object will be passed to the global
                //// evaluation callback.

                PlcNotifications.EvaluateStatus = Program.EvaluateStatus;

                // The following assumes that 'DB111.DBW 6' does not exists in the PLC.
                PlcInt16 value = new PlcInt16("DB111.DBW 6");
                connection.ReadValues(value);
            }
            #endregion

            connection.Close();
            Console.ReadKey();
        }

        private static bool EvaluateStatus(IPlcStatusProvider provider)
        {
            //// The IPlcStatusProvider interface is implemented by the IPlcValue interface and
            //// PlcDeviceConnection class. This does not only allow to determine and evaluate the
            //// status of the object affected by a status change, it does also support the re-use
            //// of the affected object to provide additional custom evaluation mechanism (like
            //// in the code below).

            Console.WriteLine(provider.Status.Text);
            Console.WriteLine("-> Code: {0} ({1})", provider.Status.Code, (int)provider.Status.Code);

            PlcDeviceConnection connection = provider as PlcDeviceConnection;
            IPlcValue value = provider as IPlcValue;

            if (connection != null)
                Console.WriteLine("-> A connection to '{0}' was affected.", connection.Device.EndPoint);
            else if (value != null)
                Console.WriteLine("-> An access to the address '{0}' was affected.", value.Type.Address);

            return true;
        }
    }
}

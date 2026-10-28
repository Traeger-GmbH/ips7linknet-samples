// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace License
{
    using System;
    using IPS7Lnk;
    using IPS7Lnk.Advanced;

    /// <summary>
    /// This sample demonstrates how to set up the license of the SDK, how to query the license
    /// in use and how to check the license when the application starts.
    /// </summary>
    /// <remarks>
    /// Without a license key, the SDK runs as an evaluation. Pass --check as the first argument
    /// to run the check on start, which ends the application during the evaluation.
    /// </remarks>
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 1. License key
            // Set the license key before the application uses the SDK for the first time:
            // Licenser.LicenseKey = "<insert your license code here>";
            #endregion

            #region 2. License in use
            ILicenseInfo license = Licenser.LicenseInfo;

            Console.WriteLine("License: {0}", license.Type.Name);
            Console.WriteLine("Evaluation: {0}", license.IsEvaluation);
            Console.WriteLine("Subscription: {0}", license.IsSubscription);
            Console.WriteLine("Expired: {0}", license.IsExpired);
            Console.WriteLine("Expiry date: {0}", license.ExpiryDate);
            Console.WriteLine("Support expired: {0}", license.IsSupportExpired);
            Console.WriteLine("Support expiry date: {0}", license.SupportExpiryDate);
            #endregion

            #region 3. Check the license on start
            if (args.Length > 0 && args[0] == "--check") {
                // If the SDK is not licensed, FailIfUnlicensed shows a message in a debug build.
                // ThrowIfUnlicensed throws an InvalidOperationException on another thread, which
                // ends the application: a catch block around the call does not receive it.
#if DEBUG
                Licenser.FailIfUnlicensed();
#else
                Licenser.ThrowIfUnlicensed();
#endif
                Console.WriteLine("The license check has passed.");
            }
            #endregion
        }
    }
}

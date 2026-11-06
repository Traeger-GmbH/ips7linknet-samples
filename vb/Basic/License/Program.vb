' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports IPS7Lnk
Imports IPS7Lnk.Advanced

Namespace License
    ''' <summary>
    ''' This sample demonstrates how to set up the license of the SDK, how to query the license
    ''' in use and how to check the license when the application starts.
    ''' </summary>
    ''' <remarks>
    ''' Without a license key, the SDK runs as an evaluation. Pass --check as the first argument
    ''' to run the check on start, which ends the application during the evaluation.
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

#Region "1. License key"
            ' Set the license key before the application uses the SDK for the first time:
            ' Licenser.LicenseKey = "<insert your license code here>"
#End Region

#Region "2. License in use"
            Dim license As ILicenseInfo = Licenser.LicenseInfo

            Console.WriteLine("License: {0}", license.Type.Name)
            Console.WriteLine("Evaluation: {0}", license.IsEvaluation)
            Console.WriteLine("Subscription: {0}", license.IsSubscription)
            Console.WriteLine("Expired: {0}", license.IsExpired)
            Console.WriteLine("Expiry date: {0}", license.ExpiryDate)
            Console.WriteLine("Support expired: {0}", license.IsSupportExpired)
            Console.WriteLine("Support expiry date: {0}", license.SupportExpiryDate)
#End Region

#Region "3. Check the license on start"
            If args.Length > 0 AndAlso args(0) = "--check" Then
                ' If the SDK is not licensed, FailIfUnlicensed shows a message in a debug build.
                ' ThrowIfUnlicensed throws an InvalidOperationException on another thread, which
                ' ends the application: a catch block around the call does not receive it.
#If DEBUG Then
                Licenser.FailIfUnlicensed()
#Else
                Licenser.ThrowIfUnlicensed()
#End If
                Console.WriteLine("The license check has passed.")
            End If
#End Region
        End Sub
    End Class
End Namespace

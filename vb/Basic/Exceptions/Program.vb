' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace Exceptions
    ''' <summary>
    ''' This sample demonstrates the different mechanisms provided to handle exceptions.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SiemensDevice = New SiemensDevice( _
                    New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            ' 1. Way: Default Try-Catch-(Finally) block.
            If True Then
                ' An exception can occur upon different reasons, for e.g. either the IP address
                ' of the PLC device is wrong, the PLC itself is not available or the PLC address
                ' specified (like in the following example) is invalid. In such cases will then
                ' an exception of the type 'PlcException' thrown.

                Try
                    connection.ReadInt16("DB111.DBD 6")
                Catch ex As PlcException
                    Console.WriteLine(ex.Message)
                    Console.WriteLine("-> Code: {0} ({1})", ex.Code, ex.ErrorCode)
                End Try
            End If

            ' 2. Way: Global status evaluation (via connection).
            If True Then
                ' The default status evaluation which will always throw an exception in cases
                ' there the status does indicate a PlcStatusCode unequal to
                ' PlcStatusCode.NoError can be overridden by a custom callback. As soon a custom
                ' callback has been registered (like in the following example) there will be no
                ' longer an exception thrown. The callback itself is always invoked when a
                ' status of a driver based object has been changed and needs therefore to be
                ' evaluated. A status change does not implicity indicate an internal error.
                '
                ' This way does demonstrate that the connection will be passed to the global
                ' evaluation callback.

                PlcNotifications.EvaluateStatus = AddressOf Program.EvaluateStatus
                connection.ReadInt16("DB111.DBD 6")
            End If

            ' 3. Way: Global status evaluation (via value).
            If True Then
                ' The default status evaluation which will always throw an exception in cases
                ' there the status does indicate a PlcStatusCode unequal to
                ' PlcStatusCode.NoError can be overridden by a custom callback. As soon a custom
                ' callback has been registered (like in the following example) there will be no
                ' longer an exception thrown. The callback itself is always invoked when a
                ' status of a driver based object has been changed and needs therefore to be
                ' evaluated. A status change does not implicity indicate an internal error.
                '
                ' This way does demonstrate that the value object will be passed to the global
                ' evaluation callback.

                PlcNotifications.EvaluateStatus = AddressOf Program.EvaluateStatus

                Dim value As PlcInt16 = New PlcInt16("DB111.DBX 6.0")
                connection.ReadValues(value)
            End If

            connection.Close()
            Console.ReadKey()
        End Sub

        Private Shared Function EvaluateStatus(ByVal provider As IPlcStatusProvider) As Boolean
            ' The IPlcStatusProvider interface is implemented by the IPlcValue interface and
            ' PlcDeviceConnection class. This does not only allow to determine and evaluate the
            ' status of the object affected by a status change, it does also support the re-use
            ' of the affected object to provide additional custom evaluation mechanism (like
            ' in the code below).

            Console.WriteLine(provider.Status.Text)
            Console.WriteLine("-> Code: {0} ({1})", provider.Status.Code, CInt(provider.Status.Code))

            Dim connection As PlcDeviceConnection = TryCast(provider, PlcDeviceConnection)
            Dim value As IPlcValue = TryCast(provider, IPlcValue)

            If connection IsNot Nothing Then
                Console.WriteLine("-> A connection to '{0}' was affected.", connection.Device.EndPoint)
            ElseIf value IsNot Nothing Then
                Console.WriteLine("-> An access to the address '{0}' was affected.", value.Type.Address)
            End If

            Return True
        End Function
    End Class
End Namespace

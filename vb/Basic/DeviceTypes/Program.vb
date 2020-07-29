' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace DeviceTypes
    ''' <summary>
    ''' This sample demonstrates how to work with the different device types supported by the
    ''' framework.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            ' Overall there is not really any special knowledge required to establish a
            ' connection to a different Siemens device type. As you will see in the following
            ' snippets there does only differ one argument when initializing a new device
            ' instance or a single property set call to change the device type.
            
            ' 1. Way: Default device type
            If True Then
                ' The simplest and the most general way is to just initialize a device using the
                ' constructor accepting the end point information. Using this constructor will
                ' result into a device object which can be used to access S7-300 and S7-400 PLC
                ' devices.
                Dim device As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"))

                Console.WriteLine("Default Device.Type={0}", device.Type)
            End If

            ' 2. Way: Explicit device type
            If True Then
                ' The advanced way would be to initialize a new device object using the
                ' constructor which besides of an end point does also accept device type
                ' information.

                Dim device1 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.Logo)

                Console.WriteLine("Explicit Device1.Type={0}", device1.Type)

                Dim device2 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

                Console.WriteLine("Explicit Device2.Type={0}", device2.Type)

                Dim device3 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S71200)

                Console.WriteLine("Explicit Device3.Type={0}", device3.Type)

                Dim device4 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S71500)

                Console.WriteLine("Explicit Device4.Type={0}", device4.Type)
            End If

            ' 3. Way: Late device type (re-)definition
            If True Then
                ' Independent from the way how you decide to initialize your device object you
                ' are always able to change the device type at runtime.

                Dim device1 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"))

                device1.Type = SiemensDeviceType.S71500
                Console.WriteLine("Late Device1.Type={0}", device1.Type)

                Dim device2 As SiemensDevice = New SiemensDevice( _
                        New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S71500)

                device2.Type = SiemensDeviceType.S7300_400
                Console.WriteLine("Late Device2.Type={0}", device2.Type)
            End If

            Console.ReadKey()
        End Sub
    End Class
End Namespace

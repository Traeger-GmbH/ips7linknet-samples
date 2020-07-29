' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports System.Collections.Generic

Imports IPS7Lnk.Advanced

Namespace Address
    ''' <summary>
    ''' This sample demonstrates how to work with the 'PlcAddress' class.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            ' The PlcAddress class itself does represent an immutable type which describes the
            ' metadata stored within a typical compound string value used to access the
            ' different data areas defined within a PLC device.

            ' 1. Way: Pure coded ctor use.
            If True Then
                ' The use of the ctor is most useful in scenarios there the address needs to be
                ' generated during the runtime of your application/library. So you are not
                ' forced to generate the compound string value of the address manually.

                ' The following example results into the address: 'DB100.DBX 1.0'.
                Dim address1 As PlcAddress = New PlcAddress(PlcOperandType.DataBlock, 100, PlcRawType.Bit, 1, 0)

                ' If you want to address a different data area there you do not need to specify
                ' the number of the area like the PERIPHERY INOUT block you just have to leave
                ' the second parameter in the ctor.
                ' The following example results into the address: 'E 1.0'.
                Dim address2 As PlcAddress = New PlcAddress(PlcOperandType.Input, PlcRawType.Bit, 1, 0)
            End If

            ' 2. Way: Parse existing compound string values to an address.
            If True Then
                ' The use of the parse methods is useful in scenarios there you may ensure that
                ' a custom address string is valid and does maybe refer to the
                ' expected metadata.

                ' You can either use the parse method (which throws an exception if the format of
                ' the address string is invalid).
                Dim address1 As PlcAddress = PlcAddress.Parse("DB100.DBX 1.0")

                ' Or you use the try parse method (which tries to parse the address string). On
                ' success the resulting address is stored within the out parameter.
                Dim address2 As PlcAddress = Nothing
                Dim success As Boolean = PlcAddress.TryParse("E 1.0", address2)
            End If

            ' 3. Way: Implicit cast operator.
            If True Then
                ' To reduce the partially cumbersome configuration of an address for e.g. via
                ' ctor you can just create a new PlcAddress using the implicit cast operator
                ' provided by the class. Note: The operator itself does internally use the
                ' parse method, which means initializing a new instance with an invalid
                ' formatted address using the implicit cast operator will end into a format
                ' exception. In general does this method provide the most comfortably way
                ' (especially for those used to the PLC address format).
                Dim address1 As PlcAddress = "DB100.DBX 1.0"
                Dim address2 As PlcAddress = "E 1.0"
            End If

            ' The mechanism used by the PlcAddress class to interpret address strings does
            ' provide a wide range of flexibility.

            ' 1. Way: Chaos addresses.
            If True Then
                ' The following 'chaos' addresses are correctly interpreted by the PlcAddress
                ' class without to miss any information within the malformed address strings.

                ' The following example results into the address: 'DB100.DBX 1.0'.
                Dim address1 As PlcAddress = " dB 100  . Db 1    . 0  "

                ' The following example results into the address: 'E 1.0'.
                Dim address2 As PlcAddress = " E x 1  "
            End If

            ' 2. Way: Siemens and IEC conform address support.
            If True Then
                ' While Siemes and the IEC do not completly match within the address formats
                ' specified in their specifications, the use of the PlcAddress does support
                ' both specifications.

                ' You can define an address either in Siemens notation.
                Dim address1 As PlcAddress = "E 1.0"

                ' Or in IEC notation.
                Dim address2 As PlcAddress = "I 1.0"

                ' By default does the PlcAddress class format the address metadata into
                ' the Siemens format but using a different ToString overload you can
                ' specify the desired standard to format the address.

                ' The following example results into the address: 'I 1.0'.
                Dim addressStringAsIec As String = address1.ToString(PlcOperandStandard.IEC)

                ' The following example results into the address: 'E 1.0'.
                Dim addressStringAsSiemens As String = address2.ToString(PlcOperandStandard.Siemens)
            End If

            ' Additional operators/methods provided by the PlcAddress class support the use of
            ' PLC addresses like numerical values.

            ' 1. Way: Comparison operators.
            If True Then
                ' As soon both addresses are within the same data area they can be easly
                ' compared with each other. Otherwise an address of a different data area
                ' will be always greater or lower than an address refering to a different
                ' data area.
                ' In general addresses from different data areas are compared/sorted within
                ' the following order:
                ' Input, PeripheryInput, Output, PeripheryOutput, Flag, DataBlock,
                ' Counter, Timer.

                Dim address1 As PlcAddress = "DB100.DBB 10"
                Dim address2 As PlcAddress = "DB100.DBB 20"

                ' You can check whether one of them is lower.
                If address1 < address2 Then address2 = address1

                ' You can check whether both are equals.
                If address1 = address2 Then address2 = "DB100.DBB 4"

                ' You can check whether one of them is greater.
                If address1 > address2 Then address2 = address1

                ' Additionally to the represented operators there are also implemented <= and >=
                ' to compare two addresses with each other.
            End If

            ' 2. Way: Sorting support.
            If True Then
                ' Instances of the PlcAddress class do also support sorting mechanism through
                ' implementing the required default framework interfaces.

                Dim addresses As List(Of PlcAddress) = New List(Of PlcAddress)()
                addresses.Add("DB100.DBB 10")
                addresses.Add("DB100.DBB 1")
                addresses.Add("DB100.DBB 12")
                addresses.Add("DB100.DBB 11")
                addresses.Add("DB100.DBB 50")
                addresses.Add("DB100.DBB 8")
                addresses.Add("DB100.DBB 5")
                addresses.Add("DB100.DBB 6")
                addresses.Add("DB100.DBB 100")
                addresses.Add("DB100.DBB 99")
                Console.WriteLine("Before sort...")
                addresses.ForEach(Sub(address) Console.WriteLine(address))
                addresses.Sort()
                Console.WriteLine("After sort...")
                addresses.ForEach(Sub(address) Console.WriteLine(address))
            End If

            Console.ReadKey()
        End Sub
    End Class
End Namespace

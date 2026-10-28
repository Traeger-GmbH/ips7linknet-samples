' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports System
Imports IPS7Lnk.Advanced

Namespace DeviceInfo
    ''' <summary>
    ''' This sample demonstrates how to retrieve the device data of a PLC and information about
    ''' its data blocks.
    ''' </summary>
    ''' <remarks>
    ''' Without an argument, the sample uses the simulation, which generates its own device data
    ''' and reports no data blocks. Pass the IP address of your PLC as the first argument to
    ''' query the PLC.
    ''' </remarks>
    Public Class Program

        Public Shared Sub Main(ByVal args As String())

            Dim device As PlcDevice = Program.CreateDevice(args)

#Region "1. Device data"
            ' Not every PLC provides all device data: the Has... properties tell which it does.
            Dim info As IPlcDeviceInfo = device.GetInfo()

            Program.Print("Name", info.HasName, info.Name)
            Program.Print("Location", info.HasLocation, info.Location)
            Program.Print("Plant ID", info.HasPlantId, info.PlantId)
            Program.Print("Module name", info.HasModuleName, info.ModuleName)
            Program.Print("Module type", info.HasModuleType, info.ModuleType)
            Program.Print("Module serial", info.HasModuleSerial, info.ModuleSerial)
            Program.Print("Memory card serial", info.HasMemoryCardSerial, info.MemoryCardSerial)
            Program.Print("Time", info.HasTime, info.Time)
            Program.Print("Copyright", info.HasCopyright, info.Copyright)
#End Region

#Region "2. Data blocks"
            Try
                ' Blocks with optimized access of an S7-1200/1500 can be missing in the list.
                For Each dataBlock As PlcBlockInfo In device.GetBlocks()
                    Console.WriteLine("{0}: {1} bytes", dataBlock.Operand, dataBlock.Size)
                Next

            Catch ex As NotSupportedException
                ' The PLC does not provide the list, for example because of its protection level.
                Console.WriteLine(ex.Message)
            End Try

            Dim block As PlcBlockInfo = device.GetBlock(PlcOperand.DataBlock(1))
            Console.WriteLine("DB1 exists: {0}, size: {1} bytes", block.Exists, block.Size)
#End Region
        End Sub

        Private Shared Function CreateDevice(ByVal args As String()) As PlcDevice

            If args.Length = 0 Then
                Return New VirtualDevice()
            End If
            ' Pass an IPDeviceEndPoint with rack and slot and the device type of your PLC if they
            ' differ from the defaults (see the sample DeviceTypes).
            Return New SimaticDevice(args(0))
        End Function

        Private Shared Sub Print(
                ByVal name As String,
                ByVal hasValue As Boolean,
                ByVal value As Object)

            Console.WriteLine("{0}: {1}", name, If(hasValue, value, "(not provided)"))
        End Sub
    End Class
End Namespace

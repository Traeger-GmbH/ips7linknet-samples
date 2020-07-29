' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports System.IO
Imports System.Threading
Imports IPS7Lnk.Advanced

Namespace CsvToPlc
    ''' <summary>
    ''' This sample demonstrates a price label printing application which uses a CSV file as data
    ''' source of the information required to print the price labels.
    ''' </summary>
    ''' <remarks>
    ''' This application does read the print job information from a CSV file, writes the job
    ''' configuration to the PLC and when waits until the job has been processed by the PLC.
    ''' </remarks>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim device As SiemensDevice = New SiemensDevice( _
                    New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()
            connection.Open()

            ' 1. Way: Sequential Write.
            If True Then
                ' Either use the primitive low level methods of the PLC device connection to
                ' sequential write the data.

                For Each line As String In File.ReadAllLines(".\Data.csv")
                    Dim values As String() = line.Split(";"c)

                    connection.WriteByte("DB111.DBB 2", Convert.ToByte(values(0)))     ' Number of pages.
                    connection.WriteInt16("DB111.DBW 4", Convert.ToInt16(values(1)))   ' Resolution in dpi.
                    connection.WriteInt32("DB111.DBD 6", Convert.ToInt32(values(2)))   ' Line Height in pixels.
                    connection.WriteReal("DB111.DBD 10", Convert.ToSingle(values(3)))  ' Price.
                    connection.WriteString("DB111.DBB 20", values(4))                  ' Article Number.

                    connection.WriteBoolean("DB111.DBX 1.0", True)

                    ' Wait while printing.
                    While connection.ReadBoolean("E 1.0")
                        Thread.Sleep(TimeSpan.FromMilliseconds(100))
                    End While
                Next
            End If

            ' 2. Way: Bulk write (with variables).
            If True Then
                ' Or use the higher level methods of the PLC device connection to write a whole
                ' set of variables at once. While this way would be much faster than the
                ' previous one, because the values are write within one transaction instead of
                ' processing each value within a transaction for each action.

                Dim numberOfPages As PlcByte = New PlcByte("DB111.DBB 2")
                Dim resolution As PlcInt16 = New PlcInt16("DB111.DBW 4")
                Dim lineHeight As PlcInt32 = New PlcInt32("DB111.DBD 6")
                Dim price As PlcReal = New PlcReal("DB111.DBD 10")
                Dim articleNumber As PlcString = New PlcString("DB111.DBB 20", 16)

                Dim startPrint As PlcBoolean = New PlcBoolean("DB111.DBX 1.0", True)

                For Each line As String In File.ReadAllLines(".\Data.csv")
                    Dim values As String() = line.Split(";"c)

                    numberOfPages.Value = Convert.ToByte(values(0))     ' Number of pages.
                    resolution.Value = Convert.ToInt16(values(1))       ' Resolution in dpi.
                    lineHeight.Value = Convert.ToInt32(values(2))       ' Line Height in pixels.
                    price.Value = Convert.ToSingle(values(3))           ' Price.
                    articleNumber.Value = Convert.ToString(values(4))   ' Article Number.

                    connection.WriteValues(numberOfPages, resolution, lineHeight, price, articleNumber, startPrint)

                    ' Wait while printing.
                    While connection.ReadBoolean("E 1.0")
                        Thread.Sleep(TimeSpan.FromMilliseconds(100))
                    End While
                Next
            End If

            ' 3. Way: Bulk write (with object).
            If True Then
                ' Or use the methods of the PLC device connection at the highest abstraction
                ' layer to write the whole PLC data at once from a user defined PLC object.

                For Each line As String In File.ReadAllLines(".\Data.csv")
                    Dim values As String() = line.Split(";"c)
                    Dim data As PrintJobData = New PrintJobData()

                    data.NumberOfPages = Convert.ToByte(values(0))     ' Number of pages.
                    data.Resolution = Convert.ToInt16(values(1))       ' Resolution in dpi.
                    data.LineHeight = Convert.ToInt32(values(2))       ' Line Height in pixels.
                    data.Price = Convert.ToSingle(values(3))           ' Price.
                    data.ArticleNumber = Convert.ToString(values(4))   ' Article Number.

                    connection.WriteObject(data)

                    ' Wait while printing.
                    While connection.ReadBoolean("E 1.0")
                        Thread.Sleep(TimeSpan.FromMilliseconds(100))
                    End While
                Next
            End If

            connection.Close()
        End Sub
    End Class
End Namespace

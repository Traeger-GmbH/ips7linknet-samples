' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports IPS7Lnk.Advanced

Namespace QueryDynamicData
    Public Class Data
        Inherits PlcObject

        Private Shared _dataBlockNumber As Integer = 1

        Private _byteValue As PlcByte
        Private _int16Value As PlcInt16
        Private _int32Value As PlcInt32
        Private _realValue As PlcReal
        Private _stringValue As PlcString

        Public Sub New()
            MyBase.New()

            ' The following lines initialize the local PLC specific members using the
            ' different PLC types to generate the dynamic members of the PLC Object.
            ' While the PlcByte, PlcInt16, etc. are initialized with a dynamically
            ' created PLC address which uses a static member to identify the number
            ' of the DataBlock to address within the new instance.

            ' DBx.DBB 2
            Me._byteValue = New PlcByte(New PlcAddress(
                    PlcOperand.DataBlock(Data._dataBlockNumber), PlcRawType.Byte, 2))
            Me.Members.Add("ByteValue", Me._byteValue)

            ' DBx.DBW 4
            Me._int16Value = New PlcInt16(New PlcAddress(
                    PlcOperand.DataBlock(Data._dataBlockNumber), PlcRawType.Word, 4))
            Me.Members.Add("Int16Value", Me._int16Value)

            ' DBx.DBD 6
            Me._int32Value = New PlcInt32(New PlcAddress(
                    PlcOperand.DataBlock(Data._dataBlockNumber), PlcRawType.DWord, 6))
            Me.Members.Add("Int32Value", Me._int32Value)

            ' DBx.DBD 10
            Me._realValue = New PlcReal(New PlcAddress(
                    PlcOperand.DataBlock(Data._dataBlockNumber), PlcRawType.DWord, 10))
            Me.Members.Add("RealValue", Me._realValue)

            ' DBx.DBB 20
            Me._stringValue = New PlcString(New PlcAddress(
                    PlcOperand.DataBlock(Data._dataBlockNumber), PlcRawType.Byte, 20), 16)
            Me.Members.Add("StringValue", Me._stringValue)
        End Sub

        Public Shared Property DataBlockNumber As Integer
            Get
                Return Data._dataBlockNumber
            End Get
            Set(ByVal value As Integer)
                Data._dataBlockNumber = value
            End Set
        End Property

        Public Property ByteValue As Byte
            Get
                Return Me._byteValue.Value
            End Get
            Set(ByVal value As Byte)
                Me._byteValue.Value = value
            End Set
        End Property

        Public Property Int16Value As Short
            Get
                Return Me._int16Value.Value
            End Get
            Set(ByVal value As Short)
                Me._int16Value.Value = value
            End Set
        End Property

        Public Property Int32Value As Integer
            Get
                Return Me._int32Value.Value
            End Get
            Set(ByVal value As Integer)
                Me._int32Value.Value = value
            End Set
        End Property

        Public Property RealValue As Single
            Get
                Return Me._realValue.Value
            End Get
            Set(ByVal value As Single)
                Me._realValue.Value = value
            End Set
        End Property

        Public Property StringValue As String
            Get
                Return Me._stringValue.Value
            End Get
            Set(ByVal value As String)
                Me._stringValue.Value = value
            End Set
        End Property
    End Class
End Namespace

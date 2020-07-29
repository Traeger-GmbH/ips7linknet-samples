' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports IPS7Lnk.Advanced

Namespace QueryData
    Public Class Data
        Inherits PlcObject

        <PlcMember("DB111.DBB 2")>
        Public ByteValue As Byte

        <PlcMember("DB111.DBW 4")>
        Public Int16Value As Short

        <PlcMember("DB111.DBD 6")>
        Public Int32Value As Integer

        <PlcMember("DB111.DBD 10")>
        Public RealValue As Single
        
        <PlcMember("DB111.DBB 20", Length:=16)>
        Public StringValue As String
    End Class
End Namespace

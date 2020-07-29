' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports IPS7Lnk.Advanced

Namespace XlsToPlc
    Public Class PrintJobData
        Inherits PlcObject

        <PlcMember("DB111.DBB 2")>
        Public NumberOfPages As Byte

        <PlcMember("DB111.DBW 4")>
        Public Resolution As Short

        <PlcMember("DB111.DBD 6")>
        Public LineHeight As Integer

        <PlcMember("DB111.DBD 10")>
        Public Price As Single

        <PlcMember("DB111.DBB 20", Length:=16)>
        Public ArticleNumber As String

        <PlcMember("DB111.DBX 1.0")>
        Public ReadOnly StartPrint As Boolean = True
    End Class
End Namespace

' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports IPS7Lnk.Advanced

Namespace PlcToSql
    Public Class WeatherData
        Inherits PlcObject

        <PlcMember("DB111.DBB 2")>
        Public ChanceOfRain As Byte

        <PlcMember("DB111.DBW 4")>
        Public WindSpeed As Short

        <PlcMember("DB111.DBD 6")>
        Public Pressure As Integer

        <PlcMember("DB111.DBD 10")>
        Public Temperature As Single

        <PlcMember("DB111.DBB 20", Length:=16)>
        Public Forecast As String
    End Class
End Namespace

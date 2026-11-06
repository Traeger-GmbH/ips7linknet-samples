' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Option Strict On
Imports IPS7Lnk.Advanced

Namespace Binding
    ''' <summary>
    ''' Represents the data of a machine in DB1 of the PLC.
    ''' </summary>
    ''' <remarks>
    ''' PlcObject raises PropertyChanged for properties only; a field raises MemberChanged.
    ''' </remarks>
    Public Class MachineData
        Inherits PlcObject
        <PlcMember("DB1.DBD 0")>
        Public Property Speed As Integer

        <PlcMember("DB1.DBD 4")>
        Public Property Temperature As Single
    End Class
End Namespace

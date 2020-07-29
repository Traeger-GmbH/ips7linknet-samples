' Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

Imports System
Imports IPS7Lnk.Advanced

Namespace Notifications
    ''' <summary>
    ''' This sample demonstrates how to work with notifications in an advanced manner.
    ''' </summary>
    Public Class Program
        Public Shared Sub Main(ByVal args As String())
            ' As already demonstrated in the basic sample 'Notifications' it is possible to
            ' handle the state changes of a connection using the events provided by the static
            ' PlcNotifications class or using the events provided by the instance of a
            ' connection itself.
            ' Additionally taking the basic sample 'Exceptions' into account it is obvious that
            ' developers using the framework are able to fully control and monitor the creation
            ' of connections, the state of these connections and they can influence the status
            ' evaluation of every operation performed by the low level driver used within the
            ' framework.

            ' This sample does therefore demonstrate how a developer can get the most out of
            ' the features represented by the basic samples 'Notifications' and 'Exceptions'.

            ' Assign an event handler to the ConnectionCreated event to get notified when ever
            ' a new connection has been created within the framework. This handler does then
            ' subscribe further events on the connection.
            AddHandler PlcNotifications.ConnectionCreated, AddressOf Program.HandleConnectionCreated

            ' Assign a callback method to evaluate the operational status in a custom way to
            ' ensure that only in specific conditions failed operations will lead to an exception.
            PlcNotifications.EvaluateStatus = AddressOf Program.EvaluateStatus

            Dim device As SiemensDevice = New SiemensDevice( _
                    New IPDeviceEndPoint("192.168.0.80"), SiemensDeviceType.S7300_400)

            Dim connection As PlcDeviceConnection = device.CreateConnection()

            Console.WriteLine()
            Console.WriteLine("= Open =")
            connection.Open()

            ' An explicit call to Connect() is not required. But it will be done here to
            ' demonstrate the appropriate state changes which occur in case of the
            ' first attempt to acccess the PLC device.
            Console.WriteLine()
            Console.WriteLine("= Connect =")
            connection.Connect()

            Program.ReadBoolean(connection)
            Program.ReadBooleanArray(connection)

            Console.WriteLine()
            Console.WriteLine("= Close =")
            connection.Close()

            Console.ReadKey()
        End Sub

        Private Shared Function EvaluateStatus(ByVal provider As IPlcStatusProvider) As Boolean
            ' By default any PlcStatusCode unequal to NoError will lead to a PlcException.
            ' In this scenario we ignore the case that a PLC device can be temporary offline
            ' and therefore unavailable. Additionally we do also ignore Timeouts in cases of
            ' a bad network connection and access to missing data areas (NoData).
            Dim status As PlcStatus = provider.Status

            Console.WriteLine()
            Console.WriteLine("--> {0}.Status.Evaluate: {1}", provider.GetType().Name, status.Code)
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"))
            Console.WriteLine("----> Text: {0}", status.Text)

            If status.Type IsNot Nothing Then Console.WriteLine("----> Type: {0}", status.Type)

            Dim statusCode As PlcStatusCode = status.Code

            Return statusCode = PlcStatusCode.NoError _
                    OrElse statusCode = PlcStatusCode.CpuNotFound _
                    OrElse statusCode = PlcStatusCode.Timeout _
                    OrElse statusCode = PlcStatusCode.NoData

            ' Instead of the whole code above is also possible to clear out all types of error and
            ' information codes by just always returning the value true. The value true does
            ' indicate that each status of any provider has been evaluated by custom code and no
            ' further framework evaluation is required.
            ' return true;
        End Function

        Private Shared Sub HandleConnectionCreated(ByVal sender As Object, ByVal e As PlcNotifications.PlcDeviceConnectionEventArgs)
            AddHandler e.Connection.StateChanged, AddressOf Program.HandleStateChanged
            AddHandler e.Connection.Status.Changed, AddressOf Program.HandleConnectionStatusChanged
        End Sub

        Private Shared Sub HandleStateChanged(ByVal sender As Object, ByVal e As PlcDeviceConnectionStateChangedEventArgs)
            ' The StateChanged event occurs when ever the PlcDeviceConnection.State property gets
            ' changed. This property does provide general low level driver independent state
            ' information of a connection. This state information does indicate whether a
            ' a connection is in a useful state and whether read/write operations can be
            ' performed.
            Dim connection As PlcDeviceConnection = CType(sender, PlcDeviceConnection)

            Console.WriteLine()
            Console.WriteLine("Connection to '{0}'", connection.Device.EndPoint)
            Console.WriteLine("-> State.Changed from '{0}' to '{1}'.", e.OldState, e.NewState)
        End Sub

        Private Shared Sub HandleConnectionStatusChanged(ByVal sender As Object, ByVal e As EventArgs)
            ' The Status.Changed event does provide low level operational status information and
            ' does therefore reflect the result of the most recent operation performed on the low
            ' level driver used by the framework.
            Dim status As PlcStatus = CType(sender, PlcStatus)

            Console.WriteLine()
            Console.WriteLine("--> Connection.Status.Change: {0}", status.Code)
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"))
            Console.WriteLine("----> Text: {0}", status.Text)

            If status.Type IsNot Nothing Then Console.WriteLine("----> Type: {0}", status.Type)
        End Sub

        Private Shared Sub HandleValueStatusChanged(ByVal sender As Object, ByVal e As EventArgs)
            ' The Status.Changed event does provide low level operational status information and
            ' does therefore reflect the result of the most recent operation performed on the low
            ' level driver used by the framework.
            Dim status As PlcStatus = CType(sender, PlcStatus)

            Console.WriteLine()
            Console.WriteLine("--> Value.Status.Change: {0}", status.Code)
            Console.WriteLine("----> TimeStamp: {0}", status.TimeStamp.ToString("hh:mm:ss.fff"))
            Console.WriteLine("----> Text: {0}", status.Text)

            If status.Type IsNot Nothing Then Console.WriteLine("----> Type: {0}", status.Type)
        End Sub

        Private Shared Sub ReadBoolean(ByVal connection As PlcDeviceConnection)
            Console.WriteLine()
            Console.WriteLine("= Read Boolean =")
            Dim value1 As Boolean = connection.ReadBoolean("DB1.DBX 1.0")

            Console.WriteLine()
            Console.WriteLine("= Read Boolean (NoData) =")
            Dim value2 As Boolean = connection.ReadBoolean("DB10000.DBX 1.0")

            Console.WriteLine()
            Console.WriteLine("= Read PlcBoolean =")
            Dim plcValue1 As PlcBoolean = New PlcBoolean("DB1.DBX 1.0")
            AddHandler plcValue1.Status.Changed, AddressOf Program.HandleValueStatusChanged
            Dim value3 As Boolean = connection.ReadValue(plcValue1)

            Console.WriteLine()
            Console.WriteLine("= Read PlcBoolean (NoData) =")
            Dim plcValue2 As PlcBoolean = New PlcBoolean("DB10000.DBX 1.0")
            AddHandler plcValue2.Status.Changed, AddressOf Program.HandleValueStatusChanged
            Dim value4 As Boolean = connection.ReadValue(plcValue2)
        End Sub

        Private Shared Sub ReadBooleanArray(ByVal connection As PlcDeviceConnection)
            Console.WriteLine()
            Console.WriteLine("= Read Boolean Array =")
            Dim values1 As Boolean() = connection.ReadBoolean("DB1.DBX 1.0", 2)

            Console.WriteLine()
            Console.WriteLine("= Read Boolean Array (NoData) =")
            Dim values2 As Boolean() = connection.ReadBoolean("DB10000.DBX 1.0", 2)

            Console.WriteLine()
            Console.WriteLine("= Read PlcBooleanArray =")
            Dim plcValueArray1 As PlcBooleanArray = New PlcBooleanArray("DB1.DBX 1.0", 2)
            AddHandler plcValueArray1.Status.Changed, AddressOf Program.HandleValueStatusChanged
            Dim values3 As Boolean() = connection.ReadValue(plcValueArray1)
            
            Console.WriteLine()
            Console.WriteLine("= Read PlcBooleanArray (NoData) =")
            Dim plcValueArray2 As PlcBooleanArray = New PlcBooleanArray("DB10000.DBX 1.0", 2)
            AddHandler plcValueArray2.Status.Changed, AddressOf Program.HandleValueStatusChanged
            Dim values4 As Boolean() = connection.ReadValue(plcValueArray2)
        End Sub
    End Class
End Namespace

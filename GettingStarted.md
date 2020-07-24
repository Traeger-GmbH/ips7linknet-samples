# Getting started

## 1. Create Solution

```bash
dotnet new sln

mkdir S7App
cd S7App
dotnet new console
dotnet add package IPS7LnkNet.Advanced

cd ..
dotnet sln add ./S7App/S7App.csproj

```

## 2. Implement S7 App

```csharp
var device = new SiemensDevice(
        new IPDeviceEndPoint("192.168.0.80"),
        SiemensDeviceType.S71500);

using (var connection = device.CreateConnection()) {
    connection.Open();
    Console.WriteLine("Device connection is opened...");
    
    var temperature = connection.ReadDouble("DB10.DBD 20");
    Console.WriteLine($"Current Temperature is {temperature} °C");
}
```

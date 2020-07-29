// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace PlcToSql
{
    using IPS7Lnk.Advanced;

    public class WeatherData : PlcObject
    {
        [PlcMember("DB111.DBB 2")]
        public byte ChanceOfRain;

        [PlcMember("DB111.DBW 4")]
        public short WindSpeed;

        [PlcMember("DB111.DBD 6")]
        public int Pressure;

        [PlcMember("DB111.DBD 10")]
        public float Temperature;

        [PlcMember("DB111.DBB 20", Length = 16)]
        public string Forecast;
    }
}

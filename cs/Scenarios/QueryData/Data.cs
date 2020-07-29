// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace QueryData
{
    using IPS7Lnk.Advanced;

    public class Data : PlcObject
    {
        [PlcMember("DB111.DBB 2")]
        public byte ByteValue;

        [PlcMember("DB111.DBW 4")]
        public short Int16Value;

        [PlcMember("DB111.DBD 6")]
        public int Int32Value;

        [PlcMember("DB111.DBD 10")]
        public float RealValue;

        [PlcMember("DB111.DBB 20", Length = 16)]
        public string StringValue;
    }
}

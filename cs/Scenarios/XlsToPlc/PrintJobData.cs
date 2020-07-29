// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace XlsToPlc
{
    using IPS7Lnk.Advanced;

    public class PrintJobData : PlcObject
    {
        [PlcMember("DB111.DBB 2")]
        public byte NumberOfPages;

        [PlcMember("DB111.DBW 4")]
        public short Resolution;

        [PlcMember("DB111.DBD 6")]
        public int LineHeight;

        [PlcMember("DB111.DBD 10")]
        public float Price;

        [PlcMember("DB111.DBB 20", Length = 16)]
        public string ArticleNumber;

        [PlcMember("DB111.DBX 1.0")]
        public readonly bool StartPrint = true;
    }
}

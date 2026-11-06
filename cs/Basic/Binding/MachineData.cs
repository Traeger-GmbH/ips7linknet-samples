// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace Binding
{
    using IPS7Lnk.Advanced;

    /// <summary>
    /// Represents the data of a machine in DB1 of the PLC.
    /// </summary>
    /// <remarks>
    /// PlcObject raises PropertyChanged for properties only; a field raises MemberChanged.
    /// </remarks>
    public class MachineData : PlcObject
    {
        [PlcMember("DB1.DBD 0")]
        public int Speed { get; set; }

        [PlcMember("DB1.DBD 4")]
        public float Temperature { get; set; }
    }
}

// Copyright (c) Traeger Industry Components GmbH.  All Rights Reserved.

namespace QueryDynamicData
{
    using IPS7Lnk.Advanced;

    public class Data : PlcObject
    {
        private static int dataBlockNumber = 1;

        private PlcByte byteValue;
        private PlcInt16 int16Value;
        private PlcInt32 int32Value;
        private PlcReal realValue;
        private PlcString stringValue;

        public Data()
            : base()
        {
            //// The following lines initialize the local PLC specific members using the
            //// different PLC types to generate the dynamic members of the PLC Object.
            //// While the PlcByte, PlcInt16, etc. are initialized with a dynamically
            //// created PLC address which uses a static member to identify the number
            //// of the DataBlock to address within the new instance.

            // DBx.DBB 2
            this.byteValue = new PlcByte(new PlcAddress(
                    PlcOperand.DataBlock(Data.dataBlockNumber), PlcRawType.Byte, 2));

            this.Members.Add("ByteValue", this.byteValue);

            // DBx.DBW 4
            this.int16Value = new PlcInt16(new PlcAddress(
                    PlcOperand.DataBlock(Data.dataBlockNumber), PlcRawType.Word, 4));
            this.Members.Add("Int16Value", this.int16Value);

            // DBx.DBD 6
            this.int32Value = new PlcInt32(new PlcAddress(
                    PlcOperand.DataBlock(Data.dataBlockNumber), PlcRawType.DWord, 6));
            this.Members.Add("Int32Value", this.int32Value);

            // DBx.DBD 10
            this.realValue = new PlcReal(new PlcAddress(
                    PlcOperand.DataBlock(Data.dataBlockNumber), PlcRawType.DWord, 10));
            this.Members.Add("RealValue", this.realValue);

            // DBx.DBB 20
            this.stringValue = new PlcString(new PlcAddress(
                    PlcOperand.DataBlock(Data.dataBlockNumber), PlcRawType.Byte, 20), 16);
            this.Members.Add("StringValue", this.stringValue);
        }

        public static int DataBlockNumber
        {
            get
            {
                return Data.dataBlockNumber;
            }
            set
            {
                Data.dataBlockNumber = value;
            }
        }

        public byte ByteValue
        {
            get
            {
                return this.byteValue.Value;
            }
            set
            {
                this.byteValue.Value = value;
            }
        }

        public short Int16Value
        {
            get
            {
                return this.int16Value.Value;
            }
            set
            {
                this.int16Value.Value = value;
            }
        }

        public int Int32Value
        {
            get
            {
                return this.int32Value.Value;
            }
            set
            {
                this.int32Value.Value = value;
            }
        }

        public float RealValue
        {
            get
            {
                return this.realValue.Value;
            }
            set
            {
                this.realValue.Value = value;
            }
        }

        public string StringValue
        {
            get
            {
                return this.stringValue.Value;
            }
            set
            {
                this.stringValue.Value = value;
            }
        }
    }
}

namespace NES_Emulator
{

    public sealed class CPUOpcodes
    {
        public const byte BRK = 0x00;
        public const byte TAX = 0xAA;
        public const byte INX = 0xE8;
        public const byte STA = 0xE8;

        // LDA
        public const byte LDA_Immediate = 0xA9;
        public const byte LDA_ZeroPage = 0xA5;
        public const byte LDA_ZeroPage_X = 0xB5;
        public const byte LDA_Absolute = 0xAD;
        public const byte LDA_Absolute_X = 0xBD;
        public const byte LDA_Absolute_Y = 0xB9;
        public const byte LDA_Indirect_X = 0xA1;
        public const byte LDA_Indirect_Y = 0xB1;

        // STA
        public const byte STA_ZeroPage = 0x85;
        public const byte STA_ZeroPage_X = 0x95;
        public const byte STA_Absolute = 0x8D;
        public const byte STA_Absolute_X = 0x9D;
        public const byte STA_Absolute_Y = 0x99;
        public const byte STA_Indirect_X = 0x81;
        public const byte STA_Indirect_Y = 0x91;
    }
}

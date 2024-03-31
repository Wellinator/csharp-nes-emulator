namespace NES_Emulator
{
    /**
    7  bit  0
    ---- ----
    NV1B DIZC
    |||| ||||
    |||| |||+- Carry
    |||| ||+-- Zero
    |||| |+--- Interrupt Disable
    |||| +---- Decimal
    |||+------ (No CPU effect; see: the B flag)
    ||+------- (No CPU effect; always pushed as 1)
    |+-------- Overflow
    +--------- Negative

    Not used bits
    BFlag = 0b00010000,
    N/A =   0b00100000,
    */
    public sealed class CPUStatus
    {
        public readonly static byte Carry = 0b00000001;
        public readonly static byte Zero = 0b00000010;
        public readonly static byte Interrupt = 0b00000100;
        public readonly static byte Decimal = 0b00001000;
        public readonly static byte Overflow = 0b01000000;
        public readonly static byte Negative = 0b10000000;
    }
}

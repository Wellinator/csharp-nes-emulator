namespace NES_Emulator
{
    public enum CPUAddressingMode
    {
        Impliend,
        Accumulator,
        Immediate,
        ZeroPage,
        ZeroPage_X,
        ZeroPage_Y,
        Absolute,
        Absolute_X,
        Absolute_Y,
        Indirect_X,
        Indirect_Y,
        Absolute_Indirect,
        Relative,
        NoneAddressing
    }
}
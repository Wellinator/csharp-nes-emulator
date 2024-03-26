namespace NES_Emulator
{
    public enum CPUAddressingMode
    {
        Immediate = 0,
        ZeroPage = 1,
        ZeroPage_X = 2,
        ZeroPage_Y = 3,
        Absolute = 4,
        Absolute_X = 5,
        Absolute_Y = 6,
        Indirect_X = 7,
        Indirect_Y = 8,
        NoneAddressing = 9,
    }
}
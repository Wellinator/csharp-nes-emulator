using NES_Emulator;

namespace EmulatorTests;

public class CPUTests
{
    [Fact]
    public void test_0xa9_lda_immediate_load_data()
    {
        CPU uut = new CPU();
        byte[] data = new byte[] { 0xa9, 0x05, 0x00 };
        uut.register_a = 10;
        uut.interprete(data);

        // Testing operation
        Assert.Equal(0x05, uut.register_a);

        // Testing status
        Assert.Equal(0b00, 0b00000010 & uut.status);
        Assert.Equal(0, 0b10000000 & uut.status);
    }
}
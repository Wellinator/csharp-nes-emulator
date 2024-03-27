using NES_Emulator;

namespace EmulatorTests;

public class CPUTests
{
    [Fact]
    public void test_0xa9_lda_immediate_load_data()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xa9, 0x05, 0x00 };
        uut.register_acc = 10;

        uut.loadAndRun(data);


        // Testing operation
        Assert.Equal(0x05, uut.register_acc);
        // Testing status
        Assert.Equal(0b00, 0b00000010 & uut.status);
        Assert.Equal(0, 0b10000000 & uut.status);
    }

    [Fact]
    public void test_0xa9_lda_zero_flag()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xa9, 0x00, 0x00 };

        uut.loadAndRun(data);

        // Testing status
        Assert.Equal(0b10, 0b00000010 & uut.status);
    }

    [Fact]
    public void test_0xaa_tax_move_a_to_x()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xaa, 0x00 };

        uut.load(data);
        uut.reset();

        uut.register_acc = 10;

        uut.run();

        Assert.Equal(10, uut.register_x);
    }

    [Fact]
    public void test_5_ops_working_together()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xa9, 0xc0, 0xaa, 0xe8, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0xc1, uut.register_x);
    }

    [Fact]
    public void test_inx_overflow_should_wrap_to_zero()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xe8, 0xe8, 0x00 };

        uut.load(data);
        uut.reset();

        uut.register_x = 0xFF;

        uut.run();


        Assert.Equal(0x01, uut.register_x);
    }

    [Fact]
    public void test_lda_from_memory()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0xA5, 0x10, 0x00 };

        mem.write(0x10, 0x55);
        uut.loadAndRun(data);


        Assert.Equal(0x55, uut.register_acc);
    }

    [Fact]
    public void test_clear_carry_bit()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0x18, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Carry);
        uut.run();

        Assert.Equal(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0x18, 0xA9, 0x4C, 0x6D, 0x10, 0x00 };


        mem.write(0x10, 0x55);
        uut.loadAndRun(data);

        Assert.Equal(0xA1, uut.register_acc);
        Assert.Equal(0x00, uut.status & CPUStatus.Carry);
        // Assert.Equal(0x00, uut.status & CPUStatus.Overflow);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0x18, 0xA9, 0xFE, 0x6D, 0x10, 0x00 };

        mem.write(0x10, 0x55);
        uut.loadAndRun(data);

        Assert.Equal(0x54, uut.register_acc);
        Assert.NotEqual(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_and_memory_with_accumulator()
    {
        Memory mem = new Memory();
        CPU uut = new CPU(mem);
        byte[] data = new byte[] { 0x18, 0xA9, 0xF0, 0x29, 0x55, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x50, uut.register_acc);
    }
}
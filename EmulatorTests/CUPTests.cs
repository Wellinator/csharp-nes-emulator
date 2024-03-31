using NES_Emulator;

namespace EmulatorTests;

public class CPUTests
{
    private readonly Memory mem;
    private readonly CPU uut;

    public CPUTests(){
        mem = new Memory();
        uut = new CPU(mem);
    }

    
    [Fact]
    public void test_0xa9_lda_immediate_load_data()
    {
        byte[] data = new byte[] { 0xa9, 0x05, 0x00 };
        uut.register_acc = 10;

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
        Assert.Equal(0b00, 0b00000010 & uut.status);
        Assert.Equal(0, 0b10000000 & uut.status);
    }

    [Fact]
    public void test_0xa9_lda_zero_flag()
    {
        byte[] data = new byte[] { 0xa9, 0x00, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0b10, 0b00000010 & uut.status);
    }

    [Fact]
    public void test_0xaa_tax_move_a_to_x()
    {
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
        byte[] data = new byte[] { 0xa9, 0xc0, 0xaa, 0xe8, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0xc1, uut.register_x);
    }

    [Fact]
    public void test_inx_overflow_should_wrap_to_zero()
    {
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
        byte[] data = new byte[] { 0xA5, 0x10, 0x00 };
        mem.write(0x10, 0x55);

        uut.loadAndRun(data);

        Assert.Equal(0x55, uut.register_acc);
    }

    [Fact]
    public void test_clear_carry_bit()
    {
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
        byte[] data = new byte[] { 0x18, 0xA9, 0x4C, 0x6D, 0x10, 0x00 };

        mem.write(0x10, 0x55);
        uut.loadAndRun(data);

        Assert.Equal(0xA1, uut.register_acc);
        Assert.Equal(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0xFE, 0x6D, 0x10, 0x00 };

        mem.write(0x10, 0x55);
        uut.loadAndRun(data);

        Assert.Equal(0x54, uut.register_acc);
        Assert.NotEqual(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_and_memory_with_accumulator()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0xF0, 0x29, 0x55, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x50, uut.register_acc);
    }

    [Fact]
    public void test_shift_left_one_bit_accumulator_no_carry()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0x01, 0x0A, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x02, uut.register_acc);
    }

    [Fact]
    public void test_shift_left_one_bit_accumulator_with_carry()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0x82, 0x0A, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x04, uut.register_acc);
        Assert.True((uut.status & CPUStatus.Carry) == CPUStatus.Carry);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_carry_clear()
    {
        byte[] data = new byte[] { 0x90, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_carry_set()
    {
        byte[] data = new byte[] { 0x90, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Carry);
        uut.run();

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_carry_set()
    {
        byte[] data = new byte[] { 0xB0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Carry);
        uut.run();

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_carry_clear()
    {
        byte[] data = new byte[] { 0xB0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);


        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_zero_set()
    {
        byte[] data = new byte[] { 0xF0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Zero);
        uut.run();

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_zero_clear()
    {
        byte[] data = new byte[] { 0xF0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x00, uut.register_acc);
    }
}
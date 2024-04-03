using NES_Emulator;

namespace EmulatorTests;

public class CPUTests
{
    private readonly Memory mem;
    private readonly CPU uut;

    public CPUTests()
    {
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
    public void test_ldx_immediate_load_data()
    {
        byte[] data = new byte[] { 0xA2, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_x);
        Assert.Equal(0b00, 0b00000010 & uut.status);
        Assert.Equal(0, 0b10000000 & uut.status);
    }

    [Fact]
    public void test_ldx_zero_flag()
    {
        byte[] data = new byte[] { 0xA2, 0x00, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0b10, 0b00000010 & uut.status);
    }

    [Fact]
    public void test_ldy_immediate_load_data()
    {
        byte[] data = new byte[] { 0xA0, 0x05, 0x00 };
        uut.register_y = 10;

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_y);
        Assert.Equal(0b00, 0b00000010 & uut.status);
        Assert.Equal(0, 0b10000000 & uut.status);
    }

    [Fact]
    public void test_ldy_zero_flag()
    {
        byte[] data = new byte[] { 0xA0, 0x00, 0x00 };

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

    [Fact]
    public void test_bits_in_memory_with_accumulator_zero_flag_showld_be_set()
    {
        byte[] data = new byte[] { 0xa9, 0x0f, 0x24, 0x10, 0x00 };
        mem.write(0x10, 0xf0);

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Zero) == CPUStatus.Zero);
    }

    [Fact]
    public void test_bits_in_memory_with_accumulator()
    {
        byte[] data = new byte[] { 0xa9, 0x0f, 0x24, 0x10, 0x00 };
        mem.write(0x10, 0xf0);

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Overflow) == CPUStatus.Overflow);
        Assert.True((uut.status & CPUStatus.Negative) == CPUStatus.Negative);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_negative_set()
    {
        byte[] data = new byte[] { 0x30, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Negative);
        uut.run();

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_negative_clear()
    {
        byte[] data = new byte[] { 0x30, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_zero_flag_clear()
    {
        byte[] data = new byte[] { 0xD0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_zero_flag_set()
    {
        byte[] data = new byte[] { 0xD0, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Zero);
        uut.run();

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_negative_is_clear()
    {
        byte[] data = new byte[] { 0x10, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_negative_is_set()
    {
        byte[] data = new byte[] { 0x10, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Negative);
        uut.run();

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_overflow_is_clear()
    {
        byte[] data = new byte[] { 0x50, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_overflow_is_set()
    {
        byte[] data = new byte[] { 0x50, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Overflow);
        uut.run();

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_branch_if_overflow_set()
    {
        byte[] data = new byte[] { 0x70, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Overflow);
        uut.run();

        Assert.Equal(0x05, uut.register_acc);
    }

    [Fact]
    [Trait("Category", "Branch")]
    public void test_not_branch_if_overflow_clear()
    {
        byte[] data = new byte[] { 0x70, 0x02, 0x00, 0xa9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x00, uut.register_acc);
    }

    [Fact]
    public void test_clear_decimal_bit()
    {
        byte[] data = new byte[] { 0xD8, 0x00 };

        uut.load(data);
        uut.reset();
        uut.setStatus(CPUStatus.Decimal);
        uut.run();

        Assert.Equal(0x00, uut.status & CPUStatus.Decimal);
    }

    [Fact]
    public void test_clear_interrupt_bit()
    {
        byte[] data = new byte[] { 0x58, 0x00 };
        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Interrupt);
        uut.run();

        Assert.Equal(0x00, uut.status & CPUStatus.Interrupt);
    }

    [Fact]
    public void test_clear_overflow_bit()
    {
        byte[] data = new byte[] { 0xB8, 0x00 };
        uut.load(data);
        uut.reset();

        uut.setStatus(CPUStatus.Overflow);
        uut.run();

        Assert.Equal(0x00, uut.status & CPUStatus.Overflow);
    }

    [Fact]
    public void test_compare_memory_with_accumulator()
    {
        byte[] data = new byte[] { 0xa9, 0x05, 0xC9, 0x01, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Carry) > 0);
    }

    [Fact]
    public void test_compare_memory_with_accumulator_zero_value()
    {
        byte[] data = new byte[] { 0xa9, 0x05, 0xC9, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Zero) > 0);
    }

    [Fact]
    public void test_compare_memory_with_x_reg()
    {
        byte[] data = new byte[] { 0xA2, 0x05, 0xE0, 0x01, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Carry) > 0);
    }

    [Fact]
    public void test_compare_memory_with_x_reg_zero_value()
    {
        byte[] data = new byte[] { 0xA2, 0x05, 0xE0, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Zero) > 0);
    }

    [Fact]
    public void test_compare_memory_with_y_reg()
    {
        byte[] data = new byte[] { 0xA0, 0x05, 0xC0, 0x01, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Carry) > 0);
    }

    [Fact]
    public void test_compare_memory_with_y_reg_zero_value()
    {
        byte[] data = new byte[] { 0xA0, 0x05, 0xC0, 0x05, 0x00 };

        uut.loadAndRun(data);

        Assert.True((uut.status & CPUStatus.Zero) > 0);
    }

    [Fact]
    public void test_dec_decrement_memory_by_one()
    {
        byte[] data = new byte[] { 0xC6, 0x10, 0x00 };
        mem.write(0x10, 0x02);

        uut.loadAndRun(data);

        var moddedValue = mem.read(0x10);
        Assert.Equal(0x01, moddedValue);
    }

    [Fact]
    public void test_dec_decrement_memory_by_one_zero_value()
    {
        byte[] data = new byte[] { 0xC6, 0x10, 0x00 };
        mem.write(0x10, 0x00);

        uut.loadAndRun(data);

        var moddedValue = mem.read(0x10);
        Assert.Equal(0xFF, moddedValue);
    }
}
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

    /**
    C=0, A=$FE: ADC #1   gives $FF, and C remains clear because you did not exceed what you can represent in 8 bits.
    C=1, A=$FE: ADC #0   gives $FF, and C  gets cleared because you did not exceed what you can represent in 8 bits.
    C=1, A=$FE: ADC #1   gives $00; but since the answer is $100, the 1 goes to the C flag, so it is set.
    C=0, A=$FE: ADC #$10 gives $0E; but since the answer is $10E, the 1 goes to the C flag, so it is set.
    */
    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow_1()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0xFE, 0x69, 0x01, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0xFF, uut.register_acc);
        Assert.Equal(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow_2()
    {
        byte[] data = new byte[] { 0x38, 0xA9, 0xFE, 0x69, 0x00, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0xFF, uut.register_acc);
        Assert.Equal(0x00, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow_3()
    {
        byte[] data = new byte[] { 0x38, 0xA9, 0xFE, 0x69, 0x01, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x00, uut.register_acc);
        Assert.Equal(0x01, uut.status & CPUStatus.Carry);
    }

    [Fact]
    public void test_add_memory_to_accumulator_with_carry_overflow_4()
    {
        byte[] data = new byte[] { 0x18, 0xA9, 0xFE, 0x69, 0x10, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x0E, uut.register_acc);
        Assert.Equal(0x01, uut.status & CPUStatus.Carry);
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
        byte[] data = new byte[] { 
            CPUOpcodes.BCC_Relative, 
            0x01, 
            0x00, 
            CPUOpcodes.LDA_Immediate, 
            0x05, 
            0x00 
        };

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
        byte[] data = new byte[] { 0xB0, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0xF0, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0x30, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0xD0, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0x10, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0x50, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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
        byte[] data = new byte[] { 0x70, 0x01, 0x00, 0xa9, 0x05, 0x00 };

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

    [Fact]
    public void test_dex_decrement_register_x_by_one()
    {
        byte[] data = new byte[] { 0xCA, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_x = 2;

        uut.run();

        Assert.Equal(1, uut.register_x);
    }

    [Fact]
    public void test_dex_decrement_register_x_by_one_with_zero_value()
    {
        byte[] data = new byte[] { 0xCA, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_x = 0;

        uut.run();

        Assert.Equal(255, uut.register_x);
    }

    [Fact]
    public void test_dey_decrement_register_y_by_one()
    {
        byte[] data = new byte[] { 0x88, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_y = 2;

        uut.run();

        Assert.Equal(1, uut.register_y);
    }

    [Fact]
    public void test_dey_decrement_register_y_by_one_with_zero_value()
    {
        byte[] data = new byte[] { 0x88, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_y = 0;

        uut.run();

        Assert.Equal(255, uut.register_y);
    }

    [Fact]
    public void test_eor_exclusive_or_memory_with_accumulator()
    {
        byte[] data = new byte[] { 0xA9, 0x55, 0x49, 0x5F, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x0A, uut.register_acc);
    }

    [Fact]
    public void test_inc_increment_memory_by_one()
    {
        byte[] data = new byte[] { 0xE6, 0x10, 0x00 };
        mem.write(0x10, 0x01);

        uut.loadAndRun(data);

        var moddedValue = mem.read(0x10);
        Assert.Equal(0x02, moddedValue);
    }

    [Fact]
    public void test_inc_increment_memory_by_one_zero_value()
    {
        byte[] data = new byte[] { 0xE6, 0x10, 0x00 };
        mem.write(0x10, 0xFF);

        uut.loadAndRun(data);

        var moddedValue = mem.read(0x10);
        Assert.Equal(0x00, moddedValue);
    }

    [Fact]
    public void test_inx_increment_register_x_by_one()
    {
        byte[] data = new byte[] { 0xE8, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_x = 1;

        uut.run();

        Assert.Equal(2, uut.register_x);
    }

    [Fact]
    public void test_inx_increment_register_x_by_one_with_zero_value()
    {
        byte[] data = new byte[] { 0xE8, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_x = 255;

        uut.run();

        Assert.Equal(0, uut.register_x);
    }

    [Fact]
    public void test_iny_increment_register_y_by_one()
    {
        byte[] data = new byte[] { 0xC8, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_y = 1;

        uut.run();

        Assert.Equal(2, uut.register_y);
    }

    [Fact]
    public void test_iny_increment_register_y_by_one_with_zero_value()
    {
        byte[] data = new byte[] { 0xC8, 0x00 };
        uut.load(data);
        uut.reset();
        uut.register_y = 255;

        uut.run();

        Assert.Equal(0, uut.register_y);
    }

    [Fact]
    public void test_lsr_shift_one_bit_right()
    {
        byte[] data = new byte[] { 0x46, 0x10, 0x00 };
        mem.write(0x10, 0x0A);

        uut.loadAndRun(data);

        Assert.Equal(0x05, mem.read(0x10));
        Assert.Equal(0, uut.status & CPUStatus.Carry);
        Assert.True((uut.status & CPUStatus.Negative) == 0);
        Assert.True((uut.status & CPUStatus.Zero) == 0);
    }

    [Fact]
    public void test_lsr_shift_one_bit_right_with_accumulator()
    {
        byte[] data = new byte[] { 0xA9, 0x0A, 0x4A, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x05, uut.register_acc);
        Assert.Equal(0, uut.status & CPUStatus.Carry);
        Assert.True((uut.status & CPUStatus.Negative) == 0);
        Assert.True((uut.status & CPUStatus.Zero) == 0);
    }

    [Fact]
    public void test_rol_rotate_one_bit_left_accumulator()
    {
        byte[] data = new byte[] { 0xA9, 0x81, 0x2A, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x02, uut.register_acc);
        Assert.Equal(1, uut.status & CPUStatus.Carry);
        Assert.True((uut.status & CPUStatus.Negative) == 0);
        Assert.True((uut.status & CPUStatus.Zero) == 0);
    }

    [Fact]
    public void test_ror_rotate_one_bit_right_accumulator()
    {
        byte[] data = new byte[] { 0xA9, 0x81, 0x6A, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0x40, uut.register_acc);
        Assert.Equal(1, uut.status & CPUStatus.Carry);
        Assert.True((uut.status & CPUStatus.Negative) == 0);
        Assert.True((uut.status & CPUStatus.Zero) == 0);
    }

    [Fact]
    public void test_sbc_subtract_memory_from_accumulator_with_borrow()
    {
        /*
        ; $FE - $01 (-2 - +1 in decimal)

        SEC       ; Set the Carry flag to indicate no borrow.
        LDA #$01  ; Load $01 as an immediate value into the Accumulator.
        STA $00   ; Store the Accumulator in memory at address $0000.
        LDA #$FE  ; Load $FE as an immediate value into the Accumulator.
        SBC $00   ; Subtract value at address $0000 from the Accumulator.
        */
        byte[] data = new byte[] { 0x38, 0xA9, 0x01, 0x85, 0x00, 0xA9, 0xFE, 0xE5, 0x00, 0x00 };

        uut.loadAndRun(data);

        Assert.Equal(0xFD, uut.register_acc);
        Assert.Equal(1, uut.status & CPUStatus.Carry);
        Assert.True((uut.status & CPUStatus.Overflow) == 0);
    }

    [Fact]
    public void test_jsr_and_rts()
    {
        byte[] data = new byte[] {
            CPUOpcodes.JSR,
            0x00,
            0x80,
            CPUOpcodes.LDA_Immediate,
            0x42
        };

        mem.write(0x8000, CPUOpcodes.RTS);

        uut.loadAndRun(data);

        Assert.Equal(0x42, uut.register_acc);
    }

    [Fact]
    public void test_absolute_jmp()
    {
        byte[] data = new byte[] {
            CPUOpcodes.JMP_Absolute,
            0x00,
            0x80
        };

        uut.loadAndRun(data);

        Assert.Equal(0x8000, uut.program_counter);
    }

    [Fact]
    public void test_indirect_jmp()
    {
        byte[] data = new byte[] {
            CPUOpcodes.JMP_Indirect,
            0x00,
            0x80,
        };

        mem.write(0x8000, 0x00);
        mem.write(0x8001, 0x90);

        uut.loadAndRun(data);

        Assert.Equal(0x9000, uut.program_counter);
    }
}
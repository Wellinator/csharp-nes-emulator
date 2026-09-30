
namespace NES_Emulator
{
    public delegate void OnUpdateCallBack();


    public interface iCPU
    {
        public iMemory _memory { get; set; }
        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public byte stack_pointer { get; set; }
        public ushort program_counter { get; set; }
        public long cycles { get; set; }
        public CPUInstructionTable instruction_table { get; set; }
        public void run(OnUpdateCallBack? callback);
        public byte setStatus(in byte Status);
        public void reset();
        public void load(byte[] Program);
        public void loadAndRun(byte[] Program, OnUpdateCallBack callback);

    }


    public class CPU : iCPU
    {

        public CPU(iMemory Memory)
        {

            /*
            Initial CPU Register Values
            Register	At Power	        After Reset
            A, X, Y	    0	                unchanged
            PC	        ($FFFC)	            ($FFFC)
            S[1]	    $00 - 3 = $FD	    S -= 3
            C	        0	                unchanged
            Z	        0	                unchanged
            I	        1	                1
            D	        0	                unchanged
            V	        0	                unchanged
            N	        0	                unchanged
            */

            _memory = Memory;

            register_acc = 0;
            register_x = 0;
            status = CPUStatus.Initial;
            program_counter = PC_AT_POWER;
            cycles = 1;
            instruction_table = new CPUInstructionTable();
            stack_pointer = STACK_RESET;
        }

        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte register_y { get; set; }
        public byte status { get; set; }
        public byte stack_pointer { get; set; }
        public ushort program_counter { get; set; }
        public long cycles { get; set; }
        public iMemory _memory { get; set; }
        public CPUInstructionTable instruction_table { get; set; }

        private const ushort STACK_START_ADDR = 0x0100;
        private const byte STACK_RESET = 0xFD;
        private const ushort PC_AT_POWER = 0xFFFC;

        public void run(OnUpdateCallBack? callback = null)
        {
            while (true)
            {
                if (callback != null) callback();

                ushort pc_for_log = program_counter;
                byte instruction = _memory.read(program_counter);
                program_counter++;

                CPUInstruction opcode = instruction_table.GetInstruction(instruction);
                int _cycle = opcode.cycles;

                if (instruction == CPUOpcodes.BRK)
                {
                    BRK();
                    return;
                }

                // DEBUG LOG CONTENT INIT
                List<string> data = new List<string>();
                if (opcode.bytes > 1)
                {
                    for (ushort i = 1; i < opcode.bytes; i++)
                    {
                        data.Add(_memory.read((ushort)(pc_for_log + i)).ToString("X2"));
                    }
                }
                var opcodePlusData = $"{opcode.opcode:X2} {String.Join(' ', data.ToArray())}".PadRight(10);
                var mnemonicPlusAddr = $"{opcode.mnemonic} ${String.Join("", data.ToArray().Reverse())}".PadRight(32);
                // DEBUG LOG CONTENT END

                cycles += opcode.cycles;

                Console.WriteLine($"{pc_for_log:X4}  {opcodePlusData} {mnemonicPlusAddr} A:{register_acc:X2} X:{register_x:X2} Y:{register_y:X2} P:{status:X2} SP:{stack_pointer:X2} CYC: {cycles}");

                switch (instruction)
                {
                    // ADC
                    case CPUOpcodes.ADC_Immediate:
                    case CPUOpcodes.ADC_ZeroPage:
                    case CPUOpcodes.ADC_ZeroPage_X:
                    case CPUOpcodes.ADC_Absolute:
                    case CPUOpcodes.ADC_Absolute_X:
                    case CPUOpcodes.ADC_Absolute_Y:
                    case CPUOpcodes.ADC_Indirect_X:
                    case CPUOpcodes.ADC_Indirect_Y:
                        ADC(opcode.mode);
                        break;

                    // AND
                    case CPUOpcodes.AND_Immediate:
                    case CPUOpcodes.AND_ZeroPage:
                    case CPUOpcodes.AND_ZeroPage_X:
                    case CPUOpcodes.AND_Absolute:
                    case CPUOpcodes.AND_Absolute_X:
                    case CPUOpcodes.AND_Absolute_Y:
                    case CPUOpcodes.AND_Indirect_X:
                    case CPUOpcodes.AND_Indirect_Y:
                        AND(opcode.mode);
                        break;

                    // ASL
                    case CPUOpcodes.ASL_Accumulator:
                        ASL();
                        break;

                    case CPUOpcodes.ASL_ZeroPage:
                    case CPUOpcodes.ASL_ZeroPage_X:
                    case CPUOpcodes.ASL_Absolute:
                    case CPUOpcodes.ASL_Absolute_X:
                        ASL(opcode.mode);
                        break;

                    // ASR
                    case CPUOpcodes.ASR_Accumulator:
                    case CPUOpcodes.ASR_ZeroPage:
                    case CPUOpcodes.ASR_ZeroPage_X:
                        ASR();
                        break;

                    case CPUOpcodes.BCC_Relative:
                        BCC();
                        break;

                    case CPUOpcodes.BCS_Relative:
                        BCS();
                        break;

                    case CPUOpcodes.BEQ_Relative:
                        BEQ();
                        break;

                    case CPUOpcodes.BIT_Immediate:
                    case CPUOpcodes.BIT_Absolute:
                    case CPUOpcodes.BIT_Absolute_X:
                    case CPUOpcodes.BIT_ZeroPage:
                    case CPUOpcodes.BIT_ZeroPage_X:
                        BIT(opcode.mode);
                        break;

                    case CPUOpcodes.BMI_Relative:
                        BMI();
                        break;

                    case CPUOpcodes.BNE_Relative:
                        BNE();
                        break;

                    case CPUOpcodes.BPL_Relative:
                        BPL();
                        break;

                    case CPUOpcodes.BRA_Relative:
                        NOP(opcode.mode);
                        break;
                    case CPUOpcodes.BRA_Relative_Word:
                        BRA();
                        break;

                    case CPUOpcodes.BVC:
                        BVC();
                        break;

                    case CPUOpcodes.BVS:
                        BVS();
                        break;

                    case CPUOpcodes.CLC:
                        CLC();
                        break;

                    case CPUOpcodes.CLD:
                        CLD();
                        break;

                    case CPUOpcodes.CLI:
                        CLI();
                        break;

                    case CPUOpcodes.CLV:
                        CLV();
                        break;

                    // CMP
                    case CPUOpcodes.CMP_Immediate:
                    case CPUOpcodes.CMP_ZeroPage:
                    case CPUOpcodes.CMP_ZeroPage_X:
                    case CPUOpcodes.CMP_Absolute:
                    case CPUOpcodes.CMP_Absolute_X:
                    case CPUOpcodes.CMP_Absolute_Y:
                    case CPUOpcodes.CMP_Indirect_X:
                    case CPUOpcodes.CMP_Indirect_Y:
                        CMP(opcode.mode);
                        break;

                    // CPX
                    case CPUOpcodes.CPX_Immediate:
                    case CPUOpcodes.CPX_ZeroPage:
                    case CPUOpcodes.CPX_Absolute:
                        CPX(opcode.mode);
                        break;

                    // CPY
                    case CPUOpcodes.CPY_Immediate:
                    case CPUOpcodes.CPY_ZeroPage:
                    case CPUOpcodes.CPY_Absolute:
                        CPY(opcode.mode);
                        break;

                    // DEC
                    case CPUOpcodes.DEC_Accumulator:
                        NOP(opcode.mode);
                        break;
                    case CPUOpcodes.DEC_ZeroPage:
                    case CPUOpcodes.DEC_ZeroPage_X:
                    case CPUOpcodes.DEC_Absolute:
                    case CPUOpcodes.DEC_Absolute_X:
                        DEC(opcode.mode);
                        break;

                    case CPUOpcodes.DEX:
                        DEX();
                        break;

                    case CPUOpcodes.DEY:
                        DEY();
                        break;

                    // EOR
                    case CPUOpcodes.EOR_Immediate:
                    case CPUOpcodes.EOR_ZeroPage:
                    case CPUOpcodes.EOR_ZeroPage_X:
                    case CPUOpcodes.EOR_Absolute:
                    case CPUOpcodes.EOR_Absolute_X:
                    case CPUOpcodes.EOR_Absolute_Y:
                    case CPUOpcodes.EOR_Indirect_X:
                    case CPUOpcodes.EOR_Indirect_Y:
                        EOR(opcode.mode);
                        break;

                    // INC
                    case CPUOpcodes.INC_Accumulator:
                        NOP(opcode.mode);
                        break;
                    case CPUOpcodes.INC_ZeroPage:
                    case CPUOpcodes.INC_ZeroPage_X:
                    case CPUOpcodes.INC_Absolute:
                    case CPUOpcodes.INC_Absolute_X:
                        INC(opcode.mode);
                        break;

                    case CPUOpcodes.INX:
                        INX();
                        break;

                    case CPUOpcodes.INY:
                        INY();
                        break;

                    // JMP
                    case CPUOpcodes.JMP_Absolute:
                    case CPUOpcodes.JMP_Indirect:
                        JMP(opcode.mode);
                        break;

                    case CPUOpcodes.JSR:
                        JSR();
                        break;

                    // LDA
                    case CPUOpcodes.LDA_Immediate:
                    case CPUOpcodes.LDA_ZeroPage:
                    case CPUOpcodes.LDA_ZeroPage_X:
                    case CPUOpcodes.LDA_Absolute:
                    case CPUOpcodes.LDA_Absolute_X:
                    case CPUOpcodes.LDA_Absolute_Y:
                    case CPUOpcodes.LDA_Indirect_X:
                    case CPUOpcodes.LDA_Indirect_Y:
                        LDA(opcode.mode);
                        break;
                    
                    // LAX
                    case CPUOpcodes.LAX_ZeroPage:
                    case CPUOpcodes.LAX_ZeroPage_Y:
                    case CPUOpcodes.LAX_Absolute:
                    case CPUOpcodes.LAX_Absolute_Y:
                    case CPUOpcodes.LAX_Indirect_X:
                    case CPUOpcodes.LAX_Indirect_Y:
                        LAX(opcode.mode);
                        break;

                    // LDX
                    case CPUOpcodes.LDX_Immediate:
                    case CPUOpcodes.LDX_ZeroPage:
                    case CPUOpcodes.LDX_ZeroPage_Y:
                    case CPUOpcodes.LDX_Absolute:
                    case CPUOpcodes.LDX_Absolute_Y:
                        LDX(opcode.mode);
                        break;

                    // LDY
                    case CPUOpcodes.LDY_Immediate:
                    case CPUOpcodes.LDY_ZeroPage:
                    case CPUOpcodes.LDY_ZeroPage_X:
                    case CPUOpcodes.LDY_Absolute:
                    case CPUOpcodes.LDY_Absolute_X:
                        LDY(opcode.mode);
                        break;

                    // LSR
                    case CPUOpcodes.LSR_Accumulator:
                        LSR();
                        break;

                    case CPUOpcodes.LSR_ZeroPage:
                    case CPUOpcodes.LSR_ZeroPage_X:
                    case CPUOpcodes.LSR_Absolute:
                    case CPUOpcodes.LSR_Absolute_X:
                        LSR(opcode.mode);
                        break;

                    case CPUOpcodes.NOP:
                        break;

                    case CPUOpcodes.NOP_Unofficial_F4:
                    case CPUOpcodes.NOP_Unofficial_D4:
                    case CPUOpcodes.NOP_Unofficial_5C:
                    case CPUOpcodes.NOP_Unofficial_7C:
                    case CPUOpcodes.NOP_Unofficial_DC:
                    case CPUOpcodes.NOP_Unofficial_FC:
                        NOP(opcode.mode);
                        break;

                    // ORA
                    case CPUOpcodes.ORA_Immediate:
                    case CPUOpcodes.ORA_ZeroPage:
                    case CPUOpcodes.ORA_ZeroPage_X:
                    case CPUOpcodes.ORA_Absolute:
                    case CPUOpcodes.ORA_Absolute_X:
                    case CPUOpcodes.ORA_Absolute_Y:
                    case CPUOpcodes.ORA_Indirect_X:
                    case CPUOpcodes.ORA_Indirect_Y:
                        ORA(opcode.mode);
                        break;

                    case CPUOpcodes.PHA:
                        PHA();
                        break;

                    case CPUOpcodes.PHY:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.PHX:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.PHP:
                        PHP();
                        break;

                    case CPUOpcodes.PLA:
                        PLA();
                        break;

                    case CPUOpcodes.PLY:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.PLX:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.PLP:
                        PLP();
                        break;

                    // ROL
                    case CPUOpcodes.ROL_Accumulator:
                        ROL();
                        break;

                    case CPUOpcodes.ROL_ZeroPage:
                    case CPUOpcodes.ROL_ZeroPage_X:
                    case CPUOpcodes.ROL_Absolute:
                    case CPUOpcodes.ROL_Absolute_X:
                        ROL(opcode.mode);
                        break;

                    // ROR
                    case CPUOpcodes.ROR_Accumulator:
                        ROR();
                        break;

                    case CPUOpcodes.ROR_ZeroPage:
                    case CPUOpcodes.ROR_ZeroPage_X:
                    case CPUOpcodes.ROR_Absolute:
                    case CPUOpcodes.ROR_Absolute_X:
                        ROR(opcode.mode);
                        break;

                    case CPUOpcodes.RTI:
                        RTI();
                        break;

                    case CPUOpcodes.RTS:
                        RTS();
                        break;

                    // SBC
                    case CPUOpcodes.SBC_Immediate:
                    case CPUOpcodes.SBC_ZeroPage:
                    case CPUOpcodes.SBC_ZeroPage_X:
                    case CPUOpcodes.SBC_Absolute:
                    case CPUOpcodes.SBC_Absolute_X:
                    case CPUOpcodes.SBC_Absolute_Y:
                    case CPUOpcodes.SBC_Indirect_X:
                    case CPUOpcodes.SBC_Indirect_Y:
                        SBC(opcode.mode);
                        break;

                    case CPUOpcodes.SEC:
                        SEC();
                        break;

                    case CPUOpcodes.SED:
                        SED();
                        break;

                    case CPUOpcodes.SEI:
                        SEI();
                        break;

                    // STA
                    case CPUOpcodes.STA_ZeroPage:
                    case CPUOpcodes.STA_ZeroPage_X:
                    case CPUOpcodes.STA_Absolute:
                    case CPUOpcodes.STA_Absolute_X:
                    case CPUOpcodes.STA_Absolute_Y:
                    case CPUOpcodes.STA_Indirect_X:
                    case CPUOpcodes.STA_Indirect_Y:
                        STA(opcode.mode);
                        break;

                    // STX
                    case CPUOpcodes.STX_ZeroPage:
                    case CPUOpcodes.STX_ZeroPage_Y:
                    case CPUOpcodes.STX_Absolute:
                        STX(opcode.mode);
                        break;

                    // STY
                    case CPUOpcodes.STY_ZeroPage:
                    case CPUOpcodes.STY_ZeroPage_X:
                    case CPUOpcodes.STY_Absolute:
                        STY(opcode.mode);
                        break;

                    // STZ
                    case CPUOpcodes.STZ_ZeroPage_X:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.STZ_ZeroPage:
                    case CPUOpcodes.STZ_Absolute:
                    case CPUOpcodes.STZ_Absolute_X:
                        STZ(opcode.mode);
                        break;

                    case CPUOpcodes.TAX:
                        TAX();
                        break;

                    case CPUOpcodes.TAY:
                        TAY();
                        break;

                    case CPUOpcodes.TSX:
                        TSX();
                        break;

                    case CPUOpcodes.TXA:
                        TXA();
                        break;

                    case CPUOpcodes.TXS:
                        TXS();
                        break;

                    case CPUOpcodes.TYA:
                        TYA();
                        break;

                    case CPUOpcodes.TRB_ZeroPage:
                        NOP(opcode.mode);
                        break;
                    case CPUOpcodes.TRB_Absolute:
                        NOP(opcode.mode);
                        break;

                    case CPUOpcodes.TSB_ZeroPage:
                    case CPUOpcodes.TSB_Absolute:
                        TSB(opcode.mode);
                        break;

                    default:
                        throw new Exception($"Invalid instruction: {opcode.opcode:X2}({opcode.mnemonic})!");
                }
            }
        }

        private void NOP(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr); // The value is discarded and the CPU state is not affected.
            return;
        }

        private void ADC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            addToRegisterA(value);
        }

        private void AND(CPUAddressingMode mode)
        {
            byte value = getValueByAddressingMode(mode);
            byte result = (byte)(value & register_acc);
            setRegisterAcc(result);
        }

        private void ASL()
        {
            byte value = register_acc;

            bool is7thBitSet = (value >> 7) == 1;
            if (is7thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte result = (byte)(value << 1);
            setRegisterAcc(result);
        }

        private void ASL(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte old_value = _memory.read(addr);

            bool is7thBitSet = (old_value >> 7) == 1;
            if (is7thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte result = (byte)(old_value << 1);
            _memory.write(addr, result);
            updateZeroAndNegativeFlags(result);
        }

        private void BCC()
        {
            branch((status & CPUStatus.Carry) == 0);
        }

        private void BCS()
        {
            branch((status & CPUStatus.Carry) != 0);
        }

        private void BEQ()
        {
            branch((status & CPUStatus.Zero) != 0);
        }

        private void BIT(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte data = _memory.read(addr);

            if (mode != CPUAddressingMode.ZeroPage && mode != CPUAddressingMode.Absolute)
            {
                return; // NOP instruction for other modes
            }

            byte result = (byte)(register_acc & data);

            if (result == 0x00)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                removeStatus(CPUStatus.Zero);
            }

            if ((CPUStatus.Negative & data) > 0)
            {
                setStatus(CPUStatus.Negative);
            }
            else
            {
                removeStatus(CPUStatus.Negative);
            }

            if ((CPUStatus.Overflow & data) > 0)
            {
                setStatus(CPUStatus.Overflow);
            }
            else
            {
                removeStatus(CPUStatus.Overflow);
            }
        }

        private void BMI()
        {
            branch((status & CPUStatus.Negative) != 0);
        }

        private void BNE()
        {
            branch((status & CPUStatus.Zero) == 0);
        }

        private void BPL()
        {
            branch((status & CPUStatus.Negative) == 0);
        }

        private void BRA()
        {
            branchByWord(true);
        }

        private void BRK()
        {
            stackPush(status);
            pushUshortToStack(program_counter);
            setBreakFlag();
        }

        private void BVC()
        {
            branch((status & CPUStatus.Overflow) == 0);
        }

        private void BVS()
        {
            branch((status & CPUStatus.Overflow) != 0);
        }

        private void CLC()
        {
            removeStatus(CPUStatus.Carry);
        }

        private void CLD()
        {
            removeStatus(CPUStatus.Decimal);
        }

        private void CLI()
        {
            removeStatus(CPUStatus.Interrupt);
        }

        private void CLV()
        {
            removeStatus(CPUStatus.Overflow);
        }

        private void CMP(CPUAddressingMode mode)
        {
            compare(mode, register_acc);
        }

        private void CPX(CPUAddressingMode mode)
        {
            compare(mode, register_x);
        }

        private void CPY(CPUAddressingMode mode)
        {
            compare(mode, register_y);
        }

        private void compare(CPUAddressingMode mode, byte reg)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            byte result = (byte)(reg - value);

            if (value <= reg)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            updateZeroAndNegativeFlags(result);
        }

        private void DEC()
        {
            byte decValue = (byte)(register_acc - 1);
            setRegisterAcc(decValue);
        }

        private void DEC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte decValue = (byte)(_memory.read(addr) - 1);
            _memory.write(addr, decValue);
            updateZeroAndNegativeFlags(decValue);
        }

        private void DEX()
        {
            register_x = (byte)(register_x - 1);
            updateZeroAndNegativeFlags(register_x);
        }

        private void DEY()
        {
            register_y = (byte)(register_y - 1);
            updateZeroAndNegativeFlags(register_y);
        }

        private void EOR(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);

            register_acc = (byte)(register_acc ^ value);
            updateZeroAndNegativeFlags(register_acc);
        }

        private void INC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte incValue = (byte)(_memory.read(addr) + 1);
            _memory.write(addr, incValue);
            updateZeroAndNegativeFlags(incValue);
        }

        private void INX()
        {
            register_x = (byte)(register_x + 1);
            updateZeroAndNegativeFlags(register_x);
        }

        private void INY()
        {
            register_y = (byte)(register_y + 1);
            updateZeroAndNegativeFlags(register_y);
        }

        private void JMP(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            program_counter = addr;
        }

        /// <summary>
        /// The address (16 bits) of the last byte of the JSR (that is, the next instruction minus 1) is pushed onto the stack
        /// The program counter jumps to the subroutine indicated.
        ///</summary>
        private void JSR()
        {
            var tempPC = program_counter;
            var subAddr = _memory.readU16(ref tempPC);
            pushUshortToStack((ushort)(tempPC - 1));
            program_counter = subAddr;
        }


        /// <summary>
        /// An address (16 bits) is popped off the stack.
        /// The program counter jumps to this address + 1
        ///</summary>
        private void RTS()
        {
            program_counter = (ushort)(popUshortFromStack() + 1);
        }

        private void LDA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            setRegisterAcc(value);
        }

        private void LAX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            setRegisterAcc(value);
            register_x = value;
            updateZeroAndNegativeFlags(register_x);
        }

        private void LDX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            register_x = value;
            updateZeroAndNegativeFlags(register_x);
        }

        private void LDY(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            register_y = value;
            updateZeroAndNegativeFlags(register_y);
        }

        private void LSR()
        {
            byte old_value = register_acc;

            if ((old_value & CPUStatus.Carry) == 1)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            setRegisterAcc((byte)(register_acc >> 1));
            //updateZeroAndNegativeFlags(register_acc);
        }

        private void LSR(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte old_value = _memory.read(addr);

            if ((old_value & CPUStatus.Carry) == 1)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte rightShiftedValue = (byte)(old_value >> 1);
            _memory.write(addr, rightShiftedValue);
            updateZeroAndNegativeFlags(rightShiftedValue);
        }

        private void ASR()
        {
            ushort addr = getAddressByMode(CPUAddressingMode.ZeroPage);
            byte value = _memory.read(addr); // The value is discarded and the CPU state is not affected.
            return;
        }


        private void ORA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            setRegisterAcc((byte)(register_acc | value));
        }

        private void PHA()
        {
            stackPush(register_acc);
        }

        private void PHY()
        {
            stackPush(register_y);
        }

        private void PHX()
        {
            stackPush(register_x);
        }

        private void PHP()
        {
            stackPush((byte)(status | CPUStatus.Break));
        }

        private void PLA()
        {
            setRegisterAcc(stackPop());
        }

        private void PLP()
        {
            status = (byte)((stackPop() & ~CPUStatus.Break) | CPUStatus.Reserved);
        }

        private void ROL()
        {
            register_acc = rotateOneBitLeft(register_acc);
        }

        private void ROL(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            byte rotated = rotateOneBitLeft(value);
            _memory.write(addr, rotated);
        }

        private byte rotateOneBitLeft(byte Value)
        {
            byte shiftedValue = (byte)(Value << 1);
            byte result = (byte)(shiftedValue | (status & CPUStatus.Carry));
            setStatus((byte)(CPUStatus.Carry & (Value >> 7)));
            updateZeroAndNegativeFlags(result);
            return result;
        }

        private void ROR()
        {
            register_acc = rotateOneBitRight(register_acc);
        }

        private void ROR(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            byte rotated = rotateOneBitRight(value);
            _memory.write(addr, rotated);
        }

        private byte rotateOneBitRight(byte Value)
        {
            byte shiftedValue = (byte)(Value >> 1);
            byte result = (byte)(shiftedValue | ((status & CPUStatus.Carry) << 7));
            setStatus((byte)(CPUStatus.Carry & Value));
            updateZeroAndNegativeFlags(result);

            return result;
        }

        private void RTI()
        {
            status = (byte)(stackPop() | CPUStatus.Reserved); ;
            program_counter = popUshortFromStack();
        }

        private void SBC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            addToRegisterA((byte)(~value));
        }

        private void SEC()
        {
            setStatus(CPUStatus.Carry);
        }

        private void SED()
        {
            setStatus(CPUStatus.Decimal);
        }

        private void SEI()
        {
            setStatus(CPUStatus.Interrupt);
        }

        private void STA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, register_acc);
        }

        private void STX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, register_x);
        }

        private void STY(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, register_y);
        }

        private void STZ(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, (byte)(status & CPUStatus.Zero));
        }

        private void TAX()
        {
            register_x = register_acc;
            updateZeroAndNegativeFlags(register_x);
        }

        private void TAY()
        {
            register_y = register_acc;
            updateZeroAndNegativeFlags(register_y);
        }

        private void TSX()
        {
            register_x = stack_pointer;
            updateZeroAndNegativeFlags(register_x);
        }

        private void TXA()
        {
            setRegisterAcc(register_x);
        }

        private void TXS()
        {
            stack_pointer = register_x;
        }

        private void TYA()
        {
            setRegisterAcc(register_y);
        }



        private void TRB(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);

            byte testResult = (byte)(~register_acc & value);
            _memory.write(addr, testResult);

            if (testResult == 0)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                removeStatus(CPUStatus.Zero);
            }
        }

        private void TSB(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr); // The value is discarded and the CPU state is not affected.
            return;
        }


        private void setRegisterAcc(byte Value)
        {
            register_acc = Value;
            updateZeroAndNegativeFlags(register_acc);
        }


        private void addToRegisterA(byte data)
        {
            int sum = (sbyte)register_acc + (sbyte)data + (sbyte)(status & CPUStatus.Carry);
            bool overflow = sum < -128 || sum > 127;
            bool carry = (register_acc + data + (((status & CPUStatus.Carry) != 0) ? 1 : 0)) > 0xFF;

            if (carry)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            if (overflow)
            {
                setStatus(CPUStatus.Overflow);
            }
            else
            {
                removeStatus(CPUStatus.Overflow);
            }

            byte result = (byte)(sum & 0xFF);
            setRegisterAcc(result);
        }

        private ushort getAddressByMode(CPUAddressingMode mode)
        {
            byte pos;
            byte ptr, lo, hi;
            ushort addr;
            ushort addr_base;

            switch (mode)
            {
                case CPUAddressingMode.Implied:
                    addr = program_counter;
                    return addr;

                case CPUAddressingMode.Immediate:
                case CPUAddressingMode.Relative:
                    addr = program_counter;
                    program_counter++;
                    return addr;

                case CPUAddressingMode.ZeroPage:
                    addr = _memory.read(program_counter);
                    program_counter++;
                    return addr;

                case CPUAddressingMode.ZeroPage_X:
                    pos = _memory.read(program_counter);
                    program_counter++;
                    addr = (byte)(pos + register_x);
                    return addr;

                case CPUAddressingMode.ZeroPage_Y:
                    pos = _memory.read(program_counter);
                    program_counter++;
                    addr = (byte)(pos + register_y);
                    return addr;

                case CPUAddressingMode.Absolute:
                    addr = _memory.readU16(program_counter);
                    program_counter += 2;
                    return addr;

                case CPUAddressingMode.Absolute_X:
                    addr_base = _memory.readU16(program_counter);
                    program_counter += 2;
                    addr = (ushort)(addr_base + register_x);
                    return addr;

                case CPUAddressingMode.Absolute_Y:
                    addr_base = _memory.readU16(program_counter);
                    program_counter += 2;
                    addr = (ushort)(addr_base + register_y);
                    return addr;

                case CPUAddressingMode.Indirect:
                    addr_base = _memory.readU16(program_counter);
                    program_counter += 2;

                    // 6502 bug: if the low byte of the address is 0xFF, the high byte is fetched from the beginning of the page instead of the next page
                    if ((addr_base & 0x00FF) == 0x00FF)
                    {
                        lo = _memory.read(addr_base);
                        hi = _memory.read((ushort)(addr_base & 0xFF00));
                        addr = (ushort)((hi << 8) | lo);
                    }
                    else
                    {
                        addr = _memory.readU16(addr_base);
                    }

                    return addr;

                case CPUAddressingMode.Indirect_X:
                    addr_base = _memory.read(program_counter);
                    program_counter++;
                    ptr = (byte)(addr_base + register_x);
                    lo = _memory.read(ptr);
                    hi = _memory.read((byte)(ptr + 1));
                    addr = (ushort)(hi << 8 | lo);
                    return addr;

                case CPUAddressingMode.Indirect_Y:
                    addr_base = _memory.read(program_counter);
                    program_counter++;
                    lo = _memory.read(addr_base);
                    hi = _memory.read((byte)(addr_base + 1));
                    ushort deref_base = (ushort)((hi << 8) | lo);

                    // TODO: check if the deref address has a page break, if so, add 1 cycle to the  CPU instruction 
                    ushort deref = (ushort)(deref_base + register_y);

                    return deref;

                default:
                    throw new Exception($"Invalid addressing mode: {mode}!");
            }
        }

        private byte getValueByAddressingMode(CPUAddressingMode mode)
        {
            switch (mode)
            {
                case CPUAddressingMode.Immediate:
                    byte value = _memory.read(program_counter);
                    program_counter++;
                    return value;

                case CPUAddressingMode.Relative:
                case CPUAddressingMode.ZeroPage:
                case CPUAddressingMode.Absolute:
                case CPUAddressingMode.ZeroPage_X:
                case CPUAddressingMode.ZeroPage_Y:
                case CPUAddressingMode.Absolute_X:
                case CPUAddressingMode.Absolute_Y:
                case CPUAddressingMode.Indirect_X:
                case CPUAddressingMode.Indirect_Y:
                    var addr = getAddressByMode(mode);
                    return _memory.read(addr);

                default:
                    throw new Exception($"Invalid addressing mode: {mode}!");
            }
        }



        public byte setStatus(in byte Status)
        {
            status |= Status;
            return status;
        }

        public void setBreakFlag()
        {
            status |= CPUStatus.Break;
        }

        public byte removeStatus(in byte Status)
        {
            status = (byte)(status & (~Status));
            return status;
        }

        private void updateZeroAndNegativeFlags(in byte Result)
        {
            if (Result == 0)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                removeStatus(CPUStatus.Zero);
            }

            if ((Result & CPUStatus.Negative) != 0)
            {
                setStatus(CPUStatus.Negative);
            }
            else
            {
                removeStatus(CPUStatus.Negative);
            }
        }

        public void reset()
        {
            register_acc = 0;
            register_x = 0;
            register_y = 0;
            status = CPUStatus.Initial;
            stack_pointer = STACK_RESET;
            program_counter = _memory.readU16(0xFFFC);
        }

        public void load(byte[] Program)
        {
            _memory.load(Program);

            // Default PC start
            // _memory.writeU16(0xFFFC, 0x8000);

            // Sneak game
            //_memory.writeU16(0xFFFC, 0x0600);

            // NES Test
            _memory.writeU16(0xFFFC, 0xC000);
        }

        public void branch(bool condition)
        {
            ushort addr = getAddressByMode(CPUAddressingMode.Relative);

            if (condition)
            {
                sbyte displacement = (sbyte)_memory.read(addr);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        public void branchByWord(bool condition)
        {
            if (condition)
            {
                var low = _memory.read(program_counter);
                var high = _memory.read((ushort)(program_counter + 1));

                ushort displacement = (ushort)((high << 8) | low);
                program_counter = (ushort)(program_counter + displacement + 2);
            }
        }

        public void loadAndRun(byte[] Program, OnUpdateCallBack callback)
        {
            load(Program);
            reset();
            run(callback);
        }

        public void loadAndRun(byte[] Program)
        {
            load(Program);
            reset();
            run();
        }

        public byte stackPop()
        {
            stack_pointer = (byte)(stack_pointer + 1);
            return _memory.read((ushort)(STACK_START_ADDR + stack_pointer));
        }

        public void stackPush(byte Data)
        {
            _memory.write((ushort)(STACK_START_ADDR + stack_pointer), Data);
            stack_pointer = (byte)(stack_pointer - 1);
        }

        public void pushUshortToStack(ushort data)
        {
            byte hi = (byte)(data >> 8);
            byte lo = (byte)(data & 0xff);
            stackPush(hi);
            stackPush(lo);
        }

        public ushort popUshortFromStack()
        {
            byte lo = stackPop();
            byte hi = stackPop();
            return (ushort)((hi << 8) | (lo));
        }
    }

}
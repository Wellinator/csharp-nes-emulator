
namespace NES_Emulator
{
    public delegate void OnUpdateCallBack();


    public interface iCPU
    {
        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public byte stack_pointer { get; set; }
        public ushort program_counter { get; set; }
        public long Cycles { get; set; }
        public long ExtraCycles { get; set; }
        public CPUInstructionTable instruction_table { get; set; }
        public long Step(OnUpdateCallBack? callback);
        public byte setStatus(in byte Status);
        public void Reset();
    }


    public class CPU : iCPU
    {
        private IBusDevice _bus { get; set; }
        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte register_y { get; set; }
        public byte status { get; set; }
        public byte stack_pointer { get; set; }
        public ushort program_counter { get; set; }
        public long Cycles { get; set; }
        public long ExtraCycles { get; set; }
        public CPUInstructionTable instruction_table { get; set; }

        private const ushort STACK_START_ADDR = 0x0100;
        private const byte STACK_RESET = 0xFD;
        private const ushort PC_AT_POWER = 0xFFFC;


        public CPU(IBusDevice Bus)
        {

            _bus = Bus;

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
            register_acc = 0;
            register_x = 0;
            status = CPUStatus.Initial;
            program_counter = PC_AT_POWER;
            ExtraCycles = 0;
            instruction_table = new CPUInstructionTable();
            stack_pointer = STACK_RESET;
        }


        public long Step(OnUpdateCallBack? callback = null)
        {
            if (callback != null) callback();

            ExtraCycles = 0;

            // DEBUG content
            ushort pc_for_log = program_counter;

            ICPUInstruction instruction = GetNextInstruction();

            // DEBUG LOG CONTENT INIT
            List<string> data = new List<string>();
            if (instruction.bytes > 1)
            {
                for (ushort i = 1; i < instruction.bytes; i++)
                {
                    data.Add(_bus.Read((ushort)(pc_for_log + i)).ToString("X2"));
                }
            }

            var opcodePlusData = $"{instruction.opcode:X2} {String.Join(' ', data.ToArray())}".PadRight(10);
            var mnemonicPlusAddr = $"{instruction.mnemonic} ${String.Join("", data.ToArray().Reverse())}".PadRight(32);
            Console.WriteLine($"{pc_for_log:X4}  {opcodePlusData} {mnemonicPlusAddr} A:{register_acc:X2} X:{register_x:X2} Y:{register_y:X2} P:{status:X2} SP:{stack_pointer:X2} CYC: {Cycles}");
            // DEBUG LOG CONTENT END

            // Run the instruction
            ExecuteInstruction(instruction);

            // Cycle control
            long totalCycles = instruction.BaseCycles + ExtraCycles;
            Cycles += totalCycles;

            return totalCycles;
        }

        private void ExecuteInstruction(ICPUInstruction instruction)
        {
            switch (instruction.opcode)
            {
                // BRK
                case CPUOpcodes.BRK:
                    BRK();
                    break;

                // ADC
                case CPUOpcodes.ADC_Immediate:
                case CPUOpcodes.ADC_ZeroPage:
                case CPUOpcodes.ADC_ZeroPage_X:
                case CPUOpcodes.ADC_Absolute:
                case CPUOpcodes.ADC_Absolute_X:
                case CPUOpcodes.ADC_Absolute_Y:
                case CPUOpcodes.ADC_Indirect_X:
                case CPUOpcodes.ADC_Indirect_Y:
                    ADC(instruction.mode);
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
                    AND(instruction.mode);
                    break;

                // ASL
                case CPUOpcodes.ASL_Accumulator:
                    ASL();
                    break;

                case CPUOpcodes.ASL_ZeroPage:
                case CPUOpcodes.ASL_ZeroPage_X:
                case CPUOpcodes.ASL_Absolute:
                case CPUOpcodes.ASL_Absolute_X:
                    ASL(instruction.mode);
                    break;

                // SLO
                case CPUOpcodes.SLO_ZeroPage:
                case CPUOpcodes.SLO_ZeroPage_X:
                case CPUOpcodes.SLO_Absolute:
                case CPUOpcodes.SLO_Absolute_X:
                case CPUOpcodes.SLO_Absolute_Y:
                case CPUOpcodes.SLO_Indirect_X:
                case CPUOpcodes.SLO_Indirect_Y:
                    SLO(instruction.mode);
                    break;

                // RLA
                case CPUOpcodes.RLA_ZeroPage:
                case CPUOpcodes.RLA_ZeroPage_X:
                case CPUOpcodes.RLA_Absolute:
                case CPUOpcodes.RLA_Absolute_X:
                case CPUOpcodes.RLA_Absolute_Y:
                case CPUOpcodes.RLA_Indirect_X:
                case CPUOpcodes.RLA_Indirect_Y:
                    RLA(instruction.mode);
                    break;

                // RRA
                case CPUOpcodes.RRA_ZeroPage:
                case CPUOpcodes.RRA_ZeroPage_X:
                case CPUOpcodes.RRA_Absolute:
                case CPUOpcodes.RRA_Absolute_X:
                case CPUOpcodes.RRA_Absolute_Y:
                case CPUOpcodes.RRA_Indirect_X:
                case CPUOpcodes.RRA_Indirect_Y:
                    RRA(instruction.mode);
                    break;

                // SRE
                case CPUOpcodes.SRE_ZeroPage:
                case CPUOpcodes.SRE_ZeroPage_X:
                case CPUOpcodes.SRE_Absolute:
                case CPUOpcodes.SRE_Absolute_X:
                case CPUOpcodes.SRE_Absolute_Y:
                case CPUOpcodes.SRE_Indirect_X:
                case CPUOpcodes.SRE_Indirect_Y:
                    SRE(instruction.mode);
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
                case CPUOpcodes.NOP_Unofficial_3C:
                case CPUOpcodes.BIT_ZeroPage:
                case CPUOpcodes.NOP_Unofficial_34:
                    BIT(instruction.mode);
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
                    NOP(instruction.mode);
                    break;
                case CPUOpcodes.SAX_Indirect_X:
                case CPUOpcodes.SAX_ZeroPage:
                case CPUOpcodes.SAX_Absolute:
                case CPUOpcodes.SAX_ZeroPage_Y:
                    SAX(instruction.mode);
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
                    CMP(instruction.mode);
                    break;

                // CPX
                case CPUOpcodes.CPX_Immediate:
                case CPUOpcodes.CPX_ZeroPage:
                case CPUOpcodes.CPX_Absolute:
                    CPX(instruction.mode);
                    break;

                // CPY
                case CPUOpcodes.CPY_Immediate:
                case CPUOpcodes.CPY_ZeroPage:
                case CPUOpcodes.CPY_Absolute:
                    CPY(instruction.mode);
                    break;

                // DEC
                case CPUOpcodes.NOP_Unofficial_3A:
                    NOP(instruction.mode);
                    break;
                case CPUOpcodes.DEC_ZeroPage:
                case CPUOpcodes.DEC_ZeroPage_X:
                case CPUOpcodes.DEC_Absolute:
                case CPUOpcodes.DEC_Absolute_X:
                    DEC(instruction.mode);
                    break;

                // DCP
                case CPUOpcodes.DCP_ZeroPage:
                case CPUOpcodes.DCP_ZeroPage_X:
                case CPUOpcodes.DCP_Absolute:
                case CPUOpcodes.DCP_Absolute_X:
                case CPUOpcodes.DCP_Absolute_Y:
                case CPUOpcodes.DCP_Indirect_X:
                case CPUOpcodes.DCP_Indirect_Y:
                    DCP(instruction.mode);
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
                    EOR(instruction.mode);
                    break;

                // INC
                case CPUOpcodes.NOP_Unofficial_1A:
                    NOP(instruction.mode);
                    break;
                case CPUOpcodes.INC_ZeroPage:
                case CPUOpcodes.INC_ZeroPage_X:
                case CPUOpcodes.INC_Absolute:
                case CPUOpcodes.INC_Absolute_X:
                    INC(instruction.mode);
                    break;

                // ISC
                case CPUOpcodes.ISC_ZeroPage:
                case CPUOpcodes.ISC_ZeroPage_X:
                case CPUOpcodes.ISC_Absolute:
                case CPUOpcodes.ISC_Absolute_X:
                case CPUOpcodes.ISC_Absolute_Y:
                case CPUOpcodes.ISC_Indirect_X:
                case CPUOpcodes.ISC_Indirect_Y:
                    ISC(instruction.mode);
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
                    JMP(instruction.mode);
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
                    LDA(instruction.mode);
                    break;

                // LAX
                case CPUOpcodes.LAX_ZeroPage:
                case CPUOpcodes.LAX_ZeroPage_Y:
                case CPUOpcodes.LAX_Absolute:
                case CPUOpcodes.LAX_Absolute_Y:
                case CPUOpcodes.LAX_Indirect_X:
                case CPUOpcodes.LAX_Indirect_Y:
                    LAX(instruction.mode);
                    break;

                // LDX
                case CPUOpcodes.LDX_Immediate:
                case CPUOpcodes.LDX_ZeroPage:
                case CPUOpcodes.LDX_ZeroPage_Y:
                case CPUOpcodes.LDX_Absolute:
                case CPUOpcodes.LDX_Absolute_Y:
                    LDX(instruction.mode);
                    break;

                // LDY
                case CPUOpcodes.LDY_Immediate:
                case CPUOpcodes.LDY_ZeroPage:
                case CPUOpcodes.LDY_ZeroPage_X:
                case CPUOpcodes.LDY_Absolute:
                case CPUOpcodes.LDY_Absolute_X:
                    LDY(instruction.mode);
                    break;

                // LSR
                case CPUOpcodes.LSR_Accumulator:
                    LSR();
                    break;

                case CPUOpcodes.LSR_ZeroPage:
                case CPUOpcodes.LSR_ZeroPage_X:
                case CPUOpcodes.LSR_Absolute:
                case CPUOpcodes.LSR_Absolute_X:
                    LSR(instruction.mode);
                    break;

                case CPUOpcodes.NOP:
                    break;

                case CPUOpcodes.NOP_Unofficial_04:
                case CPUOpcodes.NOP_Unofficial_44:
                case CPUOpcodes.NOP_Unofficial_54:
                case CPUOpcodes.NOP_Unofficial_F4:
                case CPUOpcodes.NOP_Unofficial_D4:
                case CPUOpcodes.NOP_Unofficial_5C:
                case CPUOpcodes.NOP_Unofficial_7C:
                case CPUOpcodes.NOP_Unofficial_DC:
                case CPUOpcodes.NOP_Unofficial_FC:
                    NOP(instruction.mode);
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
                    ORA(instruction.mode);
                    break;

                case CPUOpcodes.PHA:
                    PHA();
                    break;

                case CPUOpcodes.NOP_Unofficial_5A:
                    NOP(instruction.mode);
                    break;

                case CPUOpcodes.NOP_Unofficial_DA:
                    NOP(instruction.mode);
                    break;

                case CPUOpcodes.PHP:
                    PHP();
                    break;

                case CPUOpcodes.PLA:
                    PLA();
                    break;

                case CPUOpcodes.NOP_Unofficial_7A:
                    NOP(instruction.mode);
                    break;

                case CPUOpcodes.NOP_Unofficial_FA:
                    NOP(instruction.mode);
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
                    ROL(instruction.mode);
                    break;

                // ROR
                case CPUOpcodes.ROR_Accumulator:
                    ROR();
                    break;

                case CPUOpcodes.ROR_ZeroPage:
                case CPUOpcodes.ROR_ZeroPage_X:
                case CPUOpcodes.ROR_Absolute:
                case CPUOpcodes.ROR_Absolute_X:
                    ROR(instruction.mode);
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
                // USBC - Unofficial Opcode
                case CPUOpcodes.USBC_Immediate:
                    SBC(instruction.mode);
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
                    STA(instruction.mode);
                    break;

                // STX
                case CPUOpcodes.STX_ZeroPage:
                case CPUOpcodes.STX_ZeroPage_Y:
                case CPUOpcodes.STX_Absolute:
                    STX(instruction.mode);
                    break;

                // STY
                case CPUOpcodes.STY_ZeroPage:
                case CPUOpcodes.STY_ZeroPage_X:
                case CPUOpcodes.STY_Absolute:
                    STY(instruction.mode);
                    break;

                // STZ
                case CPUOpcodes.STZ_ZeroPage_X:
                    NOP(instruction.mode);
                    break;

                case CPUOpcodes.STZ_ZeroPage:
                case CPUOpcodes.STZ_Absolute:
                case CPUOpcodes.STZ_Absolute_X:
                    STZ(instruction.mode);
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

                case CPUOpcodes.NOP_Unofficial_14:
                    NOP(instruction.mode);
                    break;
                case CPUOpcodes.NOP_Unofficial_1C:
                    NOP(instruction.mode);
                    break;

                case CPUOpcodes.NOP_Unofficial_0C:
                    NOP(instruction.mode);
                    break;

                default:
                    throw new Exception($"Invalid instruction: {instruction.opcode:X2}({instruction.mnemonic})!");
            }
        }

        private ICPUInstruction GetNextInstruction()
        {
            byte instruction = _bus.Read(program_counter);
            program_counter++;
            CPUInstruction opcode = instruction_table.GetInstruction(instruction);
            return opcode;
        }

        // SRE - Shift Right and XOR with Accumulator (Unofficial Opcode)
        private void SRE(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);

            // LSR
            if ((value & CPUStatus.Carry) == 1)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte rightShiftedValue = (byte)(value >> 1);
            _bus.Write(addr, rightShiftedValue);

            // EOR
            byte xoredValue = (byte)(register_acc ^ rightShiftedValue);
            setRegisterAcc(xoredValue);
        }

        // SLO - Shift Left and OR with Accumulator
        private void SLO(CPUAddressingMode mode)
        {

            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);

            // ASL
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
            _bus.Write(addr, result);

            // ORA
            setRegisterAcc((byte)(register_acc | result));
        }

        // RLA - Rotate Left and AND with Accumulator
        private void RLA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);

            // ROL
            byte rotatedValue = rotateOneBitLeft(value);
            _bus.Write(addr, rotatedValue);

            // AND
            setRegisterAcc((byte)(register_acc & rotatedValue));
        }

        // RRA - Rotate Right and ADD to Accumulator
        private void RRA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);

            // ROR
            byte rotatedValue = rotateOneBitRight(value);
            _bus.Write(addr, rotatedValue);

            // ADC
            addToRegisterA(rotatedValue);
        }

        private void DCP(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);
            byte decrementedValue = (byte)(value - 1);
            _bus.Write(addr, decrementedValue);
            CompareByValue(decrementedValue, register_acc);
        }

        private void NOP(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr); // The value is discarded and the CPU state is not affected.
            return;
        }

        private void ADC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
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
            byte old_value = _bus.Read(addr);

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
            _bus.Write(addr, result);
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
            byte data = _bus.Read(addr);

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

        private void SAX(CPUAddressingMode mode)
        {
            ushort value = (ushort)(register_acc & register_x);
            ushort addr = getAddressByMode(mode);
            _bus.Write(addr, (byte)value);
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
            CompareByAddress(mode, register_acc);
        }

        private void CPX(CPUAddressingMode mode)
        {
            CompareByAddress(mode, register_x);
        }

        private void CPY(CPUAddressingMode mode)
        {
            CompareByAddress(mode, register_y);
        }

        private void CompareByAddress(CPUAddressingMode mode, byte reg)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
            CompareByValue(value, reg);
        }

        private void CompareByValue(byte value, byte reg)
        {
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
            byte decValue = (byte)(_bus.Read(addr) - 1);
            _bus.Write(addr, decValue);
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
            byte value = _bus.Read(addr);

            register_acc = (byte)(register_acc ^ value);
            updateZeroAndNegativeFlags(register_acc);
        }

        private void INC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte incValue = (byte)(_bus.Read(addr) + 1);
            _bus.Write(addr, incValue);
            updateZeroAndNegativeFlags(incValue);
        }

        private void ISC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode, MemoryAccessType.ReadModifyWrite);
            byte value = _bus.Read(addr);
            byte incrementedValue = (byte)(value + 1);
            _bus.Write(addr, incrementedValue);
            addToRegisterA((byte)~incrementedValue);
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
            ushort tempPC = program_counter;
            byte lo = _bus.Read(tempPC);
            tempPC++;
            byte hi = _bus.Read(tempPC);
            tempPC++;

            ushort subAddr = (ushort)((hi << 8) | lo);

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
            byte value = _bus.Read(addr);
            setRegisterAcc(value);
        }

        private void LAX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
            setRegisterAcc(value);
            register_x = value;
            updateZeroAndNegativeFlags(register_x);
        }

        private void LDX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
            register_x = value;
            updateZeroAndNegativeFlags(register_x);
        }

        private void LDY(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
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
            byte old_value = _bus.Read(addr);

            if ((old_value & CPUStatus.Carry) == 1)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte rightShiftedValue = (byte)(old_value >> 1);
            _bus.Write(addr, rightShiftedValue);
            updateZeroAndNegativeFlags(rightShiftedValue);
        }


        private void ORA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _bus.Read(addr);
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
            byte value = _bus.Read(addr);
            byte rotated = rotateOneBitLeft(value);
            _bus.Write(addr, rotated);
        }

        private byte rotateOneBitLeft(byte Value)
        {
            byte shiftedValue = (byte)(Value << 1);
            byte result = (byte)(shiftedValue | (status & CPUStatus.Carry));

            bool is7thBitSet = (Value >> 7) == 1;
            if (is7thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

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
            byte value = _bus.Read(addr);
            byte rotated = rotateOneBitRight(value);
            _bus.Write(addr, rotated);
        }

        private byte rotateOneBitRight(byte Value)
        {
            byte shiftedValue = (byte)(Value >> 1);
            byte result = (byte)(shiftedValue | ((status & CPUStatus.Carry) << 7));

            bool is0thBitSet = (Value & 0x01) == 1;
            if (is0thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

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
            byte value = _bus.Read(addr);
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
            ushort addr = getAddressByMode(mode, MemoryAccessType.Write);
            _bus.Write(addr, register_acc);
        }

        private void STX(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _bus.Write(addr, register_x);
        }

        private void STY(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _bus.Write(addr, register_y);
        }

        private void STZ(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _bus.Write(addr, (byte)(status & CPUStatus.Zero));
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
            byte value = _bus.Read(addr);

            byte testResult = (byte)(~register_acc & value);
            _bus.Write(addr, testResult);

            if (testResult == 0)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                removeStatus(CPUStatus.Zero);
            }
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

        private bool isPageCrossed(ushort addr1, ushort addr2)
        {
            return (addr1 & 0xFF00) != (addr2 & 0xFF00);
        }

        private ushort getAddressByMode(CPUAddressingMode mode, MemoryAccessType accessType = MemoryAccessType.Read)
        {
            byte pos;
            byte ptr, lo, hi;
            ushort addr;
            ushort addr_base;
            bool pageCrossed;

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
                    addr = _bus.Read(program_counter);
                    program_counter++;
                    return addr;

                case CPUAddressingMode.ZeroPage_X:
                    pos = _bus.Read(program_counter);
                    program_counter++;
                    addr = (byte)(pos + register_x);
                    return addr;

                case CPUAddressingMode.ZeroPage_Y:
                    pos = _bus.Read(program_counter);
                    program_counter++;
                    addr = (byte)(pos + register_y);
                    return addr;

                case CPUAddressingMode.Absolute:
                    lo = _bus.Read(program_counter);
                    program_counter++;
                    hi = _bus.Read(program_counter);
                    program_counter++;

                    addr = (ushort)((hi << 8) | lo);
                    return addr;

                case CPUAddressingMode.Absolute_X:
                    lo = _bus.Read(program_counter);
                    program_counter++;
                    hi = _bus.Read(program_counter);
                    program_counter++;

                    addr_base = (ushort)((hi << 8) | lo);
                    addr = (ushort)(addr_base + register_x);

                    pageCrossed = isPageCrossed(addr_base, addr);
                    if (pageCrossed && accessType == MemoryAccessType.Read)
                    {
                        ExtraCycles += 1; // Add 1 cycle if a page boundary is crossed
                    }

                    return addr;

                case CPUAddressingMode.Absolute_Y:
                    lo = _bus.Read(program_counter);
                    program_counter++;
                    hi = _bus.Read(program_counter);
                    program_counter++;

                    addr_base = (ushort)((hi << 8) | lo);
                    addr = (ushort)(addr_base + register_y);

                    pageCrossed = isPageCrossed(addr_base, addr);
                    if (pageCrossed && accessType == MemoryAccessType.Read)
                    {
                        ExtraCycles += 1; // Add 1 cycle if a page boundary is crossed
                    }

                    return addr;

                case CPUAddressingMode.Indirect:
                    lo = _bus.Read(program_counter);
                    program_counter++;
                    hi = _bus.Read(program_counter);
                    program_counter++;

                    addr_base = (ushort)((hi << 8) | lo);

                    // 6502 bug: if the low byte of the address is 0xFF, the high byte is fetched from the beginning of the page instead of the next page
                    if ((addr_base & 0x00FF) == 0x00FF)
                    {
                        lo = _bus.Read(addr_base);
                        hi = _bus.Read((ushort)(addr_base & 0xFF00));
                        addr = (ushort)((hi << 8) | lo);
                    }
                    else
                    {
                        lo = _bus.Read(addr_base);
                        hi = _bus.Read((ushort)(addr_base + 1));
                        addr = (ushort)((hi << 8) | lo);
                    }

                    return addr;

                case CPUAddressingMode.Indirect_X:
                    addr_base = _bus.Read(program_counter);
                    program_counter++;
                    ptr = (byte)(addr_base + register_x);
                    lo = _bus.Read(ptr);
                    hi = _bus.Read((byte)(ptr + 1));
                    addr = (ushort)(hi << 8 | lo);

                    pageCrossed = isPageCrossed(ptr, (byte)(ptr + 1));
                    if (pageCrossed && accessType == MemoryAccessType.Read)
                    {
                        ExtraCycles += 1; // Add 1 cycle if a page boundary is crossed
                    }

                    return addr;

                case CPUAddressingMode.Indirect_Y:
                    addr_base = _bus.Read(program_counter);
                    program_counter++;
                    lo = _bus.Read(addr_base);
                    hi = _bus.Read((byte)(addr_base + 1));
                    ushort deref_base = (ushort)((hi << 8) | lo);

                    // TODO: check if the deref address has a page break, if so, add 1 cycle to the  CPU instruction 
                    ushort deref = (ushort)(deref_base + register_y);

                    pageCrossed = isPageCrossed(deref_base, deref);
                    if (pageCrossed && accessType == MemoryAccessType.Read)
                    {
                        ExtraCycles += 1; // Add 1 cycle if a page boundary is crossed
                    }

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
                    byte value = _bus.Read(program_counter);
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
                    return _bus.Read(addr);

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

        public void Reset()
        {
            register_acc = 0;
            register_x = 0;
            register_y = 0;
            status = CPUStatus.Initial;
            stack_pointer = STACK_RESET;

            ushort lo = _bus.Read(PC_AT_POWER);
            ushort hi = _bus.Read(PC_AT_POWER + 1);
            program_counter = (ushort)((hi << 8) | lo);

            Cycles = 7; // Initial cycle value due to reset sequence cost
        }

        public void branch(bool condition)
        {
            ushort nextPC = (ushort)(program_counter + 2);
            ushort addr = getAddressByMode(CPUAddressingMode.Relative);

            if (condition)
            {
                sbyte displacement = (sbyte)_bus.Read(addr);
                ushort targetPC = (ushort)(program_counter + displacement);
                bool pageCrossed = (nextPC & 0xFF00) != (targetPC & 0xFF00);
                ExtraCycles = (byte)(1 + (pageCrossed ? 1 : 0)); // Add 1 cycle if a page boundary is crossed, otherwise add 1 cycle

                program_counter = targetPC;
            }
        }

        public void branchByWord(bool condition)
        {
            if (condition)
            {
                var low = _bus.Read(program_counter);
                var high = _bus.Read((ushort)(program_counter + 1));

                ushort displacement = (ushort)((high << 8) | low);
                program_counter = (ushort)(program_counter + displacement + 2);
            }
        }

        public byte stackPop()
        {
            stack_pointer = (byte)(stack_pointer + 1);
            return _bus.Read((ushort)(STACK_START_ADDR + stack_pointer));
        }

        public void stackPush(byte Data)
        {
            _bus.Write((ushort)(STACK_START_ADDR + stack_pointer), Data);
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
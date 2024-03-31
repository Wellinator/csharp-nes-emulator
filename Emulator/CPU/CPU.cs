using System.Reflection.Emit;

namespace NES_Emulator
{

    public interface iCPU
    {
        public iMemory _memory { get; set; }
        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public ushort program_counter { get; set; }
        public CPUInstructionTable instruction_table { get; set; }
        public void run();
        public byte setStatus(in byte Status);
        public void reset();
        public void load(byte[] Program);
        public void loadAndRun(byte[] Program);

    }


    public class CPU : iCPU
    {

        public CPU(iMemory Memory)
        {
            register_acc = 0;
            register_x = 0;
            status = 0;
            program_counter = 0;

            _memory = Memory;
            instruction_table = new CPUInstructionTable();
        }

        public byte register_acc { get; set; }
        public byte register_x { get; set; }
        public byte register_y { get; set; }
        public byte status { get; set; }
        public ushort program_counter { get; set; }
        public iMemory _memory { get; set; }
        public CPUInstructionTable instruction_table { get; set; }

        public void run()
        {
            while (true)
            {

                byte instruction = _memory.read(program_counter);
                program_counter++;

                ushort program_counter_state = program_counter;
                CPUInstruction opcode = instruction_table.GetInstruction(instruction);

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

                    case CPUOpcodes.BCC_Relative:
                        BCC();
                        break;

                    case CPUOpcodes.BCS_Relative:
                        BCS();
                        break;

                    case CPUOpcodes.BEQ_Relative:
                        BEQ();
                        break;

                    case CPUOpcodes.BIT_ZeroPage:
                    case CPUOpcodes.BIT_Absolute:
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

                    case CPUOpcodes.CLC:
                        CLC();
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

                    case CPUOpcodes.TAX:
                        TAX();
                        break;

                    case CPUOpcodes.INX:
                        INX();
                        break;

                    case CPUOpcodes.BRK:
                        return;

                    default:
                        throw new Exception($"Invalid instruction: {opcode.opcode}({opcode.mnemonic})!");
                }

                if (program_counter_state == program_counter)
                {
                    program_counter += (ushort)(opcode.bytes - 1);
                }

            }
        }

        private void ADC(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            addToRegisterA(value);
        }

        private void AND(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);

            byte result = (byte)(value & register_acc);
            setRegisterAcc(result);
        }

        private void ASL()
        {
            byte value = register_acc;
            byte result = (byte)(value << 1);

            bool is7thBitSet = (value & 0x80) > 0;
            if (is7thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            setRegisterAcc(result);
        }

        private void ASL(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);

            byte result = (byte)(value << 1);

            bool is7thBitSet = (value & 0x80) > 0;
            if (is7thBitSet)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            _memory.write(addr, result);
            updateZeroAndNegativeFlags(result);
        }

        private void BCC()
        {
            if ((status & CPUStatus.Carry) == 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void BCS()
        {
            if ((status & CPUStatus.Carry) != 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void BEQ()
        {
            if ((status & CPUStatus.Zero) != 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void BIT(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            byte result = (byte)(register_acc & value);

            if(result == 0x00)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                removeStatus(CPUStatus.Zero);
            }

            setStatus((byte)(CPUStatus.Overflow & value));
            setStatus((byte)(CPUStatus.Negative & value));
        }

        private void BMI()
        {
            if ((status & CPUStatus.Negative) != 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void BNE()
        {
            if ((status & CPUStatus.Zero) == 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void BPL()
        {
            if ((status & CPUStatus.Negative) == 0)
            {
                sbyte displacement = (sbyte)_memory.read(program_counter);
                program_counter = (ushort)(program_counter + displacement);
            }
        }

        private void CLC()
        {
            removeStatus(CPUStatus.Carry);
        }

        private void TAX()
        {
            register_x = register_acc;
            updateZeroAndNegativeFlags(register_x);
        }

        private void INX()
        {
            register_x += 1;
            updateZeroAndNegativeFlags(register_x);
        }

        private void LDA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);
            setRegisterAcc(value);
        }

        private void STA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, register_acc);
        }

        private void setRegisterAcc(byte Value)
        {
            register_acc = Value;
            updateZeroAndNegativeFlags(register_acc);
        }


        private void addToRegisterA(byte data)
        {
            int sum = (status & CPUStatus.Carry) > 0
                ? register_acc + data + 1
                : register_acc + data + 0;

            bool carry = sum > 0xFF;

            if (carry)
            {
                setStatus(CPUStatus.Carry);
            }
            else
            {
                removeStatus(CPUStatus.Carry);
            }

            byte result = (byte)(sum % 0xFF);

            if (((data ^ result) & (result ^ register_acc) & 0x80) != 0)
            {
                setStatus(CPUStatus.Overflow);
            }
            else
            {
                removeStatus(CPUStatus.Overflow);
            }

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

                case CPUAddressingMode.Immediate:
                case CPUAddressingMode.Relative:
                    return program_counter;

                case CPUAddressingMode.ZeroPage:
                    return _memory.read(program_counter);

                case CPUAddressingMode.Absolute:
                    return _memory.readU16(program_counter);

                case CPUAddressingMode.ZeroPage_X:
                    pos = _memory.read(program_counter);
                    addr = (ushort)((pos + register_x) % 0xFF);
                    return addr;

                case CPUAddressingMode.ZeroPage_Y:
                    pos = _memory.read(program_counter);
                    addr = (ushort)((pos + register_x) % 0xFF);
                    return addr;

                case CPUAddressingMode.Absolute_X:
                    addr_base = _memory.readU16(program_counter);
                    addr = (ushort)((addr_base + register_x) % 0xFF);
                    return addr;

                case CPUAddressingMode.Absolute_Y:
                    addr_base = _memory.readU16(program_counter);
                    addr = (ushort)((addr_base + register_y) % 0xFF);
                    return addr;

                case CPUAddressingMode.Indirect_X:
                    addr_base = _memory.read(program_counter);
                    ptr = (byte)((addr_base + register_x) % 0xFF);
                    lo = _memory.read(ptr);
                    hi = _memory.read((ushort)((addr_base + 1) % 0xFF));
                    return (ushort)(hi << 8 | lo);

                case CPUAddressingMode.Indirect_Y:
                    addr_base = _memory.read(program_counter);
                    lo = _memory.read(addr_base);
                    hi = _memory.read((ushort)((addr_base + 1) % 0xFF));
                    ushort deref_base = (ushort)(hi << 8 | lo);
                    ushort deref = (ushort)((deref_base + register_y) % 0xFF);
                    return deref;

                default:
                    throw new Exception($"Invalid addressing mode: {mode}!");
            }
        }



        public byte setStatus(in byte Status)
        {
            status |= Status;
            return status;
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
            status = 0;

            program_counter = _memory.readU16(0xFFFC);
        }

        public void load(byte[] Program)
        {
            _memory.load(Program);
            _memory.writeU16(0xFFFC, 0x8000);
        }

        public void loadAndRun(byte[] Program)
        {
            load(Program);
            reset();
            run();
        }
    }

}
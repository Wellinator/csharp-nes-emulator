namespace NES_Emulator
{

    public interface iCPU
    {
        public iMemory _memory { get; set; }
        public byte register_a { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public ushort program_counter { get; set; }
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
            register_a = 0;
            register_x = 0;
            status = 0;
            program_counter = 0;

            _memory = Memory;
        }

        public byte register_a { get; set; }
        public byte register_x { get; set; }
        public byte register_y { get; set; }
        public byte status { get; set; }
        public ushort program_counter { get; set; }
        public iMemory _memory { get; set; }

        public void run()
        {
            while (true)
            {
                byte opcode = _memory.read(program_counter);
                program_counter++;

                switch (opcode)
                {
                    // LDA
                    case CPUOpcodes.LDA_Immediate:
                        LDA(CPUAddressingMode.Immediate);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.LDA_ZeroPage:
                        LDA(CPUAddressingMode.ZeroPage);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.LDA_ZeroPage_X:
                        LDA(CPUAddressingMode.ZeroPage_X);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.LDA_Absolute:
                        LDA(CPUAddressingMode.Absolute);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.LDA_Absolute_X:
                        LDA(CPUAddressingMode.Absolute_X);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.LDA_Absolute_Y:
                        LDA(CPUAddressingMode.Absolute_Y);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.LDA_Indirect_X:
                        LDA(CPUAddressingMode.Indirect_X);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.LDA_Indirect_Y:
                        LDA(CPUAddressingMode.Indirect_Y);
                        program_counter += 1;
                        break;

                    // STA
                    case CPUOpcodes.STA_ZeroPage:
                        STA(CPUAddressingMode.ZeroPage);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.STA_ZeroPage_X:
                        STA(CPUAddressingMode.ZeroPage_X);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.STA_Absolute:
                        STA(CPUAddressingMode.Absolute);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.STA_Absolute_X:
                        STA(CPUAddressingMode.Absolute_X);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.STA_Absolute_Y:
                        STA(CPUAddressingMode.Absolute_Y);
                        program_counter += 2;
                        break;
                    case CPUOpcodes.STA_Indirect_X:
                        STA(CPUAddressingMode.Indirect_X);
                        program_counter += 1;
                        break;
                    case CPUOpcodes.STA_Indirect_Y:
                        STA(CPUAddressingMode.Indirect_Y);
                        program_counter += 1;
                        break;

                    case CPUOpcodes.TAX:
                        register_x = register_a;
                        updateZeroAndNegativeFlags(register_x);
                        break;

                    case CPUOpcodes.INX:
                        register_x += 1;
                        updateZeroAndNegativeFlags(register_x);
                        break;

                    case CPUOpcodes.BRK:
                        return;

                    default:
                        return;
                }

            }
        }

        private void LDA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            byte value = _memory.read(addr);

            register_a = value;
            updateZeroAndNegativeFlags(register_a);
        }

        private void STA(CPUAddressingMode mode)
        {
            ushort addr = getAddressByMode(mode);
            _memory.write(addr, register_a);
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

        private void updateZeroAndNegativeFlags(in byte Result)
        {
            if (Result == 0)
            {
                setStatus(CPUStatus.Zero);
            }
            else
            {
                status &= 0b11111101;
            }

            if ((Result & CPUStatus.Negative) != 0)
            {
                setStatus(CPUStatus.Negative);
            }
            else
            {
                status &= 0b01111111;
            }
        }

        public void reset()
        {
            register_a = 0;
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
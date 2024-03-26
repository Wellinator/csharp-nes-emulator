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
                    case CPUOpcodes.LDA:
                        byte param = _memory.read(program_counter);
                        program_counter++;
                        register_a = param;
                        updateZeroAndNegativeFlags(register_a);
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
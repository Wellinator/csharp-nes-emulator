namespace NES_Emulator
{
    enum CPU_OPCODES
    {
        BRK = 0x00,
        LDA = 0xA9,
        TAX = 0xAA,
    }

    public interface iCPU
    {
        public byte register_a { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public byte program_counter { get; set; }

        public void interprete(byte[] program);
    }


    public class CPU : iCPU
    {
        public CPU()
        {
            register_a = 0;
            register_x = 0;
            status = 0;
            program_counter = 0;
        }

        public byte register_a { get; set; }
        public byte register_x { get; set; }
        public byte status { get; set; }
        public byte program_counter { get; set; }

        public void interprete(byte[] program)
        {
            program_counter = 0;

            while (true)
            {
                byte opcode = program[program_counter];
                program_counter++;

                switch (opcode)
                {
                    case (byte)CPU_OPCODES.LDA:
                        byte param = program[program_counter];
                        program_counter++;
                        register_a = param;

                        if (register_a == 0)
                        {
                            status |= 0b00000010;
                        }
                        else
                        {
                            status &= 0b11111101;
                        }

                        if ((register_a & 0b10000000) != 0)
                        {
                            status |= 0b10000000;
                        }
                        else
                        {
                            status &= 0b01111111;
                        }

                        break;

                    case (byte)CPU_OPCODES.BRK:
                        return;

                    default:
                        return;
                }

            }
        }
    }

}
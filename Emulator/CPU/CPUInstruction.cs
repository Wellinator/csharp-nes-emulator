namespace NES_Emulator
{
    public interface ICPUInstruction
    {
        public byte opcode { get; set; }
        public string mnemonic { get; set; }
        public byte bytes { get; set; }
        public byte cycles { get; set; }
        public CPUAddressingMode mode { get; set; }

    }

    public class CPUInstruction : ICPUInstruction
    {
        public byte opcode { get; set; } = 0;
        public string mnemonic { get; set; } = string.Empty;
        public byte bytes { get; set; } = 0;
        public byte cycles { get; set; } = 0;
        public CPUAddressingMode mode { get; set; } = CPUAddressingMode.Immediate;
    }

}
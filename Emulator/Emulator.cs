namespace NES_Emulator
{
    public class Emulator
    {

        public readonly iMemory _mem;
        public readonly iCPU _cpu;

        public Emulator()
        {
            _mem = new Memory();
            _cpu = new CPU(_mem);
        }

        public void Run(byte[] program, OnUpdateCallBack callback)
        {
            // Load program into memory
            _mem.load(program);

            // Default PC start
            // _mem.writeU16(PC_AT_POWER, 0x8000);

            // Sneak game
            //_mem.writeU16(PC_AT_POWER, 0x0600);

            // NES Test
            _mem.writeU16(0xFFFC, 0xC000);

            // Reset CPU to initial state
            _cpu.reset();

            while (true)
            {
                long Cycles = _cpu.Step(callback);

            }
        }
    }
}

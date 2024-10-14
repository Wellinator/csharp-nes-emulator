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
            _cpu.loadAndRun(program, callback);
        }
    }
}

namespace NES_Emulator
{
    public class Emulator
    {

        public readonly IBusDevice _bus;
        public readonly iCPU _cpu;
        public readonly ICartridge _cartridge;
        public readonly IRam _ram;
        public readonly IPPU _ppu;
        private long _cycles = 0;

        public Emulator()
        {
            _cartridge = new Cartridge();
            _ram = new Ram();
            _ppu = new PPU(_cartridge);
            _bus = new Bus(_ram, _ppu, _cartridge);
            _cpu = new CPU(_bus);
        }

        public void Run(byte[] program, OnUpdateCallBack callback)
        {
            // Load the program into the cartridge
            _cartridge.LoadRawBinary(program);

            // Reset CPU to initial state
            _cpu.Reset();
            _cpu.program_counter = 0xC000;

            while (true)
            {
                long Cycles = _cpu.Step(callback);
                _cycles += Cycles;

                // Step the PPU 3x for each CPU cycle
                for (long i = 0; i < Cycles; i++)
                {
                    _ppu.Step();
                }
            }
        }
    }
}

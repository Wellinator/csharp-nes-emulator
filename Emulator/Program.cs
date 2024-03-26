// using NES_Emulator;

using NES_Emulator;

public class Program
{
    static void Main(string[] args)
    {
        Emulator emu = new Emulator(640, 480, "NES Emulator");
        emu.Run();
    }
}

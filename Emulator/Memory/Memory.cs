using System.ComponentModel;
using NES_Emulator;

public interface IRam : IBusDevice
{
    byte[] _memory { get; set; }
}

public class Ram : IRam
{
    public Ram()
    {
        _memory = new byte[0x10000];
    }

    public byte[] _memory { get; set; }

    public byte Read(ushort Address)
    {
        if (Address > 0xFFFF) throw new Exception($"Invalid memory Address: {Address}");
        return _memory[Address];
    }

    public void Write(ushort Address, byte Data)
    {
        if (Address > 0xFFFF) throw new Exception($"Invalid memory Address: {Address}");
        _memory[Address] = Data;
    }
}
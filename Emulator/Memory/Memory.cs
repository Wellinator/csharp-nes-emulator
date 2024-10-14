using System.ComponentModel;
using NES_Emulator;

public interface iMemory
{
    /// <summary>
    /// CPU RAM is accessible via [0x0000 … 0x2000] address space.
    /// Access to [0x2000 … 0x4020] is redirected to other available NES hardware modules: PPU, APU, GamePads, etc. (more on this later)
    /// Access to [0x4020 .. 0x6000] is a special space that different generations of cartridges used differently. It might be mapped to RAM, ROM, or nothing at all. The space is controlled by so-called mappers - special circuitry on a cartridge. We will ignore this space.
    /// Access to [0x6000 .. 0x8000] is reserved to a RAM space on a cartridge if a cartridge has one. It was used in games like Zelda for storing and retrieving the game state. We will ignore this space as well.
    /// Access to [0x8000 … 0xFFFF] is mapped to Program ROM (PRG ROM) space on a cartridge.
    /// Memory access is relatively slow, NES CPU has a few internal memory slots called registers with significantly lower access delay.
    /// </summary>
    byte[] _memory { get; set; }

    public byte read(ushort Address);
    public void write(ushort Address, byte Data);
    public ushort readU16(ushort Address);
    public ushort readU16(ref ushort Address);
    public void writeU16(ushort Address, ushort Data);
    public void load(byte[] Program);
}

public class Memory : iMemory
{
    public Memory()
    {
        _memory = new byte[0x10000];
    }

    public byte[] _memory { get; set; }

    public void load(byte[] Program)
    {
        // Default start 0x8000 - 0xBFFF
        const int ROM_HEADER_LENGTH = 0x0010;
        // Array.Copy(Program, ROM_HEADER_LENGTH, _memory, 0x8000, Program.Length);

        // Custom for Sneak game test
        //Array.Copy(Program, 0, _memory, 0x0600, Program.Length);

        // Custom NESTest program
        Array.Copy(Program, ROM_HEADER_LENGTH, _memory, 0x8000, 0x4000 - ROM_HEADER_LENGTH);
        Array.Copy(Program, ROM_HEADER_LENGTH, _memory, 0xC000, 0x4000 - ROM_HEADER_LENGTH);
    }

    public byte read(ushort Address)
    {
        if (Address > 0xFFFF) throw new Exception($"Invalid memory Address: {Address}");
        return _memory[Address];
    }

    public ushort readU16(ushort Address)
    {
        var lo = read(Address);
        var hi = read((ushort)(Address + 1));
        return (ushort)((hi << 8) | (lo));
    }

    public ushort readU16(ref ushort Address)
    {
        var lo = read(Address);
        Address++;
        var hi = read(Address);
        Address++;

        return (ushort)((hi << 8) | (lo));
    }

    public void write(ushort Address, byte Data)
    {
        if (Address > 0xFFFF) throw new Exception($"Invalid memory Address: {Address}");
        _memory[Address] = Data;
    }

    public void writeU16(ushort Address, ushort Data)
    {
        byte hi = (byte)(Data >> 8);
        byte lo = (byte)(Data & 0xff);
        write(Address, lo);
        write((ushort)(Address + 1), hi);
    }
}
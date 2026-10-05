/// <summary>
/// CPU RAM is accessible via [0x0000 … 0x2000] address space.
/// Access to [0x2000 … 0x4020] is redirected to other available NES hardware modules: PPU, APU, GamePads, etc. (more on this later)
/// Access to [0x4020 .. 0x6000] is a special space that different generations of cartridges used differently. It might be mapped to RAM, ROM, or nothing at all. The space is controlled by so-called mappers - special circuitry on a cartridge. We will ignore this space.
/// Access to [0x6000 .. 0x8000] is reserved to a RAM space on a cartridge if a cartridge has one. It was used in games like Zelda for storing and retrieving the game state. We will ignore this space as well.
/// Access to [0x8000 … 0xFFFF] is mapped to Program ROM (PRG ROM) space on a cartridge.
/// Memory access is relatively slow, NES CPU has a few internal memory slots called registers with significantly lower access delay.
/// </summary>
public class Bus : IBusDevice
{
    private readonly IRam _ram;
    private readonly IPPU _ppu;
    private readonly ICartridge _cartridge;

    public Bus(IRam Ram, IPPU PPU, ICartridge Cartridge)
    {
        _ram = Ram;
        _ppu = PPU;
        _cartridge = Cartridge;
    }

    public byte Read(ushort Address)
    {
        if (Address < 0x2000)
        {
            ushort mirrorAddress = (ushort)(Address & 0x07FF);
            return _ram.Read(mirrorAddress);
        }
        else if (Address < 0x4000)
        {
            ushort mirrorAddress = (ushort)(Address & 0x2007);
            return _ppu.Read(mirrorAddress);
        }
        else if (Address >= 0x8000)
        {
            return _cartridge.Read(Address);
        }
        else
        {
            throw new NotImplementedException($"Bus Read not implemented for address: {Address}");
        }
    }

    public void Write(ushort Address, byte Data)
    {
        if (Address < 0x2000)
        {
            ushort mirrorAddress = (ushort)(Address & 0x07FF);
            _ram.Write(mirrorAddress, Data);
        }
        else if (Address >= 0x2000 && Address < 0x4000)
        {
            // PPU registers are mirrored every 8 bytes in the range 0x2000 - 0x3FFF
            // Not implemented yet
        }
        else if (Address >= 0x8000)
        {
            _cartridge.Write(Address, Data);
        }
        else
        {
            // Falha silenciosa para endereços não implementados
            Console.WriteLine($"Bus Write not implemented for address: {Address}");
        }
    }
}
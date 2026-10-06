public class PPU : IPPU
{
    // PPU registers
    private byte PPUCTRL;   // $2000
    private byte PPUMASK;   // $2001
    private byte PPUSTATUS; // $2002

    // The $2003 register is used to set the address in the OAM (Object Attribute Memory) where the next write will occur. 
    // The $2004 register is used to write data to the OAM at the address specified by $2003.
    private byte OAMADDR;   // $2003


    // $2005 (PPUSCROLL) - This is a 16-bit register, but we will store it as two separate bytes for simplicity
    private byte _scrollX; // Horizontal scroll offset
    private byte _scrollY; // Vertical scroll offset

    // $2006 - This is a 16-bit register used for addressing the PPU memory (VRAM) from 0x0000 to 0x3FFF.
    private ushort PPUADDR;
    private byte _dataBuffer; // Used for buffered reads from PPU memory
    private bool _addressLatch; // Used to track whether the next write to the high or low byte

    private readonly byte[] _oamData = new byte[256]; // Object Attribute Memory (OAM) for sprites
    private readonly byte[] _vram = new byte[0x4000]; // PPU memory (VRAM) from 0x0000 to 0x3FFF



    private readonly ICartridge _cartridge;

    public PPU(ICartridge Cartridge)
    {
        _cartridge = Cartridge;
    }

    public void Step()
    {
        throw new NotImplementedException();
    }

    public byte Read(ushort Address)
    {
        ushort mappedAddress = (ushort)(Address & 0x0007); // Mirror the address to the range 0x2000 - 0x2007

        switch (mappedAddress)
        {
            case 0x0002: // PPUSTATUS
                {
                    byte result = PPUSTATUS;

                    // Colateral effects: Clear the vblank flag (bit 7) anSd reset the address latch
                    PPUSTATUS &= 0b01111111; // Clear bit 7
                    _addressLatch = false;

                    return result;
                }
            case 0x0004: // OAMDATA
                {
                    bool isValidOAMAddress = OAMADDR <= 255; // Ensure OAMADDR is within valid range

                    // TODO: limit this safecheck to only be done in debug mode, as it will slow down the emulator
                    if (!isValidOAMAddress)
                    {
                        throw new ArgumentOutOfRangeException($"OAMADDR {OAMADDR} is out of valid range (0-255).");
                    }

                    return _oamData[OAMADDR];
                }
            case 0x0007: // PPUDATA
                {

                    bool isPPUAddressPointingToPalette = PPUADDR >= 0x3F00;
                    byte result;

                    if (isPPUAddressPointingToPalette)
                    {
                        result = _vram[PPUADDR];
                    }
                    else
                    {
                        result = _dataBuffer;
                        _dataBuffer = _vram[PPUADDR];

                        // Increment the PPU address based on the setting in PPUCTRL (bit 2)
                        if ((PPUCTRL & 0b00000100) == 0)
                        {
                            PPUADDR += 1; // Increment by 1 if bit 2 is clear
                        }
                        else
                        {
                            PPUADDR += 32; // Increment by 32 if bit 2 is set
                        }
                    }

                    return result;
                }

            default:
                return 0; // For other registers, if the CPU tries to read them it gets garbage, return 0. 

        }

    }

    public void Write(ushort Address, byte Data)
    {
        throw new NotImplementedException();
    }
}
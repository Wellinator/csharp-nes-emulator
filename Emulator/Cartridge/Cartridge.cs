public class Cartridge : ICartridge
{
    public Cartridge(byte[] Program) => Load(Program);

    public Cartridge()
    {
    }

    private byte[] _prgRom;
    private byte[] _chrRom;
    private int _prgBanks;
    private int _chrBanks;
    private int _mapperId;

    public void Load(string filePath)
    {
        byte[] program = File.ReadAllBytes(filePath);
        Load(program);
    }

    public void LoadRawBinary(byte[] Program)
    {
        _prgRom = Program;
        _prgBanks = 1; // Assuming 1 bank for raw binary
        _chrBanks = 0; // No CHR data in raw binary
        _mapperId = 0; // Default mapper ID for raw binary
        const int ROM_HEADER_LENGTH = 0x0010;

        _prgRom = new byte[Program.Length];
        Array.Copy(Program, ROM_HEADER_LENGTH, _prgRom, 0, Program.Length - ROM_HEADER_LENGTH);
        _prgBanks = Program.Length / 0x4000; // Number of 16KB PRG ROM banks
        return;
    }

    public void Load(byte[] Program)
    {
        // Check if the program is a valid NES ROM
        bool isValid = Program.Length > 16 &&
                        Program[0] == 'N' &&
                        Program[1] == 'E' &&
                        Program[2] == 'S' &&
                        Program[3] == 0x1A;

        if (!isValid)
        {
            throw new ArgumentException("Invalid NES ROM file.");
        }

        // Default start 0x8000 - 0xBFFF
        const int ROM_HEADER_LENGTH = 0x0010;
        int prgOffset = ROM_HEADER_LENGTH;

        _prgBanks = Program[4]; // Number of 16KB PRG ROM banks
        _chrBanks = Program[5]; // Number of 8KB CHR ROM banks
        _mapperId = (Program[6] >> 4) | (Program[7] & 0xF0); // Mapper ID

        int prgSize = _prgBanks * 0x4000; // PRG ROM size in bytes
        int chrSize = _chrBanks * 0x2000; // CHR ROM size in bytes

        _prgRom = new byte[prgSize];
        _chrRom = new byte[chrSize];

        bool hasTrainer = (Program[6] & 0x04) != 0;
        if (hasTrainer)
        {
            // Skip the 512-byte trainer if present
            prgOffset += 512;
        }

        // Copy the PRG
        Array.Copy(Program, prgOffset, _prgRom, 0, prgSize);

        //  Copy the CHR if present
        if (chrSize > 0)
        {
            Array.Copy(Program, prgOffset + prgSize, _chrRom, 0, chrSize);
        }
    }

    public byte Read(ushort Address)
    {
        if (Address >= 0x8000)
        {
            int mappedAddress = Address - 0x8000;

            if (_prgBanks == 1)
            {
                // 16KB PRG ROM, mirror the first 16KB to the second 16KB
                mappedAddress = mappedAddress % 0x4000;
            }

            return _prgRom[mappedAddress];
        }
        return 0;
    }

    public void Write(ushort Address, byte Data)
    {
        // Typically, PRG ROM is read-only, but some mappers allow writing to certain addresses.
        // For simplicity, we will ignore writes to PRG ROM in this implementation.
        Console.WriteLine($"Attempted to write to PRG ROM at address {Address:X4} with data {Data:X2}. Ignored.");
    }
}
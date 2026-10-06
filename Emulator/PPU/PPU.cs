public class PPU : IPPU
{
    // PPU registers
    private byte PPUCTRL;   // $2000
    private byte PPUMASK;   // $2001
    private byte PPUSTATUS; // $2002
    private byte OAMADDR;   // $2003
    private byte OAMDATA;   // $2004
    private byte PPUSCROLL; // $2005
    private byte PPUADDR;   // $2006
    private byte PPUDATA;   // $2007

    public void Step()
    {
        throw new NotImplementedException();
    }

    public byte Read(ushort Address)
    {
        throw new NotImplementedException();
    }

    public void Write(ushort Address, byte Data)
    {
        throw new NotImplementedException();
    }
}
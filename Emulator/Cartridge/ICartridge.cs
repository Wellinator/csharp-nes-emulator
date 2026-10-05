public interface ICartridge : IBusDevice
{
    public void Load(byte[] Program);
    public void Load(string filePath);
    public void LoadRawBinary(byte[] Program);
}
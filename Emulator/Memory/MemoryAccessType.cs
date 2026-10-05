namespace NES_Emulator
{
    public enum MemoryAccessType
    {
        Read, // Memory read operation. Can cost +1 in case of crossing a page boundary.
        Write, // Memory write operation. Never costs extra cycles.
        ReadModifyWrite, // Memory read-modify-write operation. Never costs extra cycles.
    }
}
namespace NES_Emulator
{
    public interface ICPUInstructionTable
    {
        public CPUInstruction GetInstruction(byte opcode);
    }

    public class CPUInstructionTable : ICPUInstructionTable
    {
        private Dictionary<byte, CPUInstruction> lookupTable { get; set; }

        public CPUInstructionTable()
        {
            lookupTable = new Dictionary<byte, CPUInstruction>();
            loadInstructionTable();
        }

        public CPUInstruction GetInstruction(byte opcode)
        {
            return lookupTable[opcode];
        }

        private void loadInstructionTable()
        {
            lookupTable.Clear();

            // BRK - Force Interrupt
            lookupTable.Add(0x00, new CPUInstruction { opcode = 0x00, mnemonic = "BRK", bytes = 1, cycles = 1 });

            // TAX - Transfer Accumulator to X
            lookupTable.Add(0xAA, new CPUInstruction { opcode = 0xAA, mnemonic = "TAX", bytes = 1, cycles = 2 });

            // INX - Increment X Register
            lookupTable.Add(0xE8, new CPUInstruction { opcode = 0xE8, mnemonic = "INX", bytes = 1, cycles = 2 });

            // LDA - Load Accumulator
            lookupTable.Add(0xA9, new CPUInstruction { opcode = 0xA9, mnemonic = "LDA", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(0xA5, new CPUInstruction { opcode = 0xA5, mnemonic = "LDA", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(0xB5, new CPUInstruction { opcode = 0xB5, mnemonic = "LDA", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(0xAD, new CPUInstruction { opcode = 0xAD, mnemonic = "LDA", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(0xBD, new CPUInstruction { opcode = 0xBD, mnemonic = "LDA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(0xB9, new CPUInstruction { opcode = 0xB9, mnemonic = "LDA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(0xA1, new CPUInstruction { opcode = 0xA1, mnemonic = "LDA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(0xB1, new CPUInstruction { opcode = 0xB1, mnemonic = "LDA", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // STA - Store Accumulator
            lookupTable.Add(0x85, new CPUInstruction { opcode = 0x85, mnemonic = "STA", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(0x95, new CPUInstruction { opcode = 0x95, mnemonic = "STA", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(0x8D, new CPUInstruction { opcode = 0x8D, mnemonic = "STA", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(0x9D, new CPUInstruction { opcode = 0x9D, mnemonic = "STA", bytes = 3, cycles = 5, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(0x99, new CPUInstruction { opcode = 0x99, mnemonic = "STA", bytes = 3, cycles = 5, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(0x81, new CPUInstruction { opcode = 0x81, mnemonic = "STA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(0x91, new CPUInstruction { opcode = 0x91, mnemonic = "STA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_Y });

        }
    }
}
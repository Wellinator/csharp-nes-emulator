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

            // ADC - Add with Carry
            lookupTable.Add(CPUOpcodes.ADC_Immediate, new CPUInstruction { opcode = CPUOpcodes.ADC_Immediate, mnemonic = "ADC", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.ADC_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.ADC_ZeroPage, mnemonic = "ADC", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.ADC_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.ADC_ZeroPage_X, mnemonic = "ADC", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.ADC_Absolute, new CPUInstruction { opcode = CPUOpcodes.ADC_Absolute, mnemonic = "ADC", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.ADC_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.ADC_Absolute_X, mnemonic = "ADC", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.ADC_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.ADC_Absolute_Y, mnemonic = "ADC", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.ADC_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.ADC_Indirect_X, mnemonic = "ADC", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.ADC_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.ADC_Indirect_Y, mnemonic = "ADC", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // AND - Logical AND
            lookupTable.Add(CPUOpcodes.AND_Immediate, new CPUInstruction { opcode = CPUOpcodes.AND_Immediate, mnemonic = "AND", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.AND_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.AND_ZeroPage, mnemonic = "AND", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.AND_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.AND_ZeroPage_X, mnemonic = "AND", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.AND_Absolute, new CPUInstruction { opcode = CPUOpcodes.AND_Absolute, mnemonic = "AND", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.AND_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.AND_Absolute_X, mnemonic = "AND", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.AND_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.AND_Absolute_Y, mnemonic = "AND", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.AND_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.AND_Indirect_X, mnemonic = "AND", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.AND_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.AND_Indirect_Y, mnemonic = "AND", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // ASL - Arithmetic Shift Left
            lookupTable.Add(CPUOpcodes.ASL_Accumulator, new CPUInstruction { opcode = CPUOpcodes.ASL_Accumulator, mnemonic = "ASL", bytes = 1, cycles = 2, mode = CPUAddressingMode.Accumulator });
            lookupTable.Add(CPUOpcodes.ASL_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.ASL_ZeroPage, mnemonic = "ASL", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.ASL_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.ASL_ZeroPage_X, mnemonic = "ASL", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.ASL_Absolute, new CPUInstruction { opcode = CPUOpcodes.ASL_Absolute, mnemonic = "ASL", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.ASL_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.ASL_Absolute_X, mnemonic = "ASL", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // BCC - Branch if Carry Clear
            lookupTable.Add(CPUOpcodes.BCC_Relative, new CPUInstruction { opcode = CPUOpcodes.BCC_Relative, mnemonic = "BCC", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BCS - Branch if Carry Set
            lookupTable.Add(CPUOpcodes.BCS_Relative, new CPUInstruction { opcode = CPUOpcodes.BCS_Relative, mnemonic = "BCS", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BEQ - Branch if Equal
            lookupTable.Add(CPUOpcodes.BEQ_Relative, new CPUInstruction { opcode = CPUOpcodes.BEQ_Relative, mnemonic = "BEQ", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BIT - Bit Test
            lookupTable.Add(CPUOpcodes.BIT_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.BIT_ZeroPage, mnemonic = "BIT", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.BIT_Absolute, new CPUInstruction { opcode = CPUOpcodes.BIT_Absolute, mnemonic = "BIT", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });

            // BMI - Branch if Minus
            lookupTable.Add(CPUOpcodes.BMI_Relative, new CPUInstruction { opcode = CPUOpcodes.BMI_Relative, mnemonic = "BMI", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BNE - Branch if Not Equal
            lookupTable.Add(CPUOpcodes.BNE_Relative, new CPUInstruction { opcode = CPUOpcodes.BNE_Relative, mnemonic = "BNE", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BPL - Branch if Positive
            lookupTable.Add(CPUOpcodes.BPL_Relative, new CPUInstruction { opcode = CPUOpcodes.BPL_Relative, mnemonic = "BPL", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BRK - Force Interrupt
            lookupTable.Add(CPUOpcodes.BRK, new CPUInstruction { opcode = CPUOpcodes.BRK, mnemonic = "BRK", bytes = 1, cycles = 1 });

            // BVC - Branch if Overflow Clear
            lookupTable.Add(CPUOpcodes.BVC, new CPUInstruction { opcode = CPUOpcodes.BVC, mnemonic = "BVC", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // BVS - Branch if Overflow Set
            lookupTable.Add(CPUOpcodes.BVS, new CPUInstruction { opcode = CPUOpcodes.BVS, mnemonic = "BVS", bytes = 2, cycles = 2 /*(+1 if branch succeeds +2 if to a new page)*/, mode = CPUAddressingMode.Relative });

            // CLC - Clear Carry Flag
            lookupTable.Add(CPUOpcodes.CLC, new CPUInstruction { opcode = CPUOpcodes.CLC, mnemonic = "CLC", bytes = 1, cycles = 2 });

            // CLD - Clear Decimal Mode
            lookupTable.Add(CPUOpcodes.CLD, new CPUInstruction { opcode = CPUOpcodes.CLD, mnemonic = "CLD", bytes = 1, cycles = 2 });

            // CLI - Clear Interrupt Disable
            lookupTable.Add(CPUOpcodes.CLI, new CPUInstruction { opcode = CPUOpcodes.CLI, mnemonic = "CLI", bytes = 1, cycles = 2 });

            // CLV - Clear Overflow Flag
            lookupTable.Add(CPUOpcodes.CLV, new CPUInstruction { opcode = CPUOpcodes.CLV, mnemonic = "CLV", bytes = 1, cycles = 2 });

            // CMP - Compare
            lookupTable.Add(CPUOpcodes.CMP_Immediate, new CPUInstruction { opcode = CPUOpcodes.CMP_Immediate, mnemonic = "CMP", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.CMP_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.CMP_ZeroPage, mnemonic = "CMP", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.CMP_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.CMP_ZeroPage_X, mnemonic = "CMP", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.CMP_Absolute, new CPUInstruction { opcode = CPUOpcodes.CMP_Absolute, mnemonic = "CMP", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.CMP_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.CMP_Absolute_X, mnemonic = "CMP", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.CMP_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.CMP_Absolute_Y, mnemonic = "CMP", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.CMP_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.CMP_Indirect_X, mnemonic = "CMP", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.CMP_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.CMP_Indirect_Y, mnemonic = "CMP", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // CPX - Compare X Register
            lookupTable.Add(CPUOpcodes.CPX_Immediate, new CPUInstruction { opcode = CPUOpcodes.CPX_ZeroPage, mnemonic = "CPX", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.CPX_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.CPX_ZeroPage, mnemonic = "CPX", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.CPX_Absolute, new CPUInstruction { opcode = CPUOpcodes.CPX_Absolute, mnemonic = "CPX", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });

            // CPY - Compare Y Register
            lookupTable.Add(CPUOpcodes.CPY_Immediate, new CPUInstruction { opcode = CPUOpcodes.CPY_Immediate, mnemonic = "CPY", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.CPY_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.CPY_ZeroPage, mnemonic = "CPY", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.CPY_Absolute, new CPUInstruction { opcode = CPUOpcodes.CPY_Absolute, mnemonic = "CPY", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });

            // DEC - Decrement Memory
            lookupTable.Add(CPUOpcodes.DEC_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.DEC_ZeroPage, mnemonic = "DEC", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.DEC_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.DEC_ZeroPage_X, mnemonic = "DEC", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.DEC_Absolute, new CPUInstruction { opcode = CPUOpcodes.DEC_Absolute, mnemonic = "DEC", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.DEC_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.DEC_Absolute_X, mnemonic = "DEC", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // DEX - Decrement X Register
            lookupTable.Add(CPUOpcodes.DEX, new CPUInstruction { opcode = CPUOpcodes.DEX, mnemonic = "DEX", bytes = 1, cycles = 2 });

            // DEY - Decrement Y Register
            lookupTable.Add(CPUOpcodes.DEY, new CPUInstruction { opcode = CPUOpcodes.DEY, mnemonic = "DEY", bytes = 1, cycles = 2 });

            // EOR - Exclusive OR
            lookupTable.Add(CPUOpcodes.EOR_Immediate, new CPUInstruction { opcode = CPUOpcodes.EOR_Immediate, mnemonic = "EOR", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.EOR_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.EOR_ZeroPage, mnemonic = "EOR", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.EOR_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.EOR_ZeroPage_X, mnemonic = "EOR", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.EOR_Absolute, new CPUInstruction { opcode = CPUOpcodes.EOR_Absolute, mnemonic = "EOR", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.EOR_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.EOR_Absolute_X, mnemonic = "EOR", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.EOR_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.EOR_Absolute_Y, mnemonic = "EOR", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.EOR_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.EOR_Indirect_X, mnemonic = "EOR", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.EOR_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.EOR_Indirect_Y, mnemonic = "EOR", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // INC - Increment Memory
            lookupTable.Add(CPUOpcodes.INC_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.INC_ZeroPage, mnemonic = "INC", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.INC_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.INC_ZeroPage_X, mnemonic = "INC", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.INC_Absolute, new CPUInstruction { opcode = CPUOpcodes.INC_Absolute, mnemonic = "INC", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.INC_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.INC_Absolute_X, mnemonic = "INC", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // INX - Increment X Register
            lookupTable.Add(CPUOpcodes.INX, new CPUInstruction { opcode = CPUOpcodes.INX, mnemonic = "INX", bytes = 1, cycles = 2 });

            // INY - Increment X Register
            lookupTable.Add(CPUOpcodes.INY, new CPUInstruction { opcode = CPUOpcodes.INY, mnemonic = "INY", bytes = 1, cycles = 2 });

            // JMP - Jump
            lookupTable.Add(CPUOpcodes.JMP_Absolute, new CPUInstruction { opcode = CPUOpcodes.JMP_Absolute, mnemonic = "JMP", bytes = 3, cycles = 3, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.JMP_Indirect, new CPUInstruction { opcode = CPUOpcodes.JMP_Indirect, mnemonic = "JMP", bytes = 3, cycles = 5, mode = CPUAddressingMode.Indirect });

            // JSR - Jump to Subroutine
            lookupTable.Add(CPUOpcodes.JSR, new CPUInstruction { opcode = CPUOpcodes.JSR, mnemonic = "JSR", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });

            // LDA - Load Accumulator
            lookupTable.Add(CPUOpcodes.LDA_Immediate, new CPUInstruction { opcode = CPUOpcodes.LDA_Immediate, mnemonic = "LDA", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.LDA_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.LDA_ZeroPage, mnemonic = "LDA", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.LDA_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.LDA_ZeroPage_X, mnemonic = "LDA", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.LDA_Absolute, new CPUInstruction { opcode = CPUOpcodes.LDA_Absolute, mnemonic = "LDA", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.LDA_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.LDA_Absolute_X, mnemonic = "LDA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.LDA_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.LDA_Absolute_Y, mnemonic = "LDA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.LDA_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.LDA_Indirect_X, mnemonic = "LDA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.LDA_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.LDA_Indirect_Y, mnemonic = "LDA", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // LDX - Load X Register
            lookupTable.Add(CPUOpcodes.LDX_Immediate, new CPUInstruction { opcode = CPUOpcodes.LDX_Immediate, mnemonic = "LDX", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.LDX_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.LDX_ZeroPage, mnemonic = "LDX", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.LDX_ZeroPage_Y, new CPUInstruction { opcode = CPUOpcodes.LDX_ZeroPage_Y, mnemonic = "LDX", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_Y });
            lookupTable.Add(CPUOpcodes.LDX_Absolute, new CPUInstruction { opcode = CPUOpcodes.LDX_Absolute, mnemonic = "LDX", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.LDX_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.LDX_Absolute_Y, mnemonic = "LDX", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });

            // LDY - Load X Register
            lookupTable.Add(CPUOpcodes.LDY_Immediate, new CPUInstruction { opcode = CPUOpcodes.LDY_Immediate, mnemonic = "LDY", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.LDY_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.LDY_ZeroPage, mnemonic = "LDY", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.LDY_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.LDY_ZeroPage_X, mnemonic = "LDY", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.LDY_Absolute, new CPUInstruction { opcode = CPUOpcodes.LDY_Absolute, mnemonic = "LDY", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.LDY_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.LDY_Absolute_X, mnemonic = "LDY", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });

            // LSR - Logical Shift Right
            lookupTable.Add(CPUOpcodes.LSR_Accumulator, new CPUInstruction { opcode = CPUOpcodes.LSR_Accumulator, mnemonic = "LSR", bytes = 1, cycles = 2, mode = CPUAddressingMode.Accumulator });
            lookupTable.Add(CPUOpcodes.LSR_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.LSR_ZeroPage, mnemonic = "LSR", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.LSR_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.LSR_ZeroPage_X, mnemonic = "LSR", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.LSR_Absolute, new CPUInstruction { opcode = CPUOpcodes.LSR_Absolute, mnemonic = "LSR", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.LSR_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.LSR_Absolute_X, mnemonic = "LSR", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // NOP - No Operation
            lookupTable.Add(CPUOpcodes.NOP, new CPUInstruction { opcode = CPUOpcodes.NOP, mnemonic = "NOP", bytes = 1, cycles = 2 });

            // ORA - Logical Inclusive OR
            lookupTable.Add(CPUOpcodes.ORA_Immediate, new CPUInstruction { opcode = CPUOpcodes.ORA_Immediate, mnemonic = "ORA", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.ORA_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.ORA_ZeroPage, mnemonic = "ORA", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.ORA_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.ORA_ZeroPage_X, mnemonic = "ORA", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.ORA_Absolute, new CPUInstruction { opcode = CPUOpcodes.ORA_Absolute, mnemonic = "ORA", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.ORA_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.ORA_Absolute_X, mnemonic = "ORA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.ORA_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.ORA_Absolute_Y, mnemonic = "ORA", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.ORA_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.ORA_Indirect_X, mnemonic = "ORA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.ORA_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.ORA_Indirect_Y, mnemonic = "ORA", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // PHA - Push Accumulator
            lookupTable.Add(CPUOpcodes.PHA, new CPUInstruction { opcode = CPUOpcodes.PHA, mnemonic = "PHA", bytes = 1, cycles = 3 });

            // PHP - Push Processor Status
            lookupTable.Add(CPUOpcodes.PHP, new CPUInstruction { opcode = CPUOpcodes.PHP, mnemonic = "PHP", bytes = 1, cycles = 3 });

            // PLA - Pull Accumulator
            lookupTable.Add(CPUOpcodes.PLA, new CPUInstruction { opcode = CPUOpcodes.PLA, mnemonic = "PLA", bytes = 1, cycles = 4 });

            // PLP - Pull Processor Status
            lookupTable.Add(CPUOpcodes.PLP, new CPUInstruction { opcode = CPUOpcodes.PLP, mnemonic = "PLP", bytes = 1, cycles = 4 });

            // ROL - Rotate Left
            lookupTable.Add(CPUOpcodes.ROL_Accumulator, new CPUInstruction { opcode = CPUOpcodes.ROL_Accumulator, mnemonic = "ROL", bytes = 1, cycles = 2, mode = CPUAddressingMode.Accumulator });
            lookupTable.Add(CPUOpcodes.ROL_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.ROL_ZeroPage, mnemonic = "ROL", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.ROL_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.ROL_ZeroPage_X, mnemonic = "ROL", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.ROL_Absolute, new CPUInstruction { opcode = CPUOpcodes.ROL_Absolute, mnemonic = "ROL", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.ROL_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.ROL_Absolute_X, mnemonic = "ROL", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // ROR - Rotate Right
            lookupTable.Add(CPUOpcodes.ROR_Accumulator, new CPUInstruction { opcode = CPUOpcodes.ROR_Accumulator, mnemonic = "ROR", bytes = 1, cycles = 2, mode = CPUAddressingMode.Accumulator });
            lookupTable.Add(CPUOpcodes.ROR_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.ROR_ZeroPage, mnemonic = "ROR", bytes = 2, cycles = 5, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.ROR_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.ROR_ZeroPage_X, mnemonic = "ROR", bytes = 2, cycles = 6, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.ROR_Absolute, new CPUInstruction { opcode = CPUOpcodes.ROR_Absolute, mnemonic = "ROR", bytes = 3, cycles = 6, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.ROR_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.ROR_Absolute_X, mnemonic = "ROR", bytes = 3, cycles = 7, mode = CPUAddressingMode.Absolute_X });

            // RTI - Return from Interrupt
            lookupTable.Add(CPUOpcodes.RTI, new CPUInstruction { opcode = CPUOpcodes.RTI, mnemonic = "RTI", bytes = 1, cycles = 6 });

            // RTS - Return from Subroutine
            lookupTable.Add(CPUOpcodes.RTS, new CPUInstruction { opcode = CPUOpcodes.RTS, mnemonic = "RTS", bytes = 1, cycles = 6 });

            // SBC - Subtract with Carry
            lookupTable.Add(CPUOpcodes.SBC_Immediate, new CPUInstruction { opcode = CPUOpcodes.SBC_Immediate, mnemonic = "SBC", bytes = 2, cycles = 2, mode = CPUAddressingMode.Immediate });
            lookupTable.Add(CPUOpcodes.SBC_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.SBC_ZeroPage, mnemonic = "SBC", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.SBC_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.SBC_ZeroPage_X, mnemonic = "SBC", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.SBC_Absolute, new CPUInstruction { opcode = CPUOpcodes.SBC_Absolute, mnemonic = "SBC", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.SBC_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.SBC_Absolute_X, mnemonic = "SBC", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.SBC_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.SBC_Absolute_Y, mnemonic = "SBC", bytes = 3, cycles = 4 /*(+1 if page crossed)*/, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.SBC_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.SBC_Indirect_X, mnemonic = "SBC", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.SBC_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.SBC_Indirect_Y, mnemonic = "SBC", bytes = 2, cycles = 5,/*(+1 if page crossed)*/ mode = CPUAddressingMode.Indirect_Y });

            // SEC - Set Carry Flag
            lookupTable.Add(CPUOpcodes.SEC, new CPUInstruction { opcode = CPUOpcodes.SEC, mnemonic = "SEC", bytes = 1, cycles = 2 });

            // SED - Set Decimal Flag
            lookupTable.Add(CPUOpcodes.SED, new CPUInstruction { opcode = CPUOpcodes.SED, mnemonic = "SED", bytes = 1, cycles = 2 });

            // SEI - Set Interrupt Disable
            lookupTable.Add(CPUOpcodes.SEI, new CPUInstruction { opcode = CPUOpcodes.SEI, mnemonic = "SEI", bytes = 1, cycles = 2 });

            // STA - Store Accumulator
            lookupTable.Add(CPUOpcodes.STA_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.STA_ZeroPage, mnemonic = "STA", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.STA_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.STA_ZeroPage_X, mnemonic = "STA", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.STA_Absolute, new CPUInstruction { opcode = CPUOpcodes.STA_Absolute, mnemonic = "STA", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });
            lookupTable.Add(CPUOpcodes.STA_Absolute_X, new CPUInstruction { opcode = CPUOpcodes.STA_Absolute_X, mnemonic = "STA", bytes = 3, cycles = 5, mode = CPUAddressingMode.Absolute_X });
            lookupTable.Add(CPUOpcodes.STA_Absolute_Y, new CPUInstruction { opcode = CPUOpcodes.STA_Absolute_Y, mnemonic = "STA", bytes = 3, cycles = 5, mode = CPUAddressingMode.Absolute_Y });
            lookupTable.Add(CPUOpcodes.STA_Indirect_X, new CPUInstruction { opcode = CPUOpcodes.STA_Indirect_X, mnemonic = "STA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_X });
            lookupTable.Add(CPUOpcodes.STA_Indirect_Y, new CPUInstruction { opcode = CPUOpcodes.STA_Indirect_Y, mnemonic = "STA", bytes = 2, cycles = 6, mode = CPUAddressingMode.Indirect_Y });

            // STX - Store X Register
            lookupTable.Add(CPUOpcodes.STX_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.STX_ZeroPage, mnemonic = "STX", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.STX_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.STX_ZeroPage_X, mnemonic = "STX", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.STX_Absolute, new CPUInstruction { opcode = CPUOpcodes.STX_Absolute, mnemonic = "STX", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });

            // STY - Store Y Register
            lookupTable.Add(CPUOpcodes.STY_ZeroPage, new CPUInstruction { opcode = CPUOpcodes.STY_ZeroPage, mnemonic = "STY", bytes = 2, cycles = 3, mode = CPUAddressingMode.ZeroPage });
            lookupTable.Add(CPUOpcodes.STY_ZeroPage_X, new CPUInstruction { opcode = CPUOpcodes.STY_ZeroPage_X, mnemonic = "STY", bytes = 2, cycles = 4, mode = CPUAddressingMode.ZeroPage_X });
            lookupTable.Add(CPUOpcodes.STY_Absolute, new CPUInstruction { opcode = CPUOpcodes.STY_Absolute, mnemonic = "STY", bytes = 3, cycles = 4, mode = CPUAddressingMode.Absolute });

            // TAX - Transfer Accumulator to X
            lookupTable.Add(CPUOpcodes.TAX, new CPUInstruction { opcode = CPUOpcodes.TAX, mnemonic = "TAX", bytes = 1, cycles = 2 });

            // TAY - Transfer Accumulator to Y
            lookupTable.Add(CPUOpcodes.TAY, new CPUInstruction { opcode = CPUOpcodes.TAY, mnemonic = "TAY", bytes = 1, cycles = 2 });

            // TSX - Transfer Stack Pointer to X
            lookupTable.Add(CPUOpcodes.TSX, new CPUInstruction { opcode = CPUOpcodes.TSX, mnemonic = "TSX", bytes = 1, cycles = 2 });

            // TXA - Transfer X to Accumulator
            lookupTable.Add(CPUOpcodes.TXA, new CPUInstruction { opcode = CPUOpcodes.TXA, mnemonic = "TXA", bytes = 1, cycles = 2 });

            // TXS - Transfer X to Stack Pointer
            lookupTable.Add(CPUOpcodes.TXS, new CPUInstruction { opcode = CPUOpcodes.TXS, mnemonic = "TXS", bytes = 1, cycles = 2 });

            // TYA - Transfer Y to Accumulator
            lookupTable.Add(CPUOpcodes.TYA, new CPUInstruction { opcode = CPUOpcodes.TYA, mnemonic = "TYA", bytes = 1, cycles = 2 });
        }
    }
}
namespace NES_Emulator
{

    public sealed class CPUOpcodes
    {
        // ADC
        public const byte ADC_Immediate = 0x69;
        public const byte ADC_ZeroPage = 0x65;
        public const byte ADC_ZeroPage_X = 0x75;
        public const byte ADC_Absolute = 0x6D;
        public const byte ADC_Absolute_X = 0x7D;
        public const byte ADC_Absolute_Y = 0x79;
        public const byte ADC_Indirect_X = 0x61;
        public const byte ADC_Indirect_Y = 0x71;

        // AND
        public const byte AND_Immediate = 0x29;
        public const byte AND_ZeroPage = 0x25;
        public const byte AND_ZeroPage_X = 0x35;
        public const byte AND_Absolute = 0x2D;
        public const byte AND_Absolute_X = 0x3D;
        public const byte AND_Absolute_Y = 0x39;
        public const byte AND_Indirect_X = 0x21;
        public const byte AND_Indirect_Y = 0x31;

        // ASL
        public const byte ASL_Accumulator = 0x0A;
        public const byte ASL_ZeroPage = 0x06;
        public const byte ASL_ZeroPage_X = 0x16;
        public const byte ASL_Absolute = 0x0E;
        public const byte ASL_Absolute_X = 0x1E;

        // BCC
        public const byte BCC_Relative = 0x90;

        // BCS
        public const byte BCS_Relative = 0xB0;

        // BEQ
        public const byte BEQ_Relative = 0xF0;

        // BIT
        public const byte BIT_ZeroPage = 0x24;
        public const byte BIT_Absolute = 0x2C;

        // BMI
        public const byte BMI_Relative = 0x30;

        // BNE
        public const byte BNE_Relative = 0xD0;

        // BNE
        public const byte BPL_Relative = 0x10;

        // BRK
        public const byte BRK = 0x00;

        // BVC
        public const byte BVC = 0x50;

        // BVS
        public const byte BVS = 0x70;

        // CLC
        public const byte CLC = 0x18;

        // CLD
        public const byte CLD = 0xD8;

        // CLI
        public const byte CLI = 0x58;

        // CLV
        public const byte CLV = 0xB8;

        // CMP
        public const byte CMP_Immediate = 0xC9;
        public const byte CMP_ZeroPage = 0xC5;
        public const byte CMP_ZeroPage_X = 0xD5;
        public const byte CMP_Absolute = 0xCD;
        public const byte CMP_Absolute_X = 0xDD;
        public const byte CMP_Absolute_Y = 0xD9;
        public const byte CMP_Indirect_X = 0xC1;
        public const byte CMP_Indirect_Y = 0xD1;

        // CPX
        public const byte CPX_Immediate = 0xE0;
        public const byte CPX_ZeroPage = 0xE4;
        public const byte CPX_Absolute = 0xEC;

        // CPY
        public const byte CPY_Immediate = 0xC0;
        public const byte CPY_ZeroPage = 0xC4;
        public const byte CPY_Absolute = 0xCC;

        // DEC
        public const byte DEC_ZeroPage = 0xC6;
        public const byte DEC_ZeroPage_X = 0xD6;
        public const byte DEC_Absolute = 0xCE;
        public const byte DEC_Absolute_X = 0xDE;

        // DEX
        public const byte DEX = 0xCA;

        // DEY
        public const byte DEY = 0x88;

        // EOR
        public const byte EOR_Immediate = 0x49;
        public const byte EOR_ZeroPage = 0x45;
        public const byte EOR_ZeroPage_X = 0x55;
        public const byte EOR_Absolute = 0x4D;
        public const byte EOR_Absolute_X = 0x5D;
        public const byte EOR_Absolute_Y = 0x59;
        public const byte EOR_Indirect_X = 0x41;
        public const byte EOR_Indirect_Y = 0x51;

        // INC
        public const byte INC_ZeroPage = 0xE6;
        public const byte INC_ZeroPage_X = 0xF6;
        public const byte INC_Absolute = 0xEE;
        public const byte INC_Absolute_X = 0xFE;

        // INX
        public const byte INX = 0xE8;

        // INY
        public const byte INY = 0xC8;

        // JMP
        public const byte JMP_Absolute = 0x4C;
        public const byte JMP_Indirect = 0x6C;

        // JSR
        public const byte JSR = 0x20;

        // LDA
        public const byte LDA_Immediate = 0xA9;
        public const byte LDA_ZeroPage = 0xA5;
        public const byte LDA_ZeroPage_X = 0xB5;
        public const byte LDA_Absolute = 0xAD;
        public const byte LDA_Absolute_X = 0xBD;
        public const byte LDA_Absolute_Y = 0xB9;
        public const byte LDA_Indirect_X = 0xA1;
        public const byte LDA_Indirect_Y = 0xB1;

        // LDX
        public const byte LDX_Immediate = 0xA2;
        public const byte LDX_ZeroPage = 0xA6;
        public const byte LDX_ZeroPage_Y = 0xB6;
        public const byte LDX_Absolute = 0xAE;
        public const byte LDX_Absolute_Y = 0xBE;

        // LDY
        public const byte LDY_Immediate = 0xA0;
        public const byte LDY_ZeroPage = 0xA4;
        public const byte LDY_ZeroPage_X = 0xB4;
        public const byte LDY_Absolute = 0xAC;
        public const byte LDY_Absolute_X = 0xBC;

        // LSR
        public const byte LSR_Accumulator = 0x4A;
        public const byte LSR_ZeroPage = 0x46;
        public const byte LSR_ZeroPage_X = 0x56;
        public const byte LSR_Absolute = 0x4E;
        public const byte LSR_Absolute_X = 0x5E;

        // NOP
        public const byte NOP = 0xEA;

        // ORA
        public const byte ORA_Immediate = 0x09;
        public const byte ORA_ZeroPage = 0x05;
        public const byte ORA_ZeroPage_X = 0x15;
        public const byte ORA_Absolute = 0x0D;
        public const byte ORA_Absolute_X = 0x1D;
        public const byte ORA_Absolute_Y = 0x19;
        public const byte ORA_Indirect_X = 0x01;
        public const byte ORA_Indirect_Y = 0x11;

        // PHA
        public const byte PHA = 0x48;

        // PHP
        public const byte PHP = 0x08;

        // PLA
        public const byte PLA = 0x68;

        // PLP
        public const byte PLP = 0x28;

        // ROL
        public const byte ROL_Accumulator = 0x2A;
        public const byte ROL_ZeroPage = 0x26;
        public const byte ROL_ZeroPage_X = 0x36;
        public const byte ROL_Absolute = 0x2E;
        public const byte ROL_Absolute_X = 0x3E;

        // ROR
        public const byte ROR_Accumulator = 0x6A;
        public const byte ROR_ZeroPage = 0x66;
        public const byte ROR_ZeroPage_X = 0x76;
        public const byte ROR_Absolute = 0x6E;
        public const byte ROR_Absolute_X = 0x7E;

        // RTI
        public const byte RTI = 0x40;

        // RTS
        public const byte RTS = 0x60;

        // SBC
        public const byte SBC_Immediate = 0xE9;
        public const byte SBC_ZeroPage = 0xE5;
        public const byte SBC_ZeroPage_X = 0xF5;
        public const byte SBC_Absolute = 0xED;
        public const byte SBC_Absolute_X = 0xFD;
        public const byte SBC_Absolute_Y = 0xF9;
        public const byte SBC_Indirect_X = 0xE1;
        public const byte SBC_Indirect_Y = 0xF1;

        // SEC
        public const byte SEC = 0x38;

        // SED
        public const byte SED = 0xF8;

        // SEI
        public const byte SEI = 0x78;

        // STA
        public const byte STA_ZeroPage = 0x85;
        public const byte STA_ZeroPage_X = 0x95;
        public const byte STA_Absolute = 0x8D;
        public const byte STA_Absolute_X = 0x9D;
        public const byte STA_Absolute_Y = 0x99;
        public const byte STA_Indirect_X = 0x81;
        public const byte STA_Indirect_Y = 0x91;

        // STX
        public const byte STX_ZeroPage = 0x86;
        public const byte STX_ZeroPage_X = 0x96;
        public const byte STX_Absolute = 0x8E;

        // STY
        public const byte STY_ZeroPage = 0x84;
        public const byte STY_ZeroPage_X = 0x94;
        public const byte STY_Absolute = 0x8C;

        // TAX
        public const byte TAX = 0xAA;

        // TAY
        public const byte TAY = 0xA8;

        // TSX
        public const byte TSX = 0xBA;

        // TXA
        public const byte TXA = 0x8A;

        // TXS
        public const byte TXS = 0x9A;

        // TYA
        public const byte TYA = 0x98;
    }
}

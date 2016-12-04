using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CPUSim.Source
{
public class CPU
    {
        //  private static CPU instance;
        
        static int NWORDS = 255;
        public byte[] _words = new byte[256];
        private ulong memory_access;
        public enum opcode
        {
            NOP,
            STA,
            LDA,
            ADD,
            OR,
            AND,
            NOT,
            JMP,
            JN,
            JZ,
            HLT,
            INVALID
        }



        private byte _AC; // Acumulador
        public byte _PC; // Program Counter
        private ulong instructionCount; // Contador de instrução
        private bool _N; //Negative Flag
        private bool _Z; //Zero Flag

   /*     private CPU() {

        }
**/
 /*       public static CPU Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CPU();
                }
                return instance;
            }
        } **/
    


          public CPU()
           {
               _AC = 0;
               _PC = 0;
               instructionCount = 0;
               _N = false;
               _Z = false;

               for (int i = 0; i <= NWORDS; i++)
               {
                   _words[i] = 0;
                   memory_access = 0;
               }
           } 
    public  opcode Decode(byte valor)
        {
            byte shiftedWord = (byte)(valor >> 4);
            switch (shiftedWord)
            {
                case 0:
                    return opcode.NOP;
                case 1:
                    return opcode.STA;
                case 2:
                    return opcode.LDA;
                case 3:
                    return opcode.ADD;
                case 4:
                    return opcode.OR;
                case 5:
                    return opcode.AND;
                case 6:
                    return opcode.NOT;
                case 8:
                    return opcode.JMP;
                case 9:
                    return opcode.JN;
                case 10:
                    return opcode.JZ;
                case 15:
                    return opcode.HLT;
                default:
                    return opcode.INVALID;
            }
        }

        public bool Step()
        {
            opcode currentInstruction = Decode(read(_PC));

            _PC = (byte)(_PC + 1);
            
            instructionCount += 1;
            

            switch (currentInstruction)
            {
                case opcode.NOP:
                    break;
                case opcode.STA:
                    write(read(_PC), _AC);
                    _PC = (byte)(_PC + 1);
                    break;
                case opcode.LDA:
                    _AC = read(read(_PC));
                    _PC = (byte)(_PC + 1);
                    break;
                case opcode.ADD:
             //       byte[] conv = new byte[2];
                //    conv[0] = _AC;
                 //    conv[1] = read(read(_PC));
                    // short data = BitConverter.ToInt16(conv,0);

                    //byte g =_AC;
                    //sbyte q =(read(read(_PC));
                    //sbyte t = (sbyte)(g + q);
                    // if (t < 0)
                    int resultADD = _AC + read(read(_PC));
                    _AC = (byte)(resultADD & 0xFF);
                    _PC = (byte)(_PC + 1);
                    updateFLAGS();
                    break;
                case opcode.OR:
                    int resultOR = (_AC | read(read(_PC)));
                    _AC = (byte)resultOR;
                    _PC = (byte)(_PC + 1);
                    updateFLAGS();
                    break;
                case opcode.AND:
                    int resultAND = (_AC & read(read(_PC)));
                    _AC = (byte)resultAND;
                    _PC = (byte)(_PC + 1);
                    updateFLAGS();
                    break;
                case opcode.NOT:
                    int resultNOT = ~(_AC);
                    _AC = (byte)resultNOT;
                    updateFLAGS();
                    break;
                case opcode.JMP:
                    _PC = (byte)(read(_PC));
                    break;
                case opcode.JN:
                    if (_N == true)
                        _PC = read(_PC);
                    else
                    {
                        read(_PC); 
                        _PC = (byte)(_PC + 1);
                    }
                    break;
                case opcode.JZ:
                    if (_Z == true)
                        _PC = read(_PC);
                    else
                    {
                        read(_PC); 
                        _PC = (byte)(_PC + 1);
                    }
                    break;
                case opcode.HLT:
                    return false;
                case opcode.INVALID:
                    break;
            }
            if (_PC >= 255)
            {
              //  _PC = 0;
                return false;
            }
            return true;
        }
        public void updateFLAGS()
        {
            if (_AC == 0)
                _Z = true;
            else
                _Z = false;

            if ((_AC >> 7) == 1)
                _N = true;
            else
                _N = false;
        }
        public void Stop() {
            _AC = 0;
            _PC = 0;
            instructionCount = 0;
            _N = false;
            _Z = true;
            memory_access = 0;

        }
        public void Run()
        {
            while (Step()) ;
        }
        public byte GetAC()
        {
            return _AC;
        }

        public byte GetPC()
        {
            return _PC;
        }

       public ulong GetInstructionCount()
        {
            return instructionCount;
        }
        public bool GetN()
        {
            return _N;
        }
        public bool GetZ()
        {
            return _Z;
        }
        public ulong GetMemoryAcesses()
        {
            return memory_access;
        }

        public byte read(byte address)
        {
            memory_access = memory_access + 1;
            return _words[address];
        }
        public void write(byte address, byte value)
        {
            memory_access = memory_access + 1;
            _words[address] = value;
        }
        
    }
}

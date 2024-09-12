using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class BaseIcd
    {
        public int Id { get; set; }
        public int Location { get; set; }
        public string Name { get; set; }
        public byte Mask { get; set; } 
        public int StartBit { get; set; }
        public int Bit { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

    }
}

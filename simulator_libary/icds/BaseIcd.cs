using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class BaseIcd
    {
        public int Location { get; set; }
        public string Mask { get; set; }
        public int Size { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

    }
}

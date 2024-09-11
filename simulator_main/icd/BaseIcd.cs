﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class BaseIcd
    {
        public int Size { get; set; }
        public int Location { get; set; }
        public byte Mask { get; set; } 
        public int Min { get; set; }
        public int Max { get; set; }

    }
}

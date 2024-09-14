using simulator_main.icd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class MaskIcd : BaseIcd
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartBit { get; set; }
        public int Bit { get; set; }
    }
}

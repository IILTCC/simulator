using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class FiberBoxIcd : BaseIcd
    {
        public int Id { get; set; }
        public string Error { get; set; }
        public int CorrValue { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public string Units { get; set; }
        public int PhysicalLimitMin { get; set; }
        public int PhysicalLimitMax { get; set; }
        public int PhysicalLimitDef { get; set; }
        public string InterfaceType { get; set; }
        public int Length { get; set; }
        public string Enum { get; set; }

    }
}

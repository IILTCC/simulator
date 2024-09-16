using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class FiberBoxIcd : BaseIcd
    {
        public int Location { get; set; }
        public string Mask { get; set; }
        public int Size { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
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

        public override int GetLocation() { return this.Location; }
        public override string GetMask() { return this.Mask; }
        public override int GetSize() { return this.Size; }
        public override int GetMin() { return this.Min; }
        public override int GetMax() { return this.Max; }
        public override string GetName() { return this.Identifier; }
        public override int GetCorrValue() { return this.CorrValue; }
    }
}

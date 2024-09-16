using simulator_main.icd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class FlightBoxIcd : BaseIcd
    {
        public int Location { get; set; }
        public string Mask { get; set; }
        public int Bit { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartBit { get; set; }

        public override int GetLocation() { return this.Location; }
        public override string GetMask() { return this.Mask; }
        public override int GetSize() { return this.Bit; }
        public override int GetMin() { return this.Min; }
        public override int GetMax() { return this.Max; }

    }
}

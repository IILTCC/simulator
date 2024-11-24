using simulator_libary.icds;

namespace simulator_libary
{
    public class FlightBoxIcd : BaseBoxIcd
    {
        public int Location { get; set; }
        public string Mask { get; set; }
        public int Bit { get; set; }
        public string Name { get; set; }
        public string StartBit { get; set; }

        public override int GetLocation() { return this.Location; }
        public override string GetMask() { return this.Mask; }
        public override int GetSize() { return this.Bit; }
        public override string GetName(){return string.Empty;}
        public override int GetCorrValue(){return -1;}
        public override string  GetError() {return string.Empty;}
        public override bool IsRowCorIdentifier() { return false; }
    }
}

namespace simulator_libary
{
    public interface IParameterIcd
    {
        public int GetRowId();
        public int GetLocation();
        public string GetMask();
        public int GetSize();
        public int GetMin();
        public int GetMax();
        public string GetName();
        public int GetCorrValue();
        public string GetError();
        public bool IsRowCorIdentifier();
    }
}

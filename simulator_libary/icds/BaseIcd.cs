using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class BaseIcd
    {
        public virtual int GetLocation() { return -1; }
        public virtual string GetMask() { return string.Empty; }
        public virtual int GetSize() { return -1; }
        public virtual int GetMin() { return -1; }
        public virtual int GetMax() { return -1; }
        public virtual string GetName() { return string.Empty; }
        public virtual int GetCorrValue() { return -1; }

    }
}

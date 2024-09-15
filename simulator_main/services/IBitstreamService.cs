using simulator_main.icd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public string GetPacketData(string icdName);

    }
}

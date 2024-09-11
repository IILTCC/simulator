using simulator_main.icd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.services
{
    public class BitstreamService : IBitstreamService
    {
        private Dictionary<int, string> IcdDictionary;
        public BitstreamService()
        {
            this.IcdDictionary = new Dictionary<int, string>();
            IcdDictionary.Add(0,"mask1.json");
            IcdDictionary.Add(0,"mask2.json");
            IcdDictionary.Add(0,"cor1.json");
            IcdDictionary.Add(0,"cor2.json");
        }
        public async Task<string> GetBitstream(int icdId)
        {

            StreamReader icdFil = File.OpenText("../ice_repo/"+IcdDictionary[icdId]);
            return "";
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class GenericIcd<icd>
    {
        public GenericIcd()
        {

        }
        public async Task<string> GenerateBitStream(string json)
        {
            List<icd> items = JsonConvert.DeserializeObject<List<icd>>(json);

        }
            
            
    }
}

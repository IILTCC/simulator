using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private Dictionary<int, Type> IcdDictionary;
        public BitstreamService()
        {
            this.IcdDictionary = new Dictionary<int, Type>();
            IcdDictionary.Add(0,typeof(BaseIcd));
            IcdDictionary.Add(1,typeof(CorellatorIcd));
   
        }
        public async Task<string> GetBitstream(int icdId)
        {
            string text = File.ReadAllText("./icd_repo/" + IcdDictionary[icdId]);
            //Console.WriteLine(JToken.Parse(text));
            foreach(var item in JToken.Parse(text))
            {
                Console.WriteLine(item);
                Console.WriteLine();
            }
            /*
            using (StreamReader file = File.OpenText("./icd_repo/" + IcdDictionary[icdId]))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JArray parameter = (JArray)(JToken.ReadFrom(reader));
                //Console.WriteLine(o2);
                foreach(var item in parameter)
                {
                    Console.WriteLine(JsonConvert.DeserializeObject<BaseIcd>(item));
                    Console.WriteLine(item);
                }
            }
            */
        }
    }
}

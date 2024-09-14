using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using simulator_main;
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
        private Dictionary<int, (Type,string)> IcdDictionary;
        public BitstreamService()
        {
            this.IcdDictionary = new Dictionary<int, (Type,string)>();
            IcdDictionary.Add(0,(typeof(BaseIcd),"mask1.json"));
            IcdDictionary.Add(1,(typeof(BaseIcd),"mask2.json"));
            IcdDictionary.Add(2,(typeof(CorellatorIcd),"cor1.json"));
            IcdDictionary.Add(3,(typeof(CorellatorIcd),"cor2.json"));
   
        }
        public string GetPacketData(int icdId)
        {
            if (!IcdDictionary.ContainsKey(icdId))
                return "400";

            string jsonText = File.ReadAllText("./icd_repo/" + IcdDictionary[icdId].Item2);

            // get the icd generic type at run time
            Type genericIcdType = typeof(GenericIcd<>).MakeGenericType(IcdDictionary[icdId].Item1);
            // construct the generic icd at runtime
            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            return icdInstance.GeneratePacketBitData(jsonText);

        }
    }
}

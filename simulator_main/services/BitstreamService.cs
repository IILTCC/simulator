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
        private Dictionary<string, Type> IcdDictionary;
        const string ICD_REPO_PATH = "./icd_repo/";
        const string ICD_FILE_TYPE = ".json";
        public BitstreamService()
        {
            this.IcdDictionary = new Dictionary<string, Type>();
            IcdDictionary.Add("FlightBoxDownIcd", typeof(FlightBoxIcd));
            IcdDictionary.Add("FlightBoxUpIcd", typeof(FlightBoxIcd));
            IcdDictionary.Add("FiberBoxDownIcd", typeof(FiberBoxIcd));
            IcdDictionary.Add("FiberBoxUpIcd", typeof(FiberBoxIcd));
        }
        public string GetPacketData(string icdName)
        {
            if (!IcdDictionary.ContainsKey(icdName))
                return "400";

            string jsonText = File.ReadAllText(ICD_REPO_PATH + icdName + ICD_FILE_TYPE);

            // get the icd generic type at run time
            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdName]);
            // construct the generic icd at runtime
            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            return icdInstance.GeneratePacketBitData(jsonText);

        }
    }
}

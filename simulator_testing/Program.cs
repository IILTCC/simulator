using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using simulator_main;
using simulator_main.icd;

namespace simulator_testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(JToken.Parse(text));
            run ();
         }
        public static void run()
        {
            GenericIcd<MaskIcd> temp = new GenericIcd<MaskIcd>();
            
            string text = File.ReadAllText(@"C:\Users\user\Desktop\miniproj\mainproject\simulator\simulator_testing\icds\mask1.json");
            temp.GeneratePacketBitData(text);

        }
    }
}

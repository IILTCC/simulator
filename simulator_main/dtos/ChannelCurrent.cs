using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.Dtos
{
    public class ChannelCurrent
    {
        public int Id { get; set; }
        public int Delay { get; set; }
        public int Error { get; set; }
        public ChannelCurrent(int delay,int error , int id)
        {
            Delay = delay;
            Error = error;
            Id = id;
        }
        
    }
}

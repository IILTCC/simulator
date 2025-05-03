using simulator_libary.icds;
using System.Collections.Generic;

namespace simulator_main.Dtos
{
    public class GetChannelsCurrentDto
    {
        public Dictionary<IcdTypes, ChannelCurrent> ChannelsCurrent { get; set; }
        public GetChannelsCurrentDto(Dictionary<IcdTypes, ChannelCurrent> channelsCurrent)
        {
            ChannelsCurrent = channelsCurrent;
        }
    }
}

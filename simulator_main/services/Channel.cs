using simulator_libary;
using simulator_libary.generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace simulator_main.services
{
    public class Channel
    {
        public CancellationTokenSource ChannelTokenSource { get; set; }
        public CancellationToken ChannelToken { get; set; }
        public IGeneratePacketBit PacketGenerator { get; set; }
        public Channel(IGeneratePacketBit packetGenerator)
        {
            PacketGenerator = packetGenerator;
        }
        public Channel(CancellationTokenSource channelTokenSource, CancellationToken channelToken, IGeneratePacketBit packetGenerator)
        {
            ChannelToken = channelToken;
            ChannelTokenSource = channelTokenSource;
            PacketGenerator = packetGenerator;
        }
    }
}

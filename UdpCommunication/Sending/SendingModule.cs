using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpCommunication.Sending
{
    class SendingModule
    {
    }

    public class UdpMessage
    {
        public List<UdpPacket> Packets { get; set; }
    }

    internal class AcknowledgeWaiting
    {
        
    }
}

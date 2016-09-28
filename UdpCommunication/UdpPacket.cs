using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpCommunication
{
    public class UdpPacket
    {
        public ushort SequenceNumber { get; set; }
        public byte[] Body { get; set; }

        public DateTime? SendTime { get; set; }
        public DateTime? ConfirmationTime { get; set; }

    }
}

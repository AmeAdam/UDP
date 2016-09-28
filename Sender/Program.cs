using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MSR.LST;
using MSR.LST.Net.Rtp;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var ep = new IPEndPoint(IPAddress.Parse("192.168.0.11"), 5000);
            var ep2 = new IPEndPoint(IPAddress.Parse("192.168.0.103"), 5000);
            var name = Dns.GetHostName();
            var rtpSession = new RtpSession(ep, new RtpParticipant(name, name), true, true);
            var rtpSender = rtpSession.CreateRtpSenderFec(name, PayloadType.FileTransfer, null, 0, 200);
            var bytes = File.ReadAllBytes(@"img.jpg");
            //var bytes = Encoding.UTF8.GetBytes("sdfskjfh sskdjfh vskfskdjfhks hfksdfksdhfks dfksjdehfskhfks d");
            Thread.Sleep(2000);
            rtpSender.Send(bytes);
        }
    }
}

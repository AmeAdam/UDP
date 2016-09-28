using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MSR.LST.Net.Rtp;

namespace Receiver
{
    class Program
    {
        private static RtpSession rtpSession;
        private static RtpSender rtpSender;

        static void Main(string[] args)
        {
            var ep = new IPEndPoint(IPAddress.Parse("192.168.0.103"), 5000);
            var ep2 = new IPEndPoint(IPAddress.Parse("192.168.0.11"), 5000);
            var name = Dns.GetHostName();

            RtpEvents.RtpParticipantAdded += RtpParticipantAdded;
            RtpEvents.RtpParticipantRemoved += RtpParticipantRemoved;
            RtpEvents.RtpStreamAdded += RtpStreamAdded;
            RtpEvents.RtpStreamRemoved += RtpStreamRemoved;

            rtpSession = new RtpSession(ep, new RtpParticipant(name, name), true, true);
            rtpSender = rtpSession.CreateRtpSenderFec(name, PayloadType.FileTransfer, null, 0, 200);

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void RtpParticipantRemoved(object sender, RtpEvents.RtpParticipantEventArgs ea)
        {
        }

        private static void RtpParticipantAdded(object sender, RtpEvents.RtpParticipantEventArgs ea)
        {
        }

        private static void RtpStreamRemoved(object sender, RtpEvents.RtpStreamEventArgs ea)
        {
            ea.RtpStream.FrameReceived -= FrameReceived;
        }

        private static void RtpStreamAdded(object sender, RtpEvents.RtpStreamEventArgs ea)
        {
            ea.RtpStream.FrameReceived += FrameReceived;
        }

        private static void FrameReceived(object sender, RtpStream.FrameReceivedEventArgs ea)
        {
            Console.WriteLine("received file "+ ea.Frame.Buffer.Length+ " from "+ea.RtpStream.Properties.CName);
            File.WriteAllBytes("received.jpg", ea.Frame.Buffer);
        }
    }
}

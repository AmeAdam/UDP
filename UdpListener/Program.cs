using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace UdpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            using var subscription = UdpStream().Select(Deserialize).Subscribe(MessageReceived);
            Console.ReadLine();
        }
        private static void MessageReceived(string message)
        {
            Console.WriteLine(message);
        }

        private static string Deserialize(UdpReceiveResult packet)
        {
            var txt = Encoding.ASCII.GetString(packet.Buffer);
            return string.Format($"Received: {txt} from {packet.RemoteEndPoint.Address} at {DateTime.Now}");
        }

        private static IObservable<UdpReceiveResult> UdpStream()
        {
            return Observable.Using(
                () =>
                {
                    var listener = new UdpClient(9999);
                    listener.JoinMulticastGroup(IPAddress.Parse("224.0.0.1"));
                    return listener;
                },
                udpClient =>
                    Observable.Defer(() => Observable.FromAsync(udpClient.ReceiveAsync)).Repeat()
            );
        }
    }
}

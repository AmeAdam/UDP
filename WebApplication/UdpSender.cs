using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication
{
    internal class UdpSender : IHostedService
    {
        private IDisposable timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(SendNotification);
            return Task.CompletedTask;
        }

        private void SendNotification(long obj)
        {
            var udpSender = new UdpClient();
            var message = Encoding.ASCII.GetBytes("WebApplication is OK.");
            udpSender.Send(message, message.Length, new IPEndPoint(IPAddress.Parse("224.0.0.1"), 9999));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
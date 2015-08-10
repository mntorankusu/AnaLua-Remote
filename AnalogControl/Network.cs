using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/*namespace AnalogControl
{
    internal class Network
    {
        private readonly UdpClient udp;
        private IPEndPoint _ip;

        public Network()
        {
            _ip = new IPEndPoint(IPAddress.Loopback, 3478);
            udp = new UdpClient(_ip);
            udp.Connect(_ip);
            //udp.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.Broadcast, true);
            //udp.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.ReuseAddress, true);
        }

        public List<byte[]> Receive()
        {
            var messages = new List<byte[]>();
            var udpbuf = new byte[0];
            while (udp.Available > 0)
            {
                try
                {
                    udpbuf = udp.Receive(ref _ip);
                    messages.Add(udpbuf);
                }
                catch
                {
                    Console.WriteLine("UDP Receive Error");
                }
            }

            return messages;
        }

        public bool Send(byte[] message)
        {
            if (udp.Send(message, message.Length) > 0)
            {
                return true;
            }
            return false;
        }
    }
}*/
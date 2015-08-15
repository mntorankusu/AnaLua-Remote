using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace AnalogControl
{
    internal class Network
    {
        private UdpClient _udp;
        private IPEndPoint _ip;

        public Network(string ip, UInt16 port)
        {
            _ip = new IPEndPoint(IPAddress.Parse(ip), port);
            _udp = new UdpClient(_ip);
            Console.WriteLine("Started UDP");
        }

        public void Close()
        {
            _udp.Close();
            Console.WriteLine("Closed UDP");
        }

        public List<byte[]> Receive()
        {
            var messages = new List<byte[]>();
            var udpbuf = new byte[0];
            while (_udp.Available > 0)
            {
                try
                {
                    udpbuf = _udp.Receive(ref _ip);
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
            if (_udp.Send(message, message.Length, _ip) > 0)
            {
                return true;
            } else
            {
                Console.WriteLine("UDP Send Error");
            }
            return false;
        }
    }
}
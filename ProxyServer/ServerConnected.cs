using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ProxyServer
{
    public class ServerConnected
    {

        public string data = null;

        List<Connection> connections = new List<Connection>();

        public ServerConnected()
        {
            byte[] bytes = new Byte[1024];


            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8888);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen(100);

            new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = socket.Accept();
                    data = null;

                    var connection = new Connection(handler);
                    connection.OnPacketReceived += Connection_OnPacketReceived;
                    connections.Add(connection);
                }
            }).Start();




        }

        private void Connection_OnPacketReceived(Packet packet)
        {
            Console.WriteLine(packet.StringData);
        }
    }



}

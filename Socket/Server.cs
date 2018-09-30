using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class SocketServer
    {
        private TcpListener _serverSocket;
        private TcpClient _clientSocket;
        Thread _th;

        public SocketServer()
        {
            _serverSocket = new TcpListener(IPAddress.Any, 8888);
            _clientSocket = default(TcpClient);


            _th = new Thread(Loop);
        }

        public void Start()
        {
            _th.Start();
        }

        public void Stop()
        {
            _th.Interrupt();
            _serverSocket.Stop();

        }

        private void Loop()
        {
            _serverSocket.Start();
            _clientSocket = _serverSocket.AcceptTcpClient();

            System.Net.Socket listener = new System.Net.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            while (true)
            {
                try
                {
                    NetworkStream networkStream = _clientSocket.GetStream();

                    byte[] bytesFrom = new byte[4096];
                    int byteRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);

                    string dataFromClient = Encoding.ASCII.GetString(bytesFrom, 0, byteRead);


                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                    /*string serverResponse = "Last Message from client" + dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    Console.WriteLine(" >> " + serverResponse);*/

                    networkStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        private byte[] Fusion(params byte[][] byteArrays)
        {
            List<byte> output = new List<byte>();

            foreach (var byteArray in byteArrays)
                output.AddRange(byteArray);

            return output.ToArray();
        }

        private byte[] Random(int len)
        {
            Random rnd = new Random();
            byte[] rands = new byte[len];
            for (int i = 0; i < len; i++)
                rands[i] = (byte)rnd.Next();

            return rands;
        }


    }
}

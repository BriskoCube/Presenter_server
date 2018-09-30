using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        private TcpListener _serverSocket;
        private TcpClient _clientSocket;
        Thread _th;

        public Server()
        {
            _serverSocket = new TcpListener(8888);
            _clientSocket = default(TcpClient);

            _serverSocket.Start();
            _clientSocket = _serverSocket.AcceptTcpClient();

            _th = new Thread(Loop);
        }

        public void Start()
        {
            _th.Start();
        }

        public void Stop()
        {
            _th.Interrupt();
        }

        private void Loop()
        {
            while (true)
            {
                try
                {
                    NetworkStream networkStream = _clientSocket.GetStream();

                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, _clientSocket.ReceiveBufferSize);

                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                    string serverResponse = "Last Message from client" + dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }



    }
}

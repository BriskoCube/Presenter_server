using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ProxyServer
{
    public class Server
    {

        private IPEndPoint _localEndPoint;
        Thread _th;

        public static ManualResetEvent allDone = new ManualResetEvent(false);


        public Server()
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, 8888);
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
            Socket listener = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(_localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),  listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
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

        public static void ReadCallback(IAsyncResult ar)
        {
            string content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                int startIndex = 0;

                if (state.expectedSize == 0)
                {
                    byte[] intBytes = new byte[] { state.buffer[0], state.buffer[1], state.buffer[2], state.buffer[3] };
                    state.dataType = (DataType)state.buffer[4];


                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(intBytes);

                    state.expectedSize = BitConverter.ToInt32(intBytes);

                    // Remove header from string parse
                    startIndex = 5;
                }

                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, startIndex, bytesRead - startIndex));

                content = state.sb.ToString();

                if (content.Length >= state.expectedSize)
                {

                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket.", content.Length);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                }
            }
        }

        private static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // State object for reading client data asynchronously  
        public class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 8192;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();

            public int expectedSize = 0;

            public DataType dataType = DataType.None;
        }

        public enum DataType
        {
            None,
            Message,
            Text,
            Image
        }

    }
}

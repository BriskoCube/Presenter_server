using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ProxyServer
{
    public class Connection
    {
        private byte[] _buffer = new byte[8192];
        private Socket _socket = null;

        private byte[] _received;

        private Thread _receiveTh = null;

        private bool _alive = true;

        private Packet _currentPacket = null;

        public delegate void PacketReceived(Packet packet);
        public event PacketReceived OnPacketReceived;


        public Connection(Socket socket)
        {
            _socket = socket;
            _receiveTh = new Thread(ReceiveLoop);
            _receiveTh.Start();
        }

        private void ReceiveLoop()
        {
            while (_alive)
            {
                int bytesRec = _socket.Receive(_buffer);

                if (_currentPacket == null)
                    _currentPacket = new Packet();

                if(_currentPacket.AddFragment(_buffer, bytesRec))
                {
                    OnPacketReceived?.Invoke((Packet)_currentPacket.Clone());
                    _currentPacket = null;
                }
            }
        }

        public void Send(byte[] msg)
        {
            _socket.Send(msg);
        }

        public void Close()
        {
            _alive = false;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}

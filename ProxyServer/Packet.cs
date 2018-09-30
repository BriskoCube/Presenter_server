using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyServer
{
    public class Packet: ICloneable
    {
        private int _length = -1;
        private PacketType _type = PacketType.None;
        private int _fragmentCount = 0;
        private string _stringData = "";

        public string StringData { get => _stringData; set => _stringData = value; }

        public Packet()
        {
            Groups.Add("dsffsd", this);
        }

        public Packet(Packet packet)
        {
            _length = packet._length;
            _type = packet._type;
            _fragmentCount = packet._fragmentCount;
            _stringData = packet._stringData;

        }

        public bool AddFragment(byte[] buffer, int nbRead)
        {
            _fragmentCount++;

            if (_length < 0)
                ReadHeader(buffer, nbRead);

            switch(_type)
            {
                case PacketType.Text:
                    _stringData += Encoding.ASCII.GetString(buffer, 0, nbRead);
                    break;
            }

            return _stringData.Length >= _length;
        }

        public void ReadHeader(byte[] buffer, int nbRead)
        {
            if (_fragmentCount != 1)
                throw new NoHeaderException();

            if (nbRead >= 5)
            {
                byte[] intBytes = GetFromArray(buffer, 4);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);

                _length = BitConverter.ToInt32(intBytes);
                _type = (PacketType)buffer[4];
            }
            else
            {
                throw new NoHeaderException();
            }
        }

        public static T[] GetFromArray<T>(T[] array, int length, int offset = 0)
        {
            if (length + offset > array.Length)
                throw new ArgumentOutOfRangeException();

            T[] extracted = new T[length];

            for(int i = 0; i < length; i++)
            {
                extracted[i] = array[i + offset];
            }

            return extracted;
        }

        public object Clone()
        {
            return new Packet(this);
        }
    }

    public class NoHeaderException : Exception
    {
        public NoHeaderException()
        {
        }

        public NoHeaderException(string message) : base(message)
        {
        }
    }

    public enum PacketType
    {
        None,
        ACK,
        Command,
        Text,
        Img,
    }
}

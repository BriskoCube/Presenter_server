using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ProxyServer
{
    static class Groups
    {
        private static List<Group> _groups = new List<Group>();

        public static void Add(string gid, Packet packet)
        {
            Group gpr = _groups.Find(g => g.Gid == gid);
            if (gpr == null)
            {
                gpr = new Group(gid, packet);
                _groups.Add(gpr);
            }
            else
            {
                gpr.Add(packet);
            }
        }

        public static Packet At(string gid, Packet sender)
        {
            foreach (var packet in _groups.Find(g => g.Gid == gid).Packets)
                if (packet != sender)
                    return packet;

            return null;
        }

        private class Group
        {
            private string _gid;
            private List<Packet> _packets;

            public Group(string gid, Packet packet)
            {
                _gid = gid;
                Add(packet);
            }

            public void Add(Packet packet)
            {
                _packets.Add(packet);
            }

            public string Gid { get => _gid; set => _gid = value; }
            public List<Packet> Packets { get => _packets; set => _packets = value; }
        }

    }
}

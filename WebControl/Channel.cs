using System;
using System.Net;

namespace WebControl
{
    public class Channel
    {
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        public Room Room { get; set; }

        public Actor Actor { get; internal set; }

        internal Channel(Room room, int id, string name)
        {
            Id = id;
            Name = name;
            Room = room;
        }

        public ChannelState EnsureReady()
        {
            return Room.Source.EnsureReady(this);
        }
    }
}


using System;
namespace WebControl
{
    public class Room
    {
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        public Endpoint Source { get; internal set; }

        private ICollection<Channel> channels = null;

        public ICollection<Channel> Channels
        {
            get
            {
                if (channels is null)
                    channels = Source.GetChannels(this);
                return channels;
            }

            internal set
            {
                channels = value;
            }
        }

        internal Room(Endpoint endpoint, int id, string name) 
        {
            Source = endpoint;
            Id = id;
            Name = name;
        }


    }
}


using System;
namespace WebControl.Protocol
{
    public class ProtocolParameter
    {
        public const int COMMAND_HEADER = 0x90;

        public ProtocolCommand Command { get; private set; }

        public int RoomIndex { get; private set; }

        public int? ChannelIndex { get; private set; }

        public ProtocolOperation Operation { get; private set; }

        public int[] Values { get; private set; }

        private Endpoint target;

        internal ProtocolParameter(Endpoint endpoint, int roomIndex, int? channelIndex, ProtocolCommand command, ProtocolOperation operation, params int[] values)
        {
            target = endpoint;
            Command = command;
            Operation = operation;
            RoomIndex = roomIndex;
            ChannelIndex = channelIndex;
            Values = values;
        }

        public override string ToString()
        {
            return String.Format(
                "{0:X2}{1:X2}{2:0000}{3:00}{4:00}{5:X2}{6}",
                COMMAND_HEADER,
                target.LastCommandIndex,
                (int)Command,
                RoomIndex,
                ChannelIndex,
                Operation == ProtocolOperation.Generic ? null : (int)Operation,
                string.Join(null, Values.Select<int, string>(v => string.Format("{0:X}", v)))
                ).ToLower();
        }
    }
}


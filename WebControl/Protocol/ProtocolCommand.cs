using System;
namespace WebControl.Protocol
{
    public enum ProtocolCommand
    {
        GetRoomName = 203,
        GetChannelInfo = 347,
        GetState = 323,
        GetBlindState = 431,
        SetBlindState = 821
    }
}


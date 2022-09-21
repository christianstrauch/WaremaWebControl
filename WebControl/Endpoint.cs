namespace WebControl;
using Protocol;
using System.Threading.Channels;
using System.Xml;

public class Endpoint
{
    /// <summary>
    /// Maximum amount of rooms to scan for
    /// </summary>
    public const int MAX_ROOMS = 100;

    /// <summary>
    /// Maximum amount of channels per room to scan for
    /// </summary>
    public const int MAX_CHANNELS_PER_ROOM = 100;

    /// <summary>
    /// Maximum age in seconds of product info. After that, products will auto-update.
    /// </summary>
    public const short MAX_PRODUCT_INFO_AGE = 30;

    /// <summary>
    /// Pause, in milliseconds, between retries to get ready status from a channel
    /// </summary>
    public const short PAUSE_BETWEEN_RETRIES = 300;

    /// <summary>
    /// Increment to be added to the pause after each retry
    /// </summary>
    public const short PAUSE_BETWEEN_RETRIES_INCREMENT = 200;

    /// <summary>
    /// Timeout, in milliseconds, for requests - including all retries
    /// </summary>
    public const short REQUEST_TIMEOUT = 3000;

    /// <summary>
    /// The address of the Warema WMS WebControl
    /// </summary>
    public Uri Address { get; private set; }

    /// <summary>
    /// The last command index used in a command Id.
    /// </summary>
    public int LastCommandIndex { get; private set; } = -1;

    public long AccessIdBase { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;

    /// <summary>
    /// Creates a new instance of the WebControl endpoint
    /// </summary>
    /// <param name="url">The WebControl address</param>
    public Endpoint(string url)
    {
        UriBuilder uriBuilder = new UriBuilder(url);
        uriBuilder.Path = "protocol.xml";
        Address = uriBuilder.Uri;


    }

    private Uri createUri(ProtocolParameter parameter)
    {
        var uriBuilder = new UriBuilder(Address);
        var commandId = AccessIdBase + 700 + ++LastCommandIndex;
        uriBuilder.Query = string.Format(
            "protocol={0}&_={1}",
            parameter,
            commandId
            );
        return uriBuilder.Uri;
    }


    private ProtocolResult request(ProtocolParameter parameter)
    {
        var requestUri = createUri(parameter);
        return ProtocolResult.Request(this, requestUri);
    }



    public ICollection<Room> GetRooms()
    {
        var rooms = new List<Room>();
        for (var roomIndex = 0; roomIndex < MAX_ROOMS; roomIndex++)
        {
            var parameter = new ProtocolParameter(this, roomIndex, null, ProtocolCommand.GetRoomName, ProtocolOperation.Generic);
            var room = request(parameter).ReadRoom();

            if (room is null)
                break;
            else
                rooms.Add(room);
        }
        return rooms;
    }

    public ICollection<Channel> GetChannels(Room room)
    {
        var channels = new List<Channel>();
        for (var channelIndex = 0; channelIndex < MAX_CHANNELS_PER_ROOM; channelIndex++)
        {
            var parameter = new ProtocolParameter(this, room.Id, channelIndex, ProtocolCommand.GetChannelInfo, ProtocolOperation.Generic);
            var channel = request(parameter).ReadChannel(room);

            if (channel is null)
                break;
            else
                channels.Add(channel);
        }
        room.Channels = channels;
        return channels;
    }

    public ChannelState EnsureReady(Channel channel)
    {
        return EnsureReady(channel.Id, channel.Room.Id);
    }

    public ChannelState EnsureReady(int channelId, int roomId)
    {
        var parameter = new ProtocolParameter(this, roomId, channelId, ProtocolCommand.GetState, ProtocolOperation.Generic);
        return request(parameter).ReadChannelState();
    }

    public ProductState GetProductState(Channel channel)
    {
        return TryGetReadyFor<ProductState, Channel>(
            c =>
            {
                var parameter = new ProtocolParameter(this, c.Room.Id, c.Id, ProtocolCommand.GetBlindState, ProtocolOperation.Blinds);
                return request(parameter).ReadProduct(c);
            }, channel);
    }

    public void Move(Actor actor, decimal? position, short? angle)
    {
        TryGetReadyFor<bool, Actor>(
            a =>
            {
                var parameter = new ProtocolParameter(
                    this,
                    a.Channel.Room.Id,
                    a.Channel.Id,
                    ProtocolCommand.SetBlindState,
                    ProtocolOperation.Go,
                    position == null ? 255 : Convert.ToInt16(position * 200),
                    angle == null ? 255 : Convert.ToInt16(angle + 127),
                    255,
                    255
                );
                return request(parameter).ReadSetState(a);
            }, actor);
    }

    private TResult TryGetReadyFor<TResult, TInput>(Func<TInput, TResult> action, TInput input, Channel channel = null)
    {
        ChannelState state = ChannelState.Busy;
        TResult result = default(TResult);
        bool hasResult = false;

        if (channel is null && input is Channel)
            channel = input as Channel;

        int roomId = channel is null ? 0 : channel.Room.Id;
        int channelId = channel is null ? 0 : channel.Id;
        int pause = PAUSE_BETWEEN_RETRIES;
        DateTime start = DateTime.UtcNow;
        while (state == ChannelState.Busy || (state != ChannelState.Unknown && !hasResult))
        {
            try
            {
                state = EnsureReady(channelId, roomId);
                Thread.Sleep(pause);
                result = action(input);
                hasResult = true;
            }
            catch (ResponseErrorException e)
            {
                hasResult = false;
                if ((DateTime.UtcNow - start).TotalMilliseconds > REQUEST_TIMEOUT)
                    throw new TimeoutException("Timeout expired attempting to ensure channel is ready.", e);
            }
            pause += PAUSE_BETWEEN_RETRIES_INCREMENT;
        }

        state = EnsureReady(channelId, roomId);
        Thread.Sleep(100);

        return result;
    }

}


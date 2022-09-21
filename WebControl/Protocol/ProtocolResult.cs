using System;
using System.IO;
using System.Xml;
using WebControl.Products;
namespace WebControl.Protocol;

public class ProtocolResult
{
    public Endpoint Source { get; private set; }

    public XmlDocument Xml { get; private set; }

    public ProtocolResult(Endpoint source, XmlDocument xml)
    {
        Source = source;
        Xml = xml;
    }

    public static ProtocolResult Request(Endpoint source, Uri requestUri)
    {

        XmlDocument xml = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            CloseInput = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true
        };
        using (XmlReader reader = XmlReader.Create(requestUri.ToString(), settings))
        {
            xml.Load(reader);
            reader.Close();
        }
        return new ProtocolResult(source, xml);
    }

    public Room ReadRoom()
    {
        var nameX = Xml.SelectSingleNode("/response/raumname");
        if (nameX is null || string.IsNullOrEmpty(nameX.InnerText))
        {
            return null;
        }
        else
        {
            var idX = Xml.SelectSingleNode("/response/raumindex");
            return new Room(Source, int.Parse(idX.InnerText), nameX.InnerText);
        }
    }

    public Channel ReadChannel(Room room)
    {
        var children = Xml.SelectNodes("/response[kanalname != '']/*").OfType<XmlElement>();
        if (children is null || children.Count() == 0)
        {
            return null;
        }
        else
        {
            var values = children.ToDictionary<XmlElement, string, string>(c => c.LocalName, c => c.InnerText);
            var channel = new Channel(room, int.Parse(values["kanalindex"]), values["kanalname"]);
            Actor actor;
            switch (int.Parse(values["produkttyp"]))
            {
                case VenetianBlind.PRODUCT_TYPE:
                    actor = new VenetianBlind(channel, int.Parse(values["bedientyp"]));
                    break;
                case Awning.PRODUCT_TYPE:
                    actor = new Awning(channel, int.Parse(values["bedientyp"]));
                    break;
                case Louvre.PRODUCT_TYPE:
                    actor = new Louvre(channel, int.Parse(values["bedientyp"]));
                    break;
                case Shutter.PRODUCT_TYPE:
                    actor = new Shutter(channel, int.Parse(values["bedientyp"]));
                    break;
                case Scene.PRODUCT_TYPE:
                    actor = new Scene(channel, int.Parse(values["szeneindex"]));
                    break;
                default:
                    throw new NotImplementedException(string.Format("Unknown product type {0} in response:\n\n{1}", int.Parse(values["produkttyp"])));
            }
            return actor.Channel;
        }
    }

    public ChannelState ReadChannelState()
    {
        var stateX = Xml.SelectSingleNode("/response/feedback");
        if (stateX is null)
            return ChannelState.Unknown;

        return (ChannelState)int.Parse(stateX.InnerText);
    }

    public ProductState ReadProduct(Channel channel)
    {
        if (channel.Actor is Scene)
            throw new InvalidOperationException("Product info can only be retrieved for channels that control hardware. This channel does not.");

        return TryParse<ProductState>((values, c) =>
        {
            bool running = Convert.ToBoolean(int.Parse(values["fahrt"]));
            decimal position = Math.Round(decimal.Parse(values["position"]) / 2, 0) / 100;
            int angle = int.Parse(values["winkel"]) - 127;
            angle = Math.Abs(angle) > 80 ? Math.Sign(angle) * 80 : angle;

            return new ProductState(c, running, position, (short)angle);
        }, channel);
    }

    public bool ReadSetState(Actor actor)
    {
        return TryParse<bool>((values, c) =>
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", values.Select(v => v.Key + ": " + v.Value)));
            return true;
        }, actor.Channel);
    }

    public TResult TryParse<TResult>(Func<IDictionary<string, string>, Channel, TResult> func, Channel channel)
    {
        var children = Xml.SelectNodes(string.Format("/response/*", channel.Id)).OfType<XmlElement>();

        if (children is null || children.Count() == 0)
        {
            throw new InvalidResponseException(channel, Xml, "No data");
        }
        else
        {
            var values = children.ToDictionary<XmlElement, string, string>(c => c.LocalName, c => c.InnerText);
            try
            {
                return func(values, channel);
            }
            catch (Exception e)
            {
                var errorX = Xml.SelectSingleNode("/response/errorcode");
                int errorCode;
                if (errorX != null && int.TryParse(errorX.InnerText, out errorCode))
                {
                    throw new ResponseErrorException(channel, Xml, errorCode, $"Request returned with error code {errorCode}.");
                }
                else
                {
                    throw new InvalidResponseException(channel, Xml, "Could not deserialize", e);
                }
            }

        }
    }

    public bool HasResult()
    {
        return (Xml.SelectSingleNode("/response[not(befehl) and not(befehl = '1')]/*[0]") is not null);
    }
}


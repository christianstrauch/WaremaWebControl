using System;
namespace WebControl
{
	public class Actor
	{
		public Channel Channel { get; private set; }

		public Actor(Channel channel)
		{
			Channel = channel;
			channel.Actor = this;
		}
	}
}


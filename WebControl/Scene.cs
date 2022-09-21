using System;
namespace WebControl
{
	public class Scene: Actor
	{
		public const int PRODUCT_TYPE = 255;

		public int SceneIndex { get; private set; }

		public Scene(Channel channel, int scene): base(channel)
		{
			SceneIndex = scene;
		}
	}
}


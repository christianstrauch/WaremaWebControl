using System;
namespace WebControl.Products
{
	public class Shutter: Product
	{
		public const int PRODUCT_TYPE = 2;

        public override bool Running => throw new NotImplementedException();

        public Shutter(Channel channel, int control): base(channel, PRODUCT_TYPE, control)
		{
		}
	}
}


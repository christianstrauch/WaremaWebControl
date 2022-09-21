using System;
namespace WebControl.Products
{
	public class Louvre: Product
	{
		public const int PRODUCT_TYPE = 1;

        public override bool Running => throw new NotImplementedException();

        public Louvre(Channel channel, int control): base(channel, PRODUCT_TYPE, control)
		{
		}
	}
}


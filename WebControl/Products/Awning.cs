using System;
namespace WebControl.Products
{
	public class Awning: Product
	{
		public const int PRODUCT_TYPE = 3;

        public override bool Running => throw new NotImplementedException();

        public Awning(Channel channel, int control): base(channel, PRODUCT_TYPE, control)
		{
		}

        
    }
}


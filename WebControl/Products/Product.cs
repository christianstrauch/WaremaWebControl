using System;
namespace WebControl.Products
{
	public abstract class Product: Actor
	{

        public int ProductType { get; private set; }

        public int Control { get; private set; }

        public abstract bool Running { get; }

        public Product(Channel channel, int productType, int control) : base(channel)
        {
            ProductType = productType;
            Control = control;
		}
	}
}


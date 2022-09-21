using System;
namespace WebControl.Products
{
	public class ProductEventArgs: EventArgs
	{
		public ProductState Data { get; private set; }

		public Actor Actor { get; private set; }

		public ProductEventArgs(Actor actor, ProductState state): base()
		{
			Actor = actor;
			Data = state;
		}
	}
}


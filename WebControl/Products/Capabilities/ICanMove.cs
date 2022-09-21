using System;
namespace WebControl.Products.Capabilities
{
	public interface ICanMove: IUpdateable
	{
		public decimal Position { get; set; }
	}
}


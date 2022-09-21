using System;
namespace WebControl.Products.Capabilities
{
	public interface IUpdateable
	{
		public DateTime LastUpdate { get; }

		public void Update();

		public event EventHandler AfterUpdate;

		public event EventHandler BeforeUpdate;
	}
}


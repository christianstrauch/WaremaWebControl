using System;
using WebControl.Products.ValueTypes;

namespace WebControl.Products.Capabilities
{
	public interface ICanTilt: IUpdateable
	{
		public TiltAngle Angle { get; set; }

	}
}


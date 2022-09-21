using System;
using WebControl.Products.ValueTypes;

namespace WebControl
{
    public class ProductState
    {
        public Channel Channel { get; private set; }

        public bool Running { get; private set; }

        public decimal Position { get; private set; }
        
        public TiltAngle Angle { get; private set; }

        public ProductState(Channel channel, bool running, decimal position, short angle)
        {
            Channel = channel;
            Running = running;
            Position = position;
            Angle = angle;
        }
    }
}


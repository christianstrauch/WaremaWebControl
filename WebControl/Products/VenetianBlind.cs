using System;
using WebControl.Products.Capabilities;
using WebControl.Products.ValueTypes;

namespace WebControl.Products
{
	public class VenetianBlind: Product, ICanMove, ICanTilt
	{
		public const int PRODUCT_TYPE = 0;

		public VenetianBlind(Channel channel, int control): base(channel, PRODUCT_TYPE, control)
		{
		}

        private TiltAngle _angle;
        public TiltAngle Angle
        {
            get
            {
                EnsureUpdate();
                return _angle;
            }

            set => Move(null, value);
        }

        public DateTime LastUpdate { get; private set; }

        private decimal _position;
        public decimal Position
        {
            get
            {
                EnsureUpdate();
                return _position;
            }

            set => Move(value, null);
        }

        private bool _running;
        public override bool Running {
            get
            {
                EnsureUpdate();
                return _running;
            }
        }

        public event EventHandler AfterUpdate;

        protected virtual void OnAfterUpdate(ProductEventArgs e)
        {
            LastUpdate = DateTime.UtcNow;
            
            AfterUpdate?.Invoke(this, e);
        }

        public event EventHandler BeforeUpdate;

        protected virtual void OnBeforeUpdate(ProductEventArgs e)
        {
            BeforeUpdate?.Invoke(this, e);
        }

        private void EnsureUpdate()
        {
            if ((DateTime.UtcNow - LastUpdate).TotalSeconds > Endpoint.MAX_PRODUCT_INFO_AGE)
                Update();
        }

        public void Update()
        {
            try
            {
                OnBeforeUpdate(new ProductEventArgs(this, null));
                ProductState state = Channel.Room.Source.GetProductState(Channel);
                _angle = state.Angle;
                _position = state.Position;
                _running = state.Running;
                OnAfterUpdate(new ProductEventArgs(this, state));
            }
            catch (Exception e)
            {
                throw new ActorException(this, "Update", "Operation failed.", e);
            }

        }

        public void Move(decimal? position, TiltAngle? angle)
        {
            while (Running)
            {
                Thread.Sleep(3000);
                EnsureUpdate();
            }
            Channel.Room.Source.Move(this, position, angle);
            Update();
        }
    }
}


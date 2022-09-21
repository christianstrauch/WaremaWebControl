using System;
namespace WebControl.Products.ValueTypes
{
	public struct TiltAngle
	{
		public const sbyte MinValue = -80;
		public const sbyte MaxValue = 80;

		private sbyte _value;

		public static implicit operator TiltAngle(short value)
		{
			if (MinValue > value || value > MaxValue)
			{
				throw new InvalidCastException($"{value} is outside the bounds of {MinValue}..{MaxValue}");
			}
			else
			{
				return new TiltAngle() { _value = (sbyte)value };
			}
		}

		public static implicit operator short(TiltAngle value)
		{
			return (short)value._value;
		}

        public override string ToString()
        {
            return $"{(int)_value}º";
        }
	}
}


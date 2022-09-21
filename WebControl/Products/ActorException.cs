using System;
namespace WebControl.Products
{
	public class ActorException: Exception
	{
		public Actor Actor { get; private set; }

		public string Operation { get; private set; }

		public ActorException(Actor actor, string operation, string message, Exception innerException): base(message, innerException)
		{
			Actor = actor;
			Operation = operation;
		}
	}
}


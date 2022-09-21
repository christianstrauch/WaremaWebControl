using System;
using System.Xml;
namespace WebControl.Protocol
{
	public class InvalidResponseException: Exception
	{
		public object Data { get; private set; }

		public XmlNode Xml { get; private set; }

		public InvalidResponseException(object data, XmlDocument xml, string message): base(message)
		{
			Data = data;
			Xml = xml.Clone();
		}

		public InvalidResponseException(object data, XmlDocument xml, string message, Exception innerException): base(message, innerException)
		{
			Data = data;
			Xml = xml.Clone();
		}
	}
}


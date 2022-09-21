using System;
using System.Xml;
namespace WebControl.Protocol
{
	public class ResponseErrorException: Exception
	{
		public object Data { get; private set; }

		public int ErrorCode { get; private set; }

		public XmlNode Xml { get; private set; }

		public ResponseErrorException(object data, XmlDocument xml, int errorCode, string message): base(message)
		{
			Data = data;
			Xml = xml.Clone();
		}

		public ResponseErrorException(object data, XmlDocument xml, int errorCode, string message, Exception innerException): base(message, innerException)
		{
			Data = data;
            Xml = xml.Clone();
        }
	}
}


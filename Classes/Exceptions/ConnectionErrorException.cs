using System;

namespace HomeControl.Exceptions
{
	public sealed class ConnectionErrorException : Exception
	{
		public ConnectionErrorException() : base("")
        {
		}
		public ConnectionErrorException(string message) : base(message) 
        {
		}
		public ConnectionErrorException(string message, Exception innerException) : base(message, innerException)
        {
		}
	}
}

using System;

namespace HomeControl.Exceptions
{
	public sealed class NonCompatibleDeviceException : Exception
	{
		public NonCompatibleDeviceException() : base("")
        {
		}
		public NonCompatibleDeviceException(string message) : base(message) 
        {
		}
		public NonCompatibleDeviceException(string message, Exception innerException) : base(message, innerException)
        {
		}
	}
}

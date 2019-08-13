using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Classes
{
	public class PiCamera
	{
		public PiCamera() { }
		public PiCamera(string _rpiAddress, int _port = 8080) //, EnumDeviceType _deviceType)
		{	 
			ipAddress = _rpiAddress;
			port = _port;
			requestCount = 0;
		}

		public string ipAddress { get; set; }
		public int port { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public int requestCount { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public string url { get { return $"http://{ipAddress}:{port}"; } }
	}
}

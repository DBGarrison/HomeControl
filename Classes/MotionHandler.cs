using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;

namespace HomeControl.Classes
{
	public class MotionHandler
	{
		public MotionHandler() { }
		public MotionHandler(ControlledArea _area, string _pirName, string _cameraAddress) //, int _cameraport = 8080) //, EnumDeviceType _deviceType)
		{
			this.area = _area;
			this.pirName = _pirName;
			this.piCamera = PiCameras.list.First(c => c.ipAddress.Equals(_cameraAddress, StringComparison.OrdinalIgnoreCase));
			//this.ipAddress = _cameraAddress;
			//this.port = _cameraport;
		}
		internal ControlledArea area { get; set; }
		public string pirName { get; set; }
		public PiCamera piCamera { get; set; }

		//public string ipAddress { get; set; }
		//public int port { get; set; }

	}

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;

namespace HomeControl.Classes
{
    public class MCUDevice : SlaveDevice
    {
        public MCUDevice() { }
        public MCUDevice(string _ipAddress, int _port, Enums.EnumDeviceType _deviceType) : base(_ipAddress, _port)
        {
            this.remoteDeviceId = 0;
            this.configId = 0;
            this.remoteDeviceId = 0;
            this.deviceType = _deviceType;
        }

		public override string ToString()
		{
			return $"{ipAddress} : {remoteDeviceId}";
		}

		public int remoteDeviceId { get; set; }
        public int configId { get; set; }
        public Enums.EnumDeviceType deviceType { get; set; }
        internal WebSocketConnection connection { get; set; }
    }


}


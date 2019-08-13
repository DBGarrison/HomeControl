using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HomeControl.Classes;
using HomeControl.WebSocketManager;

namespace HomeControl
{	 
    public abstract class HomeControlDevice
	{
		static private int _nextDeviceId = 0;
		static internal int getNextDeviceId()
		{
			return ++_nextDeviceId;
		}

	internal List<ControlledArea> controlledAreas { get; set; }

	internal ILogger _logger;

	public HomeControlDevice()
		{
			this.localDeviceId = HomeControlDevice.getNextDeviceId();
			this.deviceType = Enums.EnumDeviceType.None;
			this.mcu = new MCUDevice();
		}
		public HomeControlDevice(Enums.ControlledAreas area, /*int localDeviceId,*/ string deviceName, string mcuIpAddress, Enums.EnumDeviceType deviceType)
		{
			this.area = area;
			this.localDeviceId = HomeControlDevice.getNextDeviceId();			 
			this.deviceName = deviceName;
			this.deviceType = deviceType;
			this.mcu = new MCUDevice(mcuIpAddress, 80, deviceType);
		}

		public Enums.ControlledAreas area { get; set; }
		public string areaName {
			get {
				return this.area.ToString();
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public int localDeviceId { get; set; }
		public string deviceName { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public Enums.EnumDeviceType deviceType { get; set; }

		public MCUDevice mcu { get; set; }

		internal List<AreaEvent> Events { get; set; }

		internal void setWebSocketConnection(int remoteDeviceId, WebSocketConnection connection)
		{
			mcu.remoteDeviceId = remoteDeviceId;
			mcu.connection = connection;

			if (connection != null)
			{
				connection.mcu = mcu;
				if (String.IsNullOrEmpty(mcu.ipAddress))
					mcu.ipAddress = connection.NickName;
				else if (!mcu.ipAddress.Equals(connection.NickName, StringComparison.OrdinalIgnoreCase))
				{
					;
				}				 
			}			
		}
	}


}

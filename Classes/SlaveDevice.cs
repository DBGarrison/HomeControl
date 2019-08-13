using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;

namespace HomeControl.Classes
{
 
    public class SlaveDevice
    {
        public SlaveDevice() { }
        public SlaveDevice(string _ipAddress, int _port, Enums.RemoteOSType _osType = Enums.RemoteOSType.Rpi) //, EnumDeviceType _deviceType)
        {
            this.ipAddress = _ipAddress;
            this.port = _port;
            osType = _osType;           
        }

        public string ipAddress { get; set; }
        public int port { get; set; }
        public Enums.RemoteOSType osType { get; set; }

        public static List<SlaveDevice>  buildMcuListForTrack()
        {
            List<SlaveDevice> list = new List<SlaveDevice>();
            list.Add(new SlaveDevice("RpiZero4", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero5", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero6", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero7", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero9", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero10", 8000, Enums.RemoteOSType.Rpi));
            list.Add(new SlaveDevice("RpiZero11", 8000, Enums.RemoteOSType.Rpi));
            return list;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using HomeControl.Classes;
//using HomeControl.HostedServices;
using Microsoft.Extensions.Logging;
 
namespace HomeControl.Relays

{
	public class RelayDevice : HomeControlDevice
	{
		//private ILogger _logger;
		int _swState = 0;
		public RelayDevice() : base(Enums.ControlledAreas.None, /*0,*/ "", "", Enums.EnumDeviceType.Relay) { this.Events = new List<AreaEvent>(); }

		public RelayDevice(Enums.ControlledAreas area, string name, bool state, string mcuIpAddress)
			: base(area, name, mcuIpAddress, Enums.EnumDeviceType.Relay)
		{
			this._swState = state ? 1 : 0;
			this.Events = new List<AreaEvent>();
		}

		public int swState
		{
			get { return _swState; }
			set
			{
				if (value != _swState)
				{
					if (value > 0)
						lastOnState = DateTime.Now.Ticks;
					else
						lastOffState = DateTime.Now.Ticks;
					_swState = value;
				}
			}
		}

		public long lastOnState { get; set; }
		public long lastOffState { get; set; }

        internal void setState(int _swState, bool logEvent = false)
        {
            swState = _swState;
            AreaEventType aet = swState > 0 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
            AreaEvent evt = new AreaEvent(area, deviceName, aet, mcu.ipAddress, AreaEventStatus.Complete);
            Events.Add(evt);
            EventLogger.logEvent(evt);
        }

        //public List<AreaEvent> Events { get; set; }


        internal void createLogger(ILoggerFactory _loggerFactory)
        {
            _logger = _loggerFactory.CreateLogger<RelayDevice>();
        }

        /* 
		private enum EnumCfg : Int32 { type, area, name, state, mcu }
		public static RelayDevice fromString(string cfgLine)
		{

            //NEW		
            //rfswitch:areaId:deviceName:bitLength:oncodes|offcodes:[swState]:[mculist]
            //rfswitch:1:Living Room Lamp:24:2|2698844|4323292:2|24|2698836|4323284:1:Garage|RFSwitch


            
			string[] parts = cfgLine.Split(':');
			int numParts = parts.Length;

			if (numParts < (int)EnumCfg.mcu || parts[(int)EnumCfg.type] != "relay")
			{
				Console.WriteLine($"INVALID CFG LINE: {cfgLine}");
				return null;
			}

            Enums.ControlledAreas area = ((Enums.ControlledAreas)Convert.ToInt32(parts[(int)EnumCfg.area]));
            bool state = parts[(int)EnumCfg.state] == "1";

            RelayDevice retVal = new RelayDevice( area, parts[(int)EnumCfg.name], state, parts[(int)EnumCfg.mcu]);
            return retVal;
		}

		internal static List<RelayDevice> buildDefaultListForTrack()
		{
			List<RelayDevice> list = new List<RelayDevice>();
			list.Add(new RelayDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSE, false, "RpiZero6"));
			list.Add(new RelayDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSW, false, "RpiZero7"));
			list.Add(new RelayDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerNW, false, "RpiZero10"));
			
			list.Add(new RelayDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsFront, false, "RpiZero5"));
			list.Add(new RelayDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsEntrance, false, "RpiZero11"));

			list.Add(new RelayDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateShack, false, "RpiZero4"));
			list.Add(new RelayDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateEntrance, false, "RpiZero9"));

			return list;
		}

		internal static List<RelayDevice> buildDefaultList(int count)
		{
			List<RelayDevice> list = new List<RelayDevice>();
			if (count > 0)
			{
				list.Add(new RelayDevice(Enums.ControlledAreas.GarageArea, "Garage SE", false, "ShopRfRelay"));
                if (count > 1)
                {
                    list.Add(new RelayDevice(Enums.ControlledAreas.GarageArea, "Garage SW", false, "ShopRfRelay"));
                    if (count > 2)
                    {
                        list.Add(new RelayDevice(Enums.ControlledAreas.GarageWest, "Security Light", false, "ShopRfRelay"));
                        if (count > 3)
                        {
                            list.Add(new RelayDevice(Enums.ControlledAreas.GarageSouth, "Security Light", false, "ShopRfRelay"));
                        }
                    }
                }
            }
			return list;
		}

		 
        */

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using HomeControl.Extensions;
using HomeControl.Classes;
using Microsoft.Extensions.Logging;

namespace HomeControl.RFSwitch
{
	public class RFSwitchDevice : HomeControlDevice
	{
		int _swState = 0;

		public RFSwitchDevice()
			: base(Enums.ControlledAreas.None, /*0,*/"", "", Enums.EnumDeviceType.RFSwitch)
		{
			//this.slaves = new List<MCUDevice>();
			this.Events = new List<AreaEvent>();
			this.rfCodes = new List<RFCode>();
		}

		public RFSwitchDevice(Enums.ControlledAreas areaId, /*int localDeviceId,*/ string swName, int bitLength = 24)  
			: base(areaId, /*localDeviceId,*/ swName, "", Enums.EnumDeviceType.RFSwitch)
		{			 
			this.rfCodes = new List<RFCode>();		
			//this.slaves = new List<MCUDevice>();
			this.Events = new List<AreaEvent>();
			this.bitLength = bitLength;
			this.lastOnState = 0;
			this.lastOffState = 0;
			this._swState = 0;
		}

        internal void createLogger(ILoggerFactory _loggerFactory)
        {
            _logger = _loggerFactory.CreateLogger<RFSwitchDevice>();
        }

        public void addCode(bool _isOn, long _code)
		{
			this.rfCodes.Add(new RFCode() { isOn = _isOn, code = _code });
		}
 
		public List<RFCode> rfCodes { get; set; }

		//public List<MCUDevice> slaves { get; set; }
		//public string notifyHost { get; set; }
		public long lastOnState { get; set; }
		public long lastOffState  { get; set; }

		//internal List<AreaEvent> deviceEvents { get; set; }

		public int bitLength { get; set; }

		internal void setState(int _swState, bool logEvent = false)
		{
			swState = _swState;
			if (swState > 0) lastOnState = DateTime.Now.Ticks;
			else lastOffState = DateTime.Now.Ticks;

			AreaEventType aet = swState > 0 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
			AreaEvent evt = new AreaEvent(area, deviceName, aet, mcu.ipAddress, AreaEventStatus.Complete);
			Events.Add(evt);
			EventLogger.logEvent(evt);
		}
		/*
		sw.lastOnState = DateTime.Now.Ticks;
				AreaEventType eat = (swState > 0) ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
				AreaEvent evt = new AreaEvent(sw.area, sw.deviceName, eat, clientIP);
				sw.Events.Add(evt);
				EventLogger.logEvent(evt);
				sw.swState = swState;
				sw.lastOnState = DateTime.Now.Ticks;
				*/
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

		//internal ControlledArea area { get; set; }

		public override string ToString()
		{
			if (area == Enums.ControlledAreas.None) return base.ToString();
	 
			//After adding bitLength
			//rfswitch:11:Ceiling Fan Light:32:9|2414159884|2414159995|2414159884|2414159901|2414159935|2414159935|2414159961|2414159978|2414159995:9|2414159884|2414159995|2414159884|2414159901|2414159935|2414159935|2414159961|2414159978|2414159995
			//rfswitch:1:Living Room Lamp:24:2|2698844|4323292:2|24|2698836|4323284:1:Garage|RFSwitch
			//rfswitch:areaId:deviceName:bitLength:oncodes|offcodes:[swState]:[mculist]

			long[] onCodes = rfCodes.Where(c => c.isOn).Select(c => c.code).ToArray();
			long[] offCodes = rfCodes.Where(c => !c.isOn).Select(c => c.code).ToArray();
			//string[] mcuList = slaves.Select(c => c.ipAddress).ToArray();
			return $"rfswitch:{(int)area}:{deviceName}:{bitLength}:{onCodes.Length}|{String.Join('|', onCodes)}:{offCodes.Length}|{String.Join('|', offCodes)}:{this.swState}:{String.Join('|', mcu.ipAddress)}";
		}

		public static string getActivateMsg(RFSwitchDevice sw)
		{
			//setactive:1:Living Room Lamp:1
			return $"setactive:{(int)sw.area}:{sw.deviceName}:{sw.swState}";
		}

        /*
		private enum EnumSwitchCfg : Int32 { type, area, name, bits, oncodes, offcodes, state, mcu }
		 
		public static RFSwitchDevice fromString(string cfgLine)
		{
		 
			//NEW		
			//rfswitch:areaId:deviceName:bitLength:oncodes|offcodes:[swState]:[mculist]
			//rfswitch:1:Living Room Lamp:24:2|2698844|4323292:2|24|2698836|4323284:1:Garage|RFSwitch
			 

			RFSwitchDevice retVal = new RFSwitchDevice();
			string[] parts = cfgLine.Split(':');
			int numParts = parts.Length;

			if (numParts < (int)EnumSwitchCfg.offcodes || parts[(int)EnumSwitchCfg.type] != "rfswitch")
			{
				Console.WriteLine($"INVALID CFG LINE: {cfgLine}");
				return null;
			}
	 
			retVal.area = (Enums.ControlledAreas)Convert.ToInt32(parts[(int)EnumSwitchCfg.area]);
			retVal.deviceName = parts[(int)EnumSwitchCfg.name];
			//retVal.deviceType = EnumDeviceType.RFSwitch;
			retVal.bitLength = Convert.ToInt32(parts[(int)EnumSwitchCfg.bits]);
			var onCodes = parts[(int)EnumSwitchCfg.oncodes].Split('|').ToList();
	
			onCodes.Skip(1).ToList().ForEach(c => retVal.addCode(true, (Convert.ToInt64(c))));

			var offCodes = parts[(int)EnumSwitchCfg.offcodes].Split('|').ToList();

			offCodes.Skip(1).ToList().ForEach(c => retVal.addCode(false, (Convert.ToInt32(c))));

			if (numParts >= (int)EnumSwitchCfg.state)
			{
				retVal.swState = Convert.ToInt32(parts[(int)EnumSwitchCfg.state]);


				if (numParts >= (int)EnumSwitchCfg.mcu)
				{

					//var mcuList = parts[(int)EnumSwitchCfg.mcu].Split('|').ToList();
					string ipAddr = parts[(int)EnumSwitchCfg.mcu];
					retVal.mcu.ipAddress = parts[(int)EnumSwitchCfg.mcu];
					retVal.mcu.remoteDeviceId = 0;
					retVal.mcu.port = 80;
					//TODO: Figure out if we even need this method
					//mcuList.ForEach(c => retVal.slaves.Add(new MCUDevice(c, 80, Enums.EnumDeviceType.RFSwitch)));

				}
			}


			return retVal;
		}
		 
		public static string toCfg(RFSwitchDevice sw)
		{
			if (sw.area == Enums.ControlledAreas.None) return "";

			//rfswitch:1:Living Room Lamp:2|2698844|4323292:2|2698836|4323284:1:Garage|RFSwitch

			//After adding bitLength
			//rfswitch:11:Ceiling Fan Light:32:9|2414159884|2414159995|2414159884|2414159901|2414159935|2414159935|2414159961|2414159978|2414159995:9|2414159884|2414159995|2414159884|2414159901|2414159935|2414159935|2414159961|2414159978|2414159995
			//rfswitch:1:Living Room Lamp:24:2|2698844|4323292:2|24|2698836|4323284:1
			//rfswitch:areaId:deviceName:bitLength:oncodes|offcodes:[swState]:[mculist]

			long[] onCodes = sw.rfCodes.Where(c => c.isOn).Select(c => c.code).ToArray();
			long[] offCodes = sw.rfCodes.Where(c => !c.isOn).Select(c => c.code).ToArray();
			//string[] mcuList = sw.slaves.Select(c => c.ipAddress).ToArray();
			return $"rfswitch:{(int)sw.area}:{sw.deviceName}:{sw.bitLength}:{onCodes.Length}|{String.Join('|', onCodes)}:{offCodes.Length}|{String.Join('|', offCodes)}:{sw.swState}:{String.Join('|', sw.mcu.ipAddress)}";
		}

		public static List<RFSwitchDevice> buildSwitches()
		{
			List<RFSwitchDevice> retVal = new List<RFSwitchDevice>();
			string[] cfgLines = new string[]
			   {
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Living Room Lamp:24:2|2698844|4323292:2|2698836|4323284:0:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Little Lamp:24:2|2698842|4323290:2|2698834|4323282:0:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.Kitchen.ToInt()}:Coffee Pot:24:2|2698841|4323289:2|2698833|4323281:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.RearDeck.ToInt()}:Rear Deck sw.:24:2|2698845|4323293:2|2698837|4323285:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.FrontDeck.ToInt()}:Front Deck sw.:24:2|2698843|4323291:2|2698835|4323283:1:HouseRfRelay",				 

				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Ceiling Fan Light:32:9|2414159961|2414159884|2414159995|2414159884|2414159901|2414159935|2414159935|2414159978|2414159995:0:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Ceiling Fan Off:32:3|2414159447|2414159460|2414159362:0:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Ceiling Fan Low:32:3|2414158419|2414158432|2414158449:0:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Ceiling Fan Med:32:4|2414158614|2414158629|2414158644|2414158659:0:1:HouseRfRelay",
				$"rfswitch:{Enums.ControlledAreas.LivingRoom.ToInt()}:Ceiling Fan High:32:3|2414160175|2414160190|2414160216:0:1:HouseRfRelay",

                $"rfswitch:{Enums.ControlledAreas.GarageArea.ToInt()}:Row 1:24:2|8990396|15340348:2|8990388|15340340:1:ShopRfRelay",
                $"rfswitch:{Enums.ControlledAreas.GarageArea.ToInt()}:Row 2:24:2|8990394|15340346:2|8990386|15340338:1:ShopRfRelay",
                $"rfswitch:{Enums.ControlledAreas.GarageArea.ToInt()}:Row 3:24:2|8990393|15340345:2|8990385|15340337:1:ShopRfRelay",
                $"rfswitch:{Enums.ControlledAreas.GarageArea.ToInt()}:Row 4:24:2|8990397|15340349:2|8990389|15340341:1:ShopRfRelay",
                $"rfswitch:{Enums.ControlledAreas.GarageArea.ToInt()}:Row 5:24:2|8990395|15340347:2|8990387|15340339:1:ShopRfRelay"
               };

			List<RFSwitchDevice> switches = new List<RFSwitchDevice>();
			 
			foreach (string line in cfgLines)
			{
				try
				{
					RFSwitchDevice sw = RFSwitchDevice.fromString(line);
                    sw.localDeviceId = getNextDeviceId();
					retVal.Add(sw);

				}
				catch (Exception ex)
				{
					Console.WriteLine($"buildSwitches() Client http failed: {ex.Message}");
				}

			}
			return retVal;
		}

 
		internal void CopyResultValues(RFSwitchDevice result)
		{
			//if (this.remoteDeviceId < 1 && result.remoteDeviceId > 0)
			//{
			//	this.remoteDeviceId = result.remoteDeviceId;
			//}

			this.swState = result.swState;
			this.lastOffState = result.lastOffState;
			this.lastOnState = result.lastOnState;
		}
        */
	}
}

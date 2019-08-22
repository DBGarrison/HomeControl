using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl
{
	public class Enums
	{
		public enum ControlledAreas
		{
			None = 0, LivingRoom, Kitchen, Office, MasterBedRoom, MasterBathRoom, BedRoom, BathRoom,
			FrontYard, BackYard, RearDeck, FrontDeck,
			GarageArea, GarageNorth, GarageEast, GarageWest, GarageSouth,			
			Concessions = 17, Tower = 18, FrontGate = 19, StartingLine = 20
		}

		public enum SceneType
		{
			/// <summary>
			/// Default value
			/// </summary>
			Nothing = 0,
			/// <summary>
			/// NodeMcu controlled: Can have multiple conditions: Available | LightIsOn | FanIsOn | IsReversed | LowSpeed | MediumSpeed | HighSpeed
			/// </summary>
			CeilingFan = 1,
			/// <summary>
			/// HS-100 devices, Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			SmartSwitch = 2,
			/// <summary>
			/// 433 mhz RF Devices: Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			RfSwitch = 3,
			/// <summary>
			/// Pi Camera controlled via mjpg-streamer: Can have multiple conditions: Available | Streaming | Surveilling
			/// </summary>
			PiCamera = 4,

			/// <summary>
			/// Table Top Lamp, HS-100 devices, Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			SmartLamp = 5,

			/// <summary>
			/// Overhead Light, HS-100 devices, Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			SmartLight = 6,

			/// <summary>
			/// Table Top Lamp, 433 mhz RF Devices: Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			RfLamp = 7,

			/// <summary>
			/// Overhead Light, 433 mhz RF Devices: Can have multiple conditions: Available | RelayStateOn
			/// </summary>
			RfLight = 8
		}

		[Flags]
		public enum SceneCondition
		{
			None = 0,
			/// <summary>
			/// Is the device available?
			/// </summary>
			Available = (1 << 0),
			/// <summary>
			/// If the the device can be switched on/off, is it ON?
			/// </summary>
			LightIsOn = (1 << 1),
			/// <summary>
			/// If the the device can be switched on/off, is it ON?
			/// </summary>
			FanIsOn = (1 << 2),
			/// <summary>
			/// If the device has a direction, is it reversed?
			/// </summary>
			IsReversed = (1 << 3),
			/// <summary>
			/// If the device has a speed setting, is the speed set to Low?
			/// </summary>
			LowSpeed = (1 << 4),
			/// <summary>
			/// If the device has a speed setting, is the speed set to Medium?
			/// </summary>
			MediumSpeed = (1 << 5),
			/// <summary>
			/// If the device has a speed setting, is the speed set to High?
			/// </summary>
			HighSpeed = (1 << 6),
			/// <summary>
			/// If the Device is a Camera, is It Streaming?
			/// </summary>
			Streaming = (1 << 7),
			/// <summary>
			/// If the Device is a Camera, is It Surveilling?
			/// </summary>
			Surveilling = (1 << 8),
			/// <summary>
			/// If the Device is a SmartSwitch or RfSwitch, is the RelayState On?
			/// </summary>
			RelayStateOn = (1 << 9)
		}

        public enum EnumDeviceType { None = 0, RFSwitch, HS100, CeilingFan, Speed, Motion, Relay, Weather }

        [Flags]
        enum EnumServiceType { NoService = 0, RFService = 1, HS100Service = 2, CeilingFanService = 4, MotorService = 8, MotionService = 16, RelayService = 32, WeatherService = 64 }; 

		public enum EnumPirStatus { Idle = 0, Queued, DeQueued, Streaming, Stopping }

		[Flags]
		public enum RemoteOSType { None = 0, Esp8266 = 1, Esp32 = 2, Rpi = 4}
	}
}

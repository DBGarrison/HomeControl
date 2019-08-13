using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HomeControl.Classes
{
	public interface IControlledAreas
	{

		List<ControlledArea> controlledAreas { get; }
		void load(bool startFresh, ILoggerFactory loggerFactory);
		void save();
	}

	public class ControlledArea
	{
		public ControlledArea()
		{
			//this.areaDevices = new List<HomeControlDevice>();
			this.relayHosts = new List<SlaveDevice>();
			this.motionHandlers = new List<MotionHandler>();
		}
		public ControlledArea(Enums.ControlledAreas areaId, string areaName)
		{
			this.areaId = areaId;
			this.areaName = areaName;
			this.relayHosts = new List<SlaveDevice>();
			this.motionHandlers = new List<MotionHandler>();
		}

		public Enums.ControlledAreas areaId { get; set; }
		public string areaName { get; set; }

		public List<SlaveDevice> relayHosts { get; set; }
		public List<MotionHandler> motionHandlers { get; set; }
		//public SlaveDevice cameraHost { get; set; }
		//internal List<HomeControlDevice> areaDevices { get; set; }

	}

	public class ControlledAreas : IControlledAreas
	{
		private List<ControlledArea> _controlledAreas;
		private readonly ILogger _logger;

		public const string csAreaTowerNorth = "Tower North";
		public const string csAreaTowerSE = "Tower SE";
		public const string csAreaTowerSW = "Tower SW";
		public const string csAreaTowerNW = "Tower NW";
		public const string csAreaTowerSouth = "Tower South";

		public const string csAreaConcessionsEntrance = "Concessions Entrance";
		public const string csAreaConcessionsFront = "Concessions Front";
		public const string csAreaGateShack = "Gate Shack";
		public const string csAreaGateEntrance = "Gate Entrance";

		 

		public ControlledAreas(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<ControlledAreas>();
			_controlledAreas = new List<ControlledArea>();

			bool startFresh = !System.IO.File.Exists(_xmlPath);
			load(startFresh, loggerFactory);
		}

		public string _xmlPath {
            get {
                return Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                       Environment.GetEnvironmentVariable("CONTROLLED_AREAS_PATH");
            }
        }
		public int Count { get { return _controlledAreas.Count; } }

		public List<ControlledArea> controlledAreas { get { return _controlledAreas; } }


		public void save()
		{
			Common.SaveToXML<List<ControlledArea>>(_controlledAreas, _xmlPath);
		}

		public void load(bool startFresh, ILoggerFactory loggerFactory)
		{
			_controlledAreas.Clear();

			_logger.LogInformation($"load({startFresh}) entered");

			if (startFresh)
			{
				_controlledAreas.AddRange(BuildAreasForJakes());
				save();
			}

			else
			{
				_controlledAreas.AddRange(Common.LoadFromXML<List<ControlledArea>>(_xmlPath));
			}
			//_controlledAreas.ForEach(d => d.createLogger(loggerFactory));
		}

		private List<ControlledArea> BuildAreasForJakes()
		{

			var retVal = new List<ControlledArea>();
			ControlledArea area = new ControlledArea(Enums.ControlledAreas.Tower, "Race Tower");
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaTowerSE, "RpiZero6"));
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaTowerSW, "RpiZero7"));
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaTowerNW, "RpiZero10"));
			retVal.Add(area);
			 			 
			area = new ControlledArea(Enums.ControlledAreas.Concessions, "Concessions");
			retVal.Add(area);
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaConcessionsFront, "RpiZero5"));
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaConcessionsEntrance, "RpiZero11"));
			 			 
			area = new ControlledArea(Enums.ControlledAreas.FrontGate, "Front Gate");
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaGateShack, "RpiZero4"));
			//No PIR for Gate Entrance yet
			area.motionHandlers.Add(new MotionHandler(area, ControlledAreas.csAreaGateEntrance, "RpiZero9"));
			retVal.Add(area);

			Common.SaveToXML<List<ControlledArea>>(retVal, "ControlledAreas.xml");
			return retVal;
		}

		private List<ControlledArea> BuildAreasForHome()
		{

			var retVal = new List<ControlledArea>();
			ControlledArea area = new ControlledArea(Enums.ControlledAreas.BackYard, "Back Yard");
			area.motionHandlers.Add(new MotionHandler(area, "Rear Deck", "RpiZero9"));

			retVal.Add(area);
			//List[List.Count - 1].cameraHost = new SlaveDevice("RpiZero4", 8085);

			retVal.Add(new ControlledArea(Enums.ControlledAreas.BathRoom, "Bath Room"));
			retVal.Add(new ControlledArea(Enums.ControlledAreas.BedRoom, "Bed Room"));

			area = new ControlledArea(Enums.ControlledAreas.FrontDeck, "Front Deck");
			area.motionHandlers.Add(new MotionHandler(area, "Front Deck", "RpiZero5"));
			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.FrontYard, "Front Yard");
			area.motionHandlers.Add(new MotionHandler(area, "Front Deck", "RpiZero6"));
			retVal.Add(area);


			area = new ControlledArea(Enums.ControlledAreas.LivingRoom, "Living Room");
			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.GarageArea, "Garage Area");
			area.relayHosts.Add(new SlaveDevice("ShopRfRelay", 80));
			area.motionHandlers.Add(new MotionHandler(area, "Garage SE", "RpiZero1"));
			area.motionHandlers.Add(new MotionHandler(area, "Garage SW", "RpiZero1"));
			area.motionHandlers.Add(new MotionHandler(area, "Garage NW", "RpiZero7"));
			area.motionHandlers.Add(new MotionHandler(area, "Garage North", "RpiZero7"));
			area.motionHandlers.Add(new MotionHandler(area, "Garage NE", "RpiZero7"));

			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.GarageNorth, "Garage North");
			area.relayHosts.Add(new SlaveDevice("ShopRelay", 80));

			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.GarageEast, "Garage East");
			area.relayHosts.Add(new SlaveDevice("ShopRelay", 80));
			//area.cameraHost = new SlaveDevice("RpiZero3", 8080);
			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.GarageWest, "Garage West");
			area.relayHosts.Add(new SlaveDevice("ShopRelay", 80));
			//area.cameraHost = new SlaveDevice("RpiZero4", 8085);
			retVal.Add(area);

			area = new ControlledArea(Enums.ControlledAreas.GarageSouth, "Garage South");
			area.relayHosts.Add(new SlaveDevice("ShopRelay", 80));
			//area.cameraHost = new SlaveDevice("RpiZero5", 8085);
			retVal.Add(area);

			retVal.Add(new ControlledArea(Enums.ControlledAreas.Kitchen, "Kitchen"));
			//area.cameraHost = new SlaveDevice("RpiZero2", 8080);

			retVal.Add(new ControlledArea(Enums.ControlledAreas.MasterBathRoom, "Master Bath"));
			retVal.Add(new ControlledArea(Enums.ControlledAreas.MasterBedRoom, "Master Br."));

			retVal.Add(new ControlledArea(Enums.ControlledAreas.Office, "Office"));
			//List[List.Count - 1].cameraHost = new SlaveDevice("Rpi1", 8080);

			retVal.Add(new ControlledArea(Enums.ControlledAreas.RearDeck, "Rear Deck"));

			return retVal;
		}

	}	
 
}

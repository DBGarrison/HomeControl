using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HomeControl.Classes;

namespace HomeControl.RFSwitch
{
	public interface IRFSwitches
	{
		 
		List<RFSwitchDevice> areaSwitches { get;  }
		//void load(bool startFresh, ILoggerFactory loggerFactory);
		//void save();
	}
	 
	public class RFSwitches : IRFSwitches
	{
		private List<RFSwitchDevice> _switches;
		private readonly ILogger _logger;
		private readonly IControlledAreas _controlledAreas;
        private readonly PiCollection _piCollection;

        public RFSwitches(ILoggerFactory loggerFactory, IControlledAreas controlledAreas, IPiCollection piCollection)
        {
            _logger = loggerFactory.CreateLogger<RFSwitches>();
            _switches = new List<RFSwitchDevice>();
            _controlledAreas = controlledAreas;
            _piCollection = piCollection as PiCollection;

            //         bool startFresh = !System.IO.File.Exists(_xmlPath);
            //load(startFresh, loggerFactory);	
            foreach (var rpi in _piCollection.zeros)
            {
                foreach (var rpiDevice in rpi.RFSwitches)
                {
                    RFSwitchDevice sw = new RFSwitchDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpiDevice.bitLength);
                    sw.mcu.ipAddress = rpi.ipAddress;
                    sw.mcu.port = rpi.port;

                    rpiDevice.onCodes.Skip(1).ToList().ForEach(c => sw.addCode(true, (Convert.ToInt64(c))));

                    rpiDevice.offCodes.Skip(1).ToList().ForEach(c => sw.addCode(false, (Convert.ToInt32(c))));

                    _switches.Add(sw);
                }
            }
            _switches.ForEach(d => d.createLogger(loggerFactory));
            _switches.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
            return;
        }

        ~RFSwitches()
		{
			foreach (var sw in _switches.Where(sw => sw.mcu != null && 
													 sw.mcu.connection != null && 
													 sw.mcu.connection.WebSocket != null))
			{
				sw.mcu.connection.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, 
													   "RFSwitchCollecton::Destructor()", 
													   System.Threading.CancellationToken.None);								
			}
		}

		public string _xmlPath { get { return Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                                              Environment.GetEnvironmentVariable("RF_SWITCHES_PATH"); } }
		public int Count { get { return areaSwitches.Count; } }
		public List<RFSwitchDevice> areaSwitches { get { return _switches; } }

		 
		public async Task activateSwitches(RFSocketConnection connection, uint unixTimeStamp)
		{
			_logger.LogInformation($"activateSwitches() entered");
			//load(true);
			foreach (var sw in areaSwitches)
			{	 
				//foreach (var mcu in sw.slaves.Where(c => c.ipAddress == connection.NickName)) //c.connection != null && 
				//{
					try
					{			
						sw.mcu.connection = connection;
						//If the config ID's don't match we'll have to rebuild the switches on the MCU						 
						string msg = /*(mcu.configId == unixTimeStamp) ? RFSwitchDevice.getActivateMsg(sw) :*/ sw.ToString();
						await sw.mcu.connection.SendMessageAsync(msg);
					}
					catch (Exception ex)
					{
						_logger.LogError($"activateSwitches() Client http failed: {ex.Message}");
					}
				//}
			}
		}

        /*
         public void save()
		{			 
			Common.SaveToXML<List<RFSwitchDevice>>(areaSwitches, _xmlPath);
		}

		public void load(bool startFresh, ILoggerFactory loggerFactory)
		{
			areaSwitches.Clear();

			_logger.LogInformation($"load({startFresh}) entered");
			 
			if (startFresh)
			{
				areaSwitches.AddRange(RFSwitchDevice.buildSwitches());
				save();			 
			}

			else
			{
				areaSwitches.AddRange(Common.LoadFromXML<List<RFSwitchDevice>>(_xmlPath));				 
			}
            areaSwitches.ForEach(d => d.createLogger(loggerFactory));
			areaSwitches.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
		}


		 I'll use this once i get configId working
		public async Task activateSwitches()
		{
			_logger.LogInformation($"activateSwitches() entered");

			foreach (var sw in RFSwitches)
			{
				//sw.remoteDeviceId = 0;
				//ControlledArea area =this.areas.Where(a => a.areaId == sw.areaId).FirstOrDefault();
				//area.areaDevices.Add(sw);
				//sw.area = area;

				foreach (var mcu in sw.remoteRFSwitches.Where(c => c.connection != null))
				{
					try
					{
						await mcu.connection.SendMessageAsync($"activate:{sw.ToString()}");						 
					}
					catch (Exception ex)
					{
						_logger.LogError($"activateSwitches() Client http failed: {ex.Message}");
					}
				}
			}
		}*/
    }
}

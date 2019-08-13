using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HomeControl.Classes;
using RpiConfig.Classes;

namespace HomeControl.Relays
{
	public interface IRelayDevices
	{
		 
		List<RelayDevice> relays { get;  }
		//void load(bool startFresh, ILoggerFactory loggerFactory);
		//void save();
	}
	 
	public class RelayDevices : IRelayDevices
    {
		private List<RelayDevice> _relays;		 
		private readonly ILogger _logger;
		private readonly IControlledAreas _controlledAreas;
        private readonly PiCollection _piCollection;

        public RelayDevices(ILoggerFactory loggerFactory, IControlledAreas controlledAreas, IPiCollection piCollection)
		{
			_logger = loggerFactory.CreateLogger<RelayDevices>();
			_relays = new List<RelayDevice>();
			_controlledAreas = controlledAreas;
            _piCollection = piCollection as PiCollection;

            //bool startFresh = !System.IO.File.Exists(_xmlPath);
            //load(startFresh, loggerFactory);	
            foreach (var rpi in _piCollection.zeros.Where(z => z.Relays.Count > 0))
            {
                rpi.Relays.ForEach(r => {
                    RelayDevice relay = new RelayDevice((Enums.ControlledAreas)r.area,
                                       r.deviceName,
                                       r.state == 1,
                                       rpi.ipAddress);
                    relay.mcu.port = rpi.port;
                    _relays.Add(relay);
                });

                //foreach (var rpiDevice in rpi.Relays)
                //{

                //    RelayDevice relay = new RelayDevice((Enums.ControlledAreas)rpiDevice.area,
                //                                            rpiDevice.deviceName,
                //                                            rpiDevice.state == 1,
                //                                            rpi.ipAddress);
                //    relay.mcu.port = rpi.port;
                //    _relays.Add(relay);
                //}
            }

            _relays.ForEach(d => d.createLogger(loggerFactory));
            _relays.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
            return;
        }

		~RelayDevices()
		{
			foreach (var sw in _relays)
			{
				//foreach (var mcu in sw.remoteRFSwitches)
				//{
				//	if (mcu.connection != null && mcu.connection.WebSocket != null)
				//	{
				//		mcu.connection.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "RFSwitchCollecton::Destructor()", System.Threading.CancellationToken.None);
				//	}
				//}
			}
		}

		public string _xmlPath { get { return Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                                              Environment.GetEnvironmentVariable("RELAY_DEVICES_PATH"); } }
		public int Count { get { return _relays.Count; } }
 
		public List<RelayDevice> relays { get { return _relays; } }
        /*
		public void save()
		{			 
			Common.SaveToXML<List<RelayDevice>>(_relays, _xmlPath);
		}

		public void load(bool startFresh, ILoggerFactory loggerFactory)
		{
			_relays.Clear();

			_logger.LogInformation($"load({startFresh}) entered");
			 
			if (startFresh)
			{		
				_relays.AddRange(RelayDevice.buildDefaultListForTrack());
				save();			 
			}

			else
			{
				_relays.AddRange(Common.LoadFromXML<List<RelayDevice>>(_xmlPath));				 
			}
            _relays.ForEach(d => d.createLogger(loggerFactory));
			_relays.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
		}
        */
	}
}

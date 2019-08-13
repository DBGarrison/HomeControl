using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HomeControl.Classes;

namespace HomeControl.ObjectDetection
{
	public interface IPIRDevices
	{
		 
		List<PIRDevice> detections { get;  }
		//void load(bool startFresh, ILoggerFactory loggerFactory);
		//void save();
	}
	 
	public class PIRDevices : IPIRDevices
	{
		private List<PIRDevice> _detects;		 
		private readonly ILogger _logger;
		private readonly IControlledAreas _controlledAreas;
        private readonly PiCollection _piCollection;

        public PIRDevices(ILoggerFactory loggerFactory, IControlledAreas controlledAreas, IPiCollection piCollection)
        {
            _logger = loggerFactory.CreateLogger<PIRDevices>();
            _detects = new List<PIRDevice>();
            _controlledAreas = controlledAreas;
            _piCollection = piCollection as PiCollection;

            //         bool startFresh = !System.IO.File.Exists(_xmlPath);
            //load(startFresh, loggerFactory);	
            foreach (var rpi in _piCollection.zeros)
            {
                foreach (var rpiDevice in rpi.PirDevices)
                {

                    PIRDevice pir = new PIRDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpi.ipAddress);
                    pir.mcu.port = rpi.port;
                    _detects.Add(pir);
                }
            }
            _detects.ForEach(d => d.createLogger(loggerFactory));
            _detects.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
            return;
        }

        ~PIRDevices()
		{
			foreach (var sw in _detects)
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
                                              Environment.GetEnvironmentVariable("AREA_DETECTIONS_PATH"); } }
		public int Count { get { return _detects.Count; } }
 
		public List<PIRDevice> detections { get { return _detects; } }
        /*
		public void save()
		{			 
			Common.SaveToXML<List<PIRDevice>>(_detects, _xmlPath);
		}

		public void load(bool startFresh, ILoggerFactory loggerFactory)
		{
			_detects.Clear();

			_logger.LogInformation($"load({startFresh}) entered");
			 
			if (startFresh)
			{			
				_detects.AddRange(PIRDevice.buildDefaultListForTrack());
				//_detects.AddRange(PIRDevice.buildDefaultListForHome());
				save();			 
			}

			else
			{
				_detects.AddRange(Common.LoadFromXML<List<PIRDevice>>(_xmlPath));				 
			}
            _detects.ForEach(d => d.createLogger(loggerFactory));
			_detects.ForEach(d => d.controlledAreas = _controlledAreas.controlledAreas);
		}
        */
	}
}

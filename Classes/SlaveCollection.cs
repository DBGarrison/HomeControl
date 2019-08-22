using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpiConfig.Classes;

namespace HomeControl.Classes
{
	public interface ISlaveCollection
	{

		List<RaspberryPi> zeros { get; }
		List<NodeMCU> nodeMCUs { get; }
		void load(bool startFresh);
		void save();
	}

	public class SlaveColection : ISlaveCollection
	{
		private readonly ILogger _logger;
		public List<RaspberryPi> zeros { get; set; }
		public List<NodeMCU> nodeMCUs { get; set; }

		public SlaveColection(ILoggerFactory loggerFactory)
		{
			try
			{
				_logger = loggerFactory.CreateLogger<SlaveColection>();

				zeros = new List<RaspberryPi>();
				nodeMCUs = new List<NodeMCU>();

				string file = $"{Common.configFolder}}/RemoteHosts.xml";

				load(!System.IO.File.Exists(file));

			}
			catch (Exception ex)
			{
				_logger.LogError($"Constructor(): Unexpected error: {ex.Message}");
			}
			return;
		}

		 
		public void load(bool startFresh)
		{
			List<SlaveDevice> slaves;
			zeros = new List<RaspberryPi>();
			nodeMCUs = new List<NodeMCU>();
			string file = $"{Common.configFolder}/RemoteHosts.xml";

			if (!startFresh)
			{
				slaves = Common.LoadFromXML<List<SlaveDevice>>(file);
			}
			else
			{
				slaves = SlaveDevice.buildMcuListForTrack(Enums.RemoteOSType.Rpi | Enums.RemoteOSType.Esp8266);
				Common.SaveToXML<List<SlaveDevice>>(slaves, file);
			}

			foreach (SlaveDevice slave in slaves.Where(pi => pi.osType == Enums.RemoteOSType.Rpi))
			{
				try
				{
					RaspberryPi rpi;
					file = $"{Common.configFolder}/{slave.ipAddress}.xml";

					if (System.IO.File.Exists(file))
						rpi = Common.LoadFromXML<RaspberryPi>(file);
					else
					{
						rpi = new RaspberryPi(slave.ipAddress, slave.port);
						rpi.readRemoteConfigFile();
						Common.SaveToXML<RaspberryPi>(rpi, file);
					}

					zeros.Add(rpi);
					//foreach (var rpiDevice in rpi.Relays)
					//{

					//    RelayDevice relay = new RelayDevice((Enums.ControlledAreas)rpiDevice.area,
					//                                            rpiDevice.deviceName,
					//                                            rpiDevice.state == 1,
					//                                            rpi.ipAddress);
					//    relay.mcu.port = rpi.port;
					//    _relayDevices.relays.Add(relay);
					//}

					//foreach (var rpiDevice in rpi.PirDevices)
					//{

					//    PIRDevice pir = new PIRDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpi.ipAddress);
					//    pir.mcu.port = rpi.port;
					//    _pirDevices.pirDevices.Add(pir);
					//}

					//foreach (var rpiDevice in rpi.RFSwitches)
					//{
					//    RFSwitchDevice sw = new RFSwitchDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpiDevice.bitLength);
					//    sw.mcu.ipAddress = rpi.ipAddress;
					//    sw.mcu.port = rpi.port;

					//    rpiDevice.onCodes.Skip(1).ToList().ForEach(c => sw.addCode(true, (Convert.ToInt64(c))));

					//    rpiDevice.offCodes.Skip(1).ToList().ForEach(c => sw.addCode(false, (Convert.ToInt32(c))));

					//    _rfSwitches.areaSwitches.Add(sw);
					//}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Constructor(): Unexpected error: {ex.Message}");
				}
			}

			foreach (SlaveDevice slave in slaves.Where(pi => pi.osType == Enums.RemoteOSType.Esp8266))
			{
				try
				{
					NodeMCU nodeMcu;
					file = $"{Common.configFolder}/{slave.ipAddress}.xml";

					if (System.IO.File.Exists(file))
						nodeMcu = Common.LoadFromXML<NodeMCU>(file);
					else
					{
						nodeMcu = new NodeMCU(slave.ipAddress, slave.port);
						nodeMcu.readRemoteConfigFile();
						Common.SaveToXML<NodeMCU>(nodeMcu, file);
					}

					nodeMCUs.Add(nodeMcu);
					//foreach (var rpiDevice in rpi.Relays)
					//{

					//    RelayDevice relay = new RelayDevice((Enums.ControlledAreas)rpiDevice.area,
					//                                            rpiDevice.deviceName,
					//                                            rpiDevice.state == 1,
					//                                            rpi.ipAddress);
					//    relay.mcu.port = rpi.port;
					//    _relayDevices.relays.Add(relay);
					//}

					//foreach (var rpiDevice in rpi.PirDevices)
					//{

					//    PIRDevice pir = new PIRDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpi.ipAddress);
					//    pir.mcu.port = rpi.port;
					//    _pirDevices.pirDevices.Add(pir);
					//}

					//foreach (var rpiDevice in rpi.RFSwitches)
					//{
					//    RFSwitchDevice sw = new RFSwitchDevice((Enums.ControlledAreas)rpiDevice.area, rpiDevice.deviceName, rpiDevice.bitLength);
					//    sw.mcu.ipAddress = rpi.ipAddress;
					//    sw.mcu.port = rpi.port;

					//    rpiDevice.onCodes.Skip(1).ToList().ForEach(c => sw.addCode(true, (Convert.ToInt64(c))));

					//    rpiDevice.offCodes.Skip(1).ToList().ForEach(c => sw.addCode(false, (Convert.ToInt32(c))));

					//    _rfSwitches.areaSwitches.Add(sw);
					//}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Constructor(): Unexpected error: {ex.Message}");
				}
			}
		}

		public void save()
		{
			throw new NotImplementedException();
		}
	}
}

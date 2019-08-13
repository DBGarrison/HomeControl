using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpiConfig.Classes;

namespace HomeControl.Classes
{
    public interface IPiCollection
    {

        List<RaspberryPi> zeros { get; }
        void load(bool startFresh);
        void save();
    }

    public class PiCollection : IPiCollection
    {
        private readonly ILogger _logger;
        public List<RaspberryPi> zeros { get; set; }

        public PiCollection(ILoggerFactory loggerFactory)
        {
            try
            {
                _logger = loggerFactory.CreateLogger<PiCollection>();

                zeros = new List<RaspberryPi>();
                List<SlaveDevice> slaves;
                string folder = Environment.GetEnvironmentVariable("CONFIG_FOLDER");
                string file = $"{folder}/RemoteHosts.xml";

                if (System.IO.File.Exists(file))
                {
                    slaves = Common.LoadFromXML<List<SlaveDevice>>(file);
                }
                else
                {
                    slaves = SlaveDevice.buildMcuListForTrack();
                    Common.SaveToXML<List<SlaveDevice>>(slaves, file);
                }
                
                foreach (SlaveDevice slave in slaves.Where(pi => pi.osType == Enums.RemoteOSType.Rpi))
                {
                    try
                    {
                        RaspberryPi rpi;
                        file = $"{folder}/{slave.ipAddress}.xml";

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
            }
            catch(Exception ex)
            {
                _logger.LogError($"Constructor(): Unexpected error: {ex.Message}");
            }
            return;
        }

         
        public void load(bool startFresh)
        {
            throw new NotImplementedException();
        }

        public void save()
        {
            throw new NotImplementedException();
        }
    }
}

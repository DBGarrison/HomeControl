using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets; 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

 
using HomeControl.Models;
//using HomeControl.HostedServices;
using HomeControl.ObjectDetection;
using HomeControl.RFSwitch;
using HomeControl.Relays;
using HomeControl.Classes;
using RpiConfig.Classes;

namespace HomeControl.WebSocketManager
{
	public class HomeControlSocketHandler : WebSocketHandler
	{
		private IMemoryCache _cache;
		private readonly ILogger _logger;
 
        public List<RFSwitchDevice> rfSwitches { get; private set; }
        public List<PIRDevice> pirDevices { get; private set; }
        public List<RelayDevice> relayDevices { get; private set; }

        //public List <RaspberryPi> zeroList { get { return _piCollection.zeros; } }

        public HomeControlSocketHandler(IMemoryCache memoryCache, 
                                        ILoggerFactory loggerFactory,
                                        IRFSwitches _rfSwitches, 
                                        IPIRDevices _pirDevices, 
                                        IRelayDevices _relayDevices) : base(loggerFactory) {
			_cache = memoryCache;
			_logger = loggerFactory.CreateLogger<HomeControlSocketHandler>();
            rfSwitches = _rfSwitches.areaSwitches;
            pirDevices = _pirDevices.detections;
            relayDevices = _relayDevices.relays; 
        }		 

		protected override int BufferSize { get => 1024 * 4; }

        public override async Task<WebSocketConnection> OnConnected(HttpContext context)
        {
            var name = context.Request.Query["Name"];
            string deviceType = context.Request.Query["DeviceType"];
            var clientIP = context.Connection.RemoteIpAddress.ToString();

            _logger.LogInformation($"OnConnected({clientIP}) DeviceType: {deviceType}");

            if (!string.IsNullOrEmpty(name))
            {                 
                int iDevType;
                if(!Int32.TryParse(deviceType,  out iDevType))
                {
                    _logger.LogError($"OnConnected({clientIP}) Envalid DeviceType: {deviceType}");
                    return null;
                }
                Enums.EnumDeviceType devType = (Enums.EnumDeviceType)iDevType;

                //var connection = Connections.FirstOrDefault(m => ((RFSocketConnection)m).NickName == name);
                var connection = Connections.FirstOrDefault(m => m.NickName == name && m.DeviceType == devType);

                if (connection != null)
                {                     
                    if (connection.WebSocket.State == WebSocketState.Aborted)
                    {
                        _logger.LogWarning($"OnConnected({clientIP}) Existing Connection State  is {connection.WebSocket.State} so Disposing it.");

                        Connections.Remove(connection);
                        connection.WebSocket.Dispose();                        
                        connection = null;
                    }
                    else
                    {
                        _logger.LogInformation($"OnConnected({clientIP}) Using existing {devType} Connection");
                        return connection;
                    }
                }


                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _logger.LogInformation($"OnConnected({clientIP}) Newing up {devType} Connection.");

                switch (devType)
                { //RFSwitch, HS100, CeilingFan, Speed, Motion, Relay
                    case Enums.EnumDeviceType.Relay:
                        connection = new RelaySocketConnection(this)
                        {
                            NickName = name,
                            DeviceType = devType,
                            WebSocket = webSocket
                        };
                        foreach (var relay in relayDevices.Where(s => s.mcu != null && s.mcu.ipAddress == name && s.deviceType == Enums.EnumDeviceType.Relay))
                        {
							relay.setWebSocketConnection(0, connection);
                            //relay.mcu.connection = connection;
                            //connection.mcuList.Add(relay.mcu);
							//connection.mcu = relay.mcu;
						}
                        break;

                    case Enums.EnumDeviceType.Motion:
                        connection = new PIRSocketConnection(this)
                        {
                            NickName = name,
                            DeviceType = devType,
                            WebSocket = webSocket
                        };
                        foreach (var pir in pirDevices.Where(s => s.mcu != null && s.mcu.ipAddress == name && s.deviceType == Enums.EnumDeviceType.Motion))
                        {
							pir.setWebSocketConnection(0, connection);
							//pir.mcu.connection = connection;
                            //connection.mcuList.Add(pir.mcu);
                        }
                        break;

                    case Enums.EnumDeviceType.RFSwitch:
                        connection = new RFSocketConnection(this)
                        {
                            NickName = name,
                            DeviceType = devType,
                            WebSocket = webSocket
                        };
                        foreach (var sw in rfSwitches)
                        {
							sw.setWebSocketConnection(0, connection);
							//var mcu = sw.slaves.Where(s => s.ipAddress == name && s.deviceType == Enums.EnumDeviceType.RFSwitch).FirstOrDefault();
       //                     if (mcu != null)
       //                     {
							//	sw.setWebSocketConnection(0, connection);
       //                         mcu.connection = connection;
       //                         connection.mcuList.Add(mcu);
       //                     }
                        }
                        break;

                    default:
                        _logger.LogInformation($"OnConnected({clientIP}) Unhandled MCU name: {name}");
                        break;
                }
                Connections.Add(connection);
                return connection;
            }

            _logger.LogError($"OnConnected({clientIP}) Request missing Name parameter.");
            return null;
        }       
    }
}

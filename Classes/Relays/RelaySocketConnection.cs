using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using HomeControl.Models;
using HomeControl.Extensions;

namespace HomeControl.Relays
{
    public class RelaySocketConnection : WebSocketConnection
    {
		private readonly ILogger _logger;
	 
		public uint configIdentifier { get; set; }

        public RelaySocketConnection(WebSocketHandler handler) : base(handler)
        {
			_logger = handler._loggerFactory.CreateLogger<RelaySocketConnection>();
			 
		}
 
		public override async Task ReceiveAsync(string message)
		{
			//_logger.LogInformation($"ReceiveAsync({NickName}): {message}");

			//RFSwitchResponce responce = new RFSwitchResponce() { success = false};
			string responce = "";

			try
			{				 
				 
				var rcv = JsonConvert.DeserializeObject<MCUMessage>(message);

                string[] devices = rcv.Message.Split('|');
				 
				switch (rcv.MessageType)
				{
					case "startup":
						//MCU has powered up. get the timestamp of the switch config
						{
                             if (rcv.Message == "1")
                            {
                                responce = "";
                                break;
                            }

                            int identified = 0;

                            //areaId:areaName:deviceId:deviceName:inProgress:motionPin:startMillis:runToMillis:endMillis:duration
                            for (int i = 0; i < devices.Length; i++)
							{
								//relay:areaId:deviceName:deviceId:swState
                                string[] parts = devices[i].Split(':');
                                int areaId = Convert.ToInt32(parts[0]);
								int remoteDeviceId = Convert.ToInt32(parts[1]);
								string deviceName = parts[2].Trim();
								int swState = Convert.ToInt32(parts[3]);
                                 
                                RelayDevice relaydevice = this.webSocketHandler.relayDevices.Where(d => (int)d.area == areaId && d.deviceName == deviceName && d.mcu.ipAddress == this.NickName).FirstOrDefault();

                                if (relaydevice != null)
								{
                                    identified++;
                                    relaydevice.setWebSocketConnection(remoteDeviceId, this);
									//relaydevice.mcu.remoteDeviceId = deviceId;
									if (relaydevice.swState != swState)
									{
										relaydevice.setState(swState, true);
										//AreaEventType eventType = swState == 1 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
										//AreaEvent evt = new AreaEvent(relaydevice.area, relaydevice.deviceName, eventType, NickName, AreaEventStatus.Complete);
										//relaydevice.Events.Add(evt);
										//EventLogger.logEvent(evt);
										//relaydevice.swState = swState;
									}
									//relaydevice.Connection = this;
								}
                                else
                                {
                                    _logger.LogWarning($"Failed to find motion: Area {areaId} Name: {deviceName}");
                                }
							}

                            if (this.NickName.StartsWith("RpiZero"))
                            {
                                responce = $"Startup complete. {devices.Length} relays received. {identified} identified";
                            }
                        }
				 
						break;

					case "status":
                        {
                            for (int i = 0; i < devices.Length; i++)
                            {
                                string[] parts = devices[i].Split(':');
                                int areaId = Convert.ToInt32(parts[1]);
                                string name = parts[2].Trim();
                                int deviceId = Convert.ToInt32(parts[3]);
                                string swState = parts[4];

                                RelayDevice relaydevice = this.webSocketHandler.relayDevices.Where(d => (int)d.area == areaId && d.deviceName == name && d.mcu.ipAddress == this.NickName).FirstOrDefault();

                                if (relaydevice != null)
                                {
                                    relaydevice.swState = Convert.ToInt32(swState);
                                }
                                else
                                {
                                    _logger.LogWarning($"Failed to find motion: Area {areaId} Name: {name}");
                                }
                            }
                        }
						break;

                    case "relayChanged":
                    case "relaychanged":
                        {
							//relay:area name:device name:device id:swState
							string[] parts = devices[0].Split(':');                           
							int deviceId = Convert.ToInt32(parts[3]);							                           
							int swState = Convert.ToInt32(parts[4]);

                            //RelayDevice relaydevice = this.webSocketHandler.relays.Where(d => (int)d.area == areaId && d.deviceName == name && d.mcu.ipAddress == this.NickName).FirstOrDefault();
                            RelayDevice relaydevice = this.webSocketHandler.relayDevices.Where(d => d.mcu.ipAddress == this.NickName && d.mcu.remoteDeviceId == deviceId).FirstOrDefault();

							if (relaydevice != null)
                            {
                                relaydevice.setState(swState, true);
                            }
                            else
                            {
                                _logger.LogWarning($"Failed to find motion: Area {parts[1]} Name: {parts[2]}");
                            }

                        }
                        break;

					case "ping":
						{
							responce = $"ping:received from RelayController: {NickName}";
						}
						break;
				}

			}
			catch (Exception ex)
			{
				_logger.LogError($"ReceiveAsync() Unexpected error: {ex.Message}");
			}
			bool sendResponce = true;
			if (sendResponce && !string.IsNullOrEmpty(responce))
			{
				await SendMessageAsync(responce);
			}
			/*
            var receiver = Handler.Connections.FirstOrDefault(m => ((RFSwitchConnection)m).NickName == receiveMessage.Receiver);

            if (receiver != null)
            {
                var sendMessage = JsonConvert.SerializeObject(new SendMessage
                {
                    Sender = NickName,
                    Message = receiveMessage.Message
                });

                await receiver.SendMessageAsync(sendMessage);
            }
            else
            {
                var sendMessage = JsonConvert.SerializeObject(new SendMessage
                {
                    Sender = NickName,
                    Message = "Can not seed to " + receiveMessage.Receiver
                });

                await SendMessageAsync(sendMessage);
            }*/
		}

		private class MCUMessage
		{
			public string MessageType { get; set; }

			public string Message { get; set; }
		}
	
        /*
		private class MCUDevice
		{
			//13:Garage North:1:Garage North:0:5:37524:44965:44966:7442
			public MCUDevice(string[] parts)
			{				 
				areaId = Convert.ToInt32(parts[0]);
				areaName = parts[1];
				deviceId = Convert.ToInt32(parts[2]);
				deviceName = parts[3];
				inProgress = parts[4] == "1";
				motionPin = Convert.ToInt32(parts[5]);
				startMillis = Convert.ToInt32(parts[6]);
				runToMillis = Convert.ToInt32(parts[7]);
				endMillis = Convert.ToInt32(parts[8]);
				duration = Convert.ToInt32(parts[9]);
			}
			public int areaId { get; set; }
			public string areaName { get; set; }
			public int deviceId { get; set; }
			public string deviceName { get; set; }
			public bool inProgress { get; set; }
			public int motionPin { get; set; }
			public int startMillis { get; set; }
			public int runToMillis { get; set; }
			public int endMillis { get; set; }
			public int duration { get; set; }


		}
		 */

		 
    }
}
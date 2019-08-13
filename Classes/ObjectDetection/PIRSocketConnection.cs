using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using HomeControl.Models;
using HomeControl.Extensions;

namespace HomeControl.ObjectDetection
{
    public class PIRSocketConnection : WebSocketConnection
    {
		private readonly ILogger _logger;
	 
		public uint configIdentifier { get; set; }

		public PIRSocketConnection(WebSocketHandler handler) : base(handler)
        {
			_logger = handler._loggerFactory.CreateLogger<PIRSocketConnection>();
			 
		}
		~PIRSocketConnection()
		{
			 
		}
		public override async Task ReceiveAsync(string message)
		{
			//_logger.LogInformation($"ReceiveAsync({NickName}): {message}");

			//RFSwitchResponce responce = new RFSwitchResponce() { success = false};
			string responce = "";

			try
			{
				//MessageType
				if (message.Length < 14 || message.Substring(0, 1) != "{" && message.Substring(message.Length - 1, 1) != "}")
				{

					responce = "Expecting JSON for MessageType Object.";
				}
				else
				{
					var mcuMessage = JsonConvert.DeserializeObject<MCUMessage>(message);

					switch (mcuMessage.MessageType)
					{
						case "startup":
							//MCU has powered up. get the timestamp of the switch config
							{
								int identified = 0;
								//areaId:areaName:deviceId:deviceName:inProgress:motionPin:startMillis:runToMillis:endMillis:duration
								var mcuDevices = PIRDevice.buildListFromStartUp(mcuMessage.Message, this.NickName);
								for (int i = 0; i < mcuDevices.Count; i++)
								{
									PIRDevice pirStartUp = mcuDevices[i];
									PIRDevice existingPir = this.webSocketHandler.pirDevices.Where(d => d.area == pirStartUp.area && d.deviceName == pirStartUp.deviceName && pirStartUp.mcu.ipAddress == this.NickName).FirstOrDefault();

									if (existingPir != null)
									{
										existingPir.setWebSocketConnection(pirStartUp.mcu.remoteDeviceId, this);
										//existingPir.mcu.remoteDeviceId = pirStartUp.mcu.remoteDeviceId;
										identified++;
										//existingPir.Connection = this;
									}
									/*else
									{
										_logger.LogWarning($"Adding motion from StartUp: Area {pirStartUp.areaName} Name: {pirStartUp.deviceName}");
										pirStartUp.mcu.connection = this;
										this.webSocketHandler.areaDetections.Add(pirStartUp);
									}*/
								}

								if (this.NickName.StartsWith("RpiZero"))
								{
									responce = $"Startup complete. {mcuDevices.Count} detections received. {identified} identified";
								}
							}
							break;

						case "start":
							//start:1
							{

								if (string.IsNullOrEmpty(mcuMessage.Message))
								{
									_logger.LogError($"ReceiveAsync({NickName}): Empty Message!!");
									break;
								}

								int remoteDeviceId = Convert.ToInt32(mcuMessage.Message);
								PIRDevice pirdevice = this.webSocketHandler.pirDevices.Where(d => d.mcu.ipAddress == this.NickName && d.mcu.remoteDeviceId == remoteDeviceId).FirstOrDefault();
								if (pirdevice != null)
									await pirdevice.startDetecting();
								else
									responce = $"Start failed: Unable to find PIRDevice (id={remoteDeviceId}, mcu={this.NickName})";

								//await setRelayStates(pirdevice.area, true);

							}
							break;

						case "stop":
							///stop:1
							{
								if (string.IsNullOrEmpty(mcuMessage.Message))
								{
									_logger.LogError($"ReceiveAsync({NickName}): Empty Message!!");
									break;
								}

								int remoteDeviceId = Convert.ToInt32(mcuMessage.Message);
								PIRDevice pirdevice = this.webSocketHandler.pirDevices.Where(d => d.mcu.ipAddress == this.NickName && d.mcu.remoteDeviceId == remoteDeviceId).FirstOrDefault();
								if (pirdevice != null)
									pirdevice?.stopDetecting();
								else
									responce = $"Stop failed: Unable to find PIRDevice (id={remoteDeviceId}, mcu={this.NickName})";
								//await setRelayStates(pirdevice.area, false);

							}
							break;

						case "ping":
							{
								responce = $"ping:received from PIRController: {NickName}";
							}
							break;
					}
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
	  
    }
}


/*
        private async Task setRelayStates(Enums.ControlledAreas area, bool swState)
        {
            List<Enums.ControlledAreas> areas = new List<Enums.ControlledAreas>();

            switch (area)
            {
                case Enums.ControlledAreas.GarageArea:
                case Enums.ControlledAreas.GarageNorth:
                case Enums.ControlledAreas.GarageEast:
                case Enums.ControlledAreas.GarageWest:
                case Enums.ControlledAreas.GarageSouth:
                    areas.Add(Enums.ControlledAreas.GarageArea);
                    areas.Add(Enums.ControlledAreas.GarageNorth);
                    areas.Add(Enums.ControlledAreas.GarageEast);
                    areas.Add(Enums.ControlledAreas.GarageWest);
                    areas.Add(Enums.ControlledAreas.GarageSouth);
                    break;

                case Enums.ControlledAreas.Office:

                    break;

                case Enums.ControlledAreas.BackYard:
                case Enums.ControlledAreas.RearDeck:
                    //Turn on rear porch light
                    break;

                case Enums.ControlledAreas.BathRoom:
                case Enums.ControlledAreas.BedRoom:
                case Enums.ControlledAreas.FrontDeck:
                case Enums.ControlledAreas.FrontYard:
                case Enums.ControlledAreas.Kitchen:
                case Enums.ControlledAreas.LivingRoom:
                case Enums.ControlledAreas.MasterBathRoom:
                case Enums.ControlledAreas.MasterBedRoom:

                    break;
            }

            foreach (var relay in this.webSocketHandler.relays.Where(r => areas.Contains(r.area) && r.mcu != null && r.mcu.connection != null))
            {
                relay.swState = swState ? 1 : 0;
                await relay.mcu.connection.SendMessageAsync($"setdevice:{relay.mcu.remoteDeviceId}:{relay.swState}");
            }
        }*/

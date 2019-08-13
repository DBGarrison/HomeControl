using System;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.WebSocketManager;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using HomeControl.Extensions;

namespace HomeControl.RFSwitch
{
    public class RFSocketConnection : WebSocketConnection
    {
		private readonly ILogger _logger;
		//private Dictionary<string, Tuple<uint, DateTime>> _startupConfigIds;

		public uint configIdentifier { get; set; }

		public RFSocketConnection(WebSocketHandler handler) : base(handler)
        {
			_logger = handler._loggerFactory.CreateLogger<RFSocketConnection>();
			//_startupConfigIds = new Dictionary<string, Tuple<uint, DateTime>>();
		}

		//public string NickName { get; set; }

		public override async Task ReceiveAsync(string message)
		{
			_logger.LogInformation($"ReceiveAsync({NickName}): {message}");

			//RFSwitchResponce responce = new RFSwitchResponce() { success = false};
			string responce = "";

			try
			{
				Enums.ControlledAreas area;
				HomeControlSocketHandler hdr = this.webSocketHandler as HomeControlSocketHandler;
				var rcv = JsonConvert.DeserializeObject<RFSwitchMessage>(message);
				string[] parts = rcv.Message.Split(':');
				int areaId, deviceId, swState;
				string deviceName;
				uint epoch;
				RFSwitchDevice sw = null; ;

				switch (rcv.MessageType)
				{
					case "startup":
						//MCU has powered up. get the timestamp of the switch config
						//If the the MCU's timestamp matches the WebAPI's timestamp then
						{
							epoch = Convert.ToUInt32(rcv.Message);
							if (UInt32.TryParse(rcv.Message, out epoch))
							{
								//does the MCU timestamp match the server's RFSwitch config timestamp?
								configIdentifier = epoch;

								DateTime dateTime = epoch.fromUnixTimestamp();
								//_startupConfigIds[NickName] = new Tuple<uint, DateTime>(epochTimeStamp, dateTime);							 
								_logger.LogInformation($"ReceiveAsync({NickName}): Startup Timestamp ({epoch}) converts to {dateTime.ToString()}");
                                //TODO: Activate RFSwitches
								//await hdr.rfCollection.activateSwitches(this, epoch);
							}
							else
							{
								responce = $"Invalid startup Message: {rcv.Message}";
							}
						}
						break;

					case "switchActivated":
						//switchActivated:areaId:deviceName:deviceId
						//areaId:deviceName:deviceId
						if (string.IsNullOrEmpty(rcv.Message))
						{
							_logger.LogError($"ReceiveAsync({NickName}): Empty Message!!");
							break;
						}


						if (parts.Length < 3)
						{
							_logger.LogError($"ReceiveAsync({NickName}): Invalid switchActivated Message!!");
							break;
						}

						areaId = Convert.ToInt32(parts[0]);
						deviceName = parts[1];
						deviceId = Convert.ToInt32(parts[2]);

						area = (Enums.ControlledAreas)areaId;

						sw = hdr.rfSwitches.Where(s => (s.area == area) && s.deviceName == deviceName && s.mcu.ipAddress == this.NickName).FirstOrDefault();
						if (sw != null) // && sw.swState != swState)
						{
							sw.setWebSocketConnection(deviceId, this);
							//var mcu = sw.slaves.Where(s => s.ipAddress == NickName).FirstOrDefault();
							//if (mcu != null)
							//{
							//	mcu.remoteDeviceId = deviceId;
							//}
						}

						break;

					case "configIdChanged":

						break;

					case "switchchanged":

						if (string.IsNullOrEmpty(rcv.Message))
						{
							_logger.LogError($"ReceiveAsync({NickName}): Empty Message!!");
							break;
						}

						///switchchanged:areaId:deviceName:swState:lastOnState:lastOffState
						//areaId:deviceName:swState

						if (parts.Length < 3)
						{
							_logger.LogError($"ReceiveAsync({NickName}): Invalid switchchanged Message!!");
							break;
						}

						areaId = Convert.ToInt32(parts[0]);
						deviceName = parts[1];
						swState = Convert.ToInt32(parts[2]);
						area = (Enums.ControlledAreas)areaId;

						sw = hdr.rfSwitches.Where(s => (s.area == area) && s.deviceName == deviceName && s.mcu.ipAddress == this.NickName).FirstOrDefault();
						if (sw != null)
						{
							if (sw.swState != swState)
							{
								sw.setState(swState, true);
								//sw.swState = swState;
						 
								//AreaEventType aet = swState > 0 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
								//AreaEvent evt = new AreaEvent(sw.area, deviceName, aet, NickName, AreaEventStatus.Complete);
								//sw.Events.Add(evt);
								//EventLogger.logEvent(evt);								 							
							}
							else
							{
								_logger.LogInformation($"switchChanged() Done. State was unchanged.");
							}
						}

						break;

					case "ping":
						{
							responce = $"ping:received from RFController: {NickName}";
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

		private class RFSwitchMessage
		{
			public string MessageType { get; set; }

			public string Message { get; set; }
		}

		private class RFSwitchResponce
		{
			public bool success { get; set; }

			public string message { get; set; }
		}

		 
    }
}
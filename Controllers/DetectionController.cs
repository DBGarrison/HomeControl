using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
 
using HomeControl.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using HomeControl.ObjectDetection;
using HomeControl.Relays;
using HomeControl.WebSocketManager;

namespace HomeControl.Controllers
{
	[Route("api/[controller]")]
	public class DetectionController : Controller
	{
		private PIRDevices _pirDeviceCollection;
		private RelayDevices _relayCollection;
		private List<PIRDevice> pirDevices { get { return _pirDeviceCollection.detections; } }
		private List<RelayDevice> relayDevices { get { return _relayCollection.relays; } }

		//private IBackgroundTaskQueue _queue;
		private readonly ILogger _logger;

		public DetectionController( ILoggerFactory loggerFactory, IPIRDevices pirDevices, IRelayDevices relayDevices)
		{			 			
			_logger = loggerFactory.CreateLogger<DetectionController>();
			_pirDeviceCollection = pirDevices as PIRDevices;
			_relayCollection = relayDevices as RelayDevices;


		}
 
		[HttpGet]
		public ActionResult<IEnumerable<PIRDevice>> Get()
		{
			//var xxx = Classes.ControlledAreas.List;

			return pirDevices;
		}

		//[HttpGet("{id}", Name = "GetById")]
		[HttpGet("[action]/{id}")]
		public ActionResult<PIRDevice> Get(int id)
		{
			Console.WriteLine($"GetById() Entered. deviceId: {id}");
			try
			{
				var area = pirDevices.Where(s => s.localDeviceId == id).First();
				 
				return area;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return NotFound();
			}
		}

        [HttpGet("[action]/{hostName}")]
        public ActionResult<List<PIRDevice>> getMcuDetections(string hostName)
        {
            _logger.LogInformation($"getMcuRelays() Entered. hostName: {hostName}");
            //Enums.ControlledAreas area = (Enums.ControlledAreas)areaId;
            List<PIRDevice> result = pirDevices.Where(s => s.mcu.ipAddress.Equals(hostName, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }

        [HttpGet("[action]/{deviceId}")]
		public async Task<IActionResult> start(int deviceId)
		{
			//var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
			Console.WriteLine($"startDetection() Entered. deviceId: {deviceId} ");

			try
			{
				var pirDevice = pirDevices.Where(s => s.localDeviceId == deviceId).First();
				
				await pirDevice.startDetecting();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return Ok(ex.Message);
			}
			return Ok("startDetection() Entered");
		}

		[HttpGet("[action]/{deviceId}")]
		public IActionResult end(int deviceId)
		{
			//var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

			Console.WriteLine($"Detection ending request for device: {deviceId}.");

			try
			{
				var area = pirDevices.Where(s => s.localDeviceId == deviceId).First();

				area.stopDetecting();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return NotFound();
			}
			return Ok("endDetection() Entered");
		}

		[HttpGet("[action]/{ipAddr}")]
		public async Task<ActionResult> subscribe(string ipAddr)
		{
			_logger.LogInformation($"activateSwitches() Entered. IP: {ipAddr}");
			 

			if (string.IsNullOrEmpty(ipAddr))
			{
				return Ok($"Empty Ipaddress");
			}

			foreach(var pir in pirDevices)
			{
				pir.mcu = new Classes.MCUDevice(ipAddr, 80, Enums.EnumDeviceType.Motion);
			}
			/*
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri($"http://{ipAddr}/");				 
				 
				HttpResponseMessage response = await client.GetAsync($"subscribe?hostPort={Environment.GetEnvironmentVariable("HostPort")}");
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsAsync<List<AreaDetectionResponse>>();

					foreach(var detect in result)
					{
						var ad = pirDevices.Where(d => d.deviceName == detect.areaName).FirstOrDefault();
						if(ad != null)
						{
							ad.remoteDeviceId = detect.deviceId;
						}
					}
					//_logger.LogInformation($"api returned: {await response.Content.ReadAsStringAsync()}");

				}
				else
				{
					_logger.LogError($"api Error: {await response.Content.ReadAsStringAsync()}");
				}

			};
			*/
			return Ok("subscribe() Entered");
		}
		#region AreaEvents

		[HttpGet("[action]/{cameraHost}")]
		public ActionResult<List<AreaEvent>> getCameraEvents(string cameraHost)
		{
			List<AreaEvent> events = EventLogger.getCameraEvents(cameraHost);
			return events;
		}

		[HttpGet("[action]/{mcuName}")]
		public ActionResult<List<AreaEvent>> getMcuEvents(string mcuName)
		{
			List<AreaEvent> events =   EventLogger.getMcuEvents(mcuName);
			return events;
		}

		[HttpGet("getAreaEvents/{eventType}/{area}")]
		public ActionResult<List<AreaEvent>> getAreaEvents(AreaEventType eventType, Enums.ControlledAreas area)
		{
			//_logger.LogInformation($"getAreaEvents() Entered.");
			List<AreaEvent> events = EventLogger.getAreaEvents(eventType, area);
			return events;
		}
		#endregion

		#region Relays

		[HttpGet("[action]")]
		public ActionResult<List<RelayDevice>> getRelays()
		{
			//_logger.LogInformation($"getRelays() Entered.");
			 
			return relayDevices;
		}

		[HttpGet("getSwitch/{area}/{deviceName}")]
		public ActionResult<RelayDevice> getRelay(Enums.ControlledAreas area, string deviceName)
		{
			_logger.LogInformation($"getRelay() Entered. area: {area} device: {deviceName}");

			RelayDevice relay = relayDevices.Where(s => s.area == area && s.deviceName.ToLower() == deviceName.ToLower()).FirstOrDefault();
			if (relay != null)
			{
				return relay;
			}
			return NotFound();
		}

        [HttpGet("[action]/{hostName}")]
        public ActionResult<List<RelayDevice>> getMcuRelays(string hostName)
        {
            _logger.LogInformation($"getMcuRelays() Entered. hostName: {hostName}");
            //Enums.ControlledAreas area = (Enums.ControlledAreas)areaId;
            List<RelayDevice> result = relayDevices.Where(s => s.mcu.ipAddress.Equals(hostName, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }


        [HttpGet("[action]/{area}")]
		public ActionResult<List<RelayDevice>> getAreaRelays(Enums.ControlledAreas area)
		{
			_logger.LogInformation($"getAreaRelays() Entered. area: {area}");
			//Enums.ControlledAreas area = (Enums.ControlledAreas)areaId;
			List<RelayDevice> result = relayDevices.Where(s => s.area == area).ToList();

			return result;
		}

		[HttpGet("[action]/{area}/{deviceName}/{swState}")]
		public async Task<ActionResult<RelayDevice>> setRelay(Enums.ControlledAreas area, string deviceName, int swState)
		{
			RelayDevice relay = relayDevices.Where(s => s.area == area && s.deviceName.ToLower() == deviceName.ToLower()).FirstOrDefault();
			if (relay == null)
				return NotFound();
			else
				return await setRelay(relay.localDeviceId, swState);
		}

		[HttpGet("[action]/{localId}/{swState}")]
		public async Task<ActionResult<RelayDevice>> setRelay(int localId, int swState)
		{
			//var ipAddr = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;

			var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

			if (clientIP == "::1")
			{
				string[] parts = Environment.GetEnvironmentVariable("HOSTPORT").Split(':');
				clientIP = parts[0];
			}

			_logger.LogInformation($"setRelay() Entered. deviceId: {localId} State {swState} from {clientIP}");

			RelayDevice relay = relayDevices.Where(s => s.localDeviceId == localId).FirstOrDefault();

			if (relay == null)
			{
				return NotFound();
			}
			else //if (sw.swState != swState)
			{
				relay.setState(swState, true);
				//relay.swState = swState;				 
				//AreaEventType evtType = (swState > 0) ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
				//AreaEvent evt = new AreaEvent(relay.area, relay.deviceName, evtType, clientIP, AreaEventStatus.Complete);
				//relay.Events.Add(evt);
				//EventLogger.logEvent(evt);		
  
				await relay.mcu.connection?.SendMessageAsync($"relaychanged:{relay.mcu.remoteDeviceId}:{relay.swState}");
				return relay;
			}


		}

		[HttpGet("[action]/{areaId}/{deviceName}/{swState}")]
		public ActionResult<RelayDevice> relayChanged(int areaId, string deviceName, int swState)
		{
			var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

			lock (clientIP)
			{
				_logger.LogInformation($"relayChanged() Entered. areaId: {areaId} deviceName: {deviceName} state: {swState} from {clientIP}");

				RelayDevice relay = relayDevices.Where(s => s.area == (Enums.ControlledAreas)areaId && s.deviceName == deviceName).FirstOrDefault();

				if (relay == null)
					return NotFound();

				if (relay.swState != swState)
				{
					//relay.swState = swState;					 
					//AreaEventType aet = swState > 0 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
					//AreaEvent evt = new AreaEvent(relay.area, deviceName, aet, relay.mcu.ipAddress, AreaEventStatus.Complete);
					//relay.Events.Add(evt);
					//EventLogger.logEvent(evt);
					relay.setState(swState, true);
				}
				else
				{
					_logger.LogInformation($"switchChanged() Done. State was unchanged.");
				}
				return relay;

			}

		}

		#endregion
		/*
		[HttpGet("[action]/{deviceId}")]
		public async Task<IActionResult> GetVideoContent(int deviceId)
		{
			 

			Console.WriteLine("About to end Detection().");
			try
			{
				var area = detections.Where(s => s.localDeviceId == deviceId).First();
				//if (Program.TryOpenFile("BigBuckBunny.mp4", FileMode.Open, out FileStream fs))
				//{
				//	return new FileStreamResult(fs, new MediaTypeHeaderValue("video/mp4").MediaType);
				//}
				//await area.stopDetecting();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return NotFound();
			}

			return BadRequest();
		}*/
	}
}

/*
 [Route("api/[controller]")]
	[ApiController]
	public class ObjectDetectionController  : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}

*/
using Microsoft.AspNetCore.Mvc;
 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using HomeControl.RFSwitch;
using HomeControl.Models; 
using HomeControl.Extensions;
using HomeControl.Classes;

namespace HomeControl.Controllers
{
	[Route("api/[controller]")]
	//	[Route("api/RFSwitch")]
	[ApiController]
	public class RFController : ControllerBase
	{
		private readonly ILogger _logger;
		private RFSwitches switchCollection;
 
		private List<RFSwitchDevice> rfSwitches { get { return switchCollection.areaSwitches; } }

		public RFController(ILoggerFactory loggerFactory, IRFSwitches switches)
		{
			_logger = loggerFactory.CreateLogger<RFController>();
			switchCollection = switches as RFSwitches;
			return;
		}

		#region Publics
		// GET api/RFSwitch
		[HttpGet]
		public ActionResult<IEnumerable<RFSwitchDevice>> Get()
		{
			return rfSwitches.OrderBy(t => t.areaName).ThenBy(t => t.deviceName).ToList();
		}

		[HttpGet("{id}", Name = "GetById")]
		//[HttpGet("[action]/{id}")]
		public ActionResult<RFSwitchDevice> Get(int id)
		{
			_logger.LogInformation($"GetById() Entered. deviceId: {id}");

			RFSwitchDevice sw = rfSwitches.Where(s => s.localDeviceId == id).FirstOrDefault();
			if (sw != null)
			{
				return sw;
			}
			return NotFound();
		}

		/*
		[HttpGet("getSwitch/{localDeviceId}")]
		public ActionResult<RFSwitchDevice> getSwitch(int localDeviceId)
		{
			_logger.LogInformation($"getSwitch() Entered. deviceId: {localDeviceId}");

			RFSwitchDevice sw = areaSwitches.Where(s => s.localDeviceId == localDeviceId).FirstOrDefault();
			if (sw != null)
			{
				return sw;
			}

			return NotFound();
		}*/


		[HttpGet("getSwitch/{area}/{deviceName}")]
		public ActionResult<RFSwitchDevice> getSwitch(Enums.ControlledAreas area, string deviceName)
		{
			_logger.LogInformation($"getSwitch() Entered. area: {area} device: {deviceName}");

			RFSwitchDevice sw = rfSwitches.Where(s => s.area == area && s.deviceName.ToLower() == deviceName.ToLower()).FirstOrDefault();
			if (sw != null)
			{
				return sw;
			}
			return NotFound();
		}

		[HttpGet("[action]/{area}")]
		public ActionResult<List<RFSwitchDevice>> getAreaSwitches(Enums.ControlledAreas area)
		{
			_logger.LogInformation($"GetSwitches() Entered. area: {area}");
			//Enums.ControlledAreas area = (Enums.ControlledAreas)areaId;
			List<RFSwitchDevice> result = rfSwitches.Where(s => s.area == area).ToList();

			return result;
		}

		[HttpGet("[action]/{area}/{deviceName}/{swState}")]
		public async Task<ActionResult<RFSwitchDevice>> setSwitch(Enums.ControlledAreas area, string deviceName, int swState)
		{
			RFSwitchDevice sw = rfSwitches.Where(s => s.area == area && s.deviceName.ToLower() == deviceName.ToLower()).FirstOrDefault();
			if (sw == null)
				return NotFound();
			else
				return await setSwitch(sw.localDeviceId, swState);
		}

		[HttpGet("[action]/{localId}/{swState}")]
		public async Task<ActionResult<RFSwitchDevice>> setSwitch(int localId, int swState)
		{
			//var ipAddr = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;

			var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

			if(clientIP == "::1")
			{
				string[] parts = Environment.GetEnvironmentVariable("HOSTPORT").Split(':');
				clientIP = parts[0];
			}

			_logger.LogInformation($"SetSwitch() Entered. deviceId: {localId} State {swState} from {clientIP}");

			RFSwitchDevice sw = rfSwitches.Where(s => s.localDeviceId == localId).FirstOrDefault();

			if (sw == null)
			{
				return NotFound();
			}
			else //if (sw.swState != swState)
			{
				sw.setState(swState, true);
				//sw.lastOnState = DateTime.Now.Ticks;
				//AreaEventType eat = (swState > 0) ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
				//AreaEvent evt = new AreaEvent(sw.area, sw.deviceName, eat, clientIP);
				//sw.Events.Add(evt);
				//EventLogger.logEvent(evt);
				//sw.swState = swState;
				//sw.lastOnState = DateTime.Now.Ticks;
				await sw.mcu?.connection?.SendMessageAsync($"switchchanged:{sw.mcu.remoteDeviceId}:{sw.swState}");
				//foreach (var mcu in sw.slaves.Where(s => s.connection != null && s.deviceType == Enums.EnumDeviceType.RFSwitch))
				//{
				//	//await mcu.connection.SendMessageAsync($"switchchanged:{(int)sw.area}:{sw.deviceName}:{sw.swState}");
				//	await mcu.connection.SendMessageAsync($"switchchanged:{mcu.remoteDeviceId}:{sw.swState}");
				//}
				//switchCollection.save();

				return sw;
			}

			 
		}

		[HttpGet("[action]/{areaId}/{deviceName}/{swState}")]
		public ActionResult<RFSwitchDevice> switchChanged(int areaId, string deviceName, int swState)
		{
			var clientIP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

			lock (clientIP)
			{
				_logger.LogInformation($"switchChanged() Entered. areaId: {areaId} deviceName: {deviceName} state: {swState} from {clientIP}");

				RFSwitchDevice sw = rfSwitches.Where(s => s.area == (Enums.ControlledAreas)areaId && s.deviceName == deviceName).FirstOrDefault();

				if (sw == null)
					return NotFound();

				if (sw.swState != swState)
				{
					sw.setState(swState, true);
					//sw.swState = swState;
					//if (swState > 0)
					//	sw.lastOnState = DateTime.Now.Ticks;
					//else
					//	sw.lastOffState = DateTime.Now.Ticks;

					//AreaEventType aet = swState > 0 ? AreaEventType.SwitchOn : AreaEventType.SwitchOff;
					//AreaEvent evt = new AreaEvent(sw.area, deviceName, aet, sw.mcu.ipAddress, AreaEventStatus.Complete);
					//sw.Events.Add(evt);
					//EventLogger.logEvent(evt);
				}
				else
				{
					_logger.LogInformation($"switchChanged() Done. State was unchanged.");
				}
				return sw;

			}
			 
		}

		/*
		[HttpGet("[action]")]
		public async Task<ActionResult> ClearSwitches()
		{
			_logger.LogInformation($"ClearSwitches() Entered.");

			List<string> remoteControllers = new List<string>();

			rfSwitches.ForEach(c => remoteControllers.AddRange(c.slaves.Select(s => s.ipAddress).Distinct()));

			foreach (string ipAddress in remoteControllers.Distinct())
			{
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri($"http://{ipAddress}");
				HttpResponseMessage r1 = await client.GetAsync("/clear");
				if (!r1.IsSuccessStatusCode)
				{
					_logger.LogError($"Error clearing switches on {ipAddress}: {r1.Content.ReadAsStringAsync()}");
				}
				else
				{
					_logger.LogInformation($"Switches cleared on {ipAddress}");
				}
			}

			rfSwitches.ForEach(c => c.slaves.Clear());

			return Ok($"Switches cleared.");
		}


		/// <summary>
		/// When ESP8266 comes online, he will call this API to notify HomeControl.
		/// </summary>
		/// <param name="ipAddr">Name or Address of MCU: Garage, Detector, RFSwitch, e.g.</param>
		/// <returns></returns>
		[HttpGet("[action]/{ipAddr}")]
		public async Task<ActionResult> activateMCU(string ipAddr)
		{
			_logger.LogInformation($"activateMCU() Entered. IP: {ipAddr}");

			if (string.IsNullOrEmpty(ipAddr))
			{
				return Ok($"Empty Ipaddress");
			}

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri($"http://{ipAddr}");

			string hp = Environment.GetEnvironmentVariable("HOSTPORT");
			string[] data = hp.Split(':');
			string query = $"subscribe?host={data[0]}&port={data[1]}";

			List<RFSwitchResult> switches;
			HttpResponseMessage response = await client.GetAsync(query);
			if (response.IsSuccessStatusCode)
			{
				//Returns a list of switches
				switches = await response.Content.ReadAsAsync<List<RFSwitchResult>>();
				//_logger.LogInformation("")
			}
			else
			{
				_logger.LogError($"activateSwitches() Client http failed: {response.Content.ReadAsStringAsync()}");
				switches = new List<RFSwitchResult>();
			}

			int cnt = 0;
			//string hostName = System.Net.Dns.GetHostName();

			//loop through local RF Switches
			foreach (RFSwitchDevice sw in rfSwitches) //.Where(s => s.areaId == areaId))
			{
				//Does this MCU have a matching RFSwitch for area and dev name?
				RFSwitchResult swResult = switches.Where(s => s.controledArea == sw.area && s.deviceName == sw.deviceName).FirstOrDefault();
				if (swResult == null)
				{
					continue;
				}

				sw.setWebSocketConnection(swResult.deviceId, ipAddr);
				//MCUDevice remoteRF = sw.slaves.Where(r => r.ipAddress == ipAddr).FirstOrDefault();
				//if (remoteRF == null)
				//{
				//	//switches
				//	remoteRF = new MCUDevice(ipAddr, 80, Enums.EnumDeviceType.RFSwitch);
				//	sw.slaves.Add(remoteRF);
				//}
				//remoteRF.remoteDeviceId = swResult.deviceId;

			}

			return Ok($"{cnt} switches ativated.");
			//return NotFound();
		}*/
 
	 
		#endregion
		/*
		#region Privates
 

		private void activateSwitches(List<RFSwitchDevice> switches)
		{
			foreach (var sw in switches)
			{
				//sw.remoteDeviceId = 0;
				//ControlledArea area =this.areas.Where(a => a.areaId == sw.areaId).FirstOrDefault();
				//area.areaDevices.Add(sw);
				//sw.area = area;

				foreach (var mcu in sw.rfHosts)
				{
					try
					{
						if (HomeControl.HS100.HS1XX.PingDevice(mcu.ipAddress))
						{
							HttpClient client = new HttpClient();
							client.BaseAddress = new Uri($"http://{mcu.ipAddress}");
							string query = $"/setActive?areaId={sw.area.ToInt()}&name={sw.deviceName}&isActive=1";
							HttpResponseMessage response = client.GetAsync(query).Result;
							if (response.IsSuccessStatusCode)
							{
								var result = response.Content.ReadAsAsync<RFSwitchResult>().Result;
								mcu.remoteDeviceId = result.deviceId;
								//cnt++;
							}
							else
							{
								_logger.LogError($"activateSwitches() Client http failed: {response.Content.ReadAsStringAsync()}");
							}
						}
						else
						{
							_logger.LogError($"activateSwitches() Ping failed: {mcu.ipAddress}");
						}
					}
					catch (Exception ex)
					{
						_logger.LogError($"activateSwitches() Client http failed: {ex.Message}");
					}
				}
			}
		}
		#endregion*/
	}
}
 
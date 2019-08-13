using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using HomeControl;
using HomeControl.Models;
using HomeControl.Exceptions;
using HomeControl.CountDown;
using Microsoft.Extensions.Caching.Memory;
using HomeControl.HS100;

namespace HomeControl.Controllers
{
	[Route("api/HS100")]
	[ApiController]
	public class HS100Controller : ControllerBase
	{
		private IMemoryCache _cache;
		private List<HS100Config> hs100Configs = null;
		private const int timeOut = 30;
		public HS100Controller(IMemoryCache memoryCache)
		{
			_cache = memoryCache;			
		 
			if (!_cache.TryGetValue("HS100Config", out List<HS100Config> cacheEntry))
			{
				// Key not in cache, so get data.
				cacheEntry = Common.LoadFromXML<List<HS100Config>>(Environment.GetEnvironmentVariable("HS100CONFIG_PATH"));

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					// Set cache entry size by extension method.
					.SetSize(1)
					// Keep in cache for this time, reset time if accessed.
					.SetSlidingExpiration(TimeSpan.FromHours(24));

				// Set cache entry size via property.
				// cacheEntryOptions.Size = 1;

				// Save data in cache.
				_cache.Set("HS100Config", cacheEntry, cacheEntryOptions);
			}

			this.hs100Configs = cacheEntry;

		}

		#region Publics
		// GET api/hs100
		[HttpGet]
		public ActionResult<IEnumerable<DeviceInfo>> Get()
		{
			List<DeviceInfo> result = new List<DeviceInfo>();
			var configs = hs100Configs.OrderBy(c => c.DeviceId).ToList();
			hs100Configs = configs;

			foreach (var cfg in configs)
			{
				try
				{ 
					bool isOnLine;
					HS1XX mSmartPlug = getDevice(cfg.DeviceId, out isOnLine);
					if (isOnLine && mSmartPlug != null)
					{
						result.Add(mSmartPlug.GetDeviceInfo());
					}
					else
					{
						Console.WriteLine($"Get(): HS100 (${cfg.Name} - ${cfg.IpAddr}) is not online");
					}
				}
				catch (ArgumentNullException ex)
				{
					Console.WriteLine($"Get: {ex.Message}");
				}
				catch (FormatException ex)
				{
					Console.WriteLine($"Get: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Get: {ex.Message}");
				}
			}
			return result;
		}

		[HttpGet("{id}", Name = "GetHS100")]
		//[Route("api/HS100/{id}")]
		public ActionResult<DeviceInfo> GetById(int id)
		{
			Console.WriteLine($"GetById() Entered. deviceId: {id}");
			DeviceInfo devInfo = new DeviceInfo(id);

			//Set up the light switches starting at ip 192.168.x.65
			//API user will have to know Id;


			try
			{
 
				bool isOnline;
				HS1XX mSmartPlug = getDevice(id, out isOnline);
				if (isOnline)
				{
					devInfo = mSmartPlug.GetDeviceInfo();
				}
				else
				{
					Console.WriteLine($"GetById(): HS100 id: ${id} is not online");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return NotFound();
			}


			return devInfo;
		}

		[HttpGet("[action]/{deviceId}")]
		public ActionResult<DeviceInfo> Toggle(int deviceId)
		{
			Console.WriteLine($"Toggle() Entered. deviceId: {deviceId}");

			bool isOnline;
			HS1XX mSmartPlug = getDevice(deviceId, out isOnline);

			if (mSmartPlug == null)
				return NotFound();

			return toggleHS100(mSmartPlug);
		}

		[HttpGet("[action]/{deviceId}")]
		public ActionResult<DeviceInfo> TurnOn(int deviceId)
		{
			Console.WriteLine($"TurnOn() Entered. deviceId: {deviceId}");
			bool isOnline;
			HS1XX mSmartPlug = getDevice(deviceId, out isOnline);

			if (isOnline)
			{
				return setRelayState(mSmartPlug, 1);
			}

			Console.WriteLine($"TurnOn(): HS100 id: ${deviceId} is not online");
			return NotFound();
		}

		[HttpGet("[action]/{deviceId}")]
		public ActionResult<DeviceInfo> TurnOff(int deviceId)
		{
			Console.WriteLine($"TurnOff() Entered. deviceId: {deviceId}");
			bool isOnline;
			HS1XX mSmartPlug = getDevice(deviceId, out isOnline);

			if (isOnline)
				return setRelayState(mSmartPlug, 0);

			Console.WriteLine($"TurnOff(): HS100 id: ${deviceId} is not online");
			return NotFound();
		}

		[HttpGet("[action]/{timeout}")]
		public ActionResult<string> Discover(int timeout)
		{
			Console.WriteLine($"Discover() Entered. Timeout {timeout}");
			var allDevices = HS1XX.GetAllDevices_TCP_Scanner(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet, timeout); //timeOut
			if (allDevices == null || allDevices.Count < 1)
			{
				return "No HS100's found during scan...";
			}

			Console.WriteLine($"Scan returned {allDevices.Count} HS100's");

			return Common.getXML<List<DeviceInfo>>(allDevices.Values.ToList());
		}

		[HttpGet("[action]/{timeout}")]
		public ActionResult<string> BuildConfig(int timeout)
		{
			Console.WriteLine($"BuildConfig() Entered. Timeout {timeout}");

			var allDevices = HS1XX.GetAllDevices_TCP_Scanner(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet, timeout); //timeOut
			if (allDevices == null || allDevices.Count < 1)
			{
				return "No HS100's found during scan...";
			}

			Console.WriteLine($"Scan returned {allDevices.Count} HS100's");

			List<HS100Config> configs = new List<HS100Config>();
			//allDevices.Values.ToList().ForEach(d => configs.Add(new HS100Config() { a}))
			int deviceId = 0;
			foreach (IPAddress ip in allDevices.Keys)
			{
				var devinfo = allDevices[ip];
				var addr = ip.ToString();
				Console.WriteLine($"Adding {addr}");
				configs.Add(new HS100Config(++deviceId, devinfo.Alias, addr, devinfo.MAC));
			}

			return Common.getXML<List<HS100Config>>(configs);
		}

		[HttpGet("[action]")]
		public ActionResult<string> ShowConfig()
		{
			Console.WriteLine($"ShowConfig() Entered.");			 
			return Common.getXML<List<HS100Config>>(hs100Configs);
		}

		#endregion

		#region Privates
		private ActionResult<DeviceInfo> setRelayState(HS1XX mSmartPlug, int relayState)
		{
			DeviceInfo devInfo = null;

			try
			{
				RelayAction action = (relayState == 0) ? RelayAction.TurnOff : RelayAction.TurnOn;
				mSmartPlug.SwitchRelayStateFast(action);
				devInfo = mSmartPlug.GetDeviceInfo();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in setRelayState(): {ex.Message}");
				return NotFound();
			}

			return devInfo;
		}

		private DeviceInfo toggleHS100(HS1XX mSmartPlug)
		{
			DeviceInfo devInfo = null;


			try
			{
				devInfo = mSmartPlug.GetDeviceInfo();
				byte newState = (devInfo.RelayState == 1) ? (byte)0 : (byte)1;
				RelayAction action = (devInfo.RelayState == 1) ? RelayAction.TurnOff : RelayAction.TurnOn;
				devInfo.RelayState = newState;
				mSmartPlug.SwitchRelayStateFast(action);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in toggleHS100(): {ex.Message}");
			}



			return devInfo;
		}

		private HS1XX getDevice(int deviceId, out bool isOnLine)
		{
			isOnLine = false;
			var cfg = hs100Configs.Where(c => c.DeviceId == deviceId).FirstOrDefault();
			if (cfg == null && String.IsNullOrEmpty(cfg.IpAddr))
			{
				Console.WriteLine($"getDevice() failed to find device: {deviceId}");
				return null;
			}

			HS1XX mSmartPlug = null;
			try
			{
				mSmartPlug = new HS1XX(cfg, out isOnLine, 10000, 0, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in getDevice(): {ex.Message}");
			}
			return mSmartPlug;
		}
		#endregion
	}
}

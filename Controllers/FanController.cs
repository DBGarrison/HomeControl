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

namespace HomeControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FanController : ControllerBase
    {
		//No default API call!!
		FanDevice fanDevice;
		private IMemoryCache _cache;

		public FanController(IMemoryCache memoryCache)
		{
			_cache = memoryCache;
 
			if (!_cache.TryGetValue("FanDevice", out FanDevice cacheEntry))
			{
				// Key not in cache, so get data.
				cacheEntry = Common.LoadFromXML<FanDevice>(Environment.GetEnvironmentVariable("FANDEVICE_PATH"));

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					// Set cache entry size by extension method.
					.SetSize(1)
					// Keep in cache for this time, reset time if accessed.
					.SetSlidingExpiration(TimeSpan.FromHours(24));

				// Set cache entry size via property.
				// cacheEntryOptions.Size = 1;

				// Save data in cache.
				_cache.Set("FanDevice", cacheEntry, cacheEntryOptions);
			}

			this.fanDevice = cacheEntry;
		}

        [HttpGet("[action]")]        
        public async Task<IActionResult> getStatus()
        {            
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri($"http://{fanDevice.IpAddr}/");
                    var response = await client.GetAsync("getStatus");
                    response.EnsureSuccessStatusCode();
                    //{\r\n  \"FanStatus\": {\r\n    \"fanSpeed\": 3,\r\n    \"lightsIsOn\": true,\r\n    \"isForward\": true,\r\n    \"chkInterval\": 60000\r\n  }\r\n}
                    var stringResult = await response.Content.ReadAsStringAsync();
                    int start = stringResult.LastIndexOf('{');
                    int end = stringResult.IndexOf('}');

                    string trimResult = stringResult.Substring(start, end - start + 1);
                    var rawStatus = JsonConvert.DeserializeObject<FanStatus>(trimResult);                     

                    return Ok(new
                    {
                        fanSpeed = rawStatus.fanSpeed,
                        isForward = rawStatus.isForward,
                        lightsIsOn = rawStatus.lightsIsOn,
                        chkInterval = rawStatus.chkInterval                         
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Fan Controller error: {httpRequestException.Message}");
                }
            }             
        }

        [HttpGet("[action]")]         
        public async Task<IActionResult> toggleLights()
        {
            using (var client = new HttpClient())
            {
                try
                {
					client.BaseAddress = new Uri($"http://{fanDevice.IpAddr}/");
					var response = await client.GetAsync("toggleLights");
                    response.EnsureSuccessStatusCode();
                    //{\r\n  \"FanStatus\": {\r\n    \"fanSpeed\": 3,\r\n    \"lightsIsOn\": true,\r\n    \"isForward\": true,\r\n    \"chkInterval\": 60000\r\n  }\r\n}
                    var stringResult = await response.Content.ReadAsStringAsync();
                    int start = stringResult.LastIndexOf('{');
                    int end = stringResult.IndexOf('}');

                    string trimResult = stringResult.Substring(start, end - start + 1);
                    var rawStatus = JsonConvert.DeserializeObject<FanStatus>(trimResult);

                    return Ok(new
                    {
                        fanSpeed = rawStatus.fanSpeed,
                        isForward = rawStatus.isForward,
                        lightsIsOn = rawStatus.lightsIsOn,
                        chkInterval = rawStatus.chkInterval
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Fan Controller error: {httpRequestException.Message}");
                }
            }
        }
       
        [HttpGet("[action]")]
        public async Task<IActionResult> toggleDirection()
        {
            using (var client = new HttpClient())
            {
                try
                {
					client.BaseAddress = new Uri($"http://{fanDevice.IpAddr}/");
					var response = await client.GetAsync("toggleDirection");
                    response.EnsureSuccessStatusCode();
                    //{\r\n  \"FanStatus\": {\r\n    \"fanSpeed\": 3,\r\n    \"lightsIsOn\": true,\r\n    \"isForward\": true,\r\n    \"chkInterval\": 60000\r\n  }\r\n}
                    var stringResult = await response.Content.ReadAsStringAsync();
                    int start = stringResult.LastIndexOf('{');
                    int end = stringResult.IndexOf('}');

                    string trimResult = stringResult.Substring(start, end - start + 1);
                    var rawStatus = JsonConvert.DeserializeObject<FanStatus>(trimResult);

                    return Ok(new
                    {
                        fanSpeed = rawStatus.fanSpeed,
                        isForward = rawStatus.isForward,
                        lightsIsOn = rawStatus.lightsIsOn,
                        chkInterval = rawStatus.chkInterval
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Fan Controller error: {httpRequestException.Message}");
                }
            }
        }

        [HttpGet("[action]/{speed}")]
        public async Task<IActionResult> setSpeed(int speed)
        {
            using (var client = new HttpClient())
            {
                try
                {
					client.BaseAddress = new Uri($"http://{fanDevice.IpAddr}/");
					var response = await client.GetAsync($"setSpeed?speed={speed}");
                    response.EnsureSuccessStatusCode();
                    //{\r\n  \"FanStatus\": {\r\n    \"fanSpeed\": 3,\r\n    \"lightsIsOn\": true,\r\n    \"isForward\": true,\r\n    \"chkInterval\": 60000\r\n  }\r\n}
                    var stringResult = await response.Content.ReadAsStringAsync();
                    int start = stringResult.LastIndexOf('{');
                    int end = stringResult.IndexOf('}');

                    string trimResult = stringResult.Substring(start, end - start + 1);
                    var rawStatus = JsonConvert.DeserializeObject<FanStatus>(trimResult);

                    return Ok(new
                    {
                        fanSpeed = rawStatus.fanSpeed,
                        isForward = rawStatus.isForward,
                        lightsIsOn = rawStatus.lightsIsOn,
                        chkInterval = rawStatus.chkInterval
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Fan Controller error: {httpRequestException.Message}");
                }
            }
        }
                 
    }
     
}

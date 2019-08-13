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

namespace HomeControl.Controllers
{
	[Route("api/AppScenes")]
	[ApiController]
	public class AppScenesControler : ControllerBase
	{
		private IMemoryCache _cache;
		private List<AppScene> appScenes = null;
		private const int timeOut = 30;
		public AppScenesControler(IMemoryCache memoryCache)
		{
			_cache = memoryCache;
			bool loadDefaults = true;

			if (!_cache.TryGetValue("AppScenes", out List<AppScene> cacheEntry))
			{
                string file = Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                                Environment.GetEnvironmentVariable("APPSCENE_PATH");

                // Key not in cache, so get data.
                if (!loadDefaults)
				{
					cacheEntry = Common.LoadFromXML<List<AppScene>>(file);
					cacheEntry.ForEach(e => e.initComputed());
				}
				else
				{
					cacheEntry = AppScene.buildDefaultList();
					Common.SaveToXML(cacheEntry, file);
				}

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					// Set cache entry size by extension method.
					.SetSize(1)
					// Keep in cache for this time, reset time if accessed.
					.SetSlidingExpiration(TimeSpan.FromHours(24));

				// Set cache entry size via property.
				// cacheEntryOptions.Size = 1;

				// Save data in cache.
				_cache.Set("AppScenes", cacheEntry, cacheEntryOptions);
			}

			this.appScenes = cacheEntry;

		}

		#region Publics
		// GET api/hs100
		[HttpGet]
		public ActionResult<IEnumerable<AppScene>> Get()
		{
			appScenes.ForEach(e => e.initComputed());
			return appScenes; 
		}

		//[HttpGet("{id}", Name = "GetById")]
		[HttpGet("[action]/{id}")]
		public ActionResult<AppScene> Get(int id)
		{
			Console.WriteLine($"GetById() Entered. deviceId: {id}");			 		
			try
			{
				var appscene = appScenes.Where(s => s.sceneId == id).First();
				appscene.initComputed();
				return appscene;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetById(): {ex.Message}");
				return NotFound();
			}			
		}
 
		[HttpGet("[action]")]
		public ActionResult<string> ShowConfig()
		{
			Console.WriteLine($"ShowConfig() Entered.");			 
			return Common.getXML<List<AppScene>>(appScenes);
		}

		[HttpGet("[action]")]
		public ActionResult<string> SaveConfig()
		{
			Console.WriteLine($"SaveConfig() Entered.");
            string file = Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                                Environment.GetEnvironmentVariable("APPSCENE_PATH");
            Common.SaveToXML<List<AppScene>>(appScenes, file);
			return "Config saved.";
		}
		 
		#endregion

		#region Privates
		 
		#endregion
	}
}

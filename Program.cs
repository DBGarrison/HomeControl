using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HomeControl
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())				 
				.UseUrls("http://*:5000")
				.UseIISIntegration()
				.UseStartup<Startup>()
				//.ConfigureKestrel((context, options) =>
				//{
				//	// Set properties and call methods on options
				//})
				.ConfigureAppConfiguration((context, configBuilder) =>
				{
					configBuilder
						.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
						.AddEnvironmentVariables();
				})
				.ConfigureLogging(loggerFactory => loggerFactory
					.AddConsole()
					.AddDebug());
	}
}

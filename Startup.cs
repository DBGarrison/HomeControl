using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HomeControl.WebSocketManager;
using HomeControl.RFSwitch;
//using HomeControl.HostedServices;
using HomeControl.Extensions;
using HomeControl.ObjectDetection;
using HomeControl.Relays;
using HomeControl.Classes;
using RpiConfig.Classes;

namespace HomeControl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddMemoryCache();
			services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddWebSocketManager();

            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            //services.AddHostedService<ObjectDetectionService>();

            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IPiCollection, PiCollection>();
            services.AddSingleton<IControlledAreas, ControlledAreas>();
			services.AddSingleton<IRFSwitches, RFSwitches>();
			services.AddSingleton<IPIRDevices, PIRDevices>();
            services.AddSingleton<IRelayDevices, RelayDevices>();

            //services.AddHostedService<SurveillanceService>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

			var webSocketOptions = new WebSocketOptions()
			{
				KeepAliveInterval = TimeSpan.FromSeconds(120),
				ReceiveBufferSize = 4 * 1024
			};

			//webSocketOptions.AllowedOrigins.Add("https://garage");		
			app.UseWebSockets(webSocketOptions);

			app.MapWebSocketManager("/RFSwitch", serviceProvider.GetService<HomeControlSocketHandler>());

			//app.MapWebSocketManager("/RFSwitch", serviceProvider.GetService<HomeControlSocketHandler>());

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

		 
	}
}

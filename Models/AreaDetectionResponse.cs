using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
 
//using HomeControl.HostedServices;

namespace HomeControl.Models
{
	public class AreaDetectionResponse
	{
		public AreaDetectionResponse() {  }


		public int deviceId { get; set; }
		public string areaName { get; set; }
		public bool inProgress { get; set; }
		public int motionPin { get; set; }
		public long startMillis { get; set; }
		public long runToMillis { get; set; }
		public long endMillis { get; set; }
		public int duration { get; set; }
		public List<DetectionSubscriber> Subscribers { get; set; }

		internal TimeSpan timeSpan
		{
			get
			{
				if (this.inProgress)
				{
					return new TimeSpan(0, 0, 0, 0, this.duration);
				}
				else
				{
					return new TimeSpan();
				}
			}
		}


	}

	public class DetectionSubscriber
	{
		public DetectionSubscriber() { }
		public string hostPort { get; set; }
		public int httpCode { get; set; }
		public string httpResponse { get; set; }
	}

}

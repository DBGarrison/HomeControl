using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace HomeControl.Classes
{
	public class RFCode
	{
		public RFCode() { }
		public RFCode(bool _isOn, long _code)
		{
			this.isOn = _isOn;
			this.code = _code;
		}
		public bool isOn { get; set; }
		public long code { get; set; }
	}
}

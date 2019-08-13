using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Models
{
	public class FanStatus
	{
		public int fanSpeed { get; set; }
		public bool lightsIsOn { get; set; }
		public bool isForward { get; set; }
		public int chkInterval { get; set; }
	}
}

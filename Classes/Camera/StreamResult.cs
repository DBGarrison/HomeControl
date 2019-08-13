using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Classes
{
	public class StreamResult
	{
		public bool Success { get; internal set; }
		public string ErrorMessage { get; internal set; }
		public int bytesReadTotal { get; internal set; }
		public int bytesWrittenTotal { get; internal set; }
		public TimeSpan timeSpan { get; internal set; }
		public string filePath { get; internal set; }
	}
}

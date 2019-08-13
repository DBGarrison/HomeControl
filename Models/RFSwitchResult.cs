using HomeControl.Classes;

namespace HomeControl.Models
{ 
	public class RFSwitchResult
	{
		public RFSwitchResult()
		{
			this.rfCodes = new RFCode[] { };
		}

		public Enums.ControlledAreas controledArea { get; set; }
		 
		public string areaName
		{
			get
			{
				return this.controledArea.ToString();
			}
		}
		public int deviceId { get; set; }
		public string deviceName { get; set; }
		public int swState { get; set; }
		public long lastOnCode { get; set; }
		public long lastOffCode { get; set; }
		public RFCode[] rfCodes { get; set; }
		public string apiBase { get; set; }
	}
}
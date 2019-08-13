using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl
{
 
	public class AppScene
	{
		private string _statusMsg = "Not Avaialble";
		public AppScene() { }
		public AppScene(int _id, Enums.SceneType _sceneType, string _imageName, string _name, Enums.SceneCondition _conditions) {
			this.sceneId = _id;
			this.sceneType = _sceneType;
			this.imageName = _imageName;
			this.sceneName = _name;
			this.sceneConditions = _conditions;
			initComputed();
		}

		/// <summary>
		/// The unique Identifer for this Scene
		/// </summary>
		public int sceneId { get; set; }

		/// <summary>
		/// The unique Identifer for the Smart Device. switchId, fanId, cameraId. e.g.
		/// </summary>
		public int deviceId { get; set; }

		/// <summary>
		/// The type of Scene such as  CeilingFan, SmartSwitch, RfSwitch, PiCamera, Lamp
		/// </summary>
		public Enums.SceneType sceneType { get; set; }
		public string sceneTypeDisplay { get; set; }

		public string imageName { get; set; }
		public string sceneName { get; set; }
		public string sceneStatus { get { return _statusMsg; } }

		public Enums.SceneCondition sceneConditions { get; set; }

		private bool _statusMessageSet = false;

		internal void initComputed()
		{
			sceneTypeDisplay = sceneType.ToString();
			setStatusMessage();
			setImageSrc();
		}
		private void setStatusMessage()
		{
			if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)
			{
				_statusMsg = "Not Available";
			}
			else
			{
				//Device is Available
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				//What kind of Device is it?
				switch(this.sceneType)
				{
					case Enums.SceneType.CeilingFan:
						{
							//Lights san be switched On/Off
							//IF FanIsOn, Fan Speed can be set LowSpeed, MediumSpeed or HighSpeed, so it takes 2 flag checks
							string lgt = ((sceneConditions & Enums.SceneCondition.LightIsOn) == Enums.SceneCondition.LightIsOn) ? "Lights: On" : "Lights: Off";

							if ((sceneConditions & Enums.SceneCondition.FanIsOn) == Enums.SceneCondition.FanIsOn)
							{
								//Fan is on, what speed
								if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.HighSpeed)
									sb.Append($"Fan: High {lgt}");
								else if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.MediumSpeed)
									sb.Append($"Fan: Medium {lgt}");
								else if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.LowSpeed)
									sb.Append($"Fan: Low {lgt}");
								else
									sb.Append($"Invalid Conditions for Ceiling Fan: {this.sceneConditions}");

							}
							else
								sb.Append(lgt);
						}
						break;

					case Enums.SceneType.SmartLamp:
						{
							string lgt = ((sceneConditions & Enums.SceneCondition.LightIsOn) == Enums.SceneCondition.LightIsOn) ? "Lights: On" : "Lights: Off";
							sb.Append(lgt);
						}
						break;
					case Enums.SceneType.PiCamera:
						{
							if ((sceneConditions & Enums.SceneCondition.Streaming) == Enums.SceneCondition.Streaming)
							{
								sb.Append("Streaming");
							} else if ((sceneConditions & Enums.SceneCondition.Surveilling) == Enums.SceneCondition.Surveilling)
							{
								sb.Append("Surveilling");
							} else
							{
								sb.Append($"Invalid Conditions for Pi Camera: {this.sceneConditions}");
							}

						}
						break;
					case Enums.SceneType.RfLight:
					case Enums.SceneType.SmartLight:
					case Enums.SceneType.RfSwitch:
					case Enums.SceneType.SmartSwitch:
						{
							string lgt = ((sceneConditions & Enums.SceneCondition.RelayStateOn) == Enums.SceneCondition.RelayStateOn) ? "Relay: On" : "Relay: Off";
							sb.Append(lgt);
						}
						break;
					default:
						sb.Append($"Unhandled scene type:{this.sceneType.ToString()}");
						break;
				}
				_statusMsg = sb.ToString();
			}
			_statusMessageSet = true;
		}
				
		private void setImageSrc()
		{			 
			var src = "";
			switch (this.sceneType)
			{
				case Enums.SceneType.CeilingFan:
					{
						if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)
						{
							src = "topicon000";
						}
						else
						{						
							//Lights san be switched On/Off
							//IF FanIsOn, Fan Speed can be set LowSpeed, MediumSpeed or HighSpeed, so it takes 2 flag checks							 
							var lgt = ((sceneConditions & Enums.SceneCondition.LightIsOn) == Enums.SceneCondition.LightIsOn) ? 1 : 0;
							var dir = ((sceneConditions & Enums.SceneCondition.IsReversed) == Enums.SceneCondition.IsReversed) ? 1 : 0;
							var spd = 0;

							if ((sceneConditions & Enums.SceneCondition.FanIsOn) == Enums.SceneCondition.FanIsOn)
							{
								if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.HighSpeed)
									spd = 3;
								else if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.MediumSpeed)
									spd = 2;
								else if ((sceneConditions & Enums.SceneCondition.HighSpeed) == Enums.SceneCondition.LowSpeed)
									spd = 1;
								else
									Console.WriteLine($"Invalid Conditions for Ceiling Fan: {this.sceneConditions}");
							}

							src = $"topicon{spd}{dir}{lgt}";
						}

						 
					}
					break;

				case Enums.SceneType.SmartSwitch:
					if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)					
						src = "hs100-0";					
					else					
						src = ((sceneConditions & Enums.SceneCondition.RelayStateOn) == Enums.SceneCondition.RelayStateOn) ? "hs100-1" : "hs100-0";					
					break;

				case Enums.SceneType.RfSwitch:
					 
					if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)
						src = "wallswitch0";
					else
						src = ((sceneConditions & Enums.SceneCondition.RelayStateOn) == Enums.SceneCondition.RelayStateOn) ? "wallswitch1" : "wallswitch0";
					break;


				case Enums.SceneType.PiCamera:
					src = "camera0";
					if ((sceneConditions & Enums.SceneCondition.Available) == Enums.SceneCondition.Available)
					{
						if ((sceneConditions & Enums.SceneCondition.Streaming) == Enums.SceneCondition.Streaming)
							src = "camera1";
						else if ((sceneConditions & Enums.SceneCondition.Surveilling) == Enums.SceneCondition.Surveilling)
							src = "camera1";
					}
					break;

				case Enums.SceneType.SmartLamp:
				case Enums.SceneType.RfLamp:
					src = "lamp0";
					if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)
						src = "lamp0";
					else
						src = ((sceneConditions & Enums.SceneCondition.RelayStateOn) == Enums.SceneCondition.RelayStateOn) ? "lamp1" : "lamp0";
					break;

				case Enums.SceneType.SmartLight:
				case Enums.SceneType.RfLight:
					src = "overheadlight0";
					if ((sceneConditions & Enums.SceneCondition.Available) != Enums.SceneCondition.Available)
						src = "overheadlight0";
					else
						src = ((sceneConditions & Enums.SceneCondition.RelayStateOn) == Enums.SceneCondition.RelayStateOn) ? "overheadlight1" : "overheadlight0";
					break;
			}

			this.imageName = src;
		}

		public override string ToString()
		{
			if (!_statusMessageSet) setStatusMessage();
			return this.sceneStatus;
		}

		///// <summary>
		///// Returns the Image name for a Scene based off Conditions. Each condition represents a State of a Smart Device.
		///// By naming the Images in such a way allows easy path geration for Icons to represent the current starte of a Smart Device.
		///// Example: topicon311.png is Icon for Ceiling Fan set to Speed 3, Reversed Direction, Lights On
		///// </summary>
		///// <param name="conditions">Integer Array of Conditions for a Scene, such as IsOn, IsForward, Speed, e.g.</param>
		///// <returns></returns>
		//public string getImageSrcPath(int[] conditions = null)
		//{
		//	if(conditions != null)
		//		return $"{this.imageName}{String.Join("", conditions)}.png";
		//	else
		//		return $"{this.imageName}.png";
		//}

		public static List<AppScene> buildDefaultList()
		{
			List<AppScene> list = new List<AppScene>();
			list.Add(new AppScene(1, Enums.SceneType.CeilingFan, "topicon", "Living Room Fan", Enums.SceneCondition.Available | Enums.SceneCondition.FanIsOn | Enums.SceneCondition.HighSpeed | Enums.SceneCondition.IsReversed));
			list.Add(new AppScene(2, Enums.SceneType.CeilingFan, "topicon", "Rec Room Fan", Enums.SceneCondition.Available | Enums.SceneCondition.FanIsOn | Enums.SceneCondition.HighSpeed | Enums.SceneCondition.IsReversed | Enums.SceneCondition.LightIsOn));
			list.Add(new AppScene(3, Enums.SceneType.RfLight, "overheadlight", "Living Room Overhead Lamp", Enums.SceneCondition.Available | Enums.SceneCondition.RelayStateOn));
			list.Add(new AppScene(4, Enums.SceneType.RfLight, "lamp", "Living Room Lamp", Enums.SceneCondition.Available));
			list.Add(new AppScene(5, Enums.SceneType.PiCamera, "camera", "RpiZero1", Enums.SceneCondition.Available | Enums.SceneCondition.Streaming));
			list.Add(new AppScene(6, Enums.SceneType.PiCamera, "surveillance", "RpiZero2", Enums.SceneCondition.Available | Enums.SceneCondition.Surveilling));
			list.Add(new AppScene(7, Enums.SceneType.PiCamera, "camera", "RpiZero3", Enums.SceneCondition.Available | Enums.SceneCondition.Streaming));
 
			return list;
		}
	}
}
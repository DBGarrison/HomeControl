using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.Classes;

namespace HomeControl
{
	public enum AreaEventStatus
	{
		None = 0,
		/// <summary>
		/// Event is currently in progress: ie: Video Stream currently being wrote to disk
		/// Not All Events run in the backgreoud service. Currently only Motion Events have a duration
		/// </summary>
		InProgress,
		/// <summary>
		/// Event complete: ie: Fan speed changed, Switch toggles, Motion detected
		/// </summary>
		Complete
	}

	public enum AreaEventType {
		None = 0,
		/// <summary>
		/// Usage: Object Detection
		/// </summary>
		Motion,
		/// <summary>
		/// Usage: RF Switches, HS100 Smart Switches, Ceiling Fan Light, Overhead Lights, Lamps
		/// </summary>
		SwitchOn,
		/// <summary>
		/// Usage: RF Switches, HS100 Smart Switches, Ceiling Fan Light, Overhead Lights, Lamps
		/// </summary>
		SwitchOff,
		/// <summary>
		/// Usage: Ceiling Fan Speed
		/// </summary>
		Speed,
		/// <summary>
		/// Usage Ceiling Fan Direction
		/// </summary>
		Direction
	}

	public class AreaEvent
	{
		public long _startTick;
		public long _endTick;
	 

		public AreaEvent()
		{
			this.area =  Enums.ControlledAreas.None;			
			this.deviceName = string.Empty;
			this._startTick = 0;
			this._endTick = 0;			 
			this.eventType = AreaEventType.None;
			this.eventStatus = AreaEventStatus.None;
			this.mcuIPAddress = string.Empty;
			this.cameraHost = string.Empty;
			this.dataFile = string.Empty;
		}	 

		public AreaEvent(Enums.ControlledAreas areaId, string _deviceName, AreaEventType _eventType, string _mcuIPAddress, AreaEventStatus _eventStatus = AreaEventStatus.InProgress)
		{
			this.area = areaId;
			this.deviceName = _deviceName;
			this._startTick = DateTime.Now.Ticks;

			this.eventType = _eventType;
			this.mcuIPAddress = _mcuIPAddress;					
			this.eventStatus = _eventStatus;

			this.dataFile = string.Empty;

			if (_eventStatus == AreaEventStatus.Complete)
			{				 
				_endTick = _startTick;
			}
			else if (_eventStatus == AreaEventStatus.InProgress)
			{				
				_endTick = 0;
			}
		}

		public AreaEvent(string logFileLine)
		{
			//$"{this.eventType}:{area}:{deviceName}:{eventStatus}:{mcuIPAddress}:{_startTick}:{_endTick}:{dataFile}";
			string[] parts = logFileLine.Split(":");
			if (parts.Length < 8)
			{
				Console.WriteLine("BAD Logfikle data for AreaEvent: %s", logFileLine);

			}
			else
			{				 
				AreaEventType evt;
				if (Enum.TryParse<AreaEventType>(parts[0], out evt))
				{
					Enums.ControlledAreas areaId;
					if (Enum.TryParse<Enums.ControlledAreas>(parts[1], out areaId))
					{
						AreaEventStatus aes;
						if (Enum.TryParse<AreaEventStatus>(parts[3], out aes))
						{							 
							this.area = areaId;
							this.deviceName = parts[2];
						 
							this.eventType = evt;
							this.mcuIPAddress = parts[4];
							this.eventStatus = aes;

							_startTick = Convert.ToInt64(parts[5]);
							 _endTick = Convert.ToInt64(parts[6]);

							this.dataFile = parts[7];							 
						}
					}
				}

			}
		}

		public Enums.ControlledAreas area { get; set; }		 

		public string areaName { get { return this.area.ToString(); } }

		public string deviceName { get; set; }

		public string dataFile { get; set; }

		public AreaEventType eventType { get; set; }
		public AreaEventStatus eventStatus { get; set; }

		public string mcuIPAddress { get; set; }

		public string cameraHost { get; set; }

		public DateTime? getDateTime()
		{
			if (this._startTick > 0) return new DateTime(this._startTick);
			return null;			
		}

		public TimeSpan? getDuration()
		{
			if (this._endTick > this._startTick)
			{
				return new TimeSpan(this._startTick - this._endTick);
			}

			return new TimeSpan();
		}

		internal void complete(string _dataFile = "")
		{
			this._endTick = DateTime.Now.Ticks;
			this.eventStatus = AreaEventStatus.Complete;
			if (!string.IsNullOrEmpty(_dataFile))
			{
				dataFile = _dataFile;
			}
		}

		public string toLogfile()
		{
			//eventType:area:deviceName:eventStatus:mcuIPAddress:startTick:endTick:cameraHost:dataFile
			return $"{this.eventType}:{area}:{deviceName}:{eventStatus}:{mcuIPAddress}:{_startTick}:{_endTick}:{cameraHost}:{dataFile}";
		} 

		public override string ToString()
		{
			if (this.area > Enums.ControlledAreas.None)
			{
				//eventType:area:deviceName:eventStatus:initiator_IP:DateTime:Duration
				return $"{this.eventType}:{area}:{deviceName}:{eventStatus}:{mcuIPAddress}:{getDateTime()?.ToString()}:{this.getDuration().ToString()}";
			}

			return base.ToString();
		}

		 
	}


}

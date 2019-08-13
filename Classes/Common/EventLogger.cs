using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace HomeControl
{

	public static class EventLogger
	{
		private static AreaEvent lockObject = new AreaEvent();
		//private static FileStream _fsLog;

		private static string _xmlPath
        {
            get
            {
                return Environment.GetEnvironmentVariable("CONFIG_FOLDER") + "/" +
                       Environment.GetEnvironmentVariable("EVENT_LOG");
            }
        }
		static EventLogger()
		{
			//_fsLog = new FileStream(_xmlPath, FileMode.Append, FileAccess.Write);
		}

		internal static void logEvent(AreaEvent evt)
		{
			lock (lockObject)
			{
				//byte[] bytes = ASCIIEncoding.ASCII.GetBytes("\n" + evt.ToString());
				//_fsLog.Write(bytes, 0, bytes.Length);
				using (StreamWriter sw = File.AppendText(_xmlPath))
				{
					sw.WriteLine(evt.toLogfile());
					sw.Flush();
				}
			}
		}

		//public static List<AreaEvent> LoadLogFile()
		//{
		//	List<AreaEvent> retVal = new List<AreaEvent>();
		//	if (File.Exists(_xmlPath))
		//	{
		//		lock (lockObject)
		//		{
		//			string[] lines = File.ReadAllLines(_xmlPath);
		//			foreach (string line in lines)
		//			{
		//				retVal.Add(new AreaEvent(line));
		//			}
		//		}
		//	}
		//	return retVal;
		//	//Motion:FrontGate:Gate Entrance:Complete:RpiZero9:7/22/2019 10:58:32 AM:-00:00:08.9728597
		//	//
		//}

		internal static List<AreaEvent> getMcuEvents(string mcuName)
		{
			//eventType:area:deviceName:eventStatus:mcuIPAddress:startTick:endTick:cameraHost:dataFile
			List<AreaEvent> retVal = new List<AreaEvent>();
			if (File.Exists(_xmlPath))
			{
				lock (lockObject)
				{
					using (StreamReader sr = new StreamReader(_xmlPath))
					{						 
						while (!sr.EndOfStream)
						{
							string line = sr.ReadLine();
							try
							{

								//Area parameter was passed so only search in second field for Area
								int p1 = line.IndexOf(':');
								int p2 = line.IndexOf(':', p1 + 1);						  
								int p3 = line.IndexOf(':', p2 + 1);
								int p4 = line.IndexOf(':', p3 + 1);
								int p5 = line.IndexOf(':', p4 + 1);

								string _mcuName = line.Substring(p4 + 1, p5 - p4 - 1);
								if (_mcuName == mcuName)
								{
									retVal.Add(new AreaEvent(line));
								}

							}
							finally { }
						}
					}
					//retVal.Add(new AreaEvent(line));

				}
			}
			return retVal;
		}

		internal static List<AreaEvent> getCameraEvents(string camHost)
		{
			//eventType:area:deviceName:eventStatus:mcuIPAddress:startTick:endTick:cameraHost:dataFile
			List<AreaEvent> retVal = new List<AreaEvent>();
			if (File.Exists(_xmlPath))
			{
				lock (lockObject)
				{
					using (StreamReader sr = new StreamReader(_xmlPath))
					{
						while (!sr.EndOfStream)
						{
							string line = sr.ReadLine();
							try
							{
								if (line.StartsWith("Motion:"))
								{
									//Area parameter was passed so only search in second field for Area
									int p1 = line.IndexOf(':');
									int p2 = line.IndexOf(':', p1 + 1);
									int p3 = line.IndexOf(':', p2 + 1);
									int p4 = line.IndexOf(':', p3 + 1);
									int p5 = line.IndexOf(':', p4 + 1);
									int p6 = line.IndexOf(':', p5 + 1);
									int p7 = line.IndexOf(':', p6 + 1);
									int p8 = line.IndexOf(':', p7 + 1);

									string _cameraName = line.Substring(p7 + 1, p8 - p7 - 1);
									if (_cameraName.Equals(camHost, StringComparison.OrdinalIgnoreCase))
									{
										retVal.Add(new AreaEvent(line));
									}
								}
							}
							finally { }
						}
					}
					//retVal.Add(new AreaEvent(line));

				}
			}
			return retVal;
		}


		/// <summary>
		/// Get Events for a specific Area
		/// </summary>
		/// <param name="area">The Area for which to return events</param>
		/// <param name="eventType">The type of even such as Motion, SwitchOn, SwitchOff, e.g. All Event types are returned for this area when no parameter is passed.</param>
		/// <returns>Events for a specific Area</returns>
		internal static List<AreaEvent> getAreaEvents(AreaEventType eventType = AreaEventType.None, Enums.ControlledAreas area = Enums.ControlledAreas.None)

		{
			List<AreaEvent> retVal = new List<AreaEvent>();
			if (File.Exists(_xmlPath))
			{
				lock (lockObject)
				{
					using (StreamReader sr = new StreamReader(_xmlPath))
					{
						string _typeSearch = eventType.ToString();
						string _searchArea = area.ToString();
						while (!sr.EndOfStream)
						{
							string line = sr.ReadLine();
							try
							{
								//eventType:area:deviceName:eventStatus:mcuIPAddress:startTick:endTick:cameraHost:dataFile
								//No parameters specified so add this event
								if (area == Enums.ControlledAreas.None && eventType == AreaEventType.None)
								{
									retVal.Add(new AreaEvent(line));
								}
								else //if (eventType == AreaEventType.None) {
								{
									//Area parameter was passed so only search in second field for Area
									int p1 = line.IndexOf(':');
									string _eventType = line.Substring(0, p1);

									if (eventType == AreaEventType.None || _typeSearch == _eventType)
									{
										if (area == Enums.ControlledAreas.None)
										{
											retVal.Add(new AreaEvent(line));
										}
										else
										{
											int p2 = line.IndexOf(':', p1 + 1);

											string _area = line.Substring(p1 + 1, p2 - p1 - 1);
											if (_area == _searchArea)
											{
												retVal.Add(new AreaEvent(line));
											}
										}
									}
								}
							}
							finally { }
						}
					}
					//retVal.Add(new AreaEvent(line));

				}
			}
			return retVal;
		} 

		//public static void CloseLogfile()
		//{
		//	if (_fsLog != null)
		//	{
		//		Console.WriteLine("Closing logfile...");
		//		try
		//		{
		//			_fsLog.Dispose();
		//			_fsLog = null;
		//		}
		//		finally { }
		//	}
		//}
	}
}

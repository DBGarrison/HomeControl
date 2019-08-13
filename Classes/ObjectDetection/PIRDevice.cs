using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using HomeControl.Classes;
//using HomeControl.HostedServices;
using Microsoft.Extensions.Logging;
using StremResult = HomeControl.HS100.DeviceActionResult;

namespace HomeControl.ObjectDetection

{
	 
	public class PIRDevice : HomeControlDevice
	{	 
        //private ILogger _logger;
		private PiCamera _piCamera = null;
		private Enums.EnumPirStatus _pirStatus = Enums.EnumPirStatus.Idle;
		private PiCamera piCamera
		{
			get
			{
				if (_piCamera == null)
				{
					var controllerArea = base.controlledAreas.Where(ca => ca.areaId == this.area).First();

					var motionHandler = controllerArea.motionHandlers.Where(h => h.pirName.Equals(deviceName, StringComparison.OrdinalIgnoreCase)).First();
					_piCamera = motionHandler.piCamera;
				}
				return _piCamera;
			}
		}

		public PIRDevice() : base(Enums.ControlledAreas.None, "", "", Enums.EnumDeviceType.Motion)
		{
			this.Events = new List<AreaEvent>();
		}

		public PIRDevice(Enums.ControlledAreas area, string name, string mcuIpAddress) : base(area, name, mcuIpAddress, Enums.EnumDeviceType.Motion)
		{
			//this.inProgress = false;
			//this.isStreaming = false;
			this.pirStatus = Enums.EnumPirStatus.Idle;
			this.startTick = 0;
			this.endTick = 0;
			//this.mcu = new MCUDevice(mcuIpAddress, 80, this.deviceType);
			this.Events = new List<AreaEvent>();

			 
		}
		 
		 
		public Enums.EnumPirStatus pirStatus
		{
			get
			{
				return _pirStatus;
			}
			set
			{
				//var stk = new StackTrace();
				//var callee = stk.GetFrame(1).GetMethod().Name;
				//if (mcu != null)
				//{
				//	Console.WriteLine($"SET pirStatus entered. (device: {mcu.ToString()} ) Called from {callee}. value = {value}");
				//}

				if (_pirStatus != value)
				{
					//Console.WriteLine($"pirStatus value changed.");
					_pirStatus = value;
				}
				else
				{
					//Console.WriteLine($"pirStatus value NOT changed");
				}
			}
		}

		//public bool inProgress { get; set; }
		//public bool isStreaming { get; set; }
		//public bool isQueued { get; set; }

		internal long startTick { get; set; }
        internal long endTick { get; set; }

		//public MCUDevice mcu { get; set; }
		 
		//public List<AreaEvent> Events { get; set; }

        internal TimeSpan duration
        {
            get
            {
                if (pirStatus> Enums.EnumPirStatus.Idle)
                {
                    return new TimeSpan(DateTime.Now.Ticks - this.startTick);
                }
                else
                {
                    return new TimeSpan();
                }
            }
        }

        internal void createLogger(ILoggerFactory _loggerFactory)
        {
            _logger = _loggerFactory.CreateLogger<PIRDevice>();
        }

		/*
		internal async Task startDetecting(IBackgroundTaskQueue _queue, string clientIP)
		{
			try
			{
				bool yld = false;
				lock (piCamera)
				{
					_logger.LogInformation($"startDetecting( {ToString()} ) Entered.");
					piCamera.requestCount++;
					if (pirStatus == Enums.EnumPirStatus.Idle)
					{
						pirStatus = Enums.EnumPirStatus.Queued;
						//this.inProgress = true;
						//this.isStreaming = false;
						//this.isQueued = true;
						this.startTick = 0;
						this.endTick = 0;


						_queue.QueueBackgroundWorkItem(async token =>
						{
							await this.startRecording(clientIP);
						});

						yld = true;
						//await Task.Yield();
						//_logger.LogInformation("startDetecting() Done.");
					}
					else
					{
						_logger.LogInformation($"startDetecting( {ToString()} ) Already in Progress!!");
					}
				}

				if (yld) await Task.Yield();
			}
			catch (Exception ex)
			{
				_logger.LogError($"startDetecting(): Unexpected error: {ex.Message}");
			}
		}
		*/
		internal async Task startDetecting()
        {
            try
            {
			 
				lock (piCamera)
				{					
					_logger.LogInformation($"startDetecting( {ToString()} ) Entered.");
					piCamera.requestCount++;
					if (pirStatus == Enums.EnumPirStatus.Idle)
					{
						pirStatus = Enums.EnumPirStatus.Queued;
						//this.inProgress = true;
						//this.isStreaming = false;
						//this.isQueued = true;
						this.startTick = 0;
						this.endTick = 0;

						BackgroundWorker wt = new BackgroundWorker();
						wt.DoWork += Wt_DoWork;
						wt.RunWorkerCompleted += Wt_RunWorkerCompleted;
						wt.RunWorkerAsync();
						  
					}
					else
					{
						_logger.LogInformation($"startDetecting( {ToString()} ) Already in Progress!!");
					}					 
				} 
			}
            catch (Exception ex)
            {
                _logger.LogError($"startDetecting(): Unexpected error: {ex.Message}");
            }
        }

		private void Wt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			BackgroundWorker wt = sender as BackgroundWorker;
			StreamResult result = e.Result as StreamResult;		
			_logger.LogInformation($"RunWorkerCompleted: {result?.ErrorMessage}");
		}

		private void Wt_DoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = startRecording();
		}


		/// <summary>
		/// Start saving video stream to file
		/// </summary>
	   /// <returns></returns>
		private async Task<StreamResult> startRecording()
        {
			StreamResult retVal = new StreamResult() { bytesReadTotal = 0, bytesWrittenTotal = 0 };
            string folder = Environment.GetEnvironmentVariable("VIDEO_PATH");
            string file = $"{this.areaName}-{this.deviceName}-{piCamera.ipAddress}-{Common.GetDateTimeFileName(DateTime.Now)}.mp4";

            retVal.filePath = Path.Combine(folder, file);                                           
             //$"{Common.GetDateTimeFileName(DateTime.Now)}-{this.areaName}-{this.deviceName}-{piCamera.ipAddress}.mp4");

            Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{				 
				pirStatus = Enums.EnumPirStatus.DeQueued;
				this.startTick = DateTime.Now.Ticks;
				this.endTick = 0;

				if (piCamera == null)
				{
					retVal.ErrorMessage = $"startRecording: No Camera found for {ToString()}!!";
					return retVal;
				}
			 
				_logger.LogInformation($"startRecording Entered for {ToString()} MCU: {piCamera.ipAddress}.");

				var allDrives = DriveInfo.GetDrives().ToList();

				var videoPathRoot = Directory.GetDirectoryRoot(folder);

				var di = allDrives.Where(d => d.RootDirectory.FullName == videoPathRoot).FirstOrDefault();

				const int minSpace = 1000000000;
				var freeSpace = di?.TotalFreeSpace;

				if (freeSpace < minSpace)
				{
					retVal.ErrorMessage = $"Can't start recording when free space ({freeSpace}) less than {minSpace}!!";
					//_logger.LogWarning(retVal.ErrorMessage);
					return retVal;
				}

				//sw.Restart();

				using (var client = new HttpClient())
				{
					try
					{
						client.BaseAddress = new Uri(piCamera.url);

						_logger.LogInformation($"startRecording() remoteDeviceId: {mcu.remoteDeviceId} cam: {piCamera.url} output: {retVal.filePath}");

						using (var fileStream = new FileStream(retVal.filePath, FileMode.Create))
						{
							var evt = new AreaEvent(this.area, this.deviceName, AreaEventType.Motion, mcu.ipAddress);
							evt.dataFile = file;
							evt.cameraHost = piCamera.ipAddress;
							this.Events.Add(evt);
							
							using (Stream videoStream = await client.GetStreamAsync("?action=stream&ignored.mjpg")) {
								int bufSize = 8192, consecutiveNoReads = 0, maxFileSize = 1000000000;
								byte[] buf = new byte[bufSize];

								pirStatus = Enums.EnumPirStatus.Streaming;								 
								while (videoStream.CanRead && pirStatus == Enums.EnumPirStatus.Streaming)
								{
									int bytesRead = await videoStream.ReadAsync(buf, 0, bufSize);
									if (bytesRead > 0)
									{
										retVal.bytesReadTotal += bytesRead;
										fileStream.Write(buf, 0, bytesRead);
										retVal.bytesWrittenTotal += bytesRead;
										consecutiveNoReads = 0;
										if(retVal.bytesWrittenTotal > maxFileSize)
										{
											_logger.LogWarning($"Done Recording for {ToString()}. Maximum video file size exceeded: actual = {retVal.bytesWrittenTotal} max = {maxFileSize}");
											break;
										}
									}
									else if (pirStatus == Enums.EnumPirStatus.Streaming)
									{
										_logger.LogWarning($"Nothing to read while streaming...");
										//Continue if possible
										System.Threading.Thread.Sleep(100);
										consecutiveNoReads++;
										if (consecutiveNoReads++ > 100)
										{
											_logger.LogError($"Done Recording for {ToString()}. No data coming from video stream: {client.BaseAddress}");
											break;
										}
									}

									if (pirStatus == Enums.EnumPirStatus.Stopping)
									{
										_logger.LogInformation($"Done Recording for {ToString()} beacuse status is {pirStatus}");
										break;

									}
								}

								fileStream.Flush();							
							}
							
						}

						retVal.Success = true;
						retVal.ErrorMessage = $"Recording done for {ToString()}. Read {retVal.bytesReadTotal} bytes, Wrote {retVal.bytesWrittenTotal} bytes to {Path.GetFileName(retVal.filePath)}. time: {sw.Elapsed.ToString()}";

					}
					catch (HttpRequestException ex)
					{
						_logger.LogError($"Error in PIRDevice::startRecording(): {ex.Message}");
					}
				}

			}
            catch (Exception ex)
            {
                _logger.LogError($"Error in PIRDevice::startRecording(): {ex.Message}");
            }
			finally
			{
				sw.Stop();
				this.endTick = DateTime.Now.Ticks;
				pirStatus = Enums.EnumPirStatus.Idle;
				retVal.timeSpan = sw.Elapsed;

				//var msg = $"Recording done for {ToString()}. Read {bytesReadTotal} bytes, Wrote {bytesWrittenTotal} bytes to {Path.GetFileName(filePath)}. time: {sw.Elapsed.ToString()}";
				if (retVal.Success)
					_logger.LogInformation(retVal.ErrorMessage);
				else
					_logger.LogError(retVal.ErrorMessage);

			}

            return retVal;
        }


        internal bool stopDetecting()
        {
            try
            {			 
				lock (piCamera)
				{
					_logger.LogInformation($"stopDetecting({ToString()} ) Entered.");
					piCamera.requestCount--;
					if (piCamera.requestCount < 1)
					{
						//this.inProgress = false;
						if (pirStatus > Enums.EnumPirStatus.Idle && pirStatus < Enums.EnumPirStatus.Stopping)
						{
							pirStatus = Enums.EnumPirStatus.Stopping;
						}

						
						var evt = this.Events.Where(w => w.eventStatus == AreaEventStatus.InProgress).LastOrDefault();
						if (evt != null)
						{
							evt.complete();
							EventLogger.logEvent(evt);							 
						}
						piCamera.requestCount = 0;
					}
				}

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error in PIRDevice::stopDetecting(): {ex.Message}");
                return false;
            }
        }


        private enum EnumCfg : Int32 { type, area, name, bits, oncodes, offcodes, state, mcus }
		//public static PIRDevice fromString(string cfgLine)
		//{

		//	//NEW		
		//	//rfswitch:areaId:deviceName:bitLength:oncodes|offcodes:[swState]:[mculist]
		//	//rfswitch:1:Living Room Lamp:24:2|2698844|4323292:2|24|2698836|4323284:1:Garage|RFSwitch


		//	PIRDevice retVal = new PIRDevice();
		//	string[] parts = cfgLine.Split(':');
		//	int numParts = parts.Length;

		//	if (numParts < (int)EnumCfg.offcodes || parts[(int)EnumCfg.type] != "pir")
		//	{
		//		Console.WriteLine($"INVALID CFG LINE: {cfgLine}");
		//		return null;
		//	}
		//	return retVal;
		//}

		public override string ToString()
		{
			try
			{
				//return $"{areaName} : {deviceName} : InProgress: {inProgress} cam: {piCamera?.ipAddress} req: {piCamera?.requestCount}";
				return $"{deviceName} : DeviceId: {mcu?.ToString()} : State: {pirStatus} cam: {piCamera?.ipAddress} req: {piCamera?.requestCount}";
			}
			catch
			{
				return base.ToString();
			}
		}

        //2:Kitchen:1:Rec Room N:1:14:13878:53888:0:0|2:Kitchen:2:Rec Room E:0:12:0:0:0:0|2:Kitchen:3:Rec Room W:1:0:13879:53888:0:0|2:Kitchen:4:Rec Room S:0:15:0:0:0:0
        internal static List<PIRDevice> buildListFromStartUp(string startUp, string mcuIPAddress)
        {
            List<PIRDevice> list = new List<PIRDevice>();
            //string startUp = "2:Kitchen:1:Rec Room N:1:14:13878:53888:0:0|2:Kitchen:2:Rec Room E:0:12:0:0:0:0|2:Kitchen:3:Rec Room W:1:0:13879:53888:0:0|2:Kitchen:4:Rec Room S:0:15:0:0:0:0";
            string[] lines = startUp.Split('|');

            foreach (string line in lines)
            {
                PIRDevice pir = fromStartUp(mcuIPAddress, line);
                list.Add(pir);
            }

            return list;
        }


        /// <summary>
        /// 2:Kitchen:1:Rec Room N:1:14:13878:53888:0:0|2:Kitchen:2:Rec Room E:0:12:0:0:0:0|2:Kitchen:3:Rec Room W:1:0:13879:53888:0:0|2:Kitchen:4:Rec Room S:0:15:0:0:0:0"
        /// </summary>
        /// <param name="cfgLine"></param>
        /// <returns></returns>
        private static PIRDevice fromStartUp(string mcuIPAddress, string line)
        {
            //areaId:areaName:deviceId:deviceName:inProgress:motionPin:startMillis:runToMillis:endMillis:duration
            //2:Kitchen:1:Rec Room N:1:14:13878:53888:0:0

            PIRDevice retVal = new PIRDevice();
            string[] parts = line.Split(':');
            int numParts = parts.Length;

            if (numParts != 4 && numParts != 10)
            {
                Console.WriteLine($"INVALID StartUp LINE: {line}");
                return null;
            }

            try
            {
                int p1 = 0;
                int p2 = numParts == 4 ? 1 : 2;
                int p3 = p2 + 1;
                int areaId = Convert.ToInt32(parts[p1]);
                int remoteDeviceId = Convert.ToInt32(parts[p2]);
                string deviceName = parts[p3];

                retVal = new PIRDevice((Enums.ControlledAreas)areaId, deviceName, mcuIPAddress);
                retVal.mcu.remoteDeviceId = remoteDeviceId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"fromStartUp(): Unexpected error: {ex.Message}");
            }
            return retVal;
        }

        /*
		internal static List<PIRDevice> buildDefaultListForTrack()
		{
			List<PIRDevice> list = new List<PIRDevice>();

			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSE, "RpiZero6"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSW, "RpiZero7"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerNW, "RpiZero10"));

			list.Add(new PIRDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsFront, "RpiZero5"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsEntrance, "RpiZero11"));

			list.Add(new PIRDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateShack, "RpiZero4"));
			list.Add(new PIRDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateEntrance, "RpiZero9"));

			 
	 
			return list;
		}

		private static List<PIRDevice> buildDefaultListForTrackOLD()
		{
			List<PIRDevice> list = new List<PIRDevice>();

			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSE, "JakesDetector1"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerSW, "JakesDetector1"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Tower, ControlledAreas.csAreaTowerNW, "JakesDetector1"));

			list.Add(new PIRDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsEntrance, "JakesDetector2"));
			list.Add(new PIRDevice(Enums.ControlledAreas.Concessions, ControlledAreas.csAreaConcessionsFront, "JakesDetector2"));

			list.Add(new PIRDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateShack, "JakesDetector3"));
			list.Add(new PIRDevice(Enums.ControlledAreas.FrontGate, ControlledAreas.csAreaGateEntrance, "JakesDetector3"));
			 
			return list;
		}

		internal static List<PIRDevice> buildDefaultListForHome()
        {
            List<PIRDevice> list = new List<PIRDevice>();

            list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage SE", "ShopRfRelay"));
            list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage SW", "ShopRfRelay"));

            list.Add(new PIRDevice(Enums.ControlledAreas.BackYard, "Back Yard", "HouseRfRelay"));
            list.Add(new PIRDevice(Enums.ControlledAreas.Kitchen, "Rec. & Kitchen", "HouseRfRelay"));
            list.Add(new PIRDevice(Enums.ControlledAreas.FrontDeck, "Front Deck", "HouseRfRelay"));
            list.Add(new PIRDevice(Enums.ControlledAreas.FrontYard, "Front Yard", "HouseRfRelay"));

            list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage NW", "ShopDetector"));
            list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage North", "ShopDetector"));
            list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage NE", "ShopDetector"));
            return list;
        }

        internal static List<PIRDevice> buildDefaultList(int count)
		{
			List<PIRDevice> list = new List<PIRDevice>();
			if (count > 0)
			{
				list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage SE", "ShopRfRelay"));
				if (count > 1)
				{
					list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage SW", "ShopRfRelay"));
					if (count > 2)
					{
						list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage NW", "ShopDetector"));
						if (count > 3)
						{
							list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage North", "ShopDetector"));
                            if (count > 4)
                            {
                                list.Add(new PIRDevice(Enums.ControlledAreas.GarageArea, "Garage NE", "ShopDetector"));
                            }
                        }
					}
				} 
			}
			return list;
		}

         
        



*/
    }
}

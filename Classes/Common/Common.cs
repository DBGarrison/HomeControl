using System;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using HomeControl.Models;
 
namespace HomeControl
{

    internal sealed class Common
    {
		 
		internal static string configFolder { get { return Environment.GetEnvironmentVariable("CONFIG_FOLDER"); } }

		internal static string GetDateTimeFileName(DateTime dt)
		{
			string m = dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString();
			string d = dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString();
			string h = dt.Hour < 10 ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
			string M = dt.Minute < 10 ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
			string s = dt.Second < 10 ? "0" + dt.Second.ToString() : dt.Second.ToString();

			return $"{dt.Year}{m}{d}{h}{M}{s}";
		}

		internal static T LoadFromXML<T>(string path) where T : class, new()
		{			 
			if (File.Exists(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
				using (var fs = File.OpenRead(path)) {
					return (T) serializer.Deserialize(fs);
				}				 
			}
			else
			{
				Console.WriteLine($"LoadFromXML: Can't find {path}");
			}
			return default(T);
		}

		[Obsolete("Use LoadFromXML instead")]
		private static List<T> LoadListFromXML<T>(string path)
		{
			List<T> retVal = new List<T>();
			if (File.Exists(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(retVal.GetType());
				var fs = File.OpenRead(path);
				retVal = serializer.Deserialize(fs) as List<T>;
				fs.Dispose();
			}
			else
			{
				Console.WriteLine($"Can't find {path}");
			}
			return retVal;
		}

		internal static void SaveToXML<T>(T obj, string path)
		{			 
			//var path = Environment.GetEnvironmentVariable("HS100CONFIG_PATH");
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
			var fs = File.Open(path, FileMode.Create, FileAccess.Write);
			serializer.Serialize(fs, obj);
			fs.Dispose();
		}

		internal static string getXML<T>(T obj)
		{
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());

			using (StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, obj);
				return textWriter.ToString();
			}
		} 
        internal static string StringParams(string value, params string[] parameters)
        {
            string result = value;
            string expression = @"(\{[0-9]+\}){1}";
            MatchCollection foundResults;
            Match foundResult;

            if (parameters != null)
            {
                foundResults = Regex.Matches(value, expression);
                for (int i = foundResults.Count - 1; i >= 0; i--)
                {
                    foundResult = foundResults[i];
                    int indexParam = 0;
                    if (int.TryParse((foundResult.Value.Substring(1, foundResult.Value.Length - 2)), out indexParam) == true)
                    {
                        if (indexParam < parameters.Length)
                        {
                            result = result.Remove(foundResult.Index, foundResult.Length);
                            result = result.Insert(foundResult.Index, parameters[indexParam]);
                        }
                    }
                }
            }

            return result;
        }

        internal static IPAddress GetLocalIPv4(NetworkInterfaceType _type)
        {
            IPAddress output = null;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();

                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                output = ip.Address;
                            }
                        }
                    }
                }
            }

            return output;
        }

		internal static IPAddress GetLocalIPv4Mask(NetworkInterfaceType _type)
		{
			IPAddress output = null;
			foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
				{
					IPInterfaceProperties adapterProperties = item.GetIPProperties();

					if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
					{
						foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
						{
							if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
							{
								output = ip.IPv4Mask;
							}
						}
					}
				}
			}

			return output;
		}

		internal static IPAddress GetGatewayIP(NetworkInterfaceType _type)
        {
            IPAddress output = null;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();

                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        output = adapterProperties.GatewayAddresses[0].Address;
                    }
                }
            }

            return output;
        }

        internal static bool IsNetworkAvailable() 
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

		internal static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
		{
			uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
			uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
			uint broadCastIpAddress = ipAddress | ~ipMaskV4;

			return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
		}

		/// <summary>
		/// Encrypt the command message
		/// </summary>
		/// <param name="pMessage">Message</param>
		/// <param name="pProtocolType">Protocol type</param>
		/// <returns>Returns the encrypted bytes of the message</returns>
		internal static byte[] EncryptMessage(byte[] pMessage, HS100.ProtocolType pProtocolType)
        {
            List<byte> mBuffer = new List<byte>();
            int key = 0xAB;

            if ((pMessage != null) && (pMessage.Length > 0))
            {
                //Añadimos el prefijo del mensaje
				if (pProtocolType == HS100.ProtocolType.TCP)
				{
					mBuffer.Add(0x00);
					mBuffer.Add(0x00);
					mBuffer.Add(0x00);
					mBuffer.Add(0x00);
				}

				//Codificamos el mensaje
				for (int i = 0; i < pMessage.Length; i++)
                {
                    byte b = (byte)(key ^ pMessage[i]);
                    key = b;
                    mBuffer.Add(b);
                }
            }

            return mBuffer.ToArray();
        }

        internal static void showCommandAndBytes(string pCommand, byte[] mEncryptedMessage)
        {
            Debug.WriteLine($"Command: {pCommand}");
            Debug.WriteLine($"Command bytes:{string.Join(',', mEncryptedMessage)}");
        }

        /// <summary>
        /// Decrypt the message
        /// </summary>
        /// <param name="pMessage">Message</param>
        /// <param name="pProtocolType">Protocol type</param>
        /// <returns>Returns the decrypted message</returns>
        internal static byte[] DecryptMessage(byte[] pMessage, HS100.ProtocolType pProtocolType)
        {
            List<byte> mBuffer = new List<byte>();
            int key = 0xAB;

			//Skip the first 4 bytes in TCP communications (4 bytes header)
			byte header = (pProtocolType == HS100.ProtocolType.UDP) ? (byte)0x00 : (byte)0x04;

            if ((pMessage != null) && (pMessage.Length > 0))
            {
                for (int i = header; i < pMessage.Length; i++)
                {
                    byte b = (byte)(key ^ pMessage[i]);
                    key = pMessage[i];
                    mBuffer.Add(b);
                }
            }

            return mBuffer.ToArray();
        }
    }	 
}

/*
		[Obsolete("Use LoadFromXML instead")]
		private static FanDevice LoadFanDevice()
		{
			FanDevice retVal = new FanDevice();
			//var path = "./FanDevice.xml";
			var path = Environment.GetEnvironmentVariable("FANDEVICE_PATH"); 

			if (File.Exists(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(FanDevice));
				var fs = File.OpenRead(path);
				retVal = serializer.Deserialize(fs) as FanDevice;
				fs.Dispose();
			}
			else
			{
				Console.WriteLine($"Can't find {path}");
			}

			return retVal;
		}

		internal static void SaveFanDevice(FanDevice fanDevice)
		{			
			var path = Environment.GetEnvironmentVariable("FANDEVICE_PATH");
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(FanDevice));
			var fs = File.Open(path, FileMode.Create, FileAccess.Write);
			serializer.Serialize(fs, fanDevice);
			fs.Dispose();
		}
	
internal static List<HS100Config> LoadHS100DevicesFromXML()
        {
            List<HS100Config> retVal = new List<HS100Config>();
			//var path = ".\\HS100DeviceList.xml";
			var path = Environment.GetEnvironmentVariable("HS100CONFIG_PATH"); 

			if (File.Exists(path))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<HS100Config>));
                var fs = File.OpenRead(path);
                retVal = serializer.Deserialize(fs) as List<HS100Config>;
                fs.Dispose();
            }
			else
			{
				Console.WriteLine($"Can't find {path}");
			}
			return retVal;
        }

        internal static void SaveHS100DevicesToXML(List<HS100Config> devices)
        {
			//var path = ".\\HS100DeviceList.xml";
			var path = Environment.GetEnvironmentVariable("HS100CONFIG_PATH");
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<HS100Config>));
            var fs = File.Open(path, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, devices);
            fs.Dispose();
        }

        internal static string getDeviceInfoXML(List<DeviceInfo> devices)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(devices.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, devices);
                return textWriter.ToString();
            }
        }

        internal static string getHS100ConfigXML(List<HS100Config> configs)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(configs.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, configs);
                return textWriter.ToString();
            }
        
	}
		
*/

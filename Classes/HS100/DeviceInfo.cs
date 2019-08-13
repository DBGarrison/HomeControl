using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HomeControl.HS100
{
    public sealed class DeviceInfo
    {
		public DeviceInfo() { }
		public DeviceInfo(int Id)
		{
			this.DeviceId = Id;
		}
		#region "Propiedades"

		public int DeviceId { get; set; }

		public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public string SoftwareVersion { get; set; }
            public string HardwareVersion { get; set; }
            public string DeviceType { get; set; }
            public string Model { get; set; }
            public string MAC { get; set; }
            public string ID { get; set; }
            public string HardwareID { get; set; }
            public string FirmwareID { get; set; }
            public string OEMID { get; set; }
            public string Alias { get; set; }
            public string DeviceName { get; set; }
            public string IconHash { get; set; }
            public byte RelayState { get; set; }
            public long OnTime { get; set; }
            public string ActiveMode { get; set; }
            public string Feature { get; set; }
            public long Updating { get; set; }
            public long RSSI { get; set; }
            public byte LedOffState { get; set; }
            public long Latitude { get; set; }
            public long Longitude { get; set; }
        #endregion

        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Alias))
            {
                return base.ToString();
            }
            else
            {
                return $"{this.Alias}, {this.Model}, {this.DeviceName}, {this.DeviceType}, {this.ErrorMessage}";
            }
        }

        #region "Metodos internos"
        internal void LoadFromJson(JToken pJson) 
            {
                this.ErrorCode = pJson["err_code"].Value<int>();
                this.ErrorMessage = (this.ErrorCode != 0) ? pJson["err_msg"].Value<string>() : "";
                this.SoftwareVersion = (this.ErrorCode == 0) ? pJson["sw_ver"].Value<string>() : "";
                this.HardwareVersion = (this.ErrorCode == 0) ? pJson["hw_ver"].Value<string>() : "";
                this.DeviceType = (this.ErrorCode == 0) ? pJson["type"].Value<string>() : "";
                this.Model = (this.ErrorCode == 0) ? pJson["model"].Value<string>() : "";
                this.MAC = (this.ErrorCode == 0) ? pJson["mac"].Value<string>() : "";
                this.ID = (this.ErrorCode == 0) ? pJson["deviceId"].Value<string>() : "";
                this.HardwareID = (this.ErrorCode == 0) ? pJson["hwId"].Value<string>() : "";
                this.FirmwareID = (this.ErrorCode == 0) ? pJson["fwId"].Value<string>() : "";
                this.OEMID = (this.ErrorCode == 0) ? pJson["oemId"].Value<string>() : "";
                this.Alias = (this.ErrorCode == 0) ? pJson["alias"].Value<string>() : "";
                this.DeviceName = (this.ErrorCode == 0) ? pJson["dev_name"].Value<string>() : "";
                this.IconHash = (this.ErrorCode == 0) ? pJson["icon_hash"].Value<string>() : "";
                this.RelayState = (this.ErrorCode == 0) ? pJson["relay_state"].Value<byte>() : (byte)0;
                this.OnTime = (this.ErrorCode == 0) ? pJson["on_time"].Value<long>() : 0;
                this.ActiveMode = (this.ErrorCode == 0) ? pJson["active_mode"].Value<string>() : "";
                this.Feature = (this.ErrorCode == 0) ? pJson["feature"].Value<string>() : "";
                this.Updating = (this.ErrorCode == 0) ? pJson["updating"].Value<long>() : 0;
                this.RSSI = (this.ErrorCode == 0) ? pJson["rssi"].Value<long>() : 0;
                this.LedOffState = (this.ErrorCode == 0) ? pJson["led_off"].Value<byte>() : (byte)0;
                this.Latitude = (this.ErrorCode == 0) ? pJson["latitude"].Value<long>() : 0;
                this.Longitude = (this.ErrorCode == 0) ? pJson["longitude"].Value<long>() : 0;
            }
        #endregion
    }

    public sealed class HS100Config
    {
		public HS100Config() { }
		public HS100Config(int deviceId, string name, string ipaddr, string mac)
		{
			this.DeviceId = deviceId;
			this.Name = name;
			this.IpAddr = ipaddr;
			this.MAC = mac;
		}

		public int DeviceId { get; set; }
        public string Name { get; set; }
        public string MAC { get; set; }
        public string IpAddr { get; set; }

		internal bool IsOnlne { get; set; }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                return base.ToString();
            }
            else
            {
                return $"DeviceId: {this.DeviceId}, Online: {this.IsOnlne}, Name: {this.Name}, IP: {this.IpAddr}, MAC: {this.MAC}";
            }
        }
    }
}

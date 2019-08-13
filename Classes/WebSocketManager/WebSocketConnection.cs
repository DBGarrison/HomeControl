using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HomeControl.Classes;

namespace HomeControl.WebSocketManager
{
    public abstract class WebSocketConnection
    {
		private readonly ILogger _logger;

		public HomeControlSocketHandler webSocketHandler { get; }
		public string NickName { get; set; }
        public Enums.EnumDeviceType DeviceType { get; set; }

        public WebSocket WebSocket { get; set; }

		//internal List<MCUDevice> mcuList { get; set; }
		internal MCUDevice mcu { get; set; }

		public WebSocketConnection(WebSocketHandler handler)
        {
            webSocketHandler = handler as HomeControlSocketHandler;
			_logger = handler._loggerFactory.CreateLogger<WebSocketConnection>();
			//mcuList = new List<MCUDevice>();
			mcu = new MCUDevice();
		}

        public virtual async Task SendMessageAsync(string message)
        {
			_logger.LogInformation($"SendMessageAsync(Mcu: {NickName}:{DeviceType} msg: {message})");

			if (WebSocket.State != WebSocketState.Open)
			{
				_logger.LogWarning($"SendMessageAsync() WebSocket state is not Open ({WebSocket.State}). Message will not be sent.");
				return;
			}
            var arr = Encoding.UTF8.GetBytes(message);

            var buffer = new ArraySegment<byte>(
                    array: arr,
                    offset: 0,
                    count: arr.Length);

            await WebSocket.SendAsync(
                buffer: buffer,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None
                );
        }

        public abstract Task ReceiveAsync(string message);
    }
}

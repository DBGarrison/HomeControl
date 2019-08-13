using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
 
namespace HomeControl.WebSocketManager
{
	 
	public abstract class WebSocketHandler
    {
		internal ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;
		
		public WebSocketHandler(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
			_logger = _loggerFactory.CreateLogger<WebSocketHandler>();
		}
		protected abstract int BufferSize { get; }

        private List<WebSocketConnection> _connections = new List<WebSocketConnection>();

        public List<WebSocketConnection> Connections { get => _connections; }

        public async Task ListenConnection(WebSocketConnection connection)
        {			
			_logger.LogInformation($"ListenConnection( {connection.NickName} - {connection.DeviceType} )entered. WebSocket.State: {connection.WebSocket.State}");

			var buffer = new byte[BufferSize];

            while (connection.WebSocket.State == WebSocketState.Open)
            {
				try
				{
					var result = await connection.WebSocket.ReceiveAsync(
						buffer: new ArraySegment<byte>(buffer),
						cancellationToken: CancellationToken.None);

					if (result.MessageType == WebSocketMessageType.Text)
					{
						var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
						_logger.LogInformation($"ListenConnection({connection.NickName} - {connection.DeviceType}). Receiving a msg: {message}");
						await connection.ReceiveAsync(message);
					}
					else if (result.MessageType == WebSocketMessageType.Close)
					{
						_logger.LogInformation("ListenConnection(). Result from WebSocket is CLOSE!!");
						await OnDisconnected(connection);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Unexpected error from WebSocket Client ({connection.NickName} - {connection.DeviceType} - {connection.DeviceType}): {ex.Message}");
					switch (connection.DeviceType)
					{
						case Enums.EnumDeviceType.Motion:
							foreach (var pir in connection.webSocketHandler.pirDevices.Where(d => d.pirStatus == Enums.EnumPirStatus.Streaming &&
																									  d.mcu.ipAddress.Equals(connection.NickName, StringComparison.OrdinalIgnoreCase)))
							{
								pir.stopDetecting();
								pir.setWebSocketConnection(0, null);
							}
							break;

						case Enums.EnumDeviceType.Relay:
							foreach (var relay in connection.webSocketHandler.relayDevices.Where(d => d.mcu.ipAddress.Equals(connection.NickName, StringComparison.OrdinalIgnoreCase)))
							{								
								relay.setWebSocketConnection(0, null);
							}
							break;

						case Enums.EnumDeviceType.RFSwitch:
							foreach (var rfSwitch in connection.webSocketHandler.rfSwitches.Where(d => d.mcu.ipAddress.Equals(connection.NickName, StringComparison.OrdinalIgnoreCase)))
							{
								rfSwitch.setWebSocketConnection(0, null);
							}
							break;

						default:
							break;
					}
				}
			}
			return;
        }

        public virtual async Task OnDisconnected(WebSocketConnection connection)
        {
            if (connection != null)
            {
				_logger.LogInformation($"OnDisconnected().  {connection.NickName}");
				_connections.Remove(connection);
				connection.mcu.connection = null;
				connection.mcu.remoteDeviceId = 0;

				//connection.mcuList.ForEach(m => m.connection = null);
				//connection.mcuList.ForEach(m => m.remoteDeviceId = 0);
				//connection.mcuList.Clear();
				await connection.WebSocket.CloseAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure,
                    statusDescription: "Closed by the WebSocketHandler",
                    cancellationToken: CancellationToken.None);
            }
        }

        public abstract Task<WebSocketConnection> OnConnected(HttpContext context);
    }
}

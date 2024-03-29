﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.WebSocketManager
{
    /// <summary>
    /// This should the last middleware in the pipeline when use websocket
    /// </summary>
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

		~WebSocketMiddleware()
		{
			//EventLogger.CloseLogfile();
		}

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var connection = await _webSocketHandler.OnConnected(context);
				try
				{
					if (connection != null)
					{
						await _webSocketHandler.ListenConnection(connection);
					}
					else
					{
						context.Response.StatusCode = 404;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Unexpected error: {ex.Message}");
				}
            }
        }
    }
}

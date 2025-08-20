// Services/WebSocketManager.cs
using LaCasa.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LaCasaBack.Sevices
{
    public interface IWebSocketManager  
    {
        Task AddConnectionAsync(WebSocket webSocket);
        Task RemoveConnectionAsync(WebSocket webSocket);
        Task BroadcastBookingEventAsync(BookingEventDto bookingEvent);
        Task HandleWebSocketAsync(WebSocket webSocket);
    }

    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();
        private readonly ILogger<WebSocketManager> _logger;

        public WebSocketManager(ILogger<WebSocketManager> logger)
        {
            _logger = logger;
        }

        public async Task AddConnectionAsync(WebSocket webSocket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _connections.TryAdd(connectionId, webSocket);
            _logger.LogInformation($"WebSocket connection added: {connectionId}");

            await HandleWebSocketAsync(webSocket);

            // Clean up when connection closes
            _connections.TryRemove(connectionId, out _);
            _logger.LogInformation($"WebSocket connection removed: {connectionId}");
        }

        public async Task RemoveConnectionAsync(WebSocket webSocket)
        {
            var connectionToRemove = _connections.FirstOrDefault(c => c.Value == webSocket);
            if (!connectionToRemove.Equals(default(KeyValuePair<string, WebSocket>)))
            {
                _connections.TryRemove(connectionToRemove.Key, out _);
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }
            }
        }

        public async Task BroadcastBookingEventAsync(BookingEventDto bookingEvent)
        {
            var message = JsonSerializer.Serialize(bookingEvent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            var tasks = new List<Task>();
            var connectionsToRemove = new List<string>();

            foreach (var connection in _connections)
            {
                if (connection.Value.State == WebSocketState.Open)
                {
                    tasks.Add(SendMessageAsync(connection.Value, segment));
                }
                else
                {
                    connectionsToRemove.Add(connection.Key);
                }
            }

            // Remove closed connections
            foreach (var connectionId in connectionsToRemove)
            {
                _connections.TryRemove(connectionId, out _);
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation($"Broadcasted booking event to {tasks.Count} connections");
        }

        public async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _logger.LogInformation($"Received WebSocket message: {message}");

                        // Handle ping/pong or other client messages if needed
                        if (message == "ping")
                        {
                            await SendMessageAsync(webSocket, Encoding.UTF8.GetBytes("pong"));
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning($"WebSocket exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WebSocket connection");
            }
        }

        private async Task SendMessageAsync(WebSocket webSocket, ArraySegment<byte> message)
        {
            try
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning($"Failed to send WebSocket message: {ex.Message}");
            }
        }

        private async Task SendMessageAsync(WebSocket webSocket, byte[] message)
        {
            await SendMessageAsync(webSocket, new ArraySegment<byte>(message));
        }
    }
}

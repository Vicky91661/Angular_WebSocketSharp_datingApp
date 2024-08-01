using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using backend;

namespace backend
{
    public class userRoomConnection
    {
        public string? User { get; set; }
        public string? Room { get; set; }
    }

    public class ChatWebSocketBehavior : WebSocketBehavior
    {
        private readonly IDictionary<string, UserRoomConnection> _connections;

        public ChatWebSocketBehavior(IDictionary<string, UserRoomConnection> connections)
        {
            _connections = connections;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var message = JsonSerializer.Deserialize<ChatMessage>(e.Data);

            if (message != null)
            {
                switch (message.Type)
                {
                    case "join":
                        JoinRoom(message);
                        break;
                    case "message":
                        SendMessageToRoom(message);
                        break;
                }
            }
        }

        private void JoinRoom(ChatMessage message)
        {
            var userRoomConnection = new UserRoomConnection
            {
                User = message.User,
                Room = message.Room
            };

            _connections[ID] = userRoomConnection;
            Sessions.SendTo(JsonSerializer.Serialize(new { Type = "info", Message = $"{message.User} has joined the room" }), userRoomConnection.Room);
            SendConnectedUsers(userRoomConnection.Room);
        }

        private void SendMessageToRoom(ChatMessage message)
        {
            if (_connections.TryGetValue(ID, out UserRoomConnection userRoomConnection))
            {
                Sessions.SendTo(JsonSerializer.Serialize(new { Type = "message", User = message.User, Message = message.Message, Time = DateTime.Now.ToString("h:mm:ss tt") }), userRoomConnection.Room);
            }
        }

        private void SendConnectedUsers(string room)
        {
            var users = _connections.Values.Where(u => u.Room == room).Select(u => u.User);
            Sessions.SendTo(JsonSerializer.Serialize(new { Type = "connectedUsers", Users = users }), room);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (_connections.TryGetValue(ID, out UserRoomConnection userRoomConnection))
            {
                _connections.Remove(ID);
                Sessions.SendTo(JsonSerializer.Serialize(new { Type = "info", Message = $"{userRoomConnection.User} has left the room" }), userRoomConnection.Room);
                SendConnectedUsers(userRoomConnection.Room);
            }
        }
    }

    public class ChatMessage
    {
        public string? Type { get; set; }
        public string? User { get; set; }
        public string? Room { get; set; }
        public string? Message { get; set; }
    }
}

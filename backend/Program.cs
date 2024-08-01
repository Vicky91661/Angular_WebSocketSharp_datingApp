using System.Net;
using WebSocketSharp.Server;
using backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(new Dictionary<string, UserRoomConnection>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();

// Configure WebSocket server
var webSocketServer = new WebSocketServer(IPAddress.Any, 6969);
webSocketServer.AddWebSocketService<ChatWebSocketBehavior>("/chat", () =>
{
    var connections = app.Services.GetRequiredService<IDictionary<string, UserRoomConnection>>();
    return new ChatWebSocketBehavior(connections);
});
webSocketServer.Start();

app.Run();

Console.WriteLine("WebSocket server started on ws://localhost:6969/chat");

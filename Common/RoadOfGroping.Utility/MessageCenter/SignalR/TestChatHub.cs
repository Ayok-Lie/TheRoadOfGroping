using Microsoft.AspNetCore.SignalR;
using RoadOfGroping.Utility.MessageCenter.SignalR.Dtos;

namespace RoadOfGroping.Utility.MessageCenter.SignalR
{
    public class TestChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            Console.WriteLine($"Received message: {message}");
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
using Microsoft.AspNetCore.SignalR;

namespace AltaSemester.Hubs
{
    public class UserActivityHub : Hub
    {
        public async Task BroadcastMessage(string message)
            => await Clients.All.SendAsync("ReceiveMessage", message);

        public async Task SendMessageLogin(string username)
            => await Clients.All.SendAsync("UserLogin", username);

        public async Task SendMessageLogout(string username) 
            => await Clients.All.SendAsync("UserLogout", username);
    }
}


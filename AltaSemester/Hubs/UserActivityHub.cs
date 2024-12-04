using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
namespace AltaSemester.Hubs
{
    public class UserActitvityHub : Hub
    {
        public async Task BroadscastMessage (string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",message);
        }
    }
}

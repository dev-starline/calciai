using NRediSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CalciAI.AdminService.ChatHubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string URL, string DomainId)
        {
            await Clients.All.SendAsync("ReceiveMessage", URL, DomainId);
        }
    }
}

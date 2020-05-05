using Microsoft.AspNetCore.SignalR;
using SocialChatSample.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialChatSample.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> userLooKup = new Dictionary<string, string>();

        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync(Messages.RECEIVE, username, message);
        }

        public async Task Register(string username)
        {
            var currentId = Context.ConnectionId;
            if (!userLooKup.ContainsKey(currentId))
            {
                userLooKup.Add(currentId,username);
                await Clients.AllExcept(currentId).SendAsync(Messages.RECEIVE, username, $"{username} joined the chat");
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconneted {e?.Message}{Context.ConnectionId}");
            string id = Context.ConnectionId;
            if(!userLooKup.TryGetValue(id, out string username))
            {
                username = "[unknown]";
            }

            userLooKup.Remove(id);
            await Clients.AllExcept(Context.ConnectionId).SendAsync(Messages.RECEIVE, username, $"{username} has left the chat");
            await base.OnConnectedAsync();
        }

    }
}

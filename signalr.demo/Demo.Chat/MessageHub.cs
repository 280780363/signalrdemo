using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat
{
    public class MessageHub : Hub
    {
        static ConcurrentDictionary<string, string> userDic;
        static MessageHub()
        {
            userDic = new ConcurrentDictionary<string, string>();
        }

        public MessageHub()
        {
            var c = Clients;
            var context = Context;
        }

        public Task Send(string user, string message)
        {
            string timestamp = DateTime.Now.ToShortTimeString();

            if (!userDic.ContainsKey(user))
                return Task.CompletedTask;

            var id = userDic[user];
            return MessageHandle.Enqueue(id, Context.User.Identity.Name, message);
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name))
                return Task.CompletedTask;

            if (userDic.ContainsKey(name))
                userDic[name] = Context.ConnectionId;
            else
                userDic.TryAdd(name, Context.ConnectionId);

            return base.OnConnectedAsync();
        }
    }
}

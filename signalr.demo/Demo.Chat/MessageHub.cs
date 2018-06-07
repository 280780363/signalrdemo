using Demo.Chat.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat
{
    [Authorize]
    public class MessageHub : Hub
    {
        MsgSender msgSender;
        MsgHandler msgQueueHandler;
        OnlineUsers onlineUsers;
        public MessageHub(MsgSender msgSender, MsgHandler msgQueueHandler, OnlineUsers onlineUsers)
        {
            this.msgSender = msgSender;
            this.msgQueueHandler = msgQueueHandler;
            this.onlineUsers = onlineUsers;
        }

        public async Task Send(string toUserId, string message)
        {
            string timestamp = DateTime.Now.ToShortTimeString();
            var toUser = onlineUsers.GetUserById(new Guid(toUserId));
            if (toUser == null)
                await SendErrorAsync("用户已离线");
            var fromUser = Context.User.GetUser();
            msgSender.Send(new Dtos.MsgDto
            {
                Content = message,
                FromUser = fromUser,
                SendTime = DateTime.Now,
                ToUser = toUser
            });

            await Task.CompletedTask;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var user = Context.User.GetUser();
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                onlineUsers.AddOrUpdateUser(user);
            }
            await RefreshUsersAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //disconnection
            await base.OnDisconnectedAsync(exception);
            var userId = Context.User?.GetUser()?.Id;
            if (userId.HasValue)
                onlineUsers.OfflineUser(userId.Value);
            await RefreshUsersAsync();
        }

        private async Task RefreshUsersAsync()
        {
            var users = onlineUsers.GetAllUser();
            await Clients.All.SendAsync("Refresh", users);
        }

        private async Task SendErrorAsync(string errorMsg)
        {
            await Clients.Caller.SendAsync("Error", errorMsg);
        }
    }
}

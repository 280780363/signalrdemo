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
            var toUser = onlineUsers.Get(new Guid(toUserId));
            if (toUser == null)
            {
                await SendErrorAsync("用户已离线");
                return;
            }
            var fromUser = Context.User.GetUser();
            msgSender.Send(new Dtos.MsgDto
            {
                Content = message,
                FromUser = fromUser,
                SendTime = DateTime.Now,
                ToUser = toUser
            });
        }

        /// <summary>
        /// 当有用户登录时 添加在线用户，并设置用户的ConnectionId
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var user = Context.User.GetUser();
            if (user == null)
            {
                await SendErrorAsync("您没有登录");
                return;
            }
            user.ConnectionId = Context.ConnectionId;
            onlineUsers.AddOrUpdateUser(user);
            await SendUserInfo();
            await RefreshUsersAsync();
        }

        /// <summary>
        /// 当有用户离开时，注销用户登录
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //disconnection
            await base.OnDisconnectedAsync(exception);
            var userId = Context.User?.GetUser()?.Id;
            if (userId.HasValue)
                onlineUsers.Remove(userId.Value);
            await SendUserInfo();
            await RefreshUsersAsync();
        }

        private async Task RefreshUsersAsync()
        {
            var users = onlineUsers.Get().ToList();
            // 发送给所有的在线客户端，通知刷新在线用户
            await Clients.All.SendAsync("Refresh", users);
        }

        private async Task SendErrorAsync(string errorMsg)
        {
            // 发送错误消息给调用者
            await Clients.Caller.SendAsync("Error", errorMsg);
        }

        private async Task SendUserInfo()
        {
            await Clients.Caller.SendAsync("UserInfo", Context.User.GetUser());
        }
    }
}

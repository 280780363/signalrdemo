using Demo.Chat.Dtos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat
{
    public class OnlineUsers
    {
        /// <summary>
        /// 用户id作为key
        /// </summary>
        private static ConcurrentDictionary<Guid, UserDto> onlineUsers { get; } = new ConcurrentDictionary<Guid, UserDto>();

        public void AddOrUpdateUser(UserDto user)
        {
            onlineUsers.AddOrUpdate(user.Id, user, (id, r) => user);
        }

        public List<UserDto> Get()
        {
            return onlineUsers.Values.ToList();
        }

        public UserDto Get(Guid userId)
        {
            onlineUsers.TryGetValue(userId, out UserDto user);
            return user;
        }

        public void Remove(Guid userId)
        {
            if (onlineUsers.ContainsKey(userId))
                onlineUsers.TryRemove(userId, out UserDto user);
        }
    }
}

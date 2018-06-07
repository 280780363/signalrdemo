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
        private static ConcurrentDictionary<Guid, UserDto> onlineUsers { get; } = new ConcurrentDictionary<Guid, UserDto>();

        public void AddOrUpdateUser(UserDto user)
        {
            if (onlineUsers.ContainsKey(user.Id))
                return;

            onlineUsers.AddOrUpdate(user.Id, user, (id, r) => user);
        }


        public List<UserDto> GetAllUser()
        {
            return onlineUsers.Values.ToList();
        }

        public UserDto GetUserById(Guid userId)
        {
            onlineUsers.TryGetValue(userId, out UserDto user);
            return user;
        }

        public void OfflineUser(Guid userId)
        {
            if (onlineUsers.ContainsKey(userId))
                onlineUsers.TryRemove(userId, out UserDto user);
        }
    }
}

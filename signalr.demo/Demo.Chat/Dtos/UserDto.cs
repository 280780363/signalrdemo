using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat.Dtos
{
    public class UserDto
    {
        // signalr当前的连接id
        public string ConnectionId { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Avatar { get; set; }
    }
}

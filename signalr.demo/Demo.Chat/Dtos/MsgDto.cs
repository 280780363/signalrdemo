using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat.Dtos
{
    public class MsgDto
    {
        public UserDto FromUser { get; set; }
        public UserDto ToUser { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Chat.Dtos
{
    public class MsgDto
    {
        public Guid Id { get; set; }

        public Guid FromUserId { get; set; }

        public Guid ToUserId { get; set; }

        public string Content { get; set; }

        public DateTime SendTime { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Data
{
    public class DemoUser : IdentityUser<Guid>
    {
        public string Avatar { get; set; }
    }
}

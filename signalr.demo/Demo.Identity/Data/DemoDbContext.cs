using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Data
{
    public class DemoDbContext : IdentityDbContext<DemoUser, DemoRole, Guid>
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }
    }
}

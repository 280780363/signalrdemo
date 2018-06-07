using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Demo.Chat
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [Route("")]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(HttpContext.User.GetUser());
        }
    }
}

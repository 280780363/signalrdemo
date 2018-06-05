using Demo.Chat.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Demo.Chat
{
    public static class Extensions
    {
        public static UserDto GetUser(this ClaimsPrincipal claimsPrincipal)
        {
            return new UserDto
            {
                Id = new Guid(claimsPrincipal.Claims.FirstOrDefault(r => r.Type == "sub").Value),
                EMail = claimsPrincipal.Claims.FirstOrDefault(r => r.Type == "email").Value,
                UserName = claimsPrincipal.Claims.FirstOrDefault(r => r.Type == "username").Value,
                Avatar = claimsPrincipal.Claims.FirstOrDefault(r => r.Type == "avatar").Value,
            };
        }
    }
}

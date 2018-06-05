using Demo.Identity.Data;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Providers
{
    public class ProfileService : IProfileService
    {
        UserManager<DemoUser> userManager;
        public ProfileService(UserManager<DemoUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims.ToList();
            var userId = claims.First(r => r.Type == "sub");
            var user = await userManager.FindByIdAsync(userId.Value);
            claims.Add(new System.Security.Claims.Claim("username", user.UserName));
            claims.Add(new System.Security.Claims.Claim("email", user.Email));
            claims.Add(new System.Security.Claims.Claim("avatar", user.Avatar));
            //这里是设置token包含的用户属性claim
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            await Task.CompletedTask;
        }
    }
}

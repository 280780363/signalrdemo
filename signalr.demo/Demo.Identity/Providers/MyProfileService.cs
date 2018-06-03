using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Providers
{
    public class MyProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {

            var claims = context.Subject.Claims.ToList();
            //这里是设置token包含的用户属性claim
            context.IssuedClaims = claims.ToList();

            await Task.CompletedTask;

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            await Task.CompletedTask;
        }
    }
}

using Demo.Identity.Data;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Data
{
    public class SeedData
    {

        public static List<Client> Clients()
        {
            return new List<Client>
            {
                new Client{
                    // 客户端id
                    ClientId ="chat_client",
                    // 客户端名称
                    ClientName ="chat client",
                    // TOKEN有效时长
                    AccessTokenLifetime = 3600,
                    // 配置TOKEN类型,reference为引用类型,数据不会存在TOKEN中
                    AccessTokenType= AccessTokenType.Jwt,
                    // 配置客户端授权模式
                    AllowedGrantTypes= GrantTypes.ResourceOwnerPassword,
                    // 配置客户端连接密码
                    ClientSecrets={ new Secret("123123".Sha256())},
                    // 客户端允许的请求范围
                    AllowedScopes={
                        "chatapi",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile },
                    //允许离线,即开启refresh_token
                    AllowOfflineAccess =true,
                    RequireClientSecret=false
                }
            };
        }

        public static IEnumerable<ApiResource> ApiResources()
        {
            return new List<ApiResource>
            {
                // 定义api资源 这里如果使用构造函数传入Name会默认创建一个同名的Scope，
                // 这点需要注意，因为这个Api如果没有Scope，那根本无法访问
                new ApiResource
                {
                    Name="chatapi",
                    DisplayName="chat api",
                    ApiSecrets= { new Secret("123123".Sha256()) },
                    Scopes={
                        new Scope("chatapi","chat api")
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                };
        }

        public static List<DemoUser> Users()
        {
            return new List<DemoUser>{
                new DemoUser
                {
                    UserName = "laowang",
                    Email = "520@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1528131041794&di=78ae71a3573dc86bc010e301005fea53&imgtype=0&src=http%3A%2F%2Fpic2.orsoon.com%2F2017%2F0309%2F20170309032925886.png"
                },
                new DemoUser
                {
                    UserName = "zhangsan",
                    Email = "521@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "http://pic20.photophoto.cn/20110804/0010023712739303_b.jpg"
                },
                new DemoUser
                {
                    UserName = "lisi",
                    Email = "521@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "http://p1.qzone.la/upload/0/14vy5x96.jpg"
                }
            };
        }
    }
}

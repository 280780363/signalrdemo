using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Identity.Providers
{
    public class SeedData
    {

        public static List<Client> Clients()
        {
            return new List<Client>
            {
                new Client{
                    //客户端id
                    ClientId ="chat_client",
                    //客户端名称
                    ClientName ="chat client",
                    //TOKEN有效时长
                    AccessTokenLifetime = 3600,
                    //配置TOKEN类型,reference为引用类型,数据不会存在TOKEN中
                    AccessTokenType= AccessTokenType.Jwt,
                    //配置客户端授权模式
                    AllowedGrantTypes= GrantTypes.ResourceOwnerPassword,
                    //配置客户端连接密码
                    ClientSecrets={ new IdentityServer4.Models.Secret("123123".Sha256())},
                    //客户端允许的请求范围
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
                //定义api资源
                // 注意坑 只能用构造函数
                new ApiResource("chatapi","chat api")
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
    }
}

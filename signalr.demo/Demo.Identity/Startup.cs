using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4;
using Demo.Identity.Providers;
using Demo.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;

namespace Demo.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        IConfiguration configuration;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 配置AspNetCore Identity 的DbContext服务
            services.AddDbContext<DemoDbContext>(r =>
            {
                r.UseNpgsql(configuration.GetConnectionString("chat"), options =>
                {
                    // 配置迁移时程序集
                    options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            // 配置AspNetCore Identity服务用户密码的验证规则
            services.AddIdentity<DemoUser, DemoRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

            })
            // 告诉AspNetCore Identity 使用DemoDbContext为数据库上下文
            .AddEntityFrameworkStores<DemoDbContext>()
            .AddDefaultTokenProviders();

            // 配置ids4服务
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                // ids4使用AspNetCore Identity为用户认证
                .AddAspNetIdentity<DemoUser>()
                // 使用数据库来存储客户端Clients ApiResource IdentityResource
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(configuration.GetConnectionString("chat"), sql =>
                        {
                            sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        });
                    };
                })
                // 使用数据库存储授权操作相关操作，数据库表PersistedGrants
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(configuration.GetConnectionString("chat"), sql =>
                        {
                            sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        });
                    };
                })
                // ids4使用自定义的用户档案服务
                .Services.AddTransient<IProfileService, ProfileService>();

            // 配置跨域，允许所有
            services.AddCors(r =>
            {
                r.AddPolicy("all", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    ;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            Migration(app).Wait();
            app.UseCors("all");
            app.UseIdentityServer();
        }



        /// <summary>
        /// 上下文迁移方法
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task Migration(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // 迁移DemoDbContext上下文
                scope.ServiceProvider.GetRequiredService<DemoDbContext>().Database.Migrate();
                // 迁移PersistedGrantDbContext上下文
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                // 迁移ConfigurationDbContext上下文
                configurationDbContext.Database.Migrate();

                // 注入用户管理 增加用户
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DemoUser>>();
                foreach (var user in SeedData.Users())
                {
                    if (userManager.FindByNameAsync(user.UserName).Result == null)
                    {
                        await userManager.CreateAsync(user, "123123");
                    }
                }

                // 增加ApiResources IdentityResources Clients
                if (!configurationDbContext.ApiResources.Any())
                    configurationDbContext.ApiResources.AddRange(SeedData.ApiResources().Select(r => r.ToEntity()));
                if (!configurationDbContext.IdentityResources.Any())
                    configurationDbContext.IdentityResources.AddRange(SeedData.IdentityResources().Select(r => r.ToEntity()));
                if (!configurationDbContext.Clients.Any())
                    configurationDbContext.Clients.AddRange(SeedData.Clients().Select(r => r.ToEntity()));
                await configurationDbContext.SaveChangesAsync();
            }
        }
    }
}

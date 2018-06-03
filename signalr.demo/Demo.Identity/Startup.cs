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
            services.AddDbContext<DemoDbContext>(r =>
            {
                r.UseNpgsql(configuration.GetConnectionString("chat"), options =>
                {
                    options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddIdentity<DemoUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

            })
            .AddEntityFrameworkStores<DemoDbContext>()
            .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<DemoUser>()
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
                .Services.AddTransient<IProfileService, MyProfileService>();

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
            Migration(app);
            app.UseCors("all");
            app.UseIdentityServer();
        }

        private void Migration(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DemoDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DemoUser>>();
                if (userManager.FindByNameAsync("gucao").Result == null)
                {
                    var result = userManager.CreateAsync(new DemoUser
                    {
                        UserName = "gucao",
                        Email = "280780363@qq.com",
                        Id = Guid.NewGuid().ToString(),
                        EmailConfirmed = true,
                        TwoFactorEnabled = false
                    }, "123123").Result;
                }

                if (!configurationDbContext.ApiResources.Any())
                    configurationDbContext.ApiResources.AddRange(SeedData.ApiResources().Select(r => r.ToEntity()));
                if (!configurationDbContext.IdentityResources.Any())
                    configurationDbContext.IdentityResources.AddRange(SeedData.IdentityResources().Select(r => r.ToEntity()));
                if (!configurationDbContext.Clients.Any())
                    configurationDbContext.Clients.AddRange(SeedData.Clients().Select(r => r.ToEntity()));
                configurationDbContext.SaveChanges();
            }
        }
    }
}

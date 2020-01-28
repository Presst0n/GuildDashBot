using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscordBot.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.IoC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using DiscordBot.Server.Security;
using System;
using DiscordBot.Server.EmailSender;
using DiscordBot.Server.Hubs;
using DiscordBot.Server.Data.ContextServices;
using DiscordBot.Server.SuperAdmin;
using DiscordBot.Server.BotData;
using DiscordBot.Server.BotData.DataAccess;
using Abstractions.Db;
using AutoMapper;
using Entities;
using DiscordBot.Server.ViewModels;

namespace DiscordBot.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<TextChannel, TextChannelViewModel>();
                cfg.CreateMap<ServerDetail, ServerDetailViewModel>();
                cfg.CreateMap<ChatMessage, ChatMessageViewModel>();
            });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services = services.AddGuildBotDependencies();

            services.AddScoped<ITwitchStreamers, TwitchStreamersService>();
            services.AddScoped<ITwitchChannel, TwitchChannelService>();
            services.AddScoped<IGuildMessages, GuildMessageService>();
            services.AddScoped<IGuildNotifications, GuildNotificationsService>();
            services.AddSingleton<BotEvents>();
            services.AddSingleton<IDataAccess, DataAccess>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // BotDbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<BotDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;
                //options.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(5));

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CreateRolePolicy",
                    policy => policy.RequireClaim("Create Role", "true"));

                options.AddPolicy("EditRolePolicy",
                    policy => policy.AddRequirements(new ManageAdminRolesRequirement()));

                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role", "true"));

                options.AddPolicy("ManageClaimsPolicy",
                    policy => policy.RequireClaim("Manage Claims", "true"));

                options.AddPolicy("AdminRolePolicy",
                    policy => policy.RequireRole("Admin", "Super Admin"));

                options.AddPolicy("ManageBotPolicy",
                    policy => policy.RequireRole("Moderator", "Admin", "Super Admin"));
            });

            services.AddSingleton(Configuration);
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<SuperAdminAccount>();
            services.AddSignalR();

            services.BuildServiceProvider().GetRequiredService<SuperAdminAccount>().CreateSuperAdmin();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<BotHub>("/botHub");
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using Data_SocialNetwork.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Services;
using Social_network.Authorization;
using Social_network.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace SocialNetwork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RepositoryOptions>(Configuration);
            services.AddDbContext<SocialNetworkContext>();


            services.AddScoped<UserService>();
            services.AddScoped<PostService>();
            services.AddScoped<FriendService>();
            services.AddScoped<GroupService>();


            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnectionString")));
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();



            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Security/Login";
                config.Cookie.Name = "SocialNetwork";
            });
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<>), typeof(SocialNetworkRepository<>)));

            //------------------------------------------------------------------------------------------------------
            services.AddScoped<IAuthorizationHandler, AdminOrPageOwner>();
            services.AddAuthorization(config =>
            {
                config.AddPolicy("OwnerPagePolicy", builder =>
                 builder.Requirements.Add(new CustomPageOwnerClaim()));
            });
            services.AddScoped<IAuthorizationHandler, AdminModeratorOrOwnerGroup>();
            services.AddAuthorization(config =>
            {
                config.AddPolicy("OwnerOrModeratorGroupPolicy", builder =>
                 builder.Requirements.Add(new CustomGroupOwnerClaim()));
            });
            //------------------------------------------------------------------------------------------------------


            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en-US");
                options.AddSupportedUICultures("en-US", "uk-UA");
            });
            services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();// добавляем локализацию представлений;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=MyPage}");
            });
        }
    }
}

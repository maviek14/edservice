using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IHttpDeletingDataClient, HttpDeletingDataClient>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DbConn")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            serviceProvider.GetService<ApplicationDbContext>().Database.EnsureCreated();
            SeedData(serviceProvider).Wait();
        }

        private async Task SeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("--> Creating roles and users...");
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetService<ApplicationDbContext>();

            if (!context.AppUsers.Any() || !context.Profiles.Any() || !context.Devices.Any() || !context.Contracts.Any())
            {
                string[] roleNames = { "Admin", "User" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                var admin = new IdentityUser { UserName = "admin@gmail.com", Email = "admin@gmail.com", EmailConfirmed = true };
                var user1 = new IdentityUser { UserName = "maciek.trochymiuk@gmail.com", Email = "maciek.trochymiuk@gmail.com", EmailConfirmed = true };
                var user2 = new IdentityUser { UserName = "tomek.nowak@gmail.com", Email = "tomek.nowak@gmail.com", EmailConfirmed = true };
                var user3 = new IdentityUser { UserName = "karolina.sulej@gmail.com", Email = "karolina.sulej@gmail.com", EmailConfirmed = true };
                string password = "zaq1@WSX";

                var _user = await UserManager.FindByEmailAsync(admin.Email);
                if (_user == null)
                {
                    var createUser = await UserManager.CreateAsync(admin, password);
                    if (createUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                _user = await UserManager.FindByEmailAsync(user1.Email);
                if (_user == null)
                {
                    var createUser = await UserManager.CreateAsync(user1, password);
                    if (createUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user1, "User");
                    }
                }

                _user = await UserManager.FindByEmailAsync(user2.Email);
                if (_user == null)
                {
                    var createUser = await UserManager.CreateAsync(user2, password);
                    if (createUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user2, "User");
                    }
                }

                _user = await UserManager.FindByEmailAsync(user3.Email);
                if (_user == null)
                {
                    var createUser = await UserManager.CreateAsync(user3, password);
                    if (createUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user3, "User");
                    }
                }
                Console.WriteLine("Done creating roles and users");

                var profiles = new List<Profile>
                {
                    new Profile{UserName=admin.UserName},
                    new Profile{UserName=user1.UserName},
                    new Profile{UserName=user2.UserName},
                    new Profile{UserName=user3.UserName}
                };
                profiles.ForEach(_ => context.Profiles.Add(_));
                context.SaveChanges();

                var users = new List<User>
                {
                    new User{
                        Name="ADMIN",
                        Surname="ADMIN",
                        AddedTime=DateTime.Parse("2020-01-01"),
                        Profile=profiles[0]
                    },
                    new User{
                        Name="Maciek",
                        Surname="Trochymiuk",
                        AddedTime=DateTime.Parse("2021-01-01"),
                        Profile=profiles[1]
                    },
                    new User{
                        Name="Tomek",
                        Surname="Nowak",
                        AddedTime=DateTime.Parse("2021-01-01"),
                        Profile=profiles[2]
                    },
                    new User{
                        Name="Karolina",
                        Surname="Sulej",
                        AddedTime=DateTime.Parse("2021-01-01"),
                        Profile=profiles[3]
                    }
                };
                users.ForEach(_ => context.AppUsers.Add(_));
                context.SaveChanges();

                var devices = new List<Device>
                {
                    new Device{
                        Type=DeviceType.PC,
                        Name="2009 computer",
                        Description="Really powerful computer",
                        Profile=profiles[1]
                    },
                    new Device{
                        Type=DeviceType.Monitor,
                        Name="MSI Monitor",
                        Description="144hz 1ms",
                        Profile=profiles[1]
                    },
                    new Device{
                        Type=DeviceType.Charger,
                        Name="Charger 1",
                        Description="Iphone 13 charger",
                        Profile=profiles[2]
                    }
                };
                devices.ForEach(_ => context.Devices.Add(_));
                context.SaveChanges();

                var contracts = new List<Contract>
                {
                    new Contract{
                        Name="Fix my PC please!!",
                        Description="I cant turn it on. I dont know whats wrong with it.",
                        Price=400,
                        Status=ContractStatus.Available,
                        CreatedTime=DateTime.Parse("2021-04-30"),
                        Device=devices[0],
                        Principal=profiles[1]
                    },
                    new Contract{
                        Name="Broken monitor",
                        Description="The screen is black after turning on",
                        Price=30,
                        Status=ContractStatus.Completed,
                        CreatedTime=DateTime.Parse("2021-04-30"),
                        Device=devices[1],
                        Principal=profiles[1],
                        CompletedTime=DateTime.Parse("2021-04-30"),
                        Mandatory=profiles[3]
                    },
                    new Contract{
                        Name="IPhone 13 charger",
                        Description="I seek fast repair, a cable is damaged",
                        Price=100.50f,
                        Status=ContractStatus.Taken,
                        CreatedTime=DateTime.Parse("2021-11-30"),
                        Device=devices[2],
                        Principal=profiles[2],
                        Mandatory=profiles[1]
                    }
                };
                contracts.ForEach(_ => context.Contracts.Add(_));
                context.SaveChanges();
            }
        }
    }
}

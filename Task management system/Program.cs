using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Task_management_system.Data;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Cards;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Identity;
using Task_management_system.Areas.Identity;
using Task_management_system.Areas;
using Microsoft.Extensions.Hosting;

namespace Task_management_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            //Важно
            builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection1")));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddErrorDescriber<LocalizedIdentityErrorDescriber>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();

            //builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            builder.Services.AddScoped<TokenProvider>();
            var app = builder.Build();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzI0NjE1QDMyMzAyZTMyMmUzMGorYUM4M3ljdDEvMkRNMUxBSVJ0bGRQc01uZ2RHbGVnamM0QWZ4MjJmLzg9");
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<Context>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    ContextSeed.SeedRolesAsync(userManager, roleManager).Wait();
                    ContextSeed.SeedAdminAsync(userManager, roleManager).Wait();
                
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Възникна грешка при запълването на базата данни с информация.");
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();



            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
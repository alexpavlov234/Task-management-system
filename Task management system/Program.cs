using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Task_management_system.Data;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Cards;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Task_management_system.Interfaces;
using Task_management_system.Services;
using Microsoft.AspNetCore.Identity;

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
            //builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<Context>();
            var app = builder.Build();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzI0NjE1QDMyMzAyZTMyMmUzMGorYUM4M3ljdDEvMkRNMUxBSVJ0bGRQc01uZ2RHbGVnamM0QWZ4MjJmLzg9");
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

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
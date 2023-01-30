using KeyValue_management_system.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Task_management_system.Areas;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Pages.Common;

namespace Task_management_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            //Важно
            builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection1")), ServiceLifetime.Scoped);

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddErrorDescriber<LocalizedIdentityErrorDescriber>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();

            //builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            builder.Services.AddScoped<TokenProvider>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();

            builder.Services.AddScoped<IKeyValueService, KeyValueService>();
            builder.Services.AddScoped<BaseHelper>();

            WebApplication app = builder.Build();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzQ3NTYzQDMyMzAyZTMzMmUzMGEyZkJKb2ltdWJxZTZKRDFVdmZqbW83cFZ3QzVQVFpTNlN2YUZyeVh0RVk9");
            using (IServiceScope scope = app.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    Context context = services.GetRequiredService<Context>();
                    UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    ContextSeed.SeedRolesAsync(userManager, roleManager).Wait();
                    ContextSeed.SeedAdminAsync(userManager, roleManager).Wait();

                }
                catch (Exception ex)
                {
                    ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
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
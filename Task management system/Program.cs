using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Task_management_system.Areas;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Services;
using Task_management_system.Services.Common;

namespace Task_management_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            _ = builder.Services.AddRazorPages();
            _ = builder.Services.AddServerSideBlazor();
            //Важно
            _ = builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection1")), ServiceLifetime.Scoped);

            _ = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddErrorDescriber<LocalizedIdentityErrorDescriber>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();

            //builder.Services.AddSingleton<WeatherForecastService>();
            _ = builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            _ = builder.Services.AddScoped<TokenProvider>();
            _ = builder.Services.AddScoped<IUserService, UserService>();
            _ = builder.Services.AddScoped<IIssueService, IssueService>();
            _ = builder.Services.AddScoped<IProjectService, ProjectService>();

            _ = builder.Services.AddScoped<IKeyValueService, KeyValueService>();
            _ = builder.Services.AddScoped<BaseHelper>();
            _ = builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

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
                    ContextSeed.SeedKeyValuesAsync(context);

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

                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseRequestLocalization("bg");


            _ = app.UseStaticFiles();

            _ = app.UseRouting();
            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapBlazorHub();
            _ = app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Syncfusion.Blazor;
using System.Configuration;
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
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            }).AddErrorDescriber<LocalizedIdentityErrorDescriber>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();

            //builder.Services.AddSingleton<WeatherForecastService>();
            _ = builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            _ = builder.Services.AddScoped<TokenProvider>();
            _ = builder.Services.AddScoped<IUserService, UserService>();
            _ = builder.Services.AddScoped<IIssueService, IssueService>();
            _ = builder.Services.AddScoped<IProjectService, ProjectService>();
            _ = builder.Services.AddTransient<IEmailSender, EmailSender>(i => new EmailSender(builder.Configuration["EmailSender:Host"], builder.Configuration.GetValue<int>("EmailSender:Port"), builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"), builder.Configuration["EmailSender:UserName"], builder.Configuration["EmailSender:Password"]));

            _ = builder.Services.AddScoped<IKeyValueService, KeyValueService>();
            _ = builder.Services.AddScoped<BaseHelper>();
            _ = builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

            WebApplication app = builder.Build();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhlXlpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jSn5QdkxhXXpXdnRXQg==;Mgo+DSMBPh8sVXJ0S0J+XE9BdFRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdUVgWXZcdHFcQGFZVQ==;ORg4AjUWIQA/Gnt2VVhkQlFac11JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkZjWn9XcnBQT2ZeV0c=;MTI0OTc3NEAzMjMwMmUzNDJlMzBjRWtCT1BPa2YraXR0N3FZMEZJdVVYWi9XNVVmZ3VPdWhTS0NDaXdiQ1hVPQ==;MTI0OTc3NUAzMjMwMmUzNDJlMzBpZ3dEY21Za0drQW8vUDN0UG53amdmMlVNT3RoY0I5RittV1M3dEhEWVBrPQ==;NRAiBiAaIQQuGjN/V0Z+WE9EaFpAVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUVgWXxeeXZRQ2laWU11;MTI0OTc3N0AzMjMwMmUzNDJlMzBma2k4T2JSZ1ByMm02MTZlQTNycmdqOXFrQU5SQUpRRXEramJpYmYwMlV3PQ==;MTI0OTc3OEAzMjMwMmUzNDJlMzBZNXZBZHR3cE4wZXQvTFQwaUdEbmFuKzZpU2FPN1FUMVRvLzlDWFZYWGE4PQ==;Mgo+DSMBMAY9C3t2VVhkQlFac11JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkZjWn9XcnBQT2dZVkc=;MTI0OTc4MEAzMjMwMmUzNDJlMzBoOUYwcTUvWU5Ra2I4WTkwSDg4WG5SUHpvNFN2cVlGR0dHUytOWDFGbndFPQ==;MTI0OTc4MUAzMjMwMmUzNDJlMzBRczQyRmRCNk10OWlvVS9BTHVBVFUxQU9xYk42NWFOeEV2M2pTZzFZbXdvPQ==;MTI0OTc4MkAzMjMwMmUzNDJlMzBma2k4T2JSZ1ByMm02MTZlQTNycmdqOXFrQU5SQUpRRXEramJpYmYwMlV3PQ==");
            using (IServiceScope scope = app.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    Context context = services.GetRequiredService<Context>();
                    context.Database.Migrate();
                    UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    ContextSeed.SeedRolesAsync(userManager, roleManager).Wait();
                    ContextSeed.SeedUsersAsync(userManager, roleManager).Wait();
                    ContextSeed.SeedKeyValues(context);

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
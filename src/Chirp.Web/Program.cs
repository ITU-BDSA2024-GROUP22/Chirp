using Chirp.Core;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<CheepRepository>();
        builder.Services.AddScoped<FollowRepository>();
        builder.Services.AddScoped<CheepService>();
        builder.Services.AddScoped<FollowService>();



        // Load database connection via configuration
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Database/Chirp.db";
        builder.Services.AddDbContext<DBContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));

        builder.Services.AddDefaultIdentity<Author>(options =>
            options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DBContext>();

        builder.Services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = "GitHub";
                options.RequireAuthenticatedSignIn = true;
            })
            //.AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication_github_clientId"];
                o.ClientSecret = builder.Configuration["authentication_github_clientSecret"];
                o.CallbackPath = "/signin-github";
                o.Scope.Add("user:email");
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            //_dbContext.Database.EnsureDeleted();

            _dbContext.Database.EnsureCreated();

            if (_dbContext.Database.IsRelational())
            {
                //_dbContext.Database.Migrate();
            }
            DbInitializer.SeedDatabase(_dbContext);
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Need this for logout to work
        app.UseAuthentication();
        app.UseAuthorization();
        //app.UseSession();

        app.MapRazorPages();

        app.Run();
    }
}

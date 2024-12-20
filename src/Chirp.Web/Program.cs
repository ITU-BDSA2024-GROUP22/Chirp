using Chirp.Core;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

/// <summary>
/// The main entry point for the Chirp application, responsible for configuring services, middleware, and
/// the application pipeline. This includes setting up database connections, authentication, and handling
/// request processing.
/// </summary>
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

        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Database/Chirp.db";
        builder.Services.AddDbContext<DBContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));

        builder.Services.AddDefaultIdentity<Author>(options =>
            options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DBContext>();

        builder.Services.AddAuthentication(options =>
            {
                options.RequireAuthenticatedSignIn = true;
            })
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication_github_clientId"];
                o.ClientSecret = builder.Configuration["authentication_github_clientSecret"];
                o.CallbackPath = "/signin-github";
                o.Scope.Add("user:email");
            });

        var app = builder.Build();

        // Source: Adapted from Lecture Slides, session 10 by Adrian Hoff
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");

            app.UseHttpsRedirection();

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            _dbContext.Database.EnsureCreated();
            DbInitializer.SeedDatabase(_dbContext);
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorPages();
        app.Run();
    }
}

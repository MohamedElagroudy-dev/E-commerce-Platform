
using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Extensions;
using E_commerce.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            #region added

            builder.AddPresentation();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.Addcore();

            #endregion

            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<AppUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.MapControllers();

            app.MapGroup("api").MapIdentityApi<AppUser>();
            app.MapHub<NotificationHub>("/hub/notifications");

            try
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                //var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await ApplicationDbContextSeed.SeedAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}

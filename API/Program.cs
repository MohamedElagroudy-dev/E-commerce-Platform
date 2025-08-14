
using E_commerce.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Extensions;
using Core.Extensions;
using Scalar.AspNetCore;
namespace API
{
    public class Program
    {
        public static void Main(string[] args)
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


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }


            app.MapControllers();

            app.Run();
        }
    }
}

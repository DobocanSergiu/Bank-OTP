
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1
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
            ///Enable inMemoryDatabase for development
            builder.Services.AddDbContext<UserContext>(opt =>
            opt.UseInMemoryDatabase("UserDatabase"));


            ///Add CORS for development purposes
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontends",
                    policy => policy
                        .WithOrigins(
                            "http://localhost:5173",
                            "https://localhost:7229")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });





            var app = builder.Build();

            ///Apply coors
            app.UseCors("AllowFrontends");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

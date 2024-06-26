using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SystemInfoApi.Classes;
using SystemInfoApi.Middleware;
using SystemInfoApi.Repositories;
using SystemInfoApi.Services;
using System.Text;

namespace SystemInfoApi
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JWT authentication setup
            string jwtKey = builder.Configuration["Jwt:Key"] ??
                throw new Exception("Invalid secret key in appsettings.json.");
            string jwtIssuer = builder.Configuration["Jwt:Issuer"] ??
                throw new Exception("Invalid issuer in appsettings.json.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddScoped<Database>();
            builder.Services.AddScoped<MachinesService>();
            builder.Services.AddScoped<MachinesRepository>();
            builder.Services.AddScoped<DrivesRepository>();
            builder.Services.AddScoped<OsRepository>();
            builder.Services.AddScoped<ApplicationsRepository>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); // auth experimental -----
            app.UseAuthorization(); // auth experimental-----
            //app.UseSession(); // auth experimental-----

            // Add 406 error code to ensure application/json accept header is present in requests
            app.UseMiddleware<NotAcceptableMiddleware>();

            // Try establishing a connection to the database and check auto-migration setting.
            Database db = new(app.Configuration, app.Environment);
            db.Init(app);

            app.MapControllers();

            app.Run();
        }
    }
}
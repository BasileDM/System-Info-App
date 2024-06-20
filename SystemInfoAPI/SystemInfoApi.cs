using SystemInfoApi.Classes;
using SystemInfoApi.Middleware;
using SystemInfoApi.Repositories;
using SystemInfoApi.Services;

namespace SystemInfoApi
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            app.UseAuthorization();

            // Add 406 error code to ensure application/json accept header is present in requests
            app.UseMiddleware<NotAcceptableMiddleware>();

            // Try establishing a connection to the database and check if tables exist
            Database db = new(app.Configuration, app.Environment);
            db.Init(app);

            app.MapControllers();

            app.Run();
        }
    }
}
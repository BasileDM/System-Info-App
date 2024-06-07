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
            builder.Services.AddScoped<MachinesService>();
            builder.Services.AddScoped<MachinesRepository>();
            builder.Services.AddScoped<DrivesRepository>();
            builder.Services.AddScoped<OsRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");  //experimental
                app.UseHsts();  //experimental
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();  //experimental
            app.MapControllers(); //experimental

            // Add error code to ensure application/json accept header is present
            //app.UseMiddleware<NotAcceptableMiddleware>();

            app.Run();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;

/*
 * Start SQL Server container:
 * docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123!!" -e "MSSQL_PID=Express" -p 1433:1433 --name sql1 -d mcr.microsoft.com/mssql/server:2019-latest
 * Add via SQL Server Object Explorer in Visual Studio:
 * for name use localhost, [PORT] (1433 is default)
 * for authentication use SQL Server Authentication
 * Trust Server Certificate: True
 * 
 */


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OurDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContainerConnection"))
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<OurDbContext>();

try
{
    if (context.Database.CanConnect())
    {
        Console.WriteLine("Successfully connected to the database.");
    }
    else
    {

        Console.WriteLine("Failed to connect to the database.");

    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
}

app.Run();
    
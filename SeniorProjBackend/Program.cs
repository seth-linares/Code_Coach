using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.Encryption;

/*
 * Start SQL Server container:
 * docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123!!" -e "MSSQL_PID=Express" -p 1433:1433 --name sql1 -d mcr.microsoft.com/mssql/server:2019-latest
 * Add via SQL Server Object Explorer in Visual Studio:
 * for name use localhost, [PORT] (1433 is default)
 * for authentication use SQL Server Authentication
 * Trust Server Certificate: True
 * 
 * 
 * SEEMS BROKEN FOR CONTAINERS FOR ME (SETH) AT LEAST SO USE THE ACTUAL DB FROM JAREDS SERVER
 * Connection string is called JaredConnection in the appsettings.development.json file
 * The JaredConnectionVS connection string is from me using VS to add a connected service to the database
 * 
 */


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddDbContext<OurDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("JaredConnectionVS"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddDbContext<OurDbContext>();

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

if (context.Database.CanConnect())
{
    var databaseName = context.Database.GetDbConnection().Database;
    Console.WriteLine($"Successfully connected to the database: {databaseName}.");

    try
    {
        var users = context.Users.ToList();
        Console.WriteLine($"Successfully retrieved {users.Count} users from the database.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to retrieve users from the database. Error: {ex.Message}");
    }
}
else
{
    Console.WriteLine("Failed to connect to the database.");
}



app.Run();
    
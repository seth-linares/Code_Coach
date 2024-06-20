using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.Encryption;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<OurDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RealConnection"))
);


Console.WriteLine($"\n\nSecure JWT Key: {TokenService.GenerateSecureKey()}\n\n");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenService, TokenService>();


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

try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<OurDbContext>();

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
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception during database connection: {ex.Message}");
}

app.Run();
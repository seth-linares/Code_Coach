using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeniorProjBackend.Data;
using SeniorProjBackend.Encryption;
//using Npgsql;



//Console.WriteLine($"Key to use: {TokenService.GenerateSecureKey()}\n\n");

//String connection_string = "Host=192.168.2.139;Port=5432;Database=CodeCoach;Username=postgres;Password=Password123!!";

//using (var conn = new NpgsqlConnection(connection_string))
//{
//    try
//    {
//        conn.Open();
//        Console.WriteLine($"\n\nConnection successfully estabilshed!");
//        using (var cmd = new NpgsqlCommand("SELECT version()", conn))
//        using (var reader = cmd.ExecuteReader())
//        {
//            while (reader.Read())
//            {
//                Console.WriteLine($"Reader getting string: {reader.GetString(0)}\n\n");
//            }
//        }

//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"\n\nException message: {ex.Message}\n\n");
//    }


//}


var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<OurDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RealConnection"))
);

builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
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
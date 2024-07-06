using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SeniorProjBackend.Data;
using SeniorProjBackend.Encryption;
using System.Text;

// This console write will appear in the Docker logs
Console.WriteLine("Starting SeniorProjBackend\n\n\n\n");

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<OurDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("RealConnection");
    Console.WriteLine($"\n\n\n\nUsing connection string: {connectionString}\n\n\n\n");
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddLogging(logging =>
{
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    logging.AddConsole();
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SeniorProjBackend API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://frontend:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidIssuer = "http://seniorprojbackend:8080",
        ValidateAudience = true,
        ValidAudience = "http://seniorprojbackend:8080",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["auth_token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configure Kestrel
builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}" } };
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SeniorProjBackend API v1");
    });
}
else
{
    // Consider adding production-specific middleware here
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// HTTPS redirection is removed as we're using Nginx for SSL termination
// app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
Console.WriteLine("\n\n\n\nSeniorProjBackend has started\n\n\n\n");
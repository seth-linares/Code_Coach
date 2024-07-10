using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SeniorProjBackend.Data;
using System.Threading.RateLimiting;
using SeniorProjBackend.Middleware;


// This console write will appear in the Docker logs
Console.WriteLine("Starting SeniorProjBackend\n\n\n\n");

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthorization();
builder.Services.AddAuthentication();



// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Configure Identity options
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;

    // Configure lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<OurDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/api/Users/Login";
    options.AccessDeniedPath = "/api/Users/AccessDenied";
    options.SlidingExpiration = true;
});

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

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender>(sp => sp.GetRequiredService<IEmailService>());

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




// Configure Kestrel
builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

// Cleanup Unconfirmed User Service
builder.Services.AddHostedService<UnconfirmedUserCleanupService>();


// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
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

//app.MapIdentityApi<User>();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
Console.WriteLine("\n\n\n\nSeniorProjBackend has started\n\n\n\n");
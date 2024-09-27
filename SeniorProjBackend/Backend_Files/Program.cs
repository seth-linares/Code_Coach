using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SeniorProjBackend.Data;
using SeniorProjBackend.Encryption;
using SeniorProjBackend.Middleware;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;


Console.WriteLine("Starting SeniorProjBackend\n\n\n\n");

var builder = WebApplication.CreateBuilder(args);


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
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
.AddEntityFrameworkStores<OurDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Perhaps could break the cookies with Nginx!!!!
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.LoginPath = "/api/Users/Login";
    options.AccessDeniedPath = "/api/Users/AccessDenied";
    options.SlidingExpiration = true;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});


/*


            NEED TO ADD SEEDING AND MIGRATION
            NEED TO ADD SEEDING AND MIGRATION
            NEED TO ADD SEEDING AND MIGRATION
            NEED TO ADD SEEDING AND MIGRATION
            NEED TO ADD SEEDING AND MIGRATION
            NEED TO ADD SEEDING AND MIGRATION
*/

// Configure services
builder.Services.AddDbContext<OurDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("RealConnection");
    Console.WriteLine($"\n\n\n\nUsing connection string: {connectionString}\n\n\n\n");
    options.UseNpgsql(connectionString);
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.MaxDepth = 32;
    });


builder.Services.AddLogging(logging =>
{
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    logging.AddConsole();
});


// Options
builder.Services.AddOptions<EmailServiceOptions>()
    .Bind(builder.Configuration.GetSection("Mailgun"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<Judge0Options>()
    .Bind(builder.Configuration.GetSection("Judge0"))
    .ValidateDataAnnotations()
    .ValidateOnStart();


// Lifetime services:

// No state, but beneficial to keep RestSharp instance over the runtime
builder.Services.AddSingleton<IEmailService, EmailService>();

// We can reuse the HttpClient, no request-specific state
builder.Services.AddSingleton<IChatGPTService, ChatGPTService>();
builder.Services.AddHttpClient<IChatGPTService, ChatGPTService>();



// There is no state, but also nothing that needs to be re-used, so we should just transient it.
builder.Services.AddTransient<Judge0AuthHandler>();
// Configure Judge0 HttpClient
builder.Services.AddHttpClient("Judge0", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Judge0:BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new InvalidOperationException("Judge0:BaseUrl is not configured");
    }
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<Judge0AuthHandler>();



builder.Services.AddEndpointsApiExplorer();

// Configure CORS for production
builder.Services.AddCors(options => {
    options.AddPolicy("AllowNextJSApp",
        builder => {
            builder
                .WithOrigins("https://codecoachapp.com")
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
                PermitLimit = 40,
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

// Use the production CORS policy
app.UseCors("AllowNextJSApp");

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

// Simplified security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Only add HSTS header if not in development
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await next();
});

app.MapControllers();

app.Run();
Console.WriteLine("\n\n\n\nSeniorProjBackend has started\n\n\n\n");

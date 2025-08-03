using Application.Clean;
using Domain.Clean;
using Infrastructure.Clean;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configure EF Core and DbContext
// Using SQL Server LocalDB for development - easy to switch to something else later
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1.1. Configure Identity with enhanced security
// I'm using ASP.NET Core Identity because it handles all the security headaches for me
// Password complexity, account lockout, email confirmation - it's all built-in
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Password policies - these are the minimum requirements for a production app
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Email uniqueness - prevents duplicate accounts
    options.User.RequireUniqueEmail = true;
    
    // Email confirmation - optional but recommended for production
    options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("Security:RequireEmailConfirmation");
    
    // Account lockout - protects against brute force attacks
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Security:LockoutDurationMinutes", 15));
    options.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("Security:MaxFailedAccessAttempts", 5);
})
.AddEntityFrameworkStores<TaskManagementDbContext>()
.AddDefaultTokenProviders();

// 2. Register application services
// Using dependency injection to keep things loosely coupled
// This makes testing much easier - I can mock these services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 2.1. Register authorization handlers and services
builder.Services.AddScoped<Application.Clean.AuthorizationHandlers.TaskAuthorizationHandler>();
builder.Services.AddScoped<Application.Clean.AuthorizationHandlers.UserAuthorizationHandler>();
builder.Services.AddScoped<Infrastructure.Clean.AuthorizationHandlers.DatabaseAuthorizationHandler>();
builder.Services.AddScoped<Application.Clean.IAuthorizationService, Infrastructure.Clean.AuthorizationService>();
builder.Services.AddHttpContextAccessor();

// 3. Add MediatR for event handling
// This is where the magic happens - when a task gets assigned, it publishes an event
// Any number of handlers can respond: create notification, send email, update dashboard
// Keeps the code loosely coupled and testable
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Application.Clean.IUserService).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Infrastructure.Clean.TaskService).Assembly);
});

// 4. Add Background Services
// This runs in the background and processes notifications asynchronously
// No need to block the main thread when sending notifications
builder.Services.AddHostedService<Infrastructure.Clean.BackgroundServices.NotificationProcessingService>();

// 5. Configure Authorization Policies
// I'm using policies instead of hardcoded roles in controllers
// This makes it easier to change authorization rules without touching controllers
builder.Services.AddAuthorization(options =>
{
    // Admin policy - only users with Admin role
    // Admins can do everything: create users, manage all tasks, etc.
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    // User policy - authenticated users
    // Basic policy for any logged-in user
    options.AddPolicy("UserOnly", policy =>
        policy.RequireAuthenticatedUser());
    
    // Task management policy - users can manage their own tasks or admins
    // Users can view tasks and manage their own, admins can manage all
    options.AddPolicy("TaskManagement", policy =>
        policy.RequireRole("Admin", "User"));
    
    // User management policy - only admins
    // Only admins can create/delete users
    options.AddPolicy("UserManagement", policy =>
        policy.RequireRole("Admin"));
    
    // Resource-based authorization policies
    options.AddPolicy("TaskRead", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("TaskUpdate", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("UserRead", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("UserUpdate", policy =>
        policy.RequireAuthenticatedUser());
});

// 6. Configure JWT Authentication with enhanced security and environment variable support
// JWT tokens are stateless and perfect for APIs
// The secret key can come from environment variables (for production) or config files (for development)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
                jwtSettings["SecretKey"] ?? 
                throw new InvalidOperationException("JWT SecretKey not configured. Set JWT_SECRET_KEY environment variable or configure in appsettings.json");

// Security check - make sure the secret key is strong enough
if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Require HTTPS in production - never send tokens over HTTP
    options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Security:RequireHttps", true);
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero  // No clock skew tolerance for better security
    };
});

builder.Services.AddControllers();

// Add CORS for API access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000") // Add your frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1h",
            Limit = 1000
        }
    };
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

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

// Add CORS middleware
app.UseCors("AllowSpecificOrigin");

// Add rate limiting middleware
app.UseIpRateLimiting();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// 7. Enable Authentication and Authorization
// Order matters here - authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

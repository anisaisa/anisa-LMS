using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Mappings;
using anisa_lms.Models;
using anisa_lms.Repositories;
using anisa_lms.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
       // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //e vendova
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//builder.Services.AddDbContext<AppDbContext>(
//  options => options.UseNpgsql(
//    builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString =
    Environment.GetEnvironmentVariable("PGHOST") != null
        ? $"Host={Environment.GetEnvironmentVariable("PGHOST")};" +
          $"Port={Environment.GetEnvironmentVariable("PGPORT")};" +
          $"Database={Environment.GetEnvironmentVariable("PGDATABASE")};" +
          $"Username={Environment.GetEnvironmentVariable("PGUSER")};" +
          $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")}"
        : builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("PGHOST = " + Environment.GetEnvironmentVariable("PGHOST"));
Console.WriteLine("PGDATABASE = " + Environment.GetEnvironmentVariable("PGDATABASE"));
Console.WriteLine("DATABASE_URL = " + Environment.GetEnvironmentVariable("DATABASE_URL"));
Console.WriteLine("Connection String = " + connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
}
).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication and authorization
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddMemoryCache();

// Authentication and authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Angular SPA sends Bearer token; also support HttpOnly cookie from login
            var authorization = context.Request.Headers.Authorization.ToString();
            if (
                !string.IsNullOrWhiteSpace(authorization)
                && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
            {
                context.Token = authorization["Bearer ".Length..].Trim();
                return Task.CompletedTask;
            }

            if (context.Request.Cookies.TryGetValue("jwt", out var cookieToken))
            {
                context.Token = cookieToken;
            }

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Auth failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
}
);
builder.Services.AddAuthorization();



// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(CourseProfile).Assembly);
});

// Services
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IAssessmentScoreRepository, AssessmentScoreRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IEnrollmentAccessService, EnrollmentAccessService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IAssessmentScoreService, AssessmentScoreService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Role seeder
//using (var scope = app.Services.CreateScope())
//{
//  var services = scope.ServiceProvider;

//try
//{
//  await RoleSeeder.Initialize(services);
//}
//catch (Exception ex)
//{
//  var logger = services.GetRequiredService<ILogger<Program>>();
//logger.LogError(ex, "An error occurred while seeding roles.");
//}
//}

//app.UseCors("Angular");
// Role seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await RoleSeeder.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles.");
    }
}

// Apply EF Core migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("Angular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<anisa_lms.Middleware.EnrollmentAccessExceptionMiddleware>();

app.MapControllers();

app.Run();

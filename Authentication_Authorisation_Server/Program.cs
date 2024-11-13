using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Authentication_Authorisation.Data;
using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Mappings;
using Authentication_Authorisation.Models;
using Authentication_Authorisation.Services;
using Authentication_Authorisation.Utils;
using Authentication_Authorisation.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the IAuthenticationService and AuthenticationService
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<TokenUtils>();
builder.Services.AddScoped<IRoleSeederService, RoleSeederService>(); 
builder.Services.AddScoped<ITokenService, TokenService>();
// Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password policy
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    // Username settings
    options.User.RequireUniqueEmail = true;
}); 

// Configure JWT authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add FluentValidation
builder.Services.AddTransient<IValidator<RegisterDto>, UserDtoValidator>();

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Swagger configuration with Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Authentication & Authorization",
        Description = "An ASP.NET Core Web API for Authentication_Authorization",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "LinkedIn",
            Url = new Uri("https://www.linkedin.com/in/abdessamadtanafaat/")
        },
    });
    
    // Add JWT Bearer  Authentication to swagger. 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorisation",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT authorisation header using the bearer scheme. Example : \"Authorisation: Bearer {token}\""
    }); 
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseMiddleware<Authentication_Authorisation.Exceptions.ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the root
    });
}

app.UseHttpsRedirection();

// Order is important: use authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Roles in the startup of the application. 
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeederService>();
    await roleSeeder.SeedRoleAsync(); 
}

app.Run();

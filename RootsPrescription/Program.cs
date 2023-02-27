using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Serilog.Expressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddHealthChecks();
builder.Services.AddAuthentication()
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "access_token";
        options.LoginPath = "/api/Login/Status_401"; // Wrokaround to deliver a 401 when cookie is missing / unauthorized
    });


builder.Services.AddAuthorization(options =>
    {
        var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
            JwtBearerDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme);

        defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
        options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
    });



Log.Logger = new LoggerConfiguration()
    .CreateLogger();

// Splunk logging - HTTP Event Collector
// https://dev.splunk.com/enterprise/docs/devtools/csharp/logging-dotnet
// See configuration in appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

string group = builder.Configuration["GroupName"];
app.Logger.LogInformation($"Hello world, {group} connected!");  

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

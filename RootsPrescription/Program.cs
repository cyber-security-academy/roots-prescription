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
builder.Services.AddHttpContextAccessor();
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
        options.LoginPath = "/api/Login/Status_401"; // CSA: Workaround to deliver a 401 when cookie is missing / unauthorized
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
    .Enrich.WithClientIp()  // CSA: Serilog..WithClientIp() trusts X-Forwarded-For blindly, and you cannot turn this off  :o
);

var app = builder.Build();

string group = builder.Configuration["GroupName"];
app.Logger.LogInformation("Hello world, {GroupName} connected!", group);

app.Use(async (context, next) => {
    await next.Invoke();

    if (context.Request.Path != "/favicon.ico" && !context.Response.HasStarted)
    {
        if (context.Response.StatusCode == 404)
        {
            // TODO: app.Logger.LogWarning("404: Unknown URL attempted: {Method} {Path}", context.Request.Method, context.Request.Path);
            await context.Response.WriteAsync("404 - Denne siden finnes ikke.");
        }
    }
});



app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

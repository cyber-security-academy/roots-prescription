using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

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



var app = builder.Build();

// Splunk logging - HTTP Event Collector
// https://dev.splunk.com/enterprise/docs/devtools/csharp/logging-dotnet
var traceSource = new TraceSource("SplunkLogger");
traceSource.Switch.Level = SourceLevels.All;
traceSource.Listeners.Clear();
traceSource.Listeners.Add(new HttpEventCollectorTraceListener(
    uri: new Uri("https://log.splunk.csa.datasnok.no"),  // must be ENV var
    token: "f727aec9-cd4c-4238-a76e-a4367ecf7ead");  // must be ENV var
// Add app id env var?
traceSource.TraceEvent(TraceEventType.Information, "Hello world, API 0 connected!");  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

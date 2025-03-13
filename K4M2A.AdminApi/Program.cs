using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using K4M2A.Entities;
using K4M2A.AdminApi.AppContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using K4M2A.AdminApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Enable legacy timestamp behavior for Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
var ConnectionString = builder.Configuration.GetConnectionString("Default");
var ConnectionStringMSSql = builder.Configuration.GetConnectionString("DefaultMSSql");

var configRepository = new ConfigurationRepository(ConnectionString);

GlobalVariables.NotificationAPIUrl = builder.Configuration.GetSection("NodeNotificationUrlLive").Value;
GlobalVariables.ElasticPostNodeUrl = builder.Configuration.GetSection("NodeElasticPostUrlLive").Value;
GlobalVariables.BookLibrary = builder.Configuration.GetSection("BookLibraryUrl").Value;
GlobalVariables.OpenAPIKey = builder.Configuration.GetSection("OpenAPIKey").Value;
GlobalVariables.OpenAIapiURL = builder.Configuration.GetSection("OpenAIURL").Value;
GlobalVariables.SiteName = await configRepository.GetConfigurationValueAsync("SiteName");
GlobalVariables.SupportEmail = await configRepository.GetConfigurationValueAsync("SupportEmail");
GlobalVariables.SiteUrl = await configRepository.GetConfigurationValueAsync("SiteUrl");
GlobalVariables.SMTPHost = await configRepository.GetConfigurationValueAsync("SMTPHost");
GlobalVariables.SMTPUsername = await configRepository.GetConfigurationValueAsync("SMTPUsername");
GlobalVariables.SMTPPassword = await configRepository.GetConfigurationValueAsync("SMTPPassword");
GlobalVariables.SMTPPort = await configRepository.GetConfigurationValueAsync("SMTPPort");
GlobalVariables.SSLEnable = await configRepository.GetConfigurationValueAsync("SSLEnable");
GlobalVariables.TwilioaccountSid = await configRepository.GetConfigurationValueAsync("TwilioaccountSid");
GlobalVariables.TwilioauthToken = await configRepository.GetConfigurationValueAsync("TwilioauthToken");

builder.Services.AddDbContext<AppDbContext>((serviceProvider, dbContextBuilder) =>
{
    dbContextBuilder.UseNpgsql(ConnectionString, dbContextBuilder => dbContextBuilder.EnableRetryOnFailure());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRestClient, RestClient>();
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
    };
});


var app = builder.Build();

// Middleware configuration
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();

// Swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
});

// Endpoint mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

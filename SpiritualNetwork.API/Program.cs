
using Microsoft.OpenApi.Models;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Hubs;
using RestSharp;
using SpiritualNetwork.API;
using SpiritualNetwork.API.GraphQLSchema;
using EntityGraphQL.AspNet;
using SpiritualNetwork.API.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


GlobalVariables.NotificationAPIUrl = builder.Configuration.GetSection("NodeNotificationUrlLive").Value;
GlobalVariables.ElasticPostNodeUrl = builder.Configuration.GetSection("NodeElasticPostUrlLive").Value;
GlobalVariables.BookLibrary = builder.Configuration.GetSection("BookLibraryUrl").Value;
GlobalVariables.OpenAPIKey = builder.Configuration.GetSection("OpenAPIKey").Value;
builder.Services.AddDbContext<AppDbContext>((serviceProvider, dbContextBuilder) =>
{
    var ConnectionString = builder.Configuration.GetConnectionString("Default");
    dbContextBuilder.UseSqlServer(ConnectionString,dbContextBuilder => dbContextBuilder.EnableRetryOnFailure());
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

builder.Services.AddGraphQLSchema<AppDbContext>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
     builder =>
     {
         builder
         .AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();
     });

    //options.AddPolicy("AllowSpecificOrigin", builder =>
    //{
    //    builder.WithOrigins("https://backoffice.generositymatrix.net", "http://localhost:3000")
    //           .AllowAnyHeader()
    //           .AllowAnyMethod();
    //});
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "JWT Autherization",
        Name = "Autherization",
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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IGlobalSettingService, GlobalSettingService>();
builder.Services.AddScoped<IQuestion, QuestionService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IReactionService,ReactionService>();
builder.Services.AddScoped<ISubcriptionService, SubcriptionService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IChatService,ChatService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<IRestClient, RestClient>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IK4M2AService, K4M2AService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IHastTagService, HashTagService>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddSingleton<RabbitMQConsumerService>();
builder.Services.AddHostedService<RabbitMQConsumerHostedService>();
builder.Services.AddHostedService<KafkaConsumerBackgroundService>();
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

app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

//app.MapGraphQL<AppDbContext>();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    // ^ This line sets the URL for the Swagger JSON file.
    // You can adjust the path and version as per your setup.
    // For example, if you have multiple versions, you can change "v1" to "v2".
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/chathub");

app.Run();


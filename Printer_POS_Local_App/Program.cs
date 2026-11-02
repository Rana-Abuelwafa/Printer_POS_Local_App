using ITravel_App.Models;
using ITravel_App.Services.POSService;

using ITravel_App.Services.PrinterService;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Printer_POS_Local_App.Services.POSService;
using Printer_POS_Local_App.Services.POSService.YourProject.Services.POS;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//var logger = new LoggerConfiguration()
//  .ReadFrom.Configuration(builder.Configuration)
//  .Enrich.FromLogContext()
//  .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);
//var logPath = Path.Combine(
//    AppContext.BaseDirectory,
//    "Logs",
//    "log-.txt"
//);

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File(
//        logPath,
//        rollingInterval: RollingInterval.Day
//    )
//    .CreateLogger();
// Add services to the container.
var logPath = Path.Combine(
    AppContext.BaseDirectory,
    "Logs",
    "log-.txt"
);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30
    )
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddRazorPages();
builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
//builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
// ----------------------------
// Run as Windows Service
// ----------------------------
//builder.Host.UseWindowsService(options =>
//{
//    options.ServiceName = "Booking Print Service";
//});
//if (!Environment.UserInteractive)
//{
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "POSPrintService";
    });
//}

// ----------------------------
// Listen only on localhost
// ----------------------------
builder.WebHost.UseUrls("http://127.0.0.1:5005");
Encoding.RegisterProvider(
    CodePagesEncodingProvider.Instance);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Expand-horizon-printer-pos", // Change this line
        Version = "v1",
        Description = "API For POS PRINTER", // Optional: add a description
                                                 // ... other info properties
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
    options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
});



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPrinterService,
                           PrinterService>();
builder.Services.Configure<PosSettings>(
    builder.Configuration.GetSection("PosSettings"));
//builder.Services.AddScoped<IPosCommunicationService,
//                           PosCommunicationService>();
builder.Services.AddSingleton<IPosCommunicationService, PosCommunicationService>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "de" }; // Add more as needed
    options.SetDefaultCulture("en");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);

//app.UseRequestLocalization(locOptions.Value);

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


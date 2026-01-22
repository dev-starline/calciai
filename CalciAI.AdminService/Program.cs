using CalciAI.AdminService.ChatHubs;
using CalciAI.Persistance;
using CalciAI.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("SignalRCors", policy =>
//    {
//        policy
//            .WithOrigins("http://localhost:55736")// live ip or domain need to all
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});
builder.Services.AddSignalR();



var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
PersistanceIoC.RegisterPersistance(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
var app = builder.Build();


app.UseCors(CalciAI.Auth.AuthFields.CORS_POLICY);
//app.UseCors("SignalRCors"); // Apply CORS policy
app.MapHub<ChatHub>("/chatHub");

ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

startup.Configure(app, null, loggerFactory, app.Environment);
app.Run();
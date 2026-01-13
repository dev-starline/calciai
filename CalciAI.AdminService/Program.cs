using CalciAI.Persistance;
using CalciAI.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
PersistanceIoC.RegisterPersistance(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
var app = builder.Build();

ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

startup.Configure(app, null, loggerFactory, app.Environment);
app.Run();
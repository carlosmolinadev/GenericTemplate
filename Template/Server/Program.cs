using Microsoft.AspNetCore.ResponseCompression;

using Serilog;
using Template.Server;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Template API Starting...");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
     .WriteTo.Console()
     .ReadFrom.Configuration(context.Configuration));

var app = builder
       .ConfigureServices()
       .ConfigurePipeline();

app.UseSerilogRequestLogging();

//await app.ResetDatabaseAsync();

app.Run();

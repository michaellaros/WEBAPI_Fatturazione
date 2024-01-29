using FatturazioneAPI.Services;
using NLog.Web;
using System.Globalization;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

// Add services to the container.

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.UseNLog();
    builder.Services.AddCors();
    builder.Services.AddTransient<DataBase>();
    builder.Services.AddTransient<RicevutaBiz>();
    builder.Services.AddTransient<PDFBiz>();
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(options => options.AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowAnyOrigin()
                          );
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
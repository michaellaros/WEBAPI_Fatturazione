using FatturazioneAPI.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

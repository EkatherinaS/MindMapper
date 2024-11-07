using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.HostedServices;
using MindMapper.WebApi.Options;
using MindMapper.WebApi.Services;
using MindMapper.WebApi.Services.Interfaces;
using FileOptions = MindMapper.WebApi.Options.FileOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITopicsService, TopicsService>();
builder.Services.AddScoped<IGptService, GptService>();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<DocumentHostedService>();

builder.Services.AddCors();

builder.Services.Configure<YandexGptOptions>(builder.Configuration.GetRequiredSection("YandexGptOptions"));
builder.Services.Configure<FileOptions>(builder.Configuration.GetRequiredSection("FileOptions"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var feature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (feature != null)
    {
        feature.MaxRequestBodySize = int.MaxValue;
    }

    await next.Invoke();
});

app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

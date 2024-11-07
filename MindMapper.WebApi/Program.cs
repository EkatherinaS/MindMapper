using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Data.Entities;
using MindMapper.WebApi.Options;
using MindMapper.WebApi.Services;
using MindMapper.WebApi.Services.Interfaces;
using MindMapper.WebApi.Services;
using MindMapper.WebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITopicsService, TopicsService>();
builder.Services.AddSingleton<IGptService, GptService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.Configure<YandexGptOptions>(builder.Configuration.GetRequiredSection("YandexGptOptions"));

var app = builder.Build();

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

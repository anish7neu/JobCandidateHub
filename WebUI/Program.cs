using Application;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using System;
using System.Text.Json;
using Application.Common.Interfaces;
using Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddMemoryCache();
builder.Services.AddDistributedRedisCache(options => {
    options.Configuration = "localhost:6379";
    options.InstanceName = "";
});
//builder.Services.AddSingleton<ICacheService,RedisCacheService>();
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//                                                        {
//                                                            var config = ConfigurationOptions.Parse("localhost:6379", true);
//                                                            config.ClientName = "SampleInstance";
//                                                            return ConnectionMultiplexer.Connect(config);
//                                                        });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Job Candidate Hub API", Version = "v1" });
});
builder.Services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();
builder.Services
    .AddApplication()
    .AddInfrastructure(configuration);
builder.Services.AddControllers(config =>
{
    var httpContextAccessor = new HttpContextAccessor();
});

builder.Services.AddFluentValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using FictiveShop.Core.Domain;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using FictiveShop.Infrastructure.Repositories;
using FictiveShop.Infrastructure.Services;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FictiveShopDbContext>();
builder.Services.AddSingleton<IInMemoryRedis, RedisContext>();
builder.Services.AddTransient<IRepository<Product>, ProductsRepository>();
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();

builder.Services.AddTransient<IBasketService, BasketService>();

builder.Services.AddMediatR(Assembly.Load("FictiveShop.Infrastructure"));
builder.Services.AddMediatR(Assembly.Load("FictiveShop.Api"));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
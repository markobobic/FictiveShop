using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using FictiveShop.Infrastructure.Services;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FictiveShopDbContext>();

builder.Services.AddMediatR(Assembly.Load("FictiveShop.Infrastructure"));
builder.Services.AddMediatR(Assembly.Load("FictiveShop.Api"));

builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
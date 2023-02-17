using FictiveShop.Core.Constants;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Middleware;
using FictiveShop.Core.Pipelines;
using FictiveShop.Core.Settings;
using FictiveShop.Infrastructure.DataAccess;
using FictiveShop.Infrastructure.Repositories;
using FictiveShop.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FictiveShopDbContext>();
builder.Services.AddSingleton<IInMemoryRedis, RedisContext>();

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
builder.Services.AddSingleton(Log.Logger);
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISupplierStockService, SupplierStockService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddMediatR(Assembly.Load(Assemblies.FictiveShopInfrastructure));
builder.Services.AddMediatR(Assembly.Load(Assemblies.FictiveShopCore));
Assembly core = Assembly.Load(Assemblies.FictiveShopCore);
AssemblyScanner.FindValidatorsInAssembly(core)
               .ForEach(x => builder.Services.AddTransient(typeof(IValidator), x.ValidatorType));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
builder.Services.Scan(x =>
{
    x.FromAssemblies(Assembly.Load(Assemblies.FictiveShopCore))
        .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime();
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
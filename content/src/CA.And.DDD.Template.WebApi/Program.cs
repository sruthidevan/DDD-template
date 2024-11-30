using CA.And.DDD.Template.Infrastructure.Installers;
using CA.And.DDD.Template.Infrastructure.Persistance.MsSql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.InstallEntityFramework();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.InstallSwagger();
builder.InstallApplicationSettings();

var redisConnection = builder.InstallRedis();
builder.InstallRedisCache();

builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

builder.InstallTelemetry(builder.Configuration, redisConnection);
builder.InstallMassTransit();
builder.InstallDependencyInjectionRegistrations();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    EntityFrameworkInstaller.SeedDatabase(appDbContext);
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

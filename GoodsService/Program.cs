using GoodsService.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace GoodsService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IShopRepository, ShopRepository>();
        builder.Services.AddScoped<IGoodsRepository, GoodsRepository>();

        builder.Services.AddDbContext<ShopsDbContext>(options =>
        {
            var databaseType = builder.Configuration.GetValue<string>("DatabaseType");
            if (databaseType == "SqlServer")
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                options.UseInMemoryDatabase("Shops");
            }
        });

        var app = builder.Build();

        var policy = Policy.Handle<SqlException>()
            .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(5));

        policy.Execute(() =>
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ShopsDbContext>();
            if (context.Database.IsSqlServer()) context.Database.Migrate();
        });

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
    }
}

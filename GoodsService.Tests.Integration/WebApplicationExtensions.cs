using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoodsService.Tests.Integration;

public static class WebApplicationExtensions
{
    public static void RemoveService<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    public static WebApplicationFactory<Program> AddContext(this WebApplicationFactory<Program> factory, ShopsDbContext context)
    {
        return factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveService<ShopsDbContext>();
                    services.RemoveService<DbContextOptions<ShopsDbContext>>();

                    services.AddSingleton(context);
                });
            });
    }

    public static ShopsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ShopsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ShopsDbContext(options);

        return context;
    }
}

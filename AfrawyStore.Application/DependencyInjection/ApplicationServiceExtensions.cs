using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AfrawyStore.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IInventoryService, InventoryService>();
        // Add other services here as they are implemented
        
        return services;
    }
}

using System.Drawing;
using BlazorFlow.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFlow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorFlow(this IServiceCollection services)
    {
        services.AddScoped<IDrawerService, DrawerService>();
        services.AddScoped<IDialogService, DialogService>();
        services.AddScoped<IToastService, ToastService>();
        services.AddScoped<ColorService>();

        return services;
    }
}
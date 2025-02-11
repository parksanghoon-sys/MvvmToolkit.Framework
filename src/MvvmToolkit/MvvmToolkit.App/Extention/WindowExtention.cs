using Microsoft.Extensions.DependencyInjection;
using MvvmToolkit.App.Services;
using MvvmToolkit.Core.Services;

namespace MvvmToolkit.App.Extention
{
    public static class WindowExtention
    {
        public static IServiceCollection AddThemeService(this IServiceCollection services)
        {
            services.AddSingleton<IThemeService, ThemeService>();
            
            return services;
        }
    }  
}

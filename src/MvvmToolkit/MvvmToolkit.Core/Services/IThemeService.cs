using System.Reflection;

namespace MvvmToolkit.Core.Services
{
    public interface IThemeService
    {
        void ApplyTheme(string themeName);
        void RegisterTheme(string themeName, string resourceName, string assemblyName = "MvvmToolkit.Ui");
    }
}

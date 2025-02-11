using Microsoft.Extensions.Hosting;
using MvvmToolkit.Core.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MvvmToolkit.App.Services
{
    public class ThemeService : IThemeService
    {
        private Dictionary<string, ResourceDictionary> _themes = new();
        public void ApplyTheme(string themeName)
        {
            ResourceDictionary theme = _themes[themeName];
            foreach(var kvp in _themes)
            {
                Application.Current.Resources.MergedDictionaries.Remove(kvp.Value);
            }
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }
        public ThemeService()
        {
            RegisterTheme("Light", "Light.xaml");
            RegisterTheme("Dark", "Dark.xaml");
            ApplyTheme("Light");            
        }
        public void RegisterTheme(string themeName, string resourceName, string assemblyName = "MvvmToolkit.App")
        {
            string urlString = $"/{assemblyName};component/Themes/Theme/{resourceName}";
            ResourceDictionary theme = new()
            {
                Source = new Uri(urlString, UriKind.RelativeOrAbsolute)
            };
            _themes.Add(themeName, theme);
        }
    }
}

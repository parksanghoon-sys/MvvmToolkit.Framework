using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using MvvmToolkit.Core.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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
            var isRightTheme = IsLightTheme();
            RegisterTheme("Light", "Light.xaml");
            RegisterTheme("Dark", "Dark.xaml");

            if (isRightTheme == false)
                ApplyTheme("Dark");
            else
                ApplyTheme("Light");            
        }
        private void RegisterTheme(string themeName, string resourceName, string assemblyName = "MvvmToolkit.App")
        {
            string urlString = $"/{assemblyName};component/Themes/Theme/{resourceName}";
            ResourceDictionary theme = new()
            {
                Source = new Uri(urlString, UriKind.RelativeOrAbsolute)
            };
            _themes.Add(themeName, theme);
        }
        private bool IsLightTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }
    }
}

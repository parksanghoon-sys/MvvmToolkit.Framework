using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmToolkit.Ui
{
    public enum ApplicationTheme
    {
        /// <summary>
        /// Unknown application theme.
        /// </summary>
        Unknown,

        /// <summary>
        /// Dark application theme.
        /// </summary>
        Dark,

        /// <summary>
        /// Light application theme.
        /// </summary>
        Light,
    }
    public static class ApplicationThemeManager
    {
        private static ApplicationTheme _applicationTheme = ApplicationTheme.Unknown;

        internal const string LibraryNamespace = "MvvmToolkit.Ui;";

        internal const string ThemesDictionaryPath = "pack://application:,,,/MvvmToolkit.Ui;component/Themes/Theme/";
    }
}

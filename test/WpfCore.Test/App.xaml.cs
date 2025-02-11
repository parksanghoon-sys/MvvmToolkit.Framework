using MvvmToolkit.App;
using System.Windows;

namespace WpfCore.Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : GenericHostApplication
    {
        public App()
        {
            
        }        
        protected override Window CreateShell()
        {
            return new MainWindow();
        }
    }

}

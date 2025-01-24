using MvvmToolkit.Core.Attributes;
using MvvmToolkit.Core.Components;
using System.Windows;

namespace WpfCore.Test
{
    public partial class MainViewModel : NotifyPropertyChangeObject
    {
        [Property]
        private string _name;
        [RelayCommand]
        private void OnClick()
        {
            MessageBox.Show("Click");
            Console.WriteLine("Click");
        }
        [AsyncRelayCommand]
        private async Task OnClickAsync()
        {
            await Task.Delay(1000);
            MessageBox.Show("Click");
            Console.WriteLine("Click");
        }
        public MainViewModel()
        {
            Name = "Hello"; 
        }
    }
}

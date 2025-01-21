using MvvmToolkit.Core.Components;

namespace WpfCore.Test
{
    public partial class MainViewModel : ViewModelBase
    {
        [Property]
        private string _name;
        [RelayCommand]
        private void OnClick()
        {
            Console.WriteLine("Click");
        }
        public MainViewModel()
        {
            
        }
    }
}

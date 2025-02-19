namespace MvvmToolkit.Core.Ioc
{
    public class ServiceType
    {
        public Type? Type { get; set; }
        public bool IsSingleton { get; set; }
        public object? Prameter { get; set; }
        public object? CalbakcFunc { get; set; }
    }
}

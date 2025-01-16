using MvvmToolkit.Core.Attributes;

namespace GeneratorTest
{ 
    public partial class PropertyGenerator
    {
        [Property]
        private string name;
        [Fact]
        public void Test1()
        {
            Name = "Test";
        }
    }
}
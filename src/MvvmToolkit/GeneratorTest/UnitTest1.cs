//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis;
//using Xunit;
//using MvvmToolkit.Core.Attributes;

//namespace GeneratorTest
//{
//    public partial class PropertyGenerator
//    {
//        [Property]
//        private string _name;
//        [Fact]
//        public Task GeneratesEnumExtensionsCorrectly()
//        {
//            // �׽�Ʈ �� �ҽ��ڵ�
//            var source = @"
//namespace GeneratorTest;
//public partial class PropertyGenerator
//{
//[Property]
//private string _name;
//}";

//            // �ҽ� �ڵ带 ����̿� �����ϰ� ������ �׽�Ʈ ���
//            return TestHelper.Verify(source);
//        }
//    }    
     
//}
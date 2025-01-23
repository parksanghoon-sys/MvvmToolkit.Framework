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
//            // 테스트 할 소스코드
//            var source = @"
//namespace GeneratorTest;
//public partial class PropertyGenerator
//{
//[Property]
//private string _name;
//}";

//            // 소스 코드를 도우미에 전달하고 스냅샷 테스트 출력
//            return TestHelper.Verify(source);
//        }
//    }    
     
//}
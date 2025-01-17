using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using MvvmToolkit.Core.CodeGenerators;
using VerifyXunit;


namespace GeneratorTest
{
    public static class TestHelper
    {
        public static Task Verify(string source)
        {
            // 제공된 문자열을 C# 구문 트리로 구문 분석
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // 구문 트리에 대한 Roslyn 컴파일 생성
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "GeneratorTest",
                syntaxTrees: new[] { syntaxTree });


            // EnumGenerator 증분 소스 생성기의 인스턴스 생성
            var generator = new PropertyCodeGen();

            // GeneratorDriver는 컴파일에 대해 생성기를 실행하는데 사용됨
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // 소스 생성기를 실행!
            driver = driver.RunGenerators(compilation);

            // 소스 생성기 출력을 스냅샷 테스트하려면 Verifier를 사용!
            return Verifier.Verify(driver);
        }
    }
}


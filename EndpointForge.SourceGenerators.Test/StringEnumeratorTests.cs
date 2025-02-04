using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace EndpointForge.SourceGenerators.Test;

public class StringEnumeratorSourceGeneratorTests
{
    [Fact]
    public async Task GeneratesExpectedMethods()
    {
        const string source = @"
        namespace TestNamespace 
        {
            public static partial class TestClass 
            {
                public static readonly string ValueA = ""A"";
                public static readonly string ValueB = ""B"";
            }
        }";

        const string expectedGeneratedCode = @"
namespace TestNamespace 
{
    public static partial class TestClass 
    {
        public static bool IsValueA(string type) => Equals(ValueA, type);
        public static bool IsValueB(string type) => Equals(ValueB, type);
        public static bool IsMember(ReadOnlySpan<char> value) => value switch {
            _ when value.SequenceEqual(ValueA) => true,
            _ when value.SequenceEqual(ValueB) => true,
            _ => false
        };
        public static bool Equals(string typeA, string typeB) 
        {
            return ReferenceEquals(typeA, typeB) || StringComparer.OrdinalIgnoreCase.Equals(typeA, typeB);
        }
    }
}";

        var test = new CSharpSourceGeneratorTest<StringEnumeratorSourceGenerator, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source },
                GeneratedSources =
                {
                    (typeof(StringEnumeratorSourceGenerator), "TestClass_Generated.cs", expectedGeneratedCode)
                }
            }
        };

        await test.RunAsync();
    }

    private static string NormalizeWhitespace(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(SourceText.From(code));
        var root = tree.GetRoot();
        var workspace = new AdhocWorkspace();
        return Formatter.Format(root, workspace).ToFullString();
    }
}
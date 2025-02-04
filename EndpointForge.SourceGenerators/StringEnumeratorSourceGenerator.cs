using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using EndpointForge.Attributes;

namespace EndpointForge.SourceGenerators
{
    [Generator]
    public class StringEnumeratorSourceGenerator: ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            foreach (var classDeclaration in receiver.Candidates)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                if (!(semanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol classSymbol) ||
                  !classSymbol.IsStatic ||
                  !classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                    continue;

                if (classSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == nameof(StringEnumeratorAttribute) || a.AttributeClass?.ToDisplayString() == "EndpointForge.Attributes.StringEnumeratorAttribute"))
                {

                    var fields = classSymbol.GetMembers().OfType<IFieldSymbol>()
                      .Where(f => f.IsStatic && f.IsReadOnly && f.Type.SpecialType == SpecialType.System_String)
                      .ToList();

                    if (!fields.Any()) continue;

                    var sb = new StringBuilder();
                    var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
                    var className = classSymbol.Name;

                    sb.AppendLine($"namespace {namespaceName} {{");
                    sb.AppendLine($"    public static partial class {className} {{");

                    foreach (var field in fields)
                    {
                        var fieldName = field.Name;
                        sb.AppendLine($"        public static bool Is{fieldName}(string type) => Equals({fieldName}, type);");
                    }

                    sb.AppendLine("        public static bool IsMember(ReadOnlySpan<char> value) => value switch {");
                    foreach (var field in fields)
                    {
                        sb.AppendLine($"            _ when value.SequenceEqual({field.Name}) => true,");
                    }
                    sb.AppendLine("            _ => false");
                    sb.AppendLine("        };");

                    sb.AppendLine("        public static bool Equals(string typeA, string typeB) {");
                    sb.AppendLine("            return ReferenceEquals(typeA, typeB) || StringComparer.OrdinalIgnoreCase.Equals(typeA, typeB);");
                    sb.AppendLine("        }");

                    sb.AppendLine("    }");
                    sb.AppendLine("}");

                    context.AddSource($"{className}_Generated.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
                }
            }
        }

        private class SyntaxReceiver: ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
                    classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) &&
                    classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                {
                    Candidates.Add(classDeclaration);
                }
            }
        }
    }
}
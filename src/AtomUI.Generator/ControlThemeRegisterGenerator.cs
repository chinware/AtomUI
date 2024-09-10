using AtomUI.Generator.ControlTheme;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

[Generator]
public class ControlThemeRegisterGenerator : IIncrementalGenerator
{
    public const string ControlDesignTokenAttribute = "AtomUI.Theme.Styling.ControlThemeProviderAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var controlThemesProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(ControlDesignTokenAttribute,
            (node, token) => true,
            (context, token) =>
            {
                if (context.TargetNode is ClassDeclarationSyntax classDeclaration)
                {
                    var ns = string.Empty;
                    if (classDeclaration.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDecl)
                    {
                        ns = fileScopedNamespaceDecl.Name.ToString();
                    }
                    else if (classDeclaration.Parent is NamespaceDeclarationSyntax namespaceDecl)
                    {
                        ns = namespaceDecl.Name.ToString();
                    }

                    if (!string.IsNullOrEmpty(ns))
                    {
                        return $"{ns}.{classDeclaration.Identifier.Text}";
                    }

                    return classDeclaration.Identifier.Text;
                }

                return string.Empty;
            }).Collect();

        initContext.RegisterImplementationSourceOutput(controlThemesProvider, (context, infos) =>
        {
            if (infos.Length > 0)
            {
                var classWriter = new ControlThemeRegisterClassSourceWriter(context, infos);
                classWriter.Write();
            }
        });
    }
}
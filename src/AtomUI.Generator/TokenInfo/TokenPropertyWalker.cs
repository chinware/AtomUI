using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class TokenPropertyWalker : CSharpSyntaxWalker
{
   public HashSet<string> TokenNames { get; private set; }
   public string? TokenResourceNamespace { get; set; }
   private SemanticModel _semanticModel;
   public const string AbstractDesignTokenName = "AbstractDesignToken";
   
   public TokenPropertyWalker(SemanticModel semanticModel)
   {
      TokenNames = new HashSet<string>();
      _semanticModel = semanticModel;
   }
   
   public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
   {
      // 判断属性是不是继承自 Token 接口
      var propertyType = _semanticModel.GetTypeInfo(node.Type).Type;
      var normalToken = true;
      while (propertyType is not null) {
         var name = propertyType;
         if (propertyType.Name == AbstractDesignTokenName) {
            normalToken = false;
            break;
         }

         propertyType = propertyType.BaseType;
      }

      if (normalToken) {
         TokenNames.Add(node.Identifier.Text);
      }

   }

   public override void VisitClassDeclaration(ClassDeclarationSyntax node)
   {
      base.VisitClassDeclaration(node);
      var classDeclaredSymbol = _semanticModel.GetDeclaredSymbol(node);
      if (classDeclaredSymbol is not null) {
         foreach (var attribute in classDeclaredSymbol.GetAttributes()) {
            if (attribute.ConstructorArguments.Any() && attribute.ConstructorArguments[0].Value is string ns) {
               TokenResourceNamespace = ns;
            }
         }
      }
   }
}
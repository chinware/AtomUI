using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class ControlTokenPropertyWalker : CSharpSyntaxWalker
{
   public ControlTokenInfo ControlTokenInfo { get; private set; }

   public ControlTokenPropertyWalker()
   {
      ControlTokenInfo = new ControlTokenInfo();
   }
   
   public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
   {
      ControlTokenInfo.Tokens.Add(node.Identifier.Text);
   }

   public override void VisitClassDeclaration(ClassDeclarationSyntax node)
   {
      ControlTokenInfo.ControlName = node.Identifier.Text;
      base.VisitClassDeclaration(node);
   }
}
using System.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace AtomUI.Theme;

public abstract class TokenResourceExtension<TTokenKind> : MarkupExtension
    where TTokenKind : Enum
{
    public TTokenKind? Kind { get; set; }
    
    public TokenResourceExtension()
    {
    }

    public TokenResourceExtension(TTokenKind kind)
    {
        Kind = kind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Debug.Assert(Kind != null);
        return new DynamicResourceExtension(Kind);
    }
}


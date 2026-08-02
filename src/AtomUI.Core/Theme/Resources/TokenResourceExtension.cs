using System.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace AtomUI.Theme.Resources;

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
        if (Kind is not { } kind)
        {
            throw new InvalidOperationException("A Token resource key is required.");
        }
        return new DynamicResourceExtension(GetResourceKey(kind));
    }

    protected virtual object GetResourceKey(TTokenKind kind) => kind;
}

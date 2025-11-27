using System.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconProvider<TIconKind> : MarkupExtension
    where TIconKind : Enum
{
    public TIconKind? Kind { get; set; }
    
    public IBrush? StrokeBrush { get; set; }
    public IBrush? FillBrush { get; set; }
    public IBrush? SecondaryStrokeBrush { get; set; }
    public IBrush? SecondaryFillBrush { get; set; }

    public double Width { get; set; } = double.NaN;
    public double Height { get; set; } = double.NaN;
    public IconAnimation? Animation { get; set; }

    public IconProvider()
    {
    }

    public IconProvider(TIconKind kind)
    {
        Kind = kind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Debug.Assert(Kind != null);
        var icon = GetIcon(Kind);

        if (Animation != null)
        {
            icon.SetCurrentValue(Icon.LoadingAnimationProperty, Animation);
        }
        
        if (StrokeBrush != null)
        {
            icon.StrokeBrush = StrokeBrush;
        }
        
        if (FillBrush != null)
        {
            icon.FillBrush = FillBrush;
        }
        
        if (SecondaryFillBrush != null)
        {
            icon.SecondaryFillBrush = SecondaryFillBrush;
        }
        
        if (SecondaryStrokeBrush != null)
        {
            icon.SecondaryStrokeBrush = SecondaryStrokeBrush;
        }
        
        if (!double.IsNaN(Width))
        {
            icon.Width = Width;
        }

        if (!double.IsNaN(Height))
        {
            icon.Height = Height;
        }

        return icon;
    }

    protected abstract Icon GetIcon(TIconKind kind);
}
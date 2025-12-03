using System.Diagnostics;
using System.Linq.Expressions;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconProvider<TIconKind> : MarkupExtension
    where TIconKind : Enum
{
    private TIconKind? _kind;
    
    public TIconKind? Kind
    {
        get => _kind;
        set
        {
            if (!EqualityComparer<TIconKind?>.Default.Equals(_kind, value))
            {
                _kind = value;
                NotifyKindChanged();
            }
        }
    }
    
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
    
    protected virtual void NotifyKindChanged()
    {
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

    protected virtual Icon GetIcon(TIconKind kind)
    {
        try
        {
            var enumType = typeof(TIconKind);
            var creator = IconProviderCache.GetOrAddCreator(
                enumType,
                kind,
                value => GetTypeForKind((TIconKind)value),
                CreateFactory);
            
            var icon = creator();
            Debug.Assert(icon != null);
            return icon;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Create icon {kind} failed", ex);
        }
    }
    
    protected abstract Type GetTypeForKind(TIconKind kind);
    
    protected virtual Func<Icon> CreateFactory(Type type)
    {
        try
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"No parameterless constructor found for {type.Name}");
            }
            
            var newExpr = Expression.New(constructor);
            var lambda  = Expression.Lambda<Func<Icon>>(newExpr);
            return lambda.Compile();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create expression tree for {type.Name}: {ex.Message}");
            return () => (Icon)Activator.CreateInstance(type)!;
        }
    }
    
    public static void ClearCache()
    {
        IconProviderCache.ClearCache(typeof(TIconKind));
    }
    
    public static void ClearAllCache()
    {
        IconProviderCache.ClearAllCache();
    }
}
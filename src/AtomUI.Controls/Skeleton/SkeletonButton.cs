using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls;

public enum SkeletonButtonShape
{
    Square,
    Round,
    Circle
}

public class SkeletonButton : SkeletonElement,
                              IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<SkeletonButtonShape> ShapeProperty =
        AvaloniaProperty.Register<SkeletonButton, SkeletonButtonShape>(nameof(Shape));
    
    public SkeletonButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion

    public SkeletonButton()
    {
        this.RegisterResources();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeightProperty || change.Property == IsBlockProperty || change.Property == ShapeProperty)
        {
            ConfigureWidth();
        }
        if (change.Property == ShapeProperty || change.Property == HeightProperty)
        {
            ConfigureShape();
        }
    }

    private void ConfigureShape()
    {
        if (Shape == SkeletonButtonShape.Round)
        {
            SetValue(CornerRadiusProperty, new CornerRadius(Height / 2), BindingPriority.Template);
        }
        else if (Shape == SkeletonButtonShape.Circle)
        {
            SetValue(WidthProperty, Height, BindingPriority.Template);
            SetValue(MinWidthProperty, Height, BindingPriority.Template);
            SetValue(CornerRadiusProperty, new CornerRadius(Height), BindingPriority.Template);
        }
    }

    private void ConfigureWidth()
    {
        if (!double.IsNaN(Height) && !IsBlock)
        {
            if (Shape != SkeletonButtonShape.Circle)
            {
                SetValue(WidthProperty, Height * 2, BindingPriority.Template);
                SetValue(MinWidthProperty, Height * 2, BindingPriority.Template);
            }
        }
    }
}
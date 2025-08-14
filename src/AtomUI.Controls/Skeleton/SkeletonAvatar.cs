using System.Diagnostics;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public class SkeletonAvatar : AbstractSkeleton, IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<SkeletonAvatar, AvatarShape>(nameof(AvatarShape), AvatarShape.Circle);
    
    public static readonly StyledProperty<AvatarSizeType> SizeTypeProperty =
        Avatar.SizeTypeProperty.AddOwner<SkeletonAvatar>();
    
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<SkeletonAvatar, double>(nameof(Size), Double.NaN);
    
    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }
    
    public AvatarSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion
    
    private AvatarSizeType? _originSizeType;
    
    public SkeletonAvatar()
    {
        this.RegisterResources();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeProperty)
        {
            if (!double.IsNaN(Size))
            {
                _originSizeType = SizeType;
                SizeType        = AvatarSizeType.Custom;
            }
            else
            {
                if (_originSizeType.HasValue)
                {
                    SizeType = _originSizeType.Value;
                }
            }
        }
        else if (change.Property == SizeTypeProperty)
        {
            ConfigureSize();
        }
        else if (change.Property == ShapeProperty)
        {
            ConfigureShape();
        }
    }
    
    private void ConfigureShape()
    {
        if (Shape == AvatarShape.Circle)
        {
            SetValue(CornerRadiusProperty, new CornerRadius(Width / 2), BindingPriority.Template);
        }
    }
    
    private void ConfigureSize()
    {
        if (SizeType == AvatarSizeType.Custom)
        {
            Debug.Assert(!double.IsNaN(Size));
            // 不影响模板设置
            SetValue(WidthProperty, Size, BindingPriority.Template);
            SetValue(HeightProperty, Size, BindingPriority.Template);
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureShape();
        ConfigureSize();
        if (!IsFollowMode)
        {
            if (IsActive)
            {
                StartActiveAnimation();
            }
        }
    }
}
using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class IconTemplatePresenter : Control, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IconTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<IconTemplatePresenter, IconTemplate?>(nameof(IconTemplate));
    
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<IconTemplatePresenter, IBrush?>(nameof(IconBrush));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconTemplatePresenter>();
    
    [Content]
    public IconTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }
    
    public IBrush? IconBrush
    {
        get => GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    private CompositeDisposable? _disposables;
    private PathIcon? _childIcon;
    private PathIcon? _configuredIcon;
    
    static IconTemplatePresenter()
    {
        AffectsMeasure<IconTemplatePresenter>(IconTemplateProperty);
        AffectsRender<IconTemplatePresenter>(IconBrushProperty);
        IconTemplateProperty.Changed.AddClassHandler<IconTemplatePresenter>((x, e) => x.HandleIconTemplateChanged(e));
    }

    private void HandleIconTemplateChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldIconTemplate = (IconTemplate?)change.OldValue;
        var newIconTemplate = (IconTemplate?)change.NewValue;
        if (oldIconTemplate != null)
        {
            ClearIcon();
        }

        if (newIconTemplate != null)
        {
            var pathIcon = newIconTemplate.Build();
            if (pathIcon != null)
            {
                AddIcon(pathIcon);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_childIcon != null && (_configuredIcon != _childIcon || _disposables == null))
        {
            ConfigureIcon(_childIcon);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _disposables?.Dispose();
        _disposables = null;
        _configuredIcon = null;
    }

    private void AddIcon(PathIcon pathIcon)
    {
        ClearIcon();
        _childIcon = pathIcon;
        ConfigureIcon(pathIcon);
        ((ISetLogicalParent)pathIcon).SetParent(null);
        pathIcon.SetVisualParent(null);
        VisualChildren.Add(pathIcon);
        LogicalChildren.Add(pathIcon);
    }

    private void ConfigureIcon(PathIcon pathIcon)
    {
        _disposables?.Dispose();
        _configuredIcon = pathIcon;
        _disposables = new CompositeDisposable(5);
        _disposables.Add(BindUtils.RelayBind(this, WidthProperty, pathIcon, WidthProperty));
        _disposables.Add(BindUtils.RelayBind(this, HeightProperty, pathIcon, HeightProperty));
        if (pathIcon is Icon icon)
        {
            _disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, Icon.IsMotionEnabledProperty));
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, icon, Icon.StrokeBrushProperty, BindingMode.Default, BindingPriority.Template));
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, icon, Icon.FillBrushProperty, BindingMode.Default, BindingPriority.Template));
        }
        else
        {
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, pathIcon, PathIcon.ForegroundProperty, BindingMode.Default, BindingPriority.Template));
        }
    }

    private void ClearIcon()
    {
        _disposables?.Dispose();
        _disposables = null;
        _configuredIcon = null;

        if (_childIcon == null)
        {
            return;
        }

        ((ISetLogicalParent)_childIcon).SetParent(null);
        _childIcon.SetVisualParent(null);
        LogicalChildren.Remove(_childIcon);
        VisualChildren.Remove(_childIcon);
        _childIcon = null;
    }
}

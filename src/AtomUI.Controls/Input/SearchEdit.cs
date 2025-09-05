using AtomUI.Controls.Themes;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public enum SearchEditButtonStyle
{
    Default,
    Primary
}

public class SearchEdit : LineEdit
{
    #region 公共属性定义

    public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
        AvaloniaProperty.Register<SearchEdit, SearchEditButtonStyle>(nameof(SearchButtonStyle));

    public static readonly StyledProperty<string> SearchButtonTextProperty =
        AvaloniaProperty.Register<SearchEdit, string>(nameof(SearchButtonText));

    public SearchEditButtonStyle SearchButtonStyle
    {
        get => GetValue(SearchButtonStyleProperty);
        set => SetValue(SearchButtonStyleProperty, value);
    }

    public object? SearchButtonText
    {
        get => GetValue(SearchButtonTextProperty);
        set => SetValue(SearchButtonTextProperty, value);
    }

    #endregion
    
    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> SearchButtonClickEvent =
        RoutedEvent.Register<SearchEdit, RoutedEventArgs>(nameof(SearchButtonClick), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? SearchButtonClick
    {
        add => AddHandler(SearchButtonClickEvent, value);
        remove => RemoveHandler(SearchButtonClickEvent, value);
    }

    #endregion
    
    private IDisposable? _borderThicknessDisposable;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var decoratedBox = e.NameScope.Find<SearchEditDecoratedBox>(TextBoxThemeConstants.DecoratedBoxPart);
        if (decoratedBox != null)
        {
            decoratedBox.OwningSearchEdit = this;
        }
    }

    internal void NotifySearchButtonClicked()
    {
        var eventArgs = new RoutedEventArgs(SearchButtonClickEvent, this);
        RaiseEvent(eventArgs);
    }
}
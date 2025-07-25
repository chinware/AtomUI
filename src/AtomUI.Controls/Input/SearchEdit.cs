using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    internal void OnSearchButtonClick(RoutedEventArgs e)
    {
        var eventArgs = new RoutedEventArgs(SearchButtonClickEvent, this);
        RaiseEvent(eventArgs);
    }
}
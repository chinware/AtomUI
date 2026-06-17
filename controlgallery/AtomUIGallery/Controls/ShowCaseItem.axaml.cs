using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUIGallery.Controls;

public enum ShowCaseItemSpan
{
    Auto,
    Full
}

public class ShowCaseItem : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ShowCaseItem, string>(nameof(Title));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<ShowCaseItem, string>(nameof(Description));

    public static readonly StyledProperty<bool> IsOccupyEntireRowProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsOccupyEntireRow));

    public static readonly StyledProperty<ShowCaseItemSpan> SpanProperty =
        AvaloniaProperty.Register<ShowCaseItem, ShowCaseItemSpan>(nameof(Span));

    internal static readonly StyledProperty<bool> IsFakeProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsFake), false);

    public static readonly StyledProperty<bool> IsDeferredContentEnabledProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsDeferredContentEnabled), false);

    public static readonly StyledProperty<IDataTemplate?> DeferredContentTemplateProperty =
        AvaloniaProperty.Register<ShowCaseItem, IDataTemplate?>(nameof(DeferredContentTemplate));

    public static readonly StyledProperty<object?> DeferredContentProperty =
        AvaloniaProperty.Register<ShowCaseItem, object?>(nameof(DeferredContent));

    public static readonly StyledProperty<double> DeferredPlaceholderHeightProperty =
        AvaloniaProperty.Register<ShowCaseItem, double>(nameof(DeferredPlaceholderHeight), 160);

    public static readonly StyledProperty<string?> BadgeTextProperty =
        AvaloniaProperty.Register<ShowCaseItem, string?>(nameof(BadgeText));

    public static readonly StyledProperty<string?> BadgeColorProperty =
        AvaloniaProperty.Register<ShowCaseItem, string?>(nameof(BadgeColor), "blue");

    public static readonly StyledProperty<bool> IsDeferredContentMaterializedProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsDeferredContentMaterialized), false);

    internal static readonly StyledProperty<bool> IsDeferredPlaceholderVisibleProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsDeferredPlaceholderVisible), false);

    internal static readonly StyledProperty<bool> IsBadgeVisibleProperty =
        AvaloniaProperty.Register<ShowCaseItem, bool>(nameof(IsBadgeVisible), false);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsOccupyEntireRow
    {
        get => GetValue(IsOccupyEntireRowProperty);
        set => SetValue(IsOccupyEntireRowProperty, value);
    }

    public ShowCaseItemSpan Span
    {
        get => GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    public bool IsFake
    {
        get => GetValue(IsFakeProperty);
        set => SetValue(IsFakeProperty, value);
    }

    public bool IsDeferredContentEnabled
    {
        get => GetValue(IsDeferredContentEnabledProperty);
        set => SetValue(IsDeferredContentEnabledProperty, value);
    }

    public IDataTemplate? DeferredContentTemplate
    {
        get => GetValue(DeferredContentTemplateProperty);
        set => SetValue(DeferredContentTemplateProperty, value);
    }

    public object? DeferredContent
    {
        get => GetValue(DeferredContentProperty);
        set => SetValue(DeferredContentProperty, value);
    }

    public double DeferredPlaceholderHeight
    {
        get => GetValue(DeferredPlaceholderHeightProperty);
        set => SetValue(DeferredPlaceholderHeightProperty, value);
    }

    public string? BadgeText
    {
        get => GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public string? BadgeColor
    {
        get => GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    public bool IsDeferredContentMaterialized
    {
        get => GetValue(IsDeferredContentMaterializedProperty);
        private set => SetValue(IsDeferredContentMaterializedProperty, value);
    }

    internal bool IsDeferredPlaceholderVisible
    {
        get => GetValue(IsDeferredPlaceholderVisibleProperty);
        set => SetValue(IsDeferredPlaceholderVisibleProperty, value);
    }

    internal bool IsBadgeVisible
    {
        get => GetValue(IsBadgeVisibleProperty);
        set => SetValue(IsBadgeVisibleProperty, value);
    }

    public void MaterializeDeferredContent()
    {
        if (IsDeferredContentMaterialized ||
            !IsDeferredContentEnabled ||
            DeferredContentTemplate is null)
        {
            return;
        }

        var data    = DeferredContent ?? DataContext;
        var content = DeferredContentTemplate.Build(data);
        if (content is StyledElement styledElement)
        {
            styledElement.DataContext = data;
        }

        SetCurrentValue(ContentProperty, content);
        IsDeferredContentMaterialized = true;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (IsDeferredContentMaterialized &&
            DeferredContent is null &&
            Content is StyledElement styledElement)
        {
            styledElement.DataContext = DataContext;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsDeferredContentEnabledProperty ||
            change.Property == DeferredContentTemplateProperty ||
            change.Property == IsDeferredContentMaterializedProperty)
        {
            MaterializeDeferredContentIfDiagnosticsDisabled();
            UpdateDeferredPlaceholderVisibility();
        }

        if (change.Property == BadgeTextProperty)
        {
            UpdateBadgeVisibility();
        }
    }

    private void MaterializeDeferredContentIfDiagnosticsDisabled()
    {
        if (GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled)
        {
            MaterializeDeferredContent();
        }
    }

    private void UpdateDeferredPlaceholderVisibility()
    {
        IsDeferredPlaceholderVisible =
            IsDeferredContentEnabled &&
            !GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled &&
            DeferredContentTemplate is not null &&
            !IsDeferredContentMaterialized;
    }

    private void UpdateBadgeVisibility()
    {
        IsBadgeVisible = !string.IsNullOrWhiteSpace(BadgeText);
    }
}

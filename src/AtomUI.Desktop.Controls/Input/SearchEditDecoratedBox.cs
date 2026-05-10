using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class SearchEditDecoratedBox : AddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
        SearchEdit.SearchButtonStyleProperty.AddOwner<SearchEditDecoratedBox>();

    public static readonly StyledProperty<string> SearchButtonTextProperty =
        SearchEdit.SearchButtonTextProperty.AddOwner<SearchEditDecoratedBox>();

    public static readonly StyledProperty<bool> IsSearchButtonLoadingProperty = AvaloniaProperty.Register<SearchEditDecoratedBox, bool>(
        nameof(IsSearchButtonLoading));
    
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
    
    public bool IsSearchButtonLoading
    {
        get => GetValue(IsSearchButtonLoadingProperty);
        set => SetValue(IsSearchButtonLoadingProperty, value);
    }

    #endregion
    
    private Button? _searchButton;
    internal SearchEdit? OwningSearchEdit { get; set; }

    protected override void NotifyAddOnBorderInfoCalculated()
    {
        RightAddOnBorderThickness = BorderThickness;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_searchButton != null)
        {
            _searchButton.Click -= HandleSearchButtonClick;
        }
        
        _searchButton = e.NameScope.Find<Button>("PART_RightAddOn");

        if (_searchButton != null)
        {
            _searchButton.Click += HandleSearchButtonClick;
        }
    }

    private void HandleSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        OwningSearchEdit?.NotifySearchButtonClicked();
    }
}
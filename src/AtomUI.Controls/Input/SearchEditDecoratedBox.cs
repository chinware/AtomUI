using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class SearchEditDecoratedBox : SimpleAddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
        SearchEdit.SearchButtonStyleProperty.AddOwner<SearchEditDecoratedBox>();

    public static readonly StyledProperty<string> SearchButtonTextProperty =
        SearchEdit.SearchButtonTextProperty.AddOwner<SearchEditDecoratedBox>();

    public static readonly StyledProperty<bool> SearchButtonLoadingProperty = AvaloniaProperty.Register<SearchEditDecoratedBox, bool>(
        nameof(SearchButtonLoading));
    
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
    
    public bool SearchButtonLoading
    {
        get => GetValue(SearchButtonLoadingProperty);
        set => SetValue(SearchButtonLoadingProperty, value);
    }

    #endregion

    private Rect? _originRect;
    private Button? _searchButton;
    internal SearchEdit? OwningSearchEdit { get; set; }
    private CompositeDisposable? _bindingDisposables;

    protected override void NotifyAddOnBorderInfoCalculated()
    {
        RightAddOnBorderThickness = BorderThickness;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_originRect is null)
        {
            _originRect = _rightAddOn?.Bounds;
        }

        if (_rightAddOn is not null && _originRect.HasValue)
        {
            _rightAddOn.Arrange(_originRect.Value.Inflate(new Thickness(BorderThickness.Left, 0, 0, 0)));
        }

        return size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _searchButton = e.NameScope.Find<Button>(AddOnDecoratedBoxThemeConstants.RightAddOnPart);

        if (_searchButton is not null)
        {
            _bindingDisposables?.Dispose();
            _bindingDisposables = new CompositeDisposable(2);
            _bindingDisposables.Add(BindUtils.RelayBind(this, RightAddOnBorderThicknessProperty, _searchButton, BorderThicknessProperty));
            _bindingDisposables.Add(BindUtils.RelayBind(this, RightAddOnCornerRadiusProperty, _searchButton, CornerRadiusProperty));
            _searchButton.Click += HandleSearchButtonClick;
        }
    }

    private void HandleSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        OwningSearchEdit?.NotifySearchButtonClicked();
    }
}
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class SearchEditDecoratedBox : AddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
        SearchEdit.SearchButtonStyleProperty.AddOwner<SearchEditDecoratedBox>();

    public static readonly StyledProperty<string> SearchButtonTextProperty =
        SearchEdit.SearchButtonTextProperty.AddOwner<SearchEditDecoratedBox>();

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

    private Rect? _originRect;
    private Button? _searchButton;
    internal SearchEdit? OwningSearchEdit { get; set; }

    protected override void NotifyAddOnBorderInfoCalculated()
    {
        RightAddOnBorderThickness = BorderThickness;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_originRect is null)
        {
            _originRect = _rightAddOnPresenter?.Bounds;
        }

        if (_rightAddOnPresenter is not null && _originRect.HasValue)
        {
            _rightAddOnPresenter.Arrange(_originRect.Value.Inflate(new Thickness(BorderThickness.Left, 0, 0, 0)));
        }

        return size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _searchButton = e.NameScope.Find<Button>(AddOnDecoratedBoxThemeConstants.RightAddOnPart);

        if (_searchButton is not null)
        {
            BindUtils.RelayBind(this, RightAddOnBorderThicknessProperty, _searchButton, BorderThicknessProperty);
            BindUtils.RelayBind(this, RightAddOnCornerRadiusProperty, _searchButton, CornerRadiusProperty);
            _searchButton.Click += HandleSearchButtonClick;
        }
    }

    private void HandleSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        OwningSearchEdit?.NotifySearchButtonClicked();
    }
}
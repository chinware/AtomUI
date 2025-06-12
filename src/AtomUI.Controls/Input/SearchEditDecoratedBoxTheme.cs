using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SearchEditDecoratedBoxTheme : AddOnDecoratedBoxTheme
{
    public SearchEditDecoratedBoxTheme() : base(typeof(SearchEditDecoratedBox))
    {
    }

    protected override void BuildRightAddOn(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
    {
        var searchIcon = AntDesignIconPackage.SearchOutlined();

        var searchButton = new Button
        {
            Name             = RightAddOnPart,
            Focusable        = false,
            Icon             = searchIcon,
            BackgroundSizing = BackgroundSizing.OuterBorderEdge,
        };

        searchButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(searchButton, Button.ContentProperty, SearchEditDecoratedBox.SearchButtonTextProperty);
        CreateTemplateParentBinding(searchButton, Button.SizeTypeProperty, AddOnDecoratedBox.SizeTypeProperty);
        CreateTemplateParentBinding(searchButton, TemplatedControl.BorderThicknessProperty,
            AddOnDecoratedBox.RightAddOnBorderThicknessProperty,
            BindingPriority.LocalValue);
        CreateTemplateParentBinding(searchButton, TemplatedControl.CornerRadiusProperty,
            AddOnDecoratedBox.RightAddOnCornerRadiusProperty,
            BindingPriority.LocalValue);

        layout.Children.Add(searchButton);
        Grid.SetColumn(searchButton, 2);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();

        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxContentPart));
        decoratorStyle.Add(Visual.ZIndexProperty, NormalZIndex);
        Add(decoratorStyle);

        var defaultButtonTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(
            SearchEditDecoratedBox.SearchButtonStyleProperty,
            SearchEditButtonStyle.Default));

        var decoratorHoverOrFocusStyle = new Style(selector => Selectors.Or(
            selector.Nesting().Template().Name(InnerBoxContentPart).Class(StdPseudoClass.FocusWithIn),
            selector.Nesting().Template().Name(InnerBoxContentPart).Class(StdPseudoClass.PointerOver)));
        decoratorHoverOrFocusStyle.Add(Visual.ZIndexProperty, ActivatedZIndex);
        defaultButtonTypeStyle.Add(decoratorHoverOrFocusStyle);

        {
            var searchButtonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
            searchButtonStyle.Add(Visual.ZIndexProperty, NormalZIndex);
            defaultButtonTypeStyle.Add(searchButtonStyle);
        }

        var searchButtonStyleHoverOrFocusStyle = new Style(selector => Selectors.Or(
            selector.Nesting().Template().Name(RightAddOnPart).Class(StdPseudoClass.Pressed),
            selector.Nesting().Template().Name(RightAddOnPart).Class(StdPseudoClass.PointerOver)));
        searchButtonStyleHoverOrFocusStyle.Add(Visual.ZIndexProperty, ActivatedZIndex);
        defaultButtonTypeStyle.Add(searchButtonStyleHoverOrFocusStyle);
        Add(defaultButtonTypeStyle);

        var primaryButtonTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(
            SearchEditDecoratedBox.SearchButtonStyleProperty,
            SearchEditButtonStyle.Primary));
        {
            var searchButtonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
            searchButtonStyle.Add(Visual.ZIndexProperty, ActivatedZIndex);
            primaryButtonTypeStyle.Add(searchButtonStyle);
        }
        Add(primaryButtonTypeStyle);

        // Icon button
        var iconSearchButtonStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(SearchEdit.SearchButtonStyleProperty, SearchEditButtonStyle.Default));
        {
            var buttonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
            buttonStyle.Add(Button.IsIconVisibleProperty, true);
            buttonStyle.Add(Button.ButtonTypeProperty, ButtonType.Default);
            iconSearchButtonStyle.Add(buttonStyle);
        }
        Add(iconSearchButtonStyle);

        // primary button
        var primarySearchButtonStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(SearchEdit.SearchButtonStyleProperty, SearchEditButtonStyle.Primary));
        {
            var buttonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
            buttonStyle.Add(Button.IsIconVisibleProperty, false);
            buttonStyle.Add(Button.ButtonTypeProperty, ButtonType.Primary);
            primarySearchButtonStyle.Add(buttonStyle);
        }
        Add(primarySearchButtonStyle);
    }
}
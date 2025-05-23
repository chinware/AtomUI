using AtomUI.Controls.Utils;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ComboBoxTheme : BaseControlTheme
{
    public const string DecoratedBoxPart = "PART_DecoratedBox";
    public const string SpinnerInnerBoxPart = "PART_SpinnerInnerBox";
    public const string OpenIndicatorButtonPart = "PART_OpenIndicatorButton";
    public const string SpinnerHandleDecoratorPart = "PART_SpinnerHandleDecorator";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string PopupPart = "PART_Popup";
    public const string PopupFramePart = "PART_PopupFrame";
    public const string PlaceholderTextPart = "PART_PlaceholderText";
    public const string SelectedContentPresenterPart = "PART_SelectedContentPresenter";

    public ComboBoxTheme() : base(typeof(ComboBox))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ComboBox>((comboBox, scope) =>
        {
            var panel        = new Panel();
            var decoratedBox = BuildSpinnerDecoratedBox(comboBox, scope);
            var innerBox     = BuildSpinnerContent(comboBox, scope);
            decoratedBox.Content = innerBox;
            innerBox.RegisterInNameScope(scope);
            var popup = BuildPopup(comboBox, scope);

            panel.Children.Add(decoratedBox);
            panel.Children.Add(popup);

            return panel;
        });
    }

    protected virtual AddOnDecoratedBox BuildSpinnerDecoratedBox(ComboBox comboBox, INameScope scope)
    {
        var decoratedBox = new AddOnDecoratedBox
        {
            Name      = DecoratedBoxPart,
            Focusable = true
        };
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty,
            ComboBox.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, ComboBox.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, ComboBox.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, ComboBox.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty, ComboBox.RightAddOnProperty);

        decoratedBox.RegisterInNameScope(scope);
        return decoratedBox;
    }

    protected virtual ComboBoxSpinnerInnerBox BuildSpinnerContent(ComboBox comboBox, INameScope scope)
    {
        var spinnerInnerBox = new ComboBoxSpinnerInnerBox
        {
            Name      = SpinnerInnerBoxPart,
            Focusable = true
        };
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            ComboBox.InnerLeftContentProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.RightAddOnContentProperty,
            ComboBox.InnerRightContentProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            ComboBox.StyleVariantProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.StatusProperty, ComboBox.StatusProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty,
            ComboBox.SizeTypeProperty);

        spinnerInnerBox.RegisterInNameScope(scope);
        var content = BuildComboBoxContent(comboBox, scope);
        spinnerInnerBox.Content = content;

        var spinnerHandleDecorator = new Border
        {
            Name             = SpinnerHandleDecoratorPart,
            BackgroundSizing = BackgroundSizing.InnerBorderEdge,
            ClipToBounds     = true
        };

        spinnerHandleDecorator.RegisterInNameScope(scope);

        var decreaseButtonIcon = AntDesignIconPackage.DownOutlined();
        
        var openButton = new IconButton
        {
            Name                = OpenIndicatorButtonPart,
            Icon                = decreaseButtonIcon,
            VerticalAlignment   = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            BackgroundSizing    = BackgroundSizing.InnerBorderEdge
        };

        openButton.RegisterInNameScope(scope);
        openButton.SetTemplatedParent(comboBox);
        
        spinnerHandleDecorator.Child   = openButton;
        spinnerInnerBox.SpinnerContent = spinnerHandleDecorator;
        
        return spinnerInnerBox;
    }

    private Panel BuildComboBoxContent(ComboBox comboBox, INameScope scope)
    {
        var contentLayout = new Panel();
        var placeholder = new TextBlock()
        {
            Name                = PlaceholderTextPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Center,
            TextTrimming        = TextTrimming.CharacterEllipsis,
            Opacity             = 0.3
        };
        placeholder.SetTemplatedParent(comboBox);

        CreateTemplateParentBinding(placeholder, Visual.IsVisibleProperty, SelectingItemsControl.SelectedItemProperty,
            BindingMode.Default,
            ObjectConverters.IsNull);
        CreateTemplateParentBinding(placeholder, TextBlock.TextProperty,
            Avalonia.Controls.ComboBox.PlaceholderTextProperty);
        contentLayout.Children.Add(placeholder);

        var contentPresenter = new ContentPresenter
        {
            Name                = SelectedContentPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Center,
        };
        
        contentPresenter.SetTemplatedParent(comboBox);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            ComboBox.SelectionBoxItemProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            ComboBox.ItemTemplateProperty);

        contentLayout.Children.Add(contentPresenter);

        return contentLayout;
    }

    private Popup BuildPopup(ComboBox comboBox, INameScope scope)
    {
        var popup = new Popup
        {
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = true,
            Placement                  = PlacementMode.BottomEdgeAlignedLeft
        };
        popup.RegisterInNameScope(scope);

        var border = new Border()
        {
            Name = PopupFramePart
        };
       
        var scrollViewer = new MenuScrollViewer();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;

        popup.Child = border;
        
        CreateTemplateParentBinding(popup, Layoutable.MaxHeightProperty,
            ComboBox.MaxDropDownHeightProperty);
        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            ComboBox.IsDropDownOpenProperty, BindingMode.TwoWay);

        return popup;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildStatusStyle();
        BuildPopupStyle();
        BuildOpenIndicatorStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSizeLG);
        {
            var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
            spinnerInnerBox.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeLG);
            largeStyle.Add(spinnerInnerBox);
        }
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        {
            var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
            spinnerInnerBox.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
            middleStyle.Add(spinnerInnerBox);
        }
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        {
            var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
            spinnerInnerBox.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeSM);
            smallStyle.Add(spinnerInnerBox);
        }
        commonStyle.Add(smallStyle);
        Add(commonStyle);
    }

    private void BuildOpenIndicatorStyle()
    {
        var openIndicatorButton = new Style(selector => selector.Nesting().Template().Name(OpenIndicatorButtonPart));
        openIndicatorButton.Add(IconButton.ActiveIconColorProperty, ButtonSpinnerTokenKey.HandleHoverColor);
        openIndicatorButton.Add(IconButton.SelectedIconColorProperty, SharedTokenKey.ColorPrimaryActive);
        openIndicatorButton.Add(IconButton.DisabledIconColorProperty, SharedTokenKey.ColorTextDisabled);
        openIndicatorButton.Add(IconButton.WidthProperty, ComboBoxTokenKey.OpenIndicatorWidth);
        openIndicatorButton.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        openIndicatorButton.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);

        Add(openIndicatorButton);
    }

    private void BuildPopupStyle()
    {
        var popupStyle = new Style(selector => selector.Nesting().Template().Name(PopupPart));
        popupStyle.Add(Popup.MarginToAnchorProperty, ComboBoxTokenKey.PopupMarginToAnchor);
        popupStyle.Add(Popup.MaskShadowsProperty, ComboBoxTokenKey.PopupBoxShadows);
        Add(popupStyle);
        
        var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
        popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        popupFrameStyle.Add(Border.CornerRadiusProperty, ComboBoxTokenKey.PopupBorderRadius);
        popupFrameStyle.Add(Border.PaddingProperty, ComboBoxTokenKey.PopupContentPadding);
        Add(popupFrameStyle);
    }

    private void BuildStatusStyle()
    {
        var borderlessStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty,
                                              AddOnDecoratedVariant.Borderless));

        {
            var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
            var contentPresenter =
                new Style(selector => selector.Nesting().Template().Name(SelectedContentPresenterPart));
            contentPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorErrorText);
            errorStyle.Add(contentPresenter);
            borderlessStyle.Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
            var contentPresenter =
                new Style(selector => selector.Nesting().Template().Name(SelectedContentPresenterPart));
            contentPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorWarningText);
            warningStyle.Add(contentPresenter);
            borderlessStyle.Add(warningStyle);
        }

        Add(borderlessStyle);

        var filledStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty,
                                              AddOnDecoratedVariant.Filled));

        {
            var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));

            var contentPresenter =
                new Style(selector => selector.Nesting().Template().Name(SelectedContentPresenterPart));
            contentPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorErrorText);
            errorStyle.Add(contentPresenter);
            filledStyle.Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
            var contentPresenter =
                new Style(selector => selector.Nesting().Template().Name(SelectedContentPresenterPart));
            contentPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorWarningText);
            warningStyle.Add(contentPresenter);
            filledStyle.Add(warningStyle);
        }

        Add(filledStyle);
    }
}
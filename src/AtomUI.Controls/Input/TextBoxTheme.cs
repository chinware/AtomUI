﻿using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TextBoxTheme : BaseControlTheme
{
    public const string DecoratedBoxPart = "PART_DecoratedBox";
    public const string TextBoxInnerBoxPart = "PART_TextBoxInnerBox";
    public const string TextPresenterPart = "PART_TextPresenter";
    public const string WatermarkPart = "PART_Watermark";
    public const string ScrollViewerPart = "PART_ScrollViewer";

    public TextBoxTheme(Type targetType) : base(targetType)
    {
    }

    public TextBoxTheme() : base(typeof(TextBox))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TextBox>((textBox, scope) => { return BuildTextBoxStructure(textBox, scope); });
    }

    protected virtual Control BuildTextBoxStructure(TextBox textBox, INameScope scope)
    {
        return BuildTextBoxKernel(textBox, scope);
    }

    protected virtual TextBoxInnerBox BuildTextBoxKernel(TextBox textBox, INameScope scope)
    {
        var editInnerBox = new TextBoxInnerBox(textBox)
        {
            Name   = TextBoxInnerBoxPart,
            Cursor = new Cursor(StandardCursorType.Ibeam)
        };

        editInnerBox.RegisterInNameScope(scope);

        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            TextBox.InnerLeftContentProperty);
        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.RightAddOnContentProperty,
            TextBox.InnerRightContentProperty);
        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty, TextBox.SizeTypeProperty);
        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            TextBox.StyleVariantProperty);
        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.IsClearButtonVisibleProperty,
            TextBox.IsEffectiveShowClearButtonProperty);
        CreateTemplateParentBinding(editInnerBox, TextBoxInnerBox.IsRevealButtonVisibleProperty,
            TextBox.IsEnableRevealButtonProperty);
        CreateTemplateParentBinding(editInnerBox, TextBoxInnerBox.IsRevealButtonCheckedProperty,
            TextBox.RevealPasswordProperty, BindingMode.TwoWay);
        CreateTemplateParentBinding(editInnerBox, AddOnDecoratedInnerBox.StatusProperty, TextBox.StatusProperty);
        CreateTemplateParentBinding(editInnerBox, TextBoxInnerBox.EmbedModeProperty, TextBox.EmbedModeProperty);

        var scrollViewer = new ScrollViewer
        {
            Name      = ScrollViewerPart,
            Focusable = true
        };

        // TODO attach 属性不知道怎么指定 Avalonia 控件所在的名称控件，无法用模板绑定的方式进行绑定
        BindUtils.RelayBind(textBox, ScrollViewer.AllowAutoHideProperty, scrollViewer,
            ScrollViewer.AllowAutoHideProperty);
        BindUtils.RelayBind(textBox, ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer,
            ScrollViewer.HorizontalScrollBarVisibilityProperty);
        BindUtils.RelayBind(textBox, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
            ScrollViewer.VerticalScrollBarVisibilityProperty);
        BindUtils.RelayBind(textBox, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
            ScrollViewer.IsScrollChainingEnabledProperty);

        scrollViewer.RegisterInNameScope(scope);

        var textPresenterLayout = new Panel();

        var watermark = new TextBlock
        {
            Name    = WatermarkPart,
            Opacity = 0.5
        };
        CreateTemplateParentBinding(watermark, Layoutable.HorizontalAlignmentProperty,
            TextBox.HorizontalContentAlignmentProperty);
        CreateTemplateParentBinding(watermark, Layoutable.VerticalAlignmentProperty,
            TextBox.VerticalContentAlignmentProperty);
        CreateTemplateParentBinding(watermark, TextBlock.TextProperty, TextBox.WatermarkProperty);
        CreateTemplateParentBinding(watermark, TextBlock.TextAlignmentProperty,
            TextBox.TextAlignmentProperty);
        CreateTemplateParentBinding(watermark, TextBlock.TextWrappingProperty,
            TextBox.TextWrappingProperty);
        CreateTemplateParentBinding(watermark, Visual.IsVisibleProperty, TextBox.TextProperty,
            BindingMode.Default,
            StringConverters.IsNullOrEmpty);

        watermark.RegisterInNameScope(scope);

        var textPresenter = new TextPresenter
        {
            Name = TextPresenterPart
        };

        CreateTemplateParentBinding(textPresenter, Layoutable.HorizontalAlignmentProperty,
            TextBox.HorizontalContentAlignmentProperty);
        CreateTemplateParentBinding(textPresenter, Layoutable.VerticalAlignmentProperty,
            TextBox.VerticalContentAlignmentProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBlinkIntervalProperty,
            TextBox.CaretBlinkIntervalProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBrushProperty,
            TextBox.CaretBrushProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.CaretIndexProperty,
            TextBox.CaretIndexProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.LineHeightProperty,
            TextBox.LineHeightProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.PasswordCharProperty,
            TextBox.PasswordCharProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.RevealPasswordProperty,
            TextBox.RevealPasswordProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionBrushProperty,
            TextBox.SelectionBrushProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionStartProperty,
            TextBox.SelectionStartProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionEndProperty,
            TextBox.SelectionEndProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionForegroundBrushProperty,
            TextBox.SelectionForegroundBrushProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.TextProperty, TextBox.TextProperty,
            BindingMode.TwoWay);
        CreateTemplateParentBinding(textPresenter, TextPresenter.TextAlignmentProperty,
            TextBox.TextAlignmentProperty);
        CreateTemplateParentBinding(textPresenter, TextPresenter.TextWrappingProperty,
            TextBox.TextWrappingProperty);

        textPresenterLayout.Children.Add(watermark);
        textPresenterLayout.Children.Add(textPresenter);

        textPresenter.RegisterInNameScope(scope);
        scrollViewer.Content = textPresenterLayout;
        editInnerBox.Content = scrollViewer;

        editInnerBox.RegisterInNameScope(scope);

        return editInnerBox;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildFixedStyle();
        BuildStatusStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TextBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TextBox.LineHeightProperty, GlobalTokenResourceKey.FontHeightLG);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TextBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TextBox.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TextBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TextBox.LineHeightProperty, GlobalTokenResourceKey.FontHeightSM);
        commonStyle.Add(smallStyle);

        Add(commonStyle);
    }

    private void BuildFixedStyle()
    {
        this.Add(TextBox.SelectionBrushProperty, GlobalTokenResourceKey.SelectionBackground);
        this.Add(TextBox.SelectionForegroundBrushProperty,
            GlobalTokenResourceKey.SelectionForeground);
        this.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        this.Add(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
    }

    private void BuildStatusStyle()
    {
        var borderlessStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty,
                                              AddOnDecoratedVariant.Borderless));

        {
            var errorStyle        = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
            var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
            scrollViewerStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorErrorText);
            errorStyle.Add(scrollViewerStyle);
            borderlessStyle.Add(errorStyle);
        }

        {
            var warningStyle      = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
            var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
            scrollViewerStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorWarningText);
            warningStyle.Add(scrollViewerStyle);
            borderlessStyle.Add(warningStyle);
        }

        Add(borderlessStyle);

        var filledStyle =
            new Style(selector =>
                selector.Nesting()
                        .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled));

        {
            var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));

            var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
            scrollViewerStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorErrorText);
            errorStyle.Add(scrollViewerStyle);
            filledStyle.Add(errorStyle);
        }

        {
            var warningStyle      = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
            var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
            scrollViewerStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorWarningText);
            warningStyle.Add(scrollViewerStyle);
            filledStyle.Add(warningStyle);
        }

        Add(filledStyle);
    }
}
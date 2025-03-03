using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AddOnDecoratedBoxTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string LeftAddOnPart = "PART_LeftAddOn";
    public const string RightAddOnPart = "PART_RightAddOn";
    public const string InnerBoxContentPart = "PART_InnerBoxContent";

    public const int NormalZIndex = 1000;
    public const int ActivatedZIndex = 2000;

    public AddOnDecoratedBoxTheme() : base(typeof(AddOnDecoratedBox))
    {
    }

    protected AddOnDecoratedBoxTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<AddOnDecoratedBox>((decoratedBox, scope) =>
        {
            var mainLayout = new Grid
            {
                Name = MainLayoutPart,
                ColumnDefinitions = new ColumnDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Star),
                    new(GridLength.Auto)
                }
            };
            BuildGridChildren(decoratedBox, mainLayout, scope);
            return mainLayout;
        });
    }

    protected override void BuildStyles()
    {
        BuildFixedStyle();
        BuildCommonStyle();
        BuildDisabledStyle();
    }

    protected virtual void BuildGridChildren(AddOnDecoratedBox decoratedBox, Grid mainLayout, INameScope scope)
    {
        BuildLeftAddOn(decoratedBox, mainLayout, scope);
        BuildInnerBox(decoratedBox, mainLayout, scope);
        BuildRightAddOn(decoratedBox, mainLayout, scope);
    }

    protected virtual void BuildLeftAddOn(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
    {
        var leftAddOnContentPresenter = new ContentPresenter
        {
            Name                     = LeftAddOnPart,
            VerticalAlignment        = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment      = HorizontalAlignment.Left,
            Focusable                = false
        };

        CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
            AddOnDecoratedBox.LeftAddOnProperty);
        CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
            AddOnDecoratedBox.LeftAddOnBorderThicknessProperty);
        CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
            AddOnDecoratedBox.LeftAddOnCornerRadiusProperty);
        CreateTemplateParentBinding(leftAddOnContentPresenter, Visual.IsVisibleProperty,
            AddOnDecoratedBox.LeftAddOnProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
        leftAddOnContentPresenter.RegisterInNameScope(scope);
        
        RegisterTokenResourceBindings(decoratedBox, () =>
        {
            decoratedBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                AddOnDecoratedBoxTokenKey.AddonBg));
            decoratedBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                SharedTokenKey.ColorBorder));
        });

        Grid.SetColumn(leftAddOnContentPresenter, 0);
        layout.Children.Add(leftAddOnContentPresenter);
    }

    protected virtual void BuildInnerBox(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
    {
        var innerBox = new ContentPresenter
        {
            Name = InnerBoxContentPart
        };

        CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, ContentControl.ContentProperty);

        layout.Children.Add(innerBox);
        Grid.SetColumn(innerBox, 1);
    }

    protected virtual void BuildRightAddOn(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
    {
        var rightAddOnContentPresenter = new ContentPresenter
        {
            Name                     = RightAddOnPart,
            VerticalAlignment        = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment      = HorizontalAlignment.Right,
            Focusable                = false
        };
        CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
            AddOnDecoratedBox.RightAddOnProperty);
        CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
            AddOnDecoratedBox.RightAddOnBorderThicknessProperty);
        CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
            AddOnDecoratedBox.RightAddOnCornerRadiusProperty);
        CreateTemplateParentBinding(rightAddOnContentPresenter, Visual.IsVisibleProperty,
            AddOnDecoratedBox.RightAddOnProperty,
            BindingMode.Default, 
            ObjectConverters.IsNotNull);
        
        RegisterTokenResourceBindings(decoratedBox, () =>
        {
            decoratedBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                AddOnDecoratedBoxTokenKey.AddonBg));
            decoratedBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                SharedTokenKey.ColorBorder));
        });

        rightAddOnContentPresenter.RegisterInNameScope(scope);
        layout.Children.Add(rightAddOnContentPresenter);
        Grid.SetColumn(rightAddOnContentPresenter, 2);
    }

    private void BuildFixedStyle()
    {
        this.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorText);

        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
        {
            var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                selector.Nesting().Template().Name(RightAddOnPart)));
            addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxTokenKey.PaddingLG);
            largeStyle.Add(addOnStyle);
        }
        
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
        {
            var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                selector.Nesting().Template().Name(RightAddOnPart)));
            addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxTokenKey.Padding);
            middleStyle.Add(addOnStyle);
        }
        
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
        {
            var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                selector.Nesting().Template().Name(RightAddOnPart)));
            addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxTokenKey.PaddingSM);
            smallStyle.Add(addOnStyle);
        }
        
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        commonStyle.Add(smallStyle);

        Add(commonStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);
        
        // TODO 暂时这么简单处理吧
        var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
        addOnStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(addOnStyle);
        Add(disabledStyle);
    }
}
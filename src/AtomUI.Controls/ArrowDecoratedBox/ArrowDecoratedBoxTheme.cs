using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ArrowDecoratedBoxTheme : BaseControlTheme
{
    public const string ContentDecoratorPart = "PART_ContentDecorator";
    public const string ContentLayoutPart = "PART_ContentLayout";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string ArrowIndicatorPart = "PART_ArrowIndicator";
    public const string ArrowIndicatorLayoutPart = "PART_ArrowIndicatorLayout";
    public const string ArrowPositionLayoutPart = "PART_ArrowPositionLayout";

    public ArrowDecoratedBoxTheme() : this(typeof(ArrowDecoratedBox))
    {
    }

    protected ArrowDecoratedBoxTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ArrowDecoratedBox>((box, scope) =>
        {
            var contentLayout = new DockPanel()
            {
                Name          = ContentLayoutPart,
                LastChildFill = true
            };
            
            var arrowIndicatorLayout = new LayoutTransformControl()
            {
                Name = ArrowIndicatorLayoutPart,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            arrowIndicatorLayout.RegisterInNameScope(scope);
            CreateTemplateParentBinding(arrowIndicatorLayout, LayoutTransformControl.IsVisibleProperty, ArrowDecoratedBox.IsShowArrowProperty);
            
            var arrowIndicator = new ArrowIndicator()
            {
                Name = ArrowIndicatorPart
            };
            CreateTemplateParentBinding(arrowIndicator, ArrowIndicator.FilledColorProperty, ArrowDecoratedBox.BackgroundProperty);
            
            arrowIndicatorLayout.Child = arrowIndicator;

            var arrowPositionLayout = new Panel()
            {
                Name = ArrowPositionLayoutPart
            };
            arrowPositionLayout.Children.Add(arrowIndicatorLayout);
            
            contentLayout.Children.Add(arrowPositionLayout);
      
            var content = BuildContent(scope);
            var contentDecorator = new Border
            {
                Name   = ContentDecoratorPart,
                Margin = new Thickness(0)
            };
            contentDecorator.RegisterInNameScope(scope);
;           CreateTemplateParentBinding(contentDecorator, Border.BackgroundSizingProperty, TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(contentDecorator, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(contentDecorator, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentDecorator, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);

            contentDecorator.Child = content;
            contentLayout.Children.Add(contentDecorator);
            return contentLayout;
        });
    }
    
    protected virtual Control BuildContent(INameScope scope)
    {
        var contentPresenter = new ContentPresenter
        {
            Name = ContentPresenterPart
        };

        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ContentControl.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty);
        return contentPresenter;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorText);
        commonStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        commonStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
        commonStyle.Add(TemplatedControl.PaddingProperty, ArrowDecoratedBoxTokenResourceKey.Padding);
        commonStyle.Add(ArrowDecoratedBox.ArrowSizeProperty, ArrowDecoratedBoxTokenResourceKey.ArrowSize);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        BuildArrowDirectionStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildArrowDirectionStyle(Style commonStyle)
    {
        var arrowIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ArrowIndicatorPart));
        arrowIndicatorStyle.Add(ArrowIndicator.ArrowSizeProperty, ArrowDecoratedBoxTokenResourceKey.ArrowSize);
        commonStyle.Add(arrowIndicatorStyle);
        
        var topDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Top));
        {
            var arrowIndicatorLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowIndicatorLayoutPart));
            topDirectionStyle.Add(arrowIndicatorLayoutStyle);
        }
        {
            var arrowPositionLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowPositionLayoutPart));
            arrowPositionLayoutStyle.Add(DockPanel.DockProperty, Dock.Top);
            topDirectionStyle.Add(arrowPositionLayoutStyle);
        }
        commonStyle.Add(topDirectionStyle);
        var rightDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Right));
        {
            var arrowIndicatorLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowIndicatorLayoutPart));
            arrowIndicatorLayoutStyle.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(90));
            rightDirectionStyle.Add(arrowIndicatorLayoutStyle);
        }
        {
            var arrowPositionLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowPositionLayoutPart));
            arrowPositionLayoutStyle.Add(DockPanel.DockProperty, Dock.Right);
            rightDirectionStyle.Add(arrowPositionLayoutStyle);
        }
        commonStyle.Add(rightDirectionStyle);
        var bottomDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Bottom));
        {
            var arrowIndicatorLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowIndicatorLayoutPart));
            arrowIndicatorLayoutStyle.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(180));
            bottomDirectionStyle.Add(arrowIndicatorLayoutStyle);
        }
        {
            var arrowPositionLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowPositionLayoutPart));
            arrowPositionLayoutStyle.Add(DockPanel.DockProperty, Dock.Bottom);
            bottomDirectionStyle.Add(arrowPositionLayoutStyle);
        }
        commonStyle.Add(bottomDirectionStyle);
        var leftDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Left));
        {
            var arrowIndicatorLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowIndicatorLayoutPart));
            arrowIndicatorLayoutStyle.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(-90));
            leftDirectionStyle.Add(arrowIndicatorLayoutStyle);
        }
        {
            var arrowPositionLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ArrowPositionLayoutPart));
            arrowPositionLayoutStyle.Add(DockPanel.DockProperty, Dock.Left);
            leftDirectionStyle.Add(arrowPositionLayoutStyle);
        }
        commonStyle.Add(leftDirectionStyle);
    }
}
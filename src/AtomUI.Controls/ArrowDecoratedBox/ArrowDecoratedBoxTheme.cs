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
    public const string ArrowContentPart = "PART_ArrowContent";

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

            var arrowContent = new Control()
            {
                Name       = ArrowContentPart
            };
            CreateTemplateParentBinding(arrowContent, Border.IsVisibleProperty,
                ArrowDecoratedBox.IsShowArrowProperty);
            contentLayout.Children.Add(arrowContent);
            arrowContent.RegisterInNameScope(scope);
      
            var content = BuildContent(scope);
            var contentDecorator = new Border
            {
                Name   = ContentDecoratorPart,
                Margin = new Thickness(0)
            };
            contentDecorator.RegisterInNameScope(scope);
;            CreateTemplateParentBinding(contentDecorator, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
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
        var topDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Top));
        {
            var arrowContentStyle = new Style(selector => selector.Nesting().Template().Name(ArrowContentPart));
            arrowContentStyle.Add(DockPanel.DockProperty, Dock.Top);
            arrowContentStyle.Add(Control.HeightProperty, ArrowDecoratedBoxTokenResourceKey.ArrowContentThickness);
            topDirectionStyle.Add(arrowContentStyle);
        }
        commonStyle.Add(topDirectionStyle);
        var rightDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Right));
        {
            var arrowContentStyle = new Style(selector => selector.Nesting().Template().Name(ArrowContentPart));
            arrowContentStyle.Add(DockPanel.DockProperty, Dock.Right);
            arrowContentStyle.Add(Control.WidthProperty, ArrowDecoratedBoxTokenResourceKey.ArrowContentThickness);
            rightDirectionStyle.Add(arrowContentStyle);
        }
        commonStyle.Add(rightDirectionStyle);
        var bottomDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Bottom));
        {
            var arrowContentStyle = new Style(selector => selector.Nesting().Template().Name(ArrowContentPart));
            arrowContentStyle.Add(DockPanel.DockProperty, Dock.Bottom);
            arrowContentStyle.Add(Control.HeightProperty, ArrowDecoratedBoxTokenResourceKey.ArrowContentThickness);
            bottomDirectionStyle.Add(arrowContentStyle);
        }
        commonStyle.Add(bottomDirectionStyle);
        var leftDirectionStyle = new Style(selector => selector.Nesting().PropertyEquals(ArrowDecoratedBox.ArrowDirectionProperty, Direction.Left));
        {
            var arrowContentStyle = new Style(selector => selector.Nesting().Template().Name(ArrowContentPart));
            arrowContentStyle.Add(DockPanel.DockProperty, Dock.Left);
            arrowContentStyle.Add(Control.WidthProperty, ArrowDecoratedBoxTokenResourceKey.ArrowContentThickness);
            leftDirectionStyle.Add(arrowContentStyle);
        }
        commonStyle.Add(leftDirectionStyle);
    }
}
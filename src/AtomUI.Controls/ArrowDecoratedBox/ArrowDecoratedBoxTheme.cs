using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ArrowDecoratedBoxTheme : BaseControlTheme
{
    public const string DecoratorPart = "PART_Decorator";
    public const string ContentPresenterPart = "PART_ContentPresenter";

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
            var decorator = new Border
            {
                Name   = DecoratorPart,
                Margin = new Thickness(0)
            };

            decorator.RegisterInNameScope(scope);

            decorator.Child = BuildContent(scope);

            CreateTemplateParentBinding(decorator, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(decorator, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(decorator, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(decorator, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);

            return decorator;
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
        Add(commonStyle);
    }
}
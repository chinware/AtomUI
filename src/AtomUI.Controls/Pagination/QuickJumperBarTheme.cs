using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class QuickJumperBarTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string JumpToContentPresenterPart = "PART_JumpToContentPresenter";
    public const string PageContentPresenterPart = "PART_PageContentPresenter";
    public const string PageLineEditPart = "PART_PageLineEdit";
    
    public QuickJumperBarTheme()
        : base(typeof(QuickJumperBar))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<QuickJumperBar>((quickJumperBar, scope) =>
        {
            var rootLayout = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Name = RootLayoutPart
            };
            var jumpToText = new ContentPresenter()
            {
                Name = JumpToContentPresenterPart
            };
            CreateTemplateParentBinding(jumpToText, ContentPresenter.ContentProperty,
                QuickJumperBar.JumpToTextProperty,
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new TextBlock()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        return o;
                    }));
            rootLayout.Children.Add(jumpToText);
            var input = new LineEdit()
            {
                Name = PageLineEditPart
            };
            
            CreateTemplateParentBinding(input, LineEdit.SizeTypeProperty, QuickJumperBar.SizeTypeProperty);
            input.RegisterInNameScope(scope);
            rootLayout.Children.Add(input);
            var pageTextContentPresenter = new ContentPresenter()
            {
                Name = PageContentPresenterPart
            };
            CreateTemplateParentBinding(pageTextContentPresenter, ContentPresenter.ContentProperty,
                QuickJumperBar.PageTextProperty,
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new TextBlock()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        return o;
                    }));
            rootLayout.Children.Add(pageTextContentPresenter);
            return rootLayout;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
        layoutStyle.Add(StackPanel.SpacingProperty, PaginationTokenKey.PaginationLayoutSpacing);
        commonStyle.Add(layoutStyle);

        var lineEditStyle = new Style(selector => selector.Nesting().Template().Name(PageLineEditPart));
        lineEditStyle.Add(LineEdit.WidthProperty, PaginationTokenKey.PaginationQuickJumperInputWidth);
        commonStyle.Add(lineEditStyle);
        Add(commonStyle);
    }
}
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridColumnGroupHeaderTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    
    public DataGridColumnGroupHeaderTheme()
        : base(typeof(DataGridColumnGroupHeader))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridColumnGroupHeader>((groupHeader, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart,
            };
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, DataGridColumnGroupHeader.BorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, DataGridColumnGroupHeader.BackgroundProperty);
            var headerPresenter = new ContentPresenter()
            {
                Name                       = HeaderPresenterPart,
                HorizontalAlignment        = HorizontalAlignment.Stretch,
                VerticalAlignment          = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty, DataGridColumnGroupHeader.HeaderProperty);
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty, DataGridColumnGroupHeader.HeaderTemplateProperty);
            frame.Child = headerPresenter;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridColumnGroupHeader.ForegroundProperty, DataGridTokenKey.TableHeaderTextColor);
        commonStyle.Add(DataGridColumnGroupHeader.BackgroundProperty, DataGridTokenKey.TableHeaderBg);
        commonStyle.Add(DataGridColumnGroupHeader.FontWeightProperty, SharedTokenKey.FontWeightStrong);
        
        var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenterStyle.Add(ContentPresenter.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
        headerPresenterStyle.Add(ContentPresenter.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        commonStyle.Add(headerPresenterStyle);
        
        var frameStyle  = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BorderBrushProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(frameStyle);
        
        BuildSizeTypeStyle(commonStyle);
        Add(commonStyle);
    }
    
    private void BuildSizeTypeStyle(Style commonStyle)
    {
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnGroupHeader.SizeTypeProperty, SizeType.Large));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePadding);
            largeStyle.Add(frameStyle);
        }
        largeStyle.Add(DataGridColumnGroupHeader.FontSizeProperty, DataGridTokenKey.TableFontSize);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnGroupHeader.SizeTypeProperty, SizeType.Middle));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingMiddle);
            middleStyle.Add(frameStyle);
        }
        middleStyle.Add(DataGridColumnGroupHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeMiddle);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnGroupHeader.SizeTypeProperty, SizeType.Small));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingSmall);
            smallStyle.Add(frameStyle);
        }
        smallStyle.Add(DataGridColumnGroupHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeSmall);
        commonStyle.Add(smallStyle);
    }
    
}
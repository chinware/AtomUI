using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ScrollBarTheme : BaseControlTheme
{
    internal const string FramePart = "PART_Frame";
    internal const string LayoutRootPart = "PART_LayoutRoot";
    internal const string PageUpButtonPart = "PART_PageUpButton";
    internal const string PageDownButtonPart = "PART_PageDownButton";
    internal const string TrackPart = "PART_Track";
    internal const string ThumbPart = "PART_Thumb";
    
    public ScrollBarTheme()
        : base(typeof(ScrollBar))
    {
    }

    protected override void BuildTemplateStyle()
    {
        BuildVerticalScrollBarTemplate();
        BuildHorizontalScrollBarTemplate();
    }

    private void BuildVerticalScrollBarTemplate()
    {
        var tpl = new FuncControlTemplate<ScrollBar>((scrollBar, scope) =>
        {
            var frame = new Border
            {
                Name       = FramePart,
                UseLayoutRounding = false,
            };
            var layout = new Grid
            {
                Name = LayoutRootPart,
                RowDefinitions = new RowDefinitions
                {
                    new RowDefinition(GridLength.Star),
                }
            };
            frame.Child = layout;

            var track = BuildTrack(true, scope);
            layout.Children.Add(track);
            
            return frame;
        });
        var style = new Style(selector => selector.Nesting().Class(StdPseudoClass.Vertical));
        style.Add(new Setter(TemplatedControl.TemplateProperty, tpl));
        Add(style);
    }

    private void BuildHorizontalScrollBarTemplate()
    {
        var tpl = new FuncControlTemplate<ScrollBar>((scrollBar, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart,
                UseLayoutRounding = false
            };
            var layout = new Grid
            {
                Name = LayoutRootPart,
                ColumnDefinitions = new ColumnDefinitions
                {
                    new ColumnDefinition(GridLength.Star),
                }
            };
            var track = BuildTrack(false, scope);
            layout.Children.Add(track);
            frame.Child = layout;
            return frame;
        });
        var style = new Style(selector => selector.Nesting().Class(StdPseudoClass.Horizontal));
        style.Add(new Setter(TemplatedControl.TemplateProperty, tpl));
        Add(style);
    }
    
    private Track BuildTrack(bool isVertical, INameScope scope)
    {
        var track = new Track
        {
            Name = TrackPart,
            IsDirectionReversed = true,
        };
        track.RegisterInNameScope(scope);
        if (isVertical)
        {
            Grid.SetRow(track, 0);    
        }
        else
        {
            Grid.SetColumn(track, 0);
        }

        CreateTemplateParentBinding(track, Track.MinimumProperty, RangeBase.MinimumProperty);
        CreateTemplateParentBinding(track, Track.MaximumProperty, RangeBase.MaximumProperty);
        CreateTemplateParentBinding(track, Track.ValueProperty, RangeBase.ValueProperty, BindingMode.TwoWay);
        CreateTemplateParentBinding(track, Track.DeferThumbDragProperty, ScrollViewer.IsDeferredScrollingEnabledProperty);
        CreateTemplateParentBinding(track, Track.ViewportSizeProperty, ScrollBar.ViewportSizeProperty);
        CreateTemplateParentBinding(track, Track.OrientationProperty, ScrollBar.OrientationProperty);

        var pageUpButton = new ScrollBarRepeatButton()
        {
            Name = PageUpButtonPart,
            MinHeight = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(0),
            Focusable = false
        };
        track.DecreaseButton = pageUpButton;
        pageUpButton.RegisterInNameScope(scope);

        var pageDownButton = new ScrollBarRepeatButton()
        {
            Name                = PageDownButtonPart,
            MinHeight           = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            CornerRadius        = new CornerRadius(0),
            Focusable           = false
        };
        
        track.IncreaseButton = pageDownButton;
        pageDownButton.RegisterInNameScope(scope);

        var thumb = new ScrollBarThumb
        {
            Name = ThumbPart
        };
        
        thumb.RegisterInNameScope(scope);
        track.Thumb = thumb;
        
        return track;
    }
    
    protected override void BuildStyles()
    {
        var verticalStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Vertical));
        verticalStyle.Add(ScrollBar.WidthProperty, ScrollBarTokenKey.ScrollBarThickness);
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(ScrollBarThumb.MarginProperty, ScrollBarTokenKey.ContentVPadding);
            verticalStyle.Add(frameStyle);
        }
        {
            var thumbStyle = new Style(selector => selector.Nesting().Template().Name(ThumbPart));
            thumbStyle.Add(ScrollBarThumb.MinHeightProperty, ScrollBarTokenKey.ScrollBarThickness);
            thumbStyle.Add(ScrollBarThumb.WidthProperty, ScrollBarTokenKey.ThumbThickness);
            verticalStyle.Add(thumbStyle);
        }
        Add(verticalStyle);
        
        var horizontalStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Horizontal));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(ScrollBarThumb.MarginProperty, ScrollBarTokenKey.ContentHPadding);
            horizontalStyle.Add(frameStyle);
        }
        {
            var thumbStyle = new Style(selector => selector.Nesting().Template().Name(ThumbPart));
            thumbStyle.Add(ScrollBarThumb.MinWidthProperty, ScrollBarTokenKey.ScrollBarThickness);
            thumbStyle.Add(ScrollBarThumb.HeightProperty, ScrollBarTokenKey.ThumbThickness);
            horizontalStyle.Add(thumbStyle);
        }
        horizontalStyle.Add(ScrollBar.HeightProperty, ScrollBarTokenKey.ScrollBarThickness);
        Add(horizontalStyle);
    }
}
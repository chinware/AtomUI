using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelAlignmentShowCase : GalleryReactiveUserControl<FlexPanelViewModel>
{
    private const double GapSmallValue = 8;
    private const double GapMiddleValue = 16;
    private const double GapLargeValue = 24;

    public FlexPanelAlignmentShowCase()
    {
        InitializeComponent();
        SetupEventHandlers();
        SetGap(GapSmallValue);
    }

    private void SetupEventHandlers()
    {
        JustifySegmented.SelectionChanged += HandleJustifySelectionChanged;
        AlignSegmented.SelectionChanged += HandleAlignSelectionChanged;
        GapSmall.IsCheckedChanged += HandleGapRadioChanged;
        GapMiddle.IsCheckedChanged += HandleGapRadioChanged;
        GapLarge.IsCheckedChanged += HandleGapRadioChanged;
        GapCustomize.IsCheckedChanged += HandleGapRadioChanged;
        GapValueSlider.ValueChanged += HandleGapValueSliderChanged;

        WrapEnabled.IsCheckedChanged += (_, _) =>
        {
            if (WrapEnabled.IsChecked == true)
            {
                WrapFlexPanel.Wrap = FlexWrap.Wrap;
                WrapScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        };

        WrapDisabled.IsCheckedChanged += (_, _) =>
        {
            if (WrapDisabled.IsChecked == true)
            {
                WrapFlexPanel.Wrap = FlexWrap.NoWrap;
                WrapScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        };
    }

    private void HandleJustifySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var index = JustifySegmented.SelectedIndex;
        AlignFlexPanel.JustifyContent = index switch
        {
            0 => JustifyContent.FlexStart,
            1 => JustifyContent.Center,
            2 => JustifyContent.FlexEnd,
            3 => JustifyContent.SpaceBetween,
            4 => JustifyContent.SpaceAround,
            5 => JustifyContent.SpaceEvenly,
            _ => AlignFlexPanel.JustifyContent
        };
    }

    private void HandleAlignSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var index = AlignSegmented.SelectedIndex;
        AlignFlexPanel.AlignItems = index switch
        {
            0 => AlignItems.FlexStart,
            1 => AlignItems.Center,
            2 => AlignItems.FlexEnd,
            3 => AlignItems.Stretch,
            _ => AlignFlexPanel.AlignItems
        };
    }

    private void HandleGapRadioChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not AtomUIRadioButton { IsChecked: true } radioButton)
        {
            return;
        }

        if (radioButton == GapSmall)
        {
            GapCustomPanel.IsVisible = false;
            SetGap(GapSmallValue);
        }
        else if (radioButton == GapMiddle)
        {
            GapCustomPanel.IsVisible = false;
            SetGap(GapMiddleValue);
        }
        else if (radioButton == GapLarge)
        {
            GapCustomPanel.IsVisible = false;
            SetGap(GapLargeValue);
        }
        else if (radioButton == GapCustomize)
        {
            GapCustomPanel.IsVisible = true;
            SetGap(GetCustomGap());
        }
    }

    private void HandleGapValueSliderChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (GapCustomPanel.IsVisible)
        {
            SetGap(GetCustomGap());
        }
    }

    private double GetCustomGap()
    {
        return Math.Max(0, GapValueSlider.Value);
    }

    private void SetGap(double gap)
    {
        GapFlexPanel.ColumnSpacing = gap;
        GapFlexPanel.RowSpacing = gap;
    }
}

using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using ReactiveUI.Avalonia;
using NumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelPlaygroundShowCase : GalleryReactiveUserControl<FlexPanelViewModel>
{
    private bool _suppressPlaygroundValueChanged;

    public FlexPanelPlaygroundShowCase()
    {
        InitializeComponent();
        SetupEventHandlers();
        InitializePropertyDemos();
    }

    private void SetupEventHandlers()
    {
        PlaygroundDirectionSegmented.SelectionChanged += HandlePlaygroundPanelSettingsChanged;
        PlaygroundWrapSegmented.SelectionChanged += HandlePlaygroundPanelSettingsChanged;
        PlaygroundJustifySegmented.SelectionChanged += HandlePlaygroundPanelSettingsChanged;
        PlaygroundAlignItemsSegmented.SelectionChanged += HandlePlaygroundPanelSettingsChanged;
        PlaygroundAlignContentSegmented.SelectionChanged += HandlePlaygroundPanelSettingsChanged;
        PlaygroundColumnSpacingSlider.ValueChanged += HandlePlaygroundSpacingChanged;
        PlaygroundRowSpacingSlider.ValueChanged += HandlePlaygroundSpacingChanged;
        PlaygroundWidthSlider.ValueChanged += HandlePlaygroundContainerSizeChanged;
        PlaygroundHeightSlider.ValueChanged += HandlePlaygroundContainerSizeChanged;

        PlaygroundItemSegmented.SelectionChanged += HandlePlaygroundItemSelectionChanged;
        PlaygroundAlignSelfSegmented.SelectionChanged += HandlePlaygroundAlignSelfChanged;
        PlaygroundBasisKindSegmented.SelectionChanged += HandlePlaygroundBasisKindChanged;
        PlaygroundBasisValueInput.PropertyChanged += HandlePlaygroundBasisValueChanged;
        PlaygroundBasisValueSlider.ValueChanged += HandlePlaygroundBasisValueSliderChanged;
        PlaygroundGrowInput.PropertyChanged += HandlePlaygroundItemNumericChanged;
        PlaygroundShrinkInput.PropertyChanged += HandlePlaygroundItemNumericChanged;
        PlaygroundOrderInput.PropertyChanged += HandlePlaygroundItemNumericChanged;
    }

    private void InitializePropertyDemos()
    {
        UpdatePlaygroundPanelSettings();
        UpdatePlaygroundContainerSize();
        UpdatePlaygroundItemEditor();
    }

    private void HandlePlaygroundPanelSettingsChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        UpdatePlaygroundPanelSettings();
    }

    private void HandlePlaygroundSpacingChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        UpdatePlaygroundPanelSettings();
    }

    private void UpdatePlaygroundPanelSettings()
    {
        PlaygroundFlexPanel.Direction = PlaygroundDirectionSegmented.SelectedIndex switch
        {
            1 => FlexDirection.Column,
            2 => FlexDirection.RowReverse,
            3 => FlexDirection.ColumnReverse,
            _ => FlexDirection.Row
        };

        PlaygroundFlexPanel.Wrap = PlaygroundWrapSegmented.SelectedIndex switch
        {
            0 => FlexWrap.NoWrap,
            2 => FlexWrap.WrapReverse,
            _ => FlexWrap.Wrap
        };

        PlaygroundFlexPanel.JustifyContent = PlaygroundJustifySegmented.SelectedIndex switch
        {
            1 => JustifyContent.Center,
            2 => JustifyContent.FlexEnd,
            3 => JustifyContent.SpaceBetween,
            4 => JustifyContent.SpaceAround,
            5 => JustifyContent.SpaceEvenly,
            _ => JustifyContent.FlexStart
        };

        PlaygroundFlexPanel.AlignItems = PlaygroundAlignItemsSegmented.SelectedIndex switch
        {
            0 => AlignItems.FlexStart,
            1 => AlignItems.Center,
            2 => AlignItems.FlexEnd,
            _ => AlignItems.Stretch
        };

        PlaygroundFlexPanel.AlignContent = PlaygroundAlignContentSegmented.SelectedIndex switch
        {
            0 => AlignContent.FlexStart,
            1 => AlignContent.Center,
            2 => AlignContent.FlexEnd,
            4 => AlignContent.SpaceBetween,
            5 => AlignContent.SpaceAround,
            6 => AlignContent.SpaceEvenly,
            _ => AlignContent.Stretch
        };

        var columnSpacing = Math.Max(0, Math.Round(PlaygroundColumnSpacingSlider.Value));
        var rowSpacing = Math.Max(0, Math.Round(PlaygroundRowSpacingSlider.Value));
        PlaygroundFlexPanel.ColumnSpacing = columnSpacing;
        PlaygroundFlexPanel.RowSpacing = rowSpacing;
        PlaygroundColumnSpacingText.Text = $"{columnSpacing:0}px";
        PlaygroundRowSpacingText.Text = $"{rowSpacing:0}px";
    }

    private void HandlePlaygroundContainerSizeChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        UpdatePlaygroundContainerSize();
    }

    private void UpdatePlaygroundContainerSize()
    {
        var width = Math.Max(0, Math.Round(PlaygroundWidthSlider.Value));
        var height = Math.Max(0, Math.Round(PlaygroundHeightSlider.Value));
        PlaygroundContainer.Width = width;
        PlaygroundContainer.Height = height;
        PlaygroundWidthText.Text = $"{width:0}px";
        PlaygroundHeightText.Text = $"{height:0}px";
    }

    private void HandlePlaygroundItemSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        UpdatePlaygroundItemEditor();
    }

    private void HandlePlaygroundAlignSelfChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        var item = GetPlaygroundItem();
        var align = PlaygroundAlignSelfSegmented.SelectedIndex switch
        {
            1 => AlignItems.FlexStart,
            2 => AlignItems.Center,
            3 => AlignItems.FlexEnd,
            4 => AlignItems.Stretch,
            _ => (AlignItems?)null
        };
        Flex.SetAlignSelf(item, align);
    }

    private void HandlePlaygroundBasisKindChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        UpdatePlaygroundBasis();
    }

    private void HandlePlaygroundBasisValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged || e.Property != NumericUpDown.ValueProperty)
        {
            return;
        }

        UpdatePlaygroundBasis();
    }

    private void HandlePlaygroundBasisValueSliderChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged)
        {
            return;
        }

        PlaygroundBasisPercentText.Text = $"{Math.Round(PlaygroundBasisValueSlider.Value)}%";
        UpdatePlaygroundBasis();
    }

    private void UpdatePlaygroundBasis()
    {
        var item = GetPlaygroundItem();
        var kindIndex = PlaygroundBasisKindSegmented.SelectedIndex;
        var value = GetNumericValue(PlaygroundBasisValueInput);

        switch (kindIndex)
        {
            case 0:
                PlaygroundBasisValuePanel.IsVisible = false;
                PlaygroundBasisPercentPanel.IsVisible = false;
                Flex.SetBasis(item, FlexBasis.Auto);
                break;
            case 1:
                PlaygroundBasisValuePanel.IsVisible = true;
                PlaygroundBasisPercentPanel.IsVisible = false;
                PlaygroundBasisValueUnit.Text = "px";
                Flex.SetBasis(item, new FlexBasis(Math.Max(0, value)));
                break;
            case 2:
                PlaygroundBasisValuePanel.IsVisible = false;
                PlaygroundBasisPercentPanel.IsVisible = true;
                var sliderValue = Math.Max(0, Math.Min(100, PlaygroundBasisValueSlider.Value));
                PlaygroundBasisPercentText.Text = $"{Math.Round(sliderValue)}%";
                Flex.SetBasis(item, new FlexBasis(sliderValue / 100, FlexBasisKind.Relative));
                break;
        }
    }

    private void HandlePlaygroundItemNumericChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_suppressPlaygroundValueChanged || e.Property != NumericUpDown.ValueProperty)
        {
            return;
        }

        UpdatePlaygroundItemNumbers();
    }

    private void UpdatePlaygroundItemNumbers()
    {
        var item = GetPlaygroundItem();
        Flex.SetGrow(item, Math.Max(0, GetNumericValue(PlaygroundGrowInput)));
        Flex.SetShrink(item, Math.Max(0, GetNumericValue(PlaygroundShrinkInput)));
        Flex.SetOrder(item, GetIntValue(PlaygroundOrderInput));
    }

    private void UpdatePlaygroundItemEditor()
    {
        _suppressPlaygroundValueChanged = true;

        var item = GetPlaygroundItem();
        var basis = Flex.GetBasis(item);
        switch (basis.Kind)
        {
            case FlexBasisKind.Auto:
                PlaygroundBasisKindSegmented.SelectedIndex = 0;
                PlaygroundBasisValuePanel.IsVisible = false;
                PlaygroundBasisPercentPanel.IsVisible = false;
                break;
            case FlexBasisKind.Absolute:
                PlaygroundBasisKindSegmented.SelectedIndex = 1;
                PlaygroundBasisValuePanel.IsVisible = true;
                PlaygroundBasisPercentPanel.IsVisible = false;
                PlaygroundBasisValueUnit.Text = "px";
                PlaygroundBasisValueInput.Value = (decimal)basis.Value;
                break;
            case FlexBasisKind.Relative:
                PlaygroundBasisKindSegmented.SelectedIndex = 2;
                PlaygroundBasisValuePanel.IsVisible = false;
                PlaygroundBasisPercentPanel.IsVisible = true;
                PlaygroundBasisValueSlider.Value = basis.Value * 100;
                PlaygroundBasisPercentText.Text = $"{Math.Round(PlaygroundBasisValueSlider.Value)}%";
                break;
        }

        PlaygroundAlignSelfSegmented.SelectedIndex = Flex.GetAlignSelf(item) switch
        {
            null => 0,
            AlignItems.FlexStart => 1,
            AlignItems.Center => 2,
            AlignItems.FlexEnd => 3,
            AlignItems.Stretch => 4,
            _ => 0
        };

        PlaygroundGrowInput.Value = (decimal)Math.Max(0, Flex.GetGrow(item));
        PlaygroundShrinkInput.Value = (decimal)Math.Max(0, Flex.GetShrink(item));
        PlaygroundOrderInput.Value = Flex.GetOrder(item);

        _suppressPlaygroundValueChanged = false;
    }

    private Layoutable GetPlaygroundItem()
    {
        return PlaygroundItemSegmented.SelectedIndex switch
        {
            1 => PlaygroundItemB,
            2 => PlaygroundItemC,
            _ => PlaygroundItemA
        };
    }

    private static double GetNumericValue(NumericUpDown input)
    {
        return Convert.ToDouble(input.Value ?? 0);
    }

    private static int GetIntValue(NumericUpDown input)
    {
        return Convert.ToInt32(input.Value ?? 0);
    }
}

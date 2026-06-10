using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelItemShowCase : GalleryReactiveUserControl<FlexPanelViewModel>
{
    private const double BasisAbsoluteDefault = 60;

    private bool _suppressBasisValueChanged;

    public FlexPanelItemShowCase()
    {
        InitializeComponent();
        SetupEventHandlers();
        InitializePropertyDemos();
    }

    private void SetupEventHandlers()
    {
        OrderSegmented.SelectionChanged += HandleOrderSelectionChanged;
        BasisKindSegmented.SelectionChanged += HandleBasisKindSelectionChanged;
        BasisValueInput.PropertyChanged += HandleBasisValueChanged;
        BasisValueSlider.ValueChanged += HandleBasisValueSliderChanged;
        AlignSelfSegmented.SelectionChanged += HandleAlignSelfSelectionChanged;
        GrowInputA.PropertyChanged += HandleGrowValueChanged;
        GrowInputB.PropertyChanged += HandleGrowValueChanged;
        ShrinkWidthSlider.ValueChanged += HandleShrinkWidthChanged;
    }

    private void InitializePropertyDemos()
    {
        ApplyOrderPreset(0);
        BasisValueInput.Value = (decimal)BasisAbsoluteDefault;
        BasisValueSlider.Value = 100;
        BasisPercentText.Text = "100%";
        UpdateBasis();
        UpdateAlignSelf();
        UpdateGrow();
        UpdateShrinkWidth();
    }

    private void HandleOrderSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ApplyOrderPreset(OrderSegmented.SelectedIndex);
    }

    private void ApplyOrderPreset(int index)
    {
        switch (index)
        {
            case 0:
                Flex.SetOrder(OrderItemA, 0);
                Flex.SetOrder(OrderItemB, 0);
                Flex.SetOrder(OrderItemC, 0);
                Flex.SetOrder(OrderItemD, 0);
                break;
            case 1:
                Flex.SetOrder(OrderItemA, 3);
                Flex.SetOrder(OrderItemB, 2);
                Flex.SetOrder(OrderItemC, 1);
                Flex.SetOrder(OrderItemD, 0);
                break;
            case 2:
                Flex.SetOrder(OrderItemA, 2);
                Flex.SetOrder(OrderItemB, -1);
                Flex.SetOrder(OrderItemC, 3);
                Flex.SetOrder(OrderItemD, 1);
                break;
        }
    }

    private void HandleBasisKindSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _suppressBasisValueChanged = true;
        if (BasisKindSegmented.SelectedIndex == 2)
        {
            BasisValueSlider.Value = 0;
            BasisPercentText.Text = "0%";
        }
        else if (BasisKindSegmented.SelectedIndex == 1 && BasisValueInput.Value is null)
        {
            BasisValueInput.Value = (decimal)BasisAbsoluteDefault;
        }
        _suppressBasisValueChanged = false;
        UpdateBasis();
    }

    private void HandleBasisValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == NumericUpDown.ValueProperty && !_suppressBasisValueChanged)
        {
            UpdateBasis();
        }
    }

    private void HandleBasisValueSliderChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_suppressBasisValueChanged)
        {
            return;
        }

        BasisPercentText.Text = $"{Math.Round(BasisValueSlider.Value)}%";
        UpdateBasis();
    }

    private void UpdateBasis()
    {
        var kindIndex = BasisKindSegmented.SelectedIndex;
        var value = Convert.ToDouble(BasisValueInput.Value ?? 0);

        switch (kindIndex)
        {
            case 0:
                BasisValuePanel.IsVisible = false;
                BasisPercentPanel.IsVisible = false;
                Flex.SetBasis(BasisItem, FlexBasis.Auto);
                break;
            case 1:
                BasisValuePanel.IsVisible = true;
                BasisPercentPanel.IsVisible = false;
                BasisValueInput.Minimum = 0;
                BasisValueInput.Maximum = 1000;
                BasisValueUnit.Text = "px";
                Flex.SetBasis(BasisItem, new FlexBasis(Math.Max(0, value)));
                break;
            case 2:
                BasisValuePanel.IsVisible = false;
                BasisPercentPanel.IsVisible = true;
                var sliderValue = Math.Max(0, Math.Min(100, BasisValueSlider.Value));
                BasisPercentText.Text = $"{Math.Round(sliderValue)}%";
                var relative = sliderValue / 100;
                Flex.SetBasis(BasisItem, new FlexBasis(relative, FlexBasisKind.Relative));
                break;
        }

        Flex.SetGrow(BasisItem, 0);
        Flex.SetGrow(BasisGrowItem1, 1);
        Flex.SetGrow(BasisGrowItem2, 1);
    }

    private void HandleAlignSelfSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateAlignSelf();
    }

    private void UpdateAlignSelf()
    {
        var align = AlignSelfSegmented.SelectedIndex switch
        {
            0 => AlignItems.FlexStart,
            1 => AlignItems.Center,
            2 => AlignItems.FlexEnd,
            3 => AlignItems.Stretch,
            _ => AlignItems.FlexStart
        };
        Flex.SetAlignSelf(AlignSelfTarget, align);
    }

    private void HandleGrowValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == NumericUpDown.ValueProperty)
        {
            UpdateGrow();
        }
    }

    private void UpdateGrow()
    {
        var growA = Convert.ToDouble(GrowInputA.Value ?? 0);
        var growB = Convert.ToDouble(GrowInputB.Value ?? 0);
        Flex.SetGrow(GrowItemA, Math.Max(0, growA));
        Flex.SetGrow(GrowItemB, Math.Max(0, growB));
        Flex.SetGrow(GrowItemC, 1);
    }

    private void HandleShrinkWidthChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        UpdateShrinkWidth();
    }

    private void UpdateShrinkWidth()
    {
        var width = Math.Max(0, Math.Round(ShrinkWidthSlider.Value));
        ShrinkContainer.Width = width;
        ShrinkWidthText.Text = $"{width:0}px";
    }
}

using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunNumericUpDownStateVerification()
    {
        var failures = new List<string>();
        VerifyNumericUpDownAccessoryLifecycle(failures);
        VerifyNumericUpDownStringMode(failures);
        VerifyNumericUpDownKeyboardMode(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("NumericUpDown state verification passed.");
            return true;
        }

        Console.Error.WriteLine("NumericUpDown state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return false;
    }

    private static void VerifyNumericUpDownAccessoryLifecycle(ICollection<string> failures)
    {
        var input = new NumericUpDown
        {
            Width = 260,
            Value = 3
        };
        using var realized = RealizeControl(input);

        Expect(FindVisualByName<InputClearIconButton>(input, "PART_ClearButton") == null,
            "NumericUpDown default should not create PART_ClearButton.", failures);
        Expect(FindVisualByName<ContentPresenter>(input, "PART_InnerRightContentPresenter") == null,
            "NumericUpDown default should not create PART_InnerRightContentPresenter.", failures);

        input.SetCurrentValue(NumericUpDown.IsAllowClearProperty, true);
        RefreshLayout(realized.Window);
        var clearButton = FindVisualByName<InputClearIconButton>(input, "PART_ClearButton");
        Expect(clearButton != null,
            "NumericUpDown should create PART_ClearButton when clear is visible.", failures);
        Expect(clearButton?.Icon is CloseCircleFilled,
            "NumericUpDown clear button should use the default clear icon when ClearIcon is unset.", failures);

        var customClearIcon = new InfoCircleOutlined();
        input.SetCurrentValue(NumericUpDown.ClearIconProperty, customClearIcon);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(clearButton?.Icon, customClearIcon),
            "NumericUpDown clear button should sync a custom ClearIcon.", failures);

        input.SetCurrentValue(NumericUpDown.ClearIconProperty, null);
        RefreshLayout(realized.Window);
        Expect(clearButton?.Icon is CloseCircleFilled,
            "NumericUpDown clear button should restore the default clear icon after ClearIcon is cleared.", failures);

        clearButton?.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, clearButton));
        RefreshLayout(realized.Window);
        Expect(input.Value == null,
            "NumericUpDown clear button should clear Value.", failures);
        Expect(FindVisualByName<InputClearIconButton>(input, "PART_ClearButton") == null,
            "NumericUpDown should remove PART_ClearButton after value is cleared.", failures);
        Expect(clearButton?.GetVisualParent() == null,
            "Removed NumericUpDown clear button should not keep a visual parent.", failures);

        var rightContent = new Avalonia.Controls.TextBlock { Text = "RMB" };
        input.SetCurrentValue(NumericUpDown.InnerRightContentProperty, rightContent);
        RefreshLayout(realized.Window);
        var innerRightPresenter = FindVisualByName<ContentPresenter>(input, "PART_InnerRightContentPresenter");
        Expect(ReferenceEquals(innerRightPresenter?.Content, rightContent),
            "NumericUpDown inner right presenter should hold InnerRightContent.", failures);

        input.SetCurrentValue(NumericUpDown.InnerRightContentProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(input, "PART_InnerRightContentPresenter") == null,
            "NumericUpDown should remove inner right presenter after clearing InnerRightContent.", failures);
        Expect(innerRightPresenter?.GetVisualParent() == null,
            "Removed NumericUpDown inner right presenter should not keep a visual parent.", failures);
    }

    private static void VerifyNumericUpDownStringMode(ICollection<string> failures)
    {
        var input = new NumericUpDown
        {
            Width = 260,
            IsStringMode = true,
            StringValue = "0.123456789012345678901234"
        };
        using var _ = RealizeControl(input);

        Expect(input.StringValue == "0.123456789012345678901234",
            "NumericUpDown string mode should preserve the raw StringValue.", failures);

        input.SetCurrentValue(NumericUpDown.StringValueProperty, "1.25");
        Expect(input.Value == 1.25m,
            $"NumericUpDown string mode should sync parseable StringValue to Value, actual '{input.Value}'.", failures);
    }

    private static void VerifyNumericUpDownKeyboardMode(ICollection<string> failures)
    {
        var input = new NumericUpDown
        {
            Width = 260,
            Value = 3,
            IsKeyboardEnabled = false
        };
        using var _ = RealizeControl(input);

        Expect(input.IsKeyboardEnabled == false,
            "NumericUpDown should keep IsKeyboardEnabled=false.", failures);
    }
}

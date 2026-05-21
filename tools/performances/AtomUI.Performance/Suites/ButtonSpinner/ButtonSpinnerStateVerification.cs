using AtomUI.Animations;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.VisualTree;
using AtomButtonSpinner = AtomUI.Desktop.Controls.ButtonSpinner;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunButtonSpinnerStateVerification()
    {
        var failures = new List<string>();
        VerifyButtonSpinnerHandleLifecycle(failures);
        VerifyButtonSpinnerFloatableTrackingDoesNotKeepIdleGlobalSubscription(failures);
        VerifyButtonSpinnerOuterAddOnLifecycle(failures);
        VerifyButtonSpinnerBorderTransitions(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ButtonSpinner state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ButtonSpinner state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyButtonSpinnerHandleLifecycle(ICollection<string> failures)
    {
        var spinner = new AtomButtonSpinner
        {
            IsButtonSpinnerVisible = false,
            Content                = CreateButtonSpinnerText()
        };
        using var realized = RealizeControl(spinner);
        var decoratedBox = FindVisualByTypeName(spinner, "ButtonSpinnerDecoratedBox");
        Expect(FindVisualByTypeName(spinner, "ButtonSpinnerHandle") == null,
            "Hidden ButtonSpinner should not create ButtonSpinnerHandle.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_SpinnerHandle") == null,
            "Hidden ButtonSpinner should not create PART_SpinnerHandle presenter.",
            failures);
        Expect(CountVisualsByTypeName(spinner, "IconButton") == 0,
            "Hidden ButtonSpinner should not create handle IconButton controls.",
            failures);
        Expect(GetPrivateField(spinner, "AtomUI.Desktop.Controls.ButtonSpinner", "_spinnerHandle") == null,
            "Hidden ButtonSpinner should not keep _spinnerHandle.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, true);
        RefreshLayout(realized.Window);
        var firstHandle = FindVisualByTypeName(spinner, "ButtonSpinnerHandle");
        Expect(firstHandle != null,
            "ButtonSpinner should create ButtonSpinnerHandle when handle is shown.",
            failures);
        Expect(CountVisualsByTypeName(spinner, "IconButton") == 2,
            "Visible ButtonSpinner should create two handle IconButton controls.",
            failures);
        var firstPresenter = FindVisualByName<ContentPresenter>(spinner, "PART_SpinnerHandle");
        Expect(firstPresenter != null,
            "ButtonSpinner should create PART_SpinnerHandle presenter when handle is shown.",
            failures);
        Expect(firstPresenter == null || ReferenceEquals(firstPresenter.TemplatedParent, decoratedBox),
            "PART_SpinnerHandle presenter should use ButtonSpinnerDecoratedBox as templated parent.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.ButtonSpinnerLocationProperty,
            AtomUI.Desktop.Controls.ButtonSpinnerLocation.Left);
        RefreshLayout(realized.Window);
        Expect(firstPresenter?.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Left,
            "PART_SpinnerHandle presenter should align left when ButtonSpinnerLocation is Left.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.ButtonSpinnerLocationProperty,
            AtomUI.Desktop.Controls.ButtonSpinnerLocation.Right);
        RefreshLayout(realized.Window);
        Expect(firstPresenter?.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Right,
            "PART_SpinnerHandle presenter should align right when ButtonSpinnerLocation is Right.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(spinner, "ButtonSpinnerHandle") == null,
            "ButtonSpinner should remove ButtonSpinnerHandle when handle is hidden.",
            failures);
        Expect(CountVisualsByTypeName(spinner, "IconButton") == 0,
            "ButtonSpinner should remove handle IconButton controls when handle is hidden.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_SpinnerHandle") == null,
            "ButtonSpinner should remove PART_SpinnerHandle presenter when handle is hidden.",
            failures);
        Expect(firstHandle?.GetVisualParent() == null,
            "Removed ButtonSpinnerHandle should not keep a visual parent.",
            failures);
        Expect(firstPresenter?.GetVisualParent() == null,
            "Removed PART_SpinnerHandle presenter should not keep a visual parent.",
            failures);
        Expect(firstPresenter == null || firstPresenter.TemplatedParent == null,
            "Removed PART_SpinnerHandle presenter should clear templated parent.",
            failures);
        Expect(GetPrivateField(spinner, "AtomUI.Desktop.Controls.ButtonSpinner", "_spinnerHandle") == null,
            "ButtonSpinner should clear _spinnerHandle when handle is hidden.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, true);
        RefreshLayout(realized.Window);
        var secondHandle = FindVisualByTypeName(spinner, "ButtonSpinnerHandle");
        Expect(secondHandle != null,
            "ButtonSpinner should recreate ButtonSpinnerHandle when handle is shown again.",
            failures);
        Expect(!ReferenceEquals(firstHandle, secondHandle),
            "ButtonSpinner should not reuse a removed ButtonSpinnerHandle instance.",
            failures);
        var secondPresenter = FindVisualByName<ContentPresenter>(spinner, "PART_SpinnerHandle");
        Expect(secondPresenter != null,
            "ButtonSpinner should recreate PART_SpinnerHandle presenter when handle is shown again.",
            failures);
        Expect(!ReferenceEquals(firstPresenter, secondPresenter),
            "ButtonSpinner should not reuse a removed PART_SpinnerHandle presenter instance.",
            failures);
        Expect(secondPresenter == null || ReferenceEquals(secondPresenter.TemplatedParent, decoratedBox),
            "Recreated PART_SpinnerHandle presenter should use ButtonSpinnerDecoratedBox as templated parent.",
            failures);
    }

    private static void VerifyButtonSpinnerFloatableTrackingDoesNotKeepIdleGlobalSubscription(ICollection<string> failures)
    {
        var spinner = new AtomButtonSpinner
        {
            IsButtonSpinnerFloatable = true,
            IsButtonSpinnerVisible   = false,
            Content                  = CreateButtonSpinnerText()
        };
        var realized = RealizeControl(spinner);
        var decoratedBox = FindVisualByTypeName(spinner, "ButtonSpinnerDecoratedBox");
        Expect(decoratedBox != null,
            "ButtonSpinner should create ButtonSpinnerDecoratedBox.",
            failures);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "Hidden floatable ButtonSpinner should not keep an idle pointer tracking subscription.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, true);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "Visible floatable ButtonSpinner should not keep an idle pointer tracking subscription.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, false);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should dispose pointer tracking subscription when handle is hidden.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerVisibleProperty, true);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should not keep an idle pointer tracking subscription when handle is shown again.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerFloatableProperty, false);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should dispose pointer tracking subscription when floatable is disabled.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.IsButtonSpinnerFloatableProperty, true);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should not keep an idle pointer tracking subscription when floatable is enabled.",
            failures);

        spinner.SetCurrentValue(InputElement.IsEnabledProperty, false);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should dispose pointer tracking subscription when disabled.",
            failures);

        spinner.SetCurrentValue(InputElement.IsEnabledProperty, true);
        RefreshLayout(realized.Window);
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should not keep an idle pointer tracking subscription when re-enabled.",
            failures);

        realized.Dispose();
        Expect(GetPointerTrackingSubscription(decoratedBox) == null,
            "ButtonSpinner should dispose pointer tracking subscription when detached.",
            failures);
    }

    private static void VerifyButtonSpinnerOuterAddOnLifecycle(ICollection<string> failures)
    {
        var spinner = new AtomButtonSpinner
        {
            Content = CreateButtonSpinnerText()
        };
        using var realized = RealizeControl(spinner);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_LeftAddOn") == null,
            "ButtonSpinner without LeftAddOn should not create PART_LeftAddOn.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_RightAddOn") == null,
            "ButtonSpinner without RightAddOn should not create PART_RightAddOn.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.LeftAddOnProperty, "http://");
        spinner.SetCurrentValue(AtomButtonSpinner.RightAddOnProperty, ".com");
        RefreshLayout(realized.Window);
        var leftAddOn = FindVisualByName<ContentPresenter>(spinner, "PART_LeftAddOn");
        var rightAddOn = FindVisualByName<ContentPresenter>(spinner, "PART_RightAddOn");
        var overlayLayout = FindVisualByName<Panel>(spinner, "PART_OverlayLayout");
        Expect(leftAddOn != null,
            "ButtonSpinner should create PART_LeftAddOn when LeftAddOn is assigned.",
            failures);
        Expect(rightAddOn != null,
            "ButtonSpinner should create PART_RightAddOn when RightAddOn is assigned.",
            failures);
        Expect(overlayLayout?.ClipToBounds == true,
            "ButtonSpinner overlay layout should clip the floating handle inside the content frame when outer add-ons exist.",
            failures);

        spinner.SetCurrentValue(AtomButtonSpinner.LeftAddOnProperty, null);
        spinner.SetCurrentValue(AtomButtonSpinner.RightAddOnProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_LeftAddOn") == null,
            "ButtonSpinner should remove PART_LeftAddOn when LeftAddOn is cleared.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(spinner, "PART_RightAddOn") == null,
            "ButtonSpinner should remove PART_RightAddOn when RightAddOn is cleared.",
            failures);
        Expect(leftAddOn?.GetVisualParent() == null,
            "Removed PART_LeftAddOn should not keep a visual parent.",
            failures);
        Expect(rightAddOn?.GetVisualParent() == null,
            "Removed PART_RightAddOn should not keep a visual parent.",
            failures);
    }

    private static void VerifyButtonSpinnerBorderTransitions(ICollection<string> failures)
    {
        var spinner = new AtomButtonSpinner
        {
            Content = CreateButtonSpinnerText()
        };
        using var _ = RealizeControl(spinner);

        var decoratedBox = FindVisualByTypeName(spinner, "ButtonSpinnerDecoratedBox");
        var colorTransitions = decoratedBox?.Transitions?.OfType<SolidColorBrushTransition>().ToList();
        // TODO: AddOnDecoratedBox.EffectiveInnerBox*Property API was removed in the Avalonia 12 migration;
        // re-enable border/background transition assertions once the new effective-brush surface stabilizes.
        Expect(colorTransitions != null && colorTransitions.Count > 0,
            "ButtonSpinnerDecoratedBox should declare SolidColorBrush transitions.",
            failures);
    }

    private static object? GetPointerTrackingSubscription(Control? decoratedBox)
    {
        return decoratedBox == null
            ? null
            : GetPrivateField(decoratedBox, "AtomUI.Desktop.Controls.ButtonSpinnerDecoratedBox", "_pointerTrackingSubscription");
    }

    private static int CountVisualsByTypeName(Control root, string typeName)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.GetType().Name == typeName);
    }
}

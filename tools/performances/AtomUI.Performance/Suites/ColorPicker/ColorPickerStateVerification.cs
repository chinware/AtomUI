using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace AtomUI.Performance;

using AtomColorPicker = AtomUI.Desktop.Controls.ColorPicker;

internal static partial class Program
{
    private static bool RunColorPickerStateVerification()
    {
        var failures = new List<string>();
        VerifyColorPickerClickTrigger(failures);
        VerifyColorPickerHoverIgnoresClickHandler(failures);
        VerifyColorPickerFocusTrigger(failures);
        VerifyColorPickerTriggerSubscriptionLifecycle(failures);
        VerifyColorPickerWindowDeactivatedSubscriptionLifecycle(failures);
        VerifyGradientColorPickerViewTemplateBindings(failures);
        VerifyColorSpectrumBrushReuse(failures);
        VerifyGradientColorPickerTrackPropertyOwner(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ColorPicker state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ColorPicker state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyColorPickerClickTrigger(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        using var realized = RealizeControl(picker);

        Expect(!GetColorPickerIsOpen(picker),
            "Click ColorPicker should start closed.",
            failures);
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "Click ColorPicker should not create per-instance trigger subscriptions.",
            failures);

        Expect(RaisePointerPressedAttemptsColorPickerOpen(picker, realized.Window, failures),
            "Click ColorPicker should attempt to open on anchor pointer press.",
            failures);
    }

    private static void VerifyColorPickerHoverIgnoresClickHandler(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Hover;

        using var realized = RealizeControl(picker);
        RaisePrimaryPointerPressed(picker, realized.Window);
        RefreshLayout(realized.Window);

        Expect(!GetColorPickerIsOpen(picker),
            "Hover ColorPicker should not be toggled by the click class handler.",
            failures);
    }

    private static void VerifyColorPickerFocusTrigger(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Focus;

        using var realized = RealizeControl(picker);
        Expect(RaisePointerPressedAttemptsColorPickerOpen(picker, realized.Window, failures),
            "Focus ColorPicker should still open through its focus-mode pointer handler.",
            failures);
    }

    private static void VerifyColorPickerTriggerSubscriptionLifecycle(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Hover;

        var realized = RealizeControl(picker);
        Expect(GetColorPickerTriggerSubscriptions(picker) != null,
            "Hover ColorPicker should create trigger subscriptions while attached.",
            failures);

        picker.TriggerType = FlyoutTriggerType.Click;
        RefreshLayout(realized.Window);
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "ColorPicker should release trigger subscriptions after switching to Click.",
            failures);

        picker.TriggerType = FlyoutTriggerType.Focus;
        RefreshLayout(realized.Window);
        Expect(GetColorPickerTriggerSubscriptions(picker) != null,
            "Focus ColorPicker should create trigger subscriptions while attached.",
            failures);

        realized.Dispose();
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "Detached ColorPicker should release trigger subscriptions.",
            failures);
    }

    private static void VerifyColorPickerWindowDeactivatedSubscriptionLifecycle(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        var realized = RealizeControl(picker);
        var deactivatedWindow = new AtomUI.Desktop.Controls.Window();

        Expect(GetColorPickerDeactivatedWindow(picker) == null,
            "Closed ColorPicker should not subscribe to Window.Deactivated.",
            failures);

        RegisterColorPickerWindowDeactivatedHandler(picker, deactivatedWindow);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(GetColorPickerDeactivatedWindow(picker), deactivatedWindow),
            "Open ColorPicker should subscribe to Window.Deactivated.",
            failures);

        InvokeColorPickerLifecycle(picker, "NotifyPickerClosed");
        RefreshLayout(realized.Window);
        Expect(GetColorPickerDeactivatedWindow(picker) == null,
            "Closed ColorPicker should release Window.Deactivated.",
            failures);

        RegisterColorPickerWindowDeactivatedHandler(picker, deactivatedWindow);
        RefreshLayout(realized.Window);
        realized.Dispose();
        Expect(GetColorPickerDeactivatedWindow(picker) == null,
            "Detached ColorPicker should release Window.Deactivated.",
            failures);
    }

    private static void VerifyGradientColorPickerTrackPropertyOwner(ICollection<string> failures)
    {
        var trackType = Type.GetType(
            "AtomUI.Desktop.Controls.GradientColorPickerTrack, AtomUI.Desktop.Controls.ColorPicker");
        var property = trackType?
            .GetField("ActivatedThumbProperty", BindingFlags.Public | BindingFlags.Static)?
            .GetValue(null) as Avalonia.AvaloniaProperty;

        Expect(trackType is not null && property?.OwnerType == trackType,
            $"GradientColorPickerTrack.ActivatedThumbProperty should be registered on GradientColorPickerTrack. Actual owner: {property?.OwnerType.FullName ?? "<null>"}.",
            failures);
    }

    private static void VerifyGradientColorPickerViewTemplateBindings(ICollection<string> failures)
    {
        var initialGradient = CreateColorPickerGradient();
        var view = new GradientColorPickerView
        {
            DefaultValue          = initialGradient,
            IsPaletteGroupEnabled = false
        };

        using var realized = RealizeControl(view);
        var slider = FindVisualByName<Control>(view, "PART_GradientColorSlider");
        Expect(slider != null,
            "GradientColorPickerView should create PART_GradientColorSlider.",
            failures);
        if (slider is null)
        {
            return;
        }

        Expect(ReferenceEquals(GetControlProperty<LinearGradientBrush?>(slider, "GradientValue"), view.Value),
            "GradientColorSlider should receive GradientColorPickerView.Value.",
            failures);

        var parentGradient = CreateColorPickerGradient("#ff4d4f", "#722ed1");
        view.Value = parentGradient;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(GetControlProperty<LinearGradientBrush?>(slider, "GradientValue"), parentGradient),
            "GradientColorSlider should update when GradientColorPickerView.Value changes.",
            failures);

        var childGradient = CreateColorPickerGradient("#52c41a", "#13c2c2");
        SetControlProperty(slider, "GradientValue", childGradient);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(view.Value, childGradient),
            "GradientColorPickerView.Value should update when GradientColorSlider.GradientValue changes.",
            failures);

        var hsv = Color.Parse("#1677ff").ToHsv();
        SetControlProperty(view, "HsvValue", hsv);
        RefreshLayout(realized.Window);
        Expect(GetControlProperty<HsvColor>(slider, "ActivatedHsvValue").Equals(hsv),
            "GradientColorSlider should receive GradientColorPickerView.HsvValue.",
            failures);

        SetControlProperty(slider, "ActivatedStopIndex", 1);
        RefreshLayout(realized.Window);
        Expect(GetControlProperty<int?>(view, "ActivatedStopIndex") == 1,
            "GradientColorPickerView.ActivatedStopIndex should update from GradientColorSlider.",
            failures);
    }

    private static void VerifyColorSpectrumBrushReuse(ICollection<string> failures)
    {
        var view = CreateColorPickerView();

        using var realized = RealizeControl(view);
        var spectrum = FindVisualByName<Control>(view, "ColorSpectrum");
        var spectrumRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumRectangle");
        var spectrumOverlayRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumOverlayRectangle");

        Expect(spectrum != null,
            "ColorPickerView should create ColorSpectrum.",
            failures);
        Expect(spectrumRectangle != null,
            "ColorSpectrum should create PART_SpectrumRectangle.",
            failures);
        Expect(spectrumOverlayRectangle != null,
            "ColorSpectrum should create PART_SpectrumOverlayRectangle.",
            failures);
        if (spectrum is null ||
            spectrumRectangle is null ||
            spectrumOverlayRectangle is null)
        {
            return;
        }

        if (!WaitForColorSpectrumBrushes(realized.Window, spectrumRectangle, spectrumOverlayRectangle))
        {
            failures.Add("ColorSpectrum should create initial spectrum brushes.");
            return;
        }

        SetControlProperty(spectrum, "HsvColor", new HsvColor(1, 220, 0.7, 0.8));
        RefreshLayout(realized.Window);
        var blueBaseBrush = spectrumRectangle.Fill;
        var blueOverlayBrush = spectrumOverlayRectangle.Fill;
        var blueOpacity = spectrumOverlayRectangle.Opacity;

        SetControlProperty(spectrum, "HsvColor", new HsvColor(1, 230, 0.65, 0.75));
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(spectrumRectangle.Fill, blueBaseBrush),
            "ColorSpectrum should reuse the base ImageBrush while hue stays in the same sextant.",
            failures);
        Expect(ReferenceEquals(spectrumOverlayRectangle.Fill, blueOverlayBrush),
            "ColorSpectrum should reuse the overlay ImageBrush while hue stays in the same sextant.",
            failures);
        Expect(Math.Abs(spectrumOverlayRectangle.Opacity - blueOpacity) > 0.001,
            "ColorSpectrum should still update overlay opacity when reusing brushes.",
            failures);

        SetControlProperty(spectrum, "HsvColor", new HsvColor(1, 30, 0.6, 0.7));
        RefreshLayout(realized.Window);
        var redBaseBrush = spectrumRectangle.Fill;
        var redOverlayBrush = spectrumOverlayRectangle.Fill;
        var redOpacity = spectrumOverlayRectangle.Opacity;
        Expect(!ReferenceEquals(redBaseBrush, blueBaseBrush),
            "ColorSpectrum should switch base ImageBrush when hue moves to another sextant.",
            failures);
        Expect(!ReferenceEquals(redOverlayBrush, blueOverlayBrush),
            "ColorSpectrum should switch overlay ImageBrush when hue moves to another sextant.",
            failures);

        SetControlProperty(spectrum, "HsvColor", new HsvColor(1, 45, 0.55, 0.65));
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(spectrumRectangle.Fill, redBaseBrush),
            "ColorSpectrum should reuse the new sextant base ImageBrush.",
            failures);
        Expect(ReferenceEquals(spectrumOverlayRectangle.Fill, redOverlayBrush),
            "ColorSpectrum should reuse the new sextant overlay ImageBrush.",
            failures);
        Expect(Math.Abs(spectrumOverlayRectangle.Opacity - redOpacity) > 0.001,
            "ColorSpectrum should keep updating opacity after a sextant switch.",
            failures);
    }

    private static bool WaitForColorSpectrumBrushes(Avalonia.Controls.Window window,
                                                    Rectangle spectrumRectangle,
                                                    Rectangle spectrumOverlayRectangle)
    {
        for (var i = 0; i < 50; i++)
        {
            RefreshLayout(window);
            if (spectrumRectangle.Fill != null &&
                spectrumOverlayRectangle.Fill != null)
            {
                return true;
            }
            System.Threading.Thread.Sleep(20);
        }
        return false;
    }

    private static bool GetColorPickerIsOpen(AbstractColorPicker picker)
        => GetColorPickerProperty<bool>(picker, "IsPickerOpen");

    private static object? GetColorPickerPresenter(AbstractColorPicker picker)
        => GetColorPickerProperty<object?>(picker, "PickerPresenter");

    private static object? GetColorPickerTriggerSubscriptions(AbstractColorPicker picker)
        => GetPrivateField(picker, "AtomUI.Desktop.Controls.AbstractColorPicker", "_triggerSubscriptions");

    private static object? GetColorPickerDeactivatedWindow(AbstractColorPicker picker)
        => GetPrivateField(picker, "AtomUI.Desktop.Controls.AbstractColorPicker", "_deactivatedWindow");

    private static void InvokeColorPickerLifecycle(AbstractColorPicker picker, string methodName)
    {
        var method = picker.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(picker, null);
    }

    private static void RegisterColorPickerWindowDeactivatedHandler(AbstractColorPicker picker,
                                                                    AtomUI.Desktop.Controls.Window window)
    {
        var method = typeof(AbstractColorPicker)
            .GetMethod(
                "RegisterWindowDeactivatedHandler",
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(AtomUI.Desktop.Controls.Window) },
                modifiers: null);
        if (method is null)
        {
            throw new MissingMethodException(
                typeof(AbstractColorPicker).FullName,
                "RegisterWindowDeactivatedHandler");
        }
        method?.Invoke(picker, new object?[] { window });
    }

    private static bool RaisePointerPressedAttemptsColorPickerOpen(AtomColorPicker picker,
                                                                   Avalonia.Controls.Window window,
                                                                   ICollection<string> failures)
    {
        try
        {
            RaisePrimaryPointerPressed(picker, window);
            RefreshLayout(window);
            return GetColorPickerIsOpen(picker) || GetColorPickerPresenter(picker) != null;
        }
        catch (InvalidOperationException exception) when (IsMissingPopupHostException(exception))
        {
            return true;
        }
        catch (Exception exception)
        {
            failures.Add($"ColorPicker pointer press should not throw {exception.GetType().Name}: {exception.Message}");
            return false;
        }
    }

    private static T GetColorPickerProperty<T>(AbstractColorPicker picker, string propertyName)
    {
        var property = typeof(AbstractColorPicker).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        return property is null ? default! : (T)property.GetValue(picker)!;
    }

    private static T GetControlProperty<T>(Control control, string propertyName)
    {
        var property = control.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return property is null ? default! : (T)property.GetValue(control)!;
    }

    private static void SetControlProperty(Control control, string propertyName, object? value)
    {
        var property = control.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(control, value);
    }
}

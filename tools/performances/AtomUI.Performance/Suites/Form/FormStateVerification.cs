using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunFormStateVerification()
    {
        _ = new Form();
        var failures = new List<string>();
        VerifyFormContentReplacementLifecycle(failures);
        VerifyFormConfigAndFeedbackPropagation(failures);
        VerifyFormTooltipIconLifecycle(failures);
        VerifyFormItemDebounceCancellation(failures);
        VerifyFormItemWindowSubscriptionLifecycle(failures);
        VerifyFormItemDecoratorReplacementLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Form state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Form state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyFormContentReplacementLifecycle(ICollection<string> failures)
    {
        var first = new ProbeFormInput();
        var second = new ProbeFormInput();
        var item = CreateProbeFormItem(first);
        var form = CreateVerificationForm(item);
        var valueChangedCount = 0;
        form.ItemValueChanged += (_, _) => valueChangedCount++;

        using var realized = RealizeControl(form);
        first.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 1,
            $"Initial FormItem content should raise one Form.ItemValueChanged event, actual {valueChangedCount}.",
            failures);

        item.Content = second;
        RefreshLayout(realized.Window);
        first.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 1,
            "Replacing FormItem.Content should unsubscribe the old content ValueChanged handler.",
            failures);

        second.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 2,
            $"Replacement FormItem content should raise Form.ItemValueChanged, actual {valueChangedCount}.",
            failures);
    }

    private static void VerifyFormConfigAndFeedbackPropagation(ICollection<string> failures)
    {
        var input = new ProbeFormInput();
        var item = CreateProbeFormItem(input);
        var form = CreateVerificationForm(item);
        form.FeedbackTemplate = new FormFeedbackTemplate();
        form.IsValidateFeedbackEnabled = true;

        using var realized = RealizeControl(form);
        Expect(input.FeedbackControl != null,
            "FormItem should provide feedback control when Form.IsValidateFeedbackEnabled=true.",
            failures);

        form.SizeType = SizeType.Large;
        form.StyleVariant = InputControlStyleVariant.Filled;
        form.IsMotionEnabled = true;
        form.ValidateTrigger = FormValidateTrigger.OnBlur;
        RefreshLayout(realized.Window);

        Expect(input.SizeType == SizeType.Large,
            $"FormItem content SizeType should follow Form.SizeType, actual {input.SizeType}.",
            failures);
        Expect(input.StyleVariant == InputControlStyleVariant.Filled,
            $"FormItem content StyleVariant should follow Form.StyleVariant, actual {input.StyleVariant}.",
            failures);
        Expect(input.IsMotionEnabled,
            "FormItem content IsMotionEnabled should follow Form.IsMotionEnabled.",
            failures);
        Expect(item.ValidateTrigger == FormValidateTrigger.OnBlur,
            $"FormItem ValidateTrigger should follow Form.ValidateTrigger, actual {item.ValidateTrigger}.",
            failures);

        form.IsValidateFeedbackEnabled = false;
        RefreshLayout(realized.Window);
        Expect(input.FeedbackControl == null,
            "Disabling Form.IsValidateFeedbackEnabled should clear the content feedback control.",
            failures);

        form.IsValidateFeedbackEnabled = true;
        RefreshLayout(realized.Window);
        Expect(input.FeedbackControl != null,
            "Re-enabling Form.IsValidateFeedbackEnabled should recreate the content feedback control.",
            failures);
    }

    private static void VerifyFormTooltipIconLifecycle(ICollection<string> failures)
    {
        var item = CreateProbeFormItem(new ProbeFormInput());
        using var realized = RealizeControl(item);

        Expect(item.TooltipIcon == null,
            "FormItem without Tooltip should not create the default tooltip icon.",
            failures);
        Expect(FindVisualByTypeName(item, "QuestionCircleOutlined") == null,
            "FormItem without Tooltip should not materialize QuestionCircleOutlined.",
            failures);

        item.Tooltip = "Help text";
        RefreshLayout(realized.Window);
        Expect(item.TooltipIcon != null,
            "Assigning Tooltip should create the default tooltip icon.",
            failures);
        Expect(FindVisualByTypeName(item, "QuestionCircleOutlined") != null,
            "Assigning Tooltip should materialize QuestionCircleOutlined.",
            failures);

        item.Tooltip = null;
        RefreshLayout(realized.Window);
        Expect(item.TooltipIcon == null,
            "Clearing Tooltip should release the default tooltip icon.",
            failures);
    }

    private static void VerifyFormItemDebounceCancellation(ICollection<string> failures)
    {
        var validator = new CountingFormValidator();
        var input = new ProbeFormInput();
        var item = CreateProbeFormItem(input);
        item.ValidateDebounce = TimeSpan.FromMilliseconds(50);
        item.ValidateTrigger = FormValidateTrigger.OnChanged;
        item.Validators = [validator];
        var form = CreateVerificationForm(item);
        var valueChangedCount = 0;
        form.ItemValueChanged += (_, _) => valueChangedCount++;

        using (RealizeControl(form))
        {
            input.RaiseValueChanged();
            input.RaiseValueChanged();
            Dispatcher.UIThread.RunJobs();
            Expect(GetPrivateField(item, "AtomUI.Desktop.Controls.FormItem", "_validationDebounceDisposable") != null,
                "Debounced FormItem validation should keep one pending timer before it fires.",
                failures);
        }

        Expect(valueChangedCount == 2,
            $"FormItem debounce test should receive both content changes, actual {valueChangedCount}.",
            failures);
        Expect(GetPrivateField(item, "AtomUI.Desktop.Controls.FormItem", "_validationDebounceDisposable") == null,
            "Detached FormItem should clear pending debounce timer disposable.",
            failures);
        Expect(GetPrivateField(item, "AtomUI.Desktop.Controls.FormItem", "_validationTokenSource") == null,
            "Detached FormItem should clear validation token source.",
            failures);
    }

    private static void VerifyFormItemWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var item = CreateProbeFormItem(new ProbeFormInput());
        using (RealizeControl(item))
        {
            // Attach once so the subscription path runs.
        }

        Expect(GetPrivateField(item, "AtomUI.Desktop.Controls.FormItem", "_attachedWindow") == null,
            "Detached FormItem should clear Window.MediaBreakPointChanged subscription state.",
            failures);
    }

    private static void VerifyFormItemDecoratorReplacementLifecycle(ICollection<string> failures)
    {
        var first = new ProbeFormInput();
        var second = new ProbeFormInput();
        var decorator = new FormItemDecorator
        {
            Child = first
        };
        var formItemAware = (IFormItemAware)decorator;
        var valueChangedCount = 0;
        formItemAware.ValueChanged += (_, _) => valueChangedCount++;

        using var realized = RealizeControl(decorator);
        decorator.SizeType = SizeType.Small;
        decorator.StyleVariant = InputControlStyleVariant.Borderless;
        decorator.IsMotionEnabled = true;
        RefreshLayout(realized.Window);
        Expect(first.SizeType == SizeType.Small &&
               first.StyleVariant == InputControlStyleVariant.Borderless &&
               first.IsMotionEnabled,
            "FormItemDecorator should propagate SizeType, StyleVariant and IsMotionEnabled to its child.",
            failures);

        first.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 1,
            $"Initial FormItemDecorator child should relay ValueChanged once, actual {valueChangedCount}.",
            failures);

        decorator.Child = second;
        RefreshLayout(realized.Window);
        first.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 1,
            "Replacing FormItemDecorator.Child should unsubscribe the old child ValueChanged handler.",
            failures);

        second.RaiseValueChanged();
        RefreshLayout(realized.Window);
        Expect(valueChangedCount == 2,
            $"Replacement FormItemDecorator child should relay ValueChanged, actual {valueChangedCount}.",
            failures);
    }

    private static Form CreateVerificationForm(FormItem item)
    {
        var form = new Form();
        form.Items.Add(item);
        return form;
    }

    private static FormItem CreateProbeFormItem(ProbeFormInput input)
    {
        return new FormItem
        {
            LabelText = "Field",
            FieldName = "field",
            Content = input
        };
    }

    private sealed class ProbeFormInput : Control,
                                          IFormItemAware,
                                          ISizeTypeAware,
                                          IMotionAwareControl,
                                          IInputControlStyleVariantAware,
                                          IFormItemFeedbackAware
    {
        public static readonly StyledProperty<SizeType> SizeTypeProperty =
            SizeTypeControlProperty.SizeTypeProperty.AddOwner<ProbeFormInput>();

        public static readonly StyledProperty<bool> IsMotionEnabledProperty =
            MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ProbeFormInput>();

        public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
            InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<ProbeFormInput>();

        private object? _value;

        public SizeType SizeType
        {
            get => GetValue(SizeTypeProperty);
            set => SetValue(SizeTypeProperty, value);
        }

        public bool IsMotionEnabled
        {
            get => GetValue(IsMotionEnabledProperty);
            set => SetValue(IsMotionEnabledProperty, value);
        }

        public InputControlStyleVariant StyleVariant
        {
            get => GetValue(StyleVariantProperty);
            set => SetValue(StyleVariantProperty, value);
        }

        public FormValidateFeedback? FeedbackControl { get; private set; }

        public event EventHandler? ValueChanged;

        public void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetFormValue(object? value)
        {
            _value = value;
        }

        public object? GetFormValue()
        {
            return _value;
        }

        public void ClearFormValue()
        {
            _value = null;
        }

        public void NotifyValidateStatus(FormValidateStatus status)
        {
        }

        public void SetFeedbackControl(FormValidateFeedback? value)
        {
            FeedbackControl = value;
        }
    }

    private sealed class FormFeedbackTemplate : Avalonia.Controls.Templates.IDataTemplate
    {
        public bool Match(object? data)
        {
            return true;
        }

        public Control Build(object? param)
        {
            return new FormValidateFeedback();
        }
    }

    private sealed class CountingFormValidator : IFormValidator
    {
        public int CallCount { get; private set; }

        public string? Message { get; set; }

        public bool WarningOnly { get; set; }

        public Task<FormValidateResult> ValidateAsync(string fieldName, object? value, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(FormValidateResult.Success);
        }
    }
}

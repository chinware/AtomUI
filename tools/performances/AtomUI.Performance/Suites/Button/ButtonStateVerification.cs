using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunButtonStateVerification()
    {
        var failures = new List<string>();
        VerifyButtonPseudoClassUpdates(failures);
        VerifyButtonTemplateFrameTypes(failures);
        VerifyButtonRuntimeSlots(failures);
        VerifyButtonWaveSpiritSlots(failures);
        VerifyWaveSpiritPropertyRegistration(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Button state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Button state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyButtonPseudoClassUpdates(ICollection<string> failures)
    {
        var iconButton = new AtomUI.Desktop.Controls.Button();
        using var iconRealized = RealizeControl(iconButton);
        iconButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IconProperty, new SearchOutlined());
        RefreshLayout(iconRealized.Window);
        Expect(HasPseudoClass(iconButton, ButtonPseudoClass.IconOnly),
            "Button should set :icononly when Icon is assigned at runtime and Content is null.",
            failures);

        iconButton.SetCurrentValue(Avalonia.Controls.ContentControl.ContentProperty, "Search");
        RefreshLayout(iconRealized.Window);
        Expect(!HasPseudoClass(iconButton, ButtonPseudoClass.IconOnly),
            "Button should clear :icononly when Content is assigned at runtime.",
            failures);

        var typedButton = new AtomUI.Desktop.Controls.Button
        {
            Content = "Type"
        };
        using var typeRealized = RealizeControl(typedButton);
        Expect(HasPseudoClass(typedButton, ButtonPseudoClass.DefaultType),
            "Default Button should start with :default.",
            failures);

        typedButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.ButtonTypeProperty, ButtonType.Primary);
        RefreshLayout(typeRealized.Window);
        Expect(HasPseudoClass(typedButton, ButtonPseudoClass.PrimaryType),
            "Button should set :primary when ButtonType changes to Primary at runtime.",
            failures);
        Expect(!HasPseudoClass(typedButton, ButtonPseudoClass.DefaultType),
            "Button should clear :default when ButtonType changes away from Default at runtime.",
            failures);

        typedButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsDangerProperty, true);
        RefreshLayout(typeRealized.Window);
        Expect(HasPseudoClass(typedButton, ButtonPseudoClass.IsDanger),
            "Button should set :danger when IsDanger changes to true at runtime.",
            failures);

        typedButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsDangerProperty, false);
        RefreshLayout(typeRealized.Window);
        Expect(!HasPseudoClass(typedButton, ButtonPseudoClass.IsDanger),
            "Button should clear :danger when IsDanger changes to false at runtime.",
            failures);

        typedButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, true);
        RefreshLayout(typeRealized.Window);
        Expect(HasPseudoClass(typedButton, ButtonPseudoClass.Loading),
            "Button should set :loading when IsLoading changes to true.",
            failures);

        typedButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, false);
        RefreshLayout(typeRealized.Window);
        Expect(!HasPseudoClass(typedButton, ButtonPseudoClass.Loading),
            "Button should clear :loading when IsLoading changes to false.",
            failures);
    }

    private static void VerifyButtonTemplateFrameTypes(ICollection<string> failures)
    {
        var defaultButton = new AtomUI.Desktop.Controls.Button
        {
            Content = "Default"
        };
        using var defaultRealized = RealizeControl(defaultButton);
        Expect(FindVisualByName<Border>(defaultButton, "Frame") != null,
            "Default Button should use Border#Frame.",
            failures);
        Expect(FindVisualByTypeName(defaultButton, "DashedBorder", "Frame") == null,
            "Default Button should not use DashedBorder#Frame.",
            failures);

        var textButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Text,
            Content    = "Text"
        };
        using var textRealized = RealizeControl(textButton);
        Expect(FindVisualByName<Border>(textButton, "Frame") != null,
            "Text Button should use Border#Frame.",
            failures);
        Expect(FindVisualByTypeName(textButton, "DashedBorder", "Frame") == null,
            "Text Button should not use DashedBorder#Frame.",
            failures);

        var dashedButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Dashed,
            Content    = "Dashed"
        };
        using var dashedRealized = RealizeControl(dashedButton);
        Expect(FindVisualByTypeName(dashedButton, "DashedBorder", "Frame") != null,
            "Dashed Button should use DashedBorder#Frame.",
            failures);
    }

    private static void VerifyButtonRuntimeSlots(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Button"
        };
        using var realized = RealizeControl(button);
        Expect(FindVisualByName<Panel>(button, "PART_LoadingIconHost") == null,
            "Non-loading Button should not create PART_LoadingIconHost.",
            failures);
        Expect(FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon") == null,
            "Non-loading Button should not create PART_LoadingIcon.",
            failures);
        Expect(FindVisualByName<IconPresenter>(button, "PART_ButtonIcon") == null,
            "Button without Icon should not create PART_ButtonIcon.",
            failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IconProperty, new SearchOutlined());
        RefreshLayout(realized.Window);
        var buttonIcon = FindVisualByName<IconPresenter>(button, "PART_ButtonIcon");
        Expect(buttonIcon != null,
            "Button should create PART_ButtonIcon when Icon is assigned.",
            failures);
        Expect((buttonIcon?.GetVisualParent() as Control)?.Name == "PART_ContentLayout",
            "Button icon presenter should be attached directly to PART_ContentLayout.",
            failures);
        Expect(buttonIcon is null || buttonIcon.Width > 0,
            "Button icon presenter should keep template selector sizing.",
            failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, true);
        RefreshLayout(realized.Window);
        var loadingIcon = FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon");
        Expect(loadingIcon != null,
            "Button should create PART_LoadingIcon when IsLoading becomes true.",
            failures);
        Expect((loadingIcon?.GetVisualParent() as Control)?.Name == "PART_ContentLayout",
            "Button loading icon should be attached directly to PART_ContentLayout.",
            failures);
        Expect(FindVisualByName<IconPresenter>(button, "PART_ButtonIcon") == null,
            "Loading Button should remove PART_ButtonIcon.",
            failures);
        Expect(buttonIcon?.GetVisualParent() == null,
            "Removed Button icon presenter should not keep a visual parent.",
            failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon") == null,
            "Button should remove PART_LoadingIcon when IsLoading becomes false.",
            failures);
        Expect(loadingIcon?.GetVisualParent() == null,
            "Removed Button loading icon should not keep a visual parent.",
            failures);
        var restoredButtonIcon = FindVisualByName<IconPresenter>(button, "PART_ButtonIcon");
        Expect(restoredButtonIcon != null,
            "Button should recreate PART_ButtonIcon when loading ends and Icon still exists.",
            failures);
        Expect(!ReferenceEquals(buttonIcon, restoredButtonIcon),
            "Button should not reuse a removed icon presenter instance.",
            failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, true);
        RefreshLayout(realized.Window);
        var secondLoadingIcon = FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon");
        Expect(secondLoadingIcon != null,
            "Button should recreate PART_LoadingIcon when loading starts again.",
            failures);
        Expect(!ReferenceEquals(loadingIcon, secondLoadingIcon),
            "Button should not reuse a removed loading icon instance.",
            failures);
        Expect(restoredButtonIcon?.GetVisualParent() == null,
            "Restored Button icon presenter should be removed again when loading starts.",
            failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.ButtonTypeProperty, ButtonType.Dashed);
        RefreshLayout(realized.Window);
        var retemplatedLoadingIcon = FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon");
        Expect(retemplatedLoadingIcon != null,
            "Loading Button should recreate PART_LoadingIcon after template changes.",
            failures);
        Expect(!ReferenceEquals(secondLoadingIcon, retemplatedLoadingIcon),
            "Button should not reuse the old loading icon instance after template changes.",
            failures);
        Expect(secondLoadingIcon?.GetVisualParent() == null,
            "Old Button loading icon should not keep a visual parent after template changes.",
            failures);
    }

    private static void VerifyButtonWaveSpiritSlots(ICollection<string> failures)
    {
        var defaultButton = new AtomUI.Desktop.Controls.Button
        {
            Content = "Default"
        };
        using var defaultRealized = RealizeControl(defaultButton);
        Expect(FindVisualByTypeName(defaultButton, "WaveSpiritDecorator", "PART_WaveSpirit") != null,
            "Default Button should create PART_WaveSpirit.",
            failures);

        var textButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Text,
            Content    = "Text"
        };
        using var textRealized = RealizeControl(textButton);
        Expect(FindVisualByTypeName(textButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Text Button should not create PART_WaveSpirit.",
            failures);

        var linkButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Link,
            Content    = "Link"
        };
        using var linkRealized = RealizeControl(linkButton);
        Expect(FindVisualByTypeName(linkButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Link Button should not create PART_WaveSpirit.",
            failures);

        var noWaveButton = new AtomUI.Desktop.Controls.Button
        {
            Content             = "NoWave",
            IsWaveSpiritEnabled = false
        };
        using var noWaveRealized = RealizeControl(noWaveButton);
        Expect(FindVisualByTypeName(noWaveButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Button should not create PART_WaveSpirit when IsWaveSpiritEnabled is false.",
            failures);

        var loadingButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Primary,
            Content    = "Loading",
            IsLoading  = true
        };
        using var loadingRealized = RealizeControl(loadingButton);
        Expect(FindVisualByTypeName(loadingButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Loading Button should not create PART_WaveSpirit.",
            failures);

        var mutableButton = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Primary,
            Content    = "Mutable"
        };
        using var mutableRealized = RealizeControl(mutableButton);
        var primaryWave = FindVisualByTypeName(mutableButton, "WaveSpiritDecorator", "PART_WaveSpirit");
        Expect(primaryWave != null,
            "Primary Button should create PART_WaveSpirit.",
            failures);

        mutableButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.ButtonTypeProperty, ButtonType.Text);
        RefreshLayout(mutableRealized.Window);
        Expect(FindVisualByTypeName(mutableButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Button should remove PART_WaveSpirit when ButtonType changes to Text.",
            failures);
        Expect(primaryWave?.GetVisualParent() == null,
            "Removed PART_WaveSpirit should not keep a visual parent after ButtonType changes.",
            failures);

        mutableButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.ButtonTypeProperty, ButtonType.Default);
        RefreshLayout(mutableRealized.Window);
        var defaultWave = FindVisualByTypeName(mutableButton, "WaveSpiritDecorator", "PART_WaveSpirit");
        Expect(defaultWave != null,
            "Button should recreate PART_WaveSpirit when ButtonType changes back to Default.",
            failures);
        Expect(!ReferenceEquals(primaryWave, defaultWave),
            "Button should not reuse a removed wave decorator instance.",
            failures);

        mutableButton.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsWaveSpiritEnabledProperty, false);
        RefreshLayout(mutableRealized.Window);
        Expect(FindVisualByTypeName(mutableButton, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Button should remove PART_WaveSpirit when IsWaveSpiritEnabled becomes false.",
            failures);
        Expect(defaultWave?.GetVisualParent() == null,
            "Removed PART_WaveSpirit should not keep a visual parent when wave is disabled.",
            failures);
    }

    private static void VerifyWaveSpiritPropertyRegistration(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            ButtonType = ButtonType.Primary,
            Content    = "Wave"
        };
        using var realized = RealizeControl(button);
        var waveSpirit = FindVisualByTypeName(button, "WaveSpiritDecorator", "PART_WaveSpirit");
        var field = waveSpirit?.GetType().GetField(
            "OpacityMotionDurationProperty",
            BindingFlags.Public | BindingFlags.Static);
        var propertyName = field?.GetValue(null)?.GetType().GetProperty("Name")?.GetValue(field.GetValue(null)) as string;
        Expect(propertyName == "OpacityMotionDuration",
            $"WaveSpiritDecorator.OpacityMotionDurationProperty should be registered as OpacityMotionDuration, actual {propertyName ?? "<null>"}.",
            failures);
    }

    private static bool HasPseudoClass(Control control, string pseudoClass)
    {
        return control.Classes.Contains(pseudoClass);
    }

    private static Control? FindVisualByTypeName(Control root, string typeName, string? name = null)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .FirstOrDefault(control => control.GetType().Name == typeName &&
                                              (name == null || control.Name == name));
    }
}

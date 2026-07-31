using System.Collections.Specialized;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Desktop.Controls;

public class DialogButtonBox : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        AvaloniaProperty.Register<DialogButtonBox, DialogStandardButtons>(nameof(StandardButtons), DialogStandardButton.NoButton);

    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        AvaloniaProperty.Register<DialogButtonBox, DialogStandardButton>(nameof(DefaultStandardButton));

    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        AvaloniaProperty.Register<DialogButtonBox, DialogStandardButton>(nameof(EscapeStandardButton));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogButtonBox>();

    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }

    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }

    public DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public AvaloniaList<DialogButton> CustomButtons { get; } = new();

    #endregion

    #region 公共事件定义

    public event EventHandler? Accepted;
    public event EventHandler<DialogButtonClickedEventArgs>? Clicked;
    public event EventHandler? HelpRequested;
    public event EventHandler? Rejected;

    #endregion

    #region 按钮语言属性定义

    public static readonly StyledProperty<string?> OkButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(OkButtonText));

    public static readonly StyledProperty<string?> OpenButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(OpenButtonText));

    public static readonly StyledProperty<string?> SaveButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(SaveButtonText));

    public static readonly StyledProperty<string?> CancelButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(CancelButtonText));

    public static readonly StyledProperty<string?> CloseButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(CloseButtonText));

    public static readonly StyledProperty<string?> DiscardButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(DiscardButtonText));

    public static readonly StyledProperty<string?> ApplyButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(ApplyButtonText));

    public static readonly StyledProperty<string?> ResetButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(ResetButtonText));

    public static readonly StyledProperty<string?> ReloadButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(ReloadButtonText));

    public static readonly StyledProperty<string?> RestoreDefaultsButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(RestoreDefaultsButtonText));

    public static readonly StyledProperty<string?> HelpButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(HelpButtonText));

    public static readonly StyledProperty<string?> SaveAllButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(SaveAllButtonText));

    public static readonly StyledProperty<string?> YesButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(YesButtonText));

    public static readonly StyledProperty<string?> YesToAllButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(YesToAllButtonText));

    public static readonly StyledProperty<string?> NoButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(NoButtonText));

    public static readonly StyledProperty<string?> NoToAllButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(NoToAllButtonText));

    public static readonly StyledProperty<string?> AbortButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(AbortButtonText));

    public static readonly StyledProperty<string?> RetryButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(RetryButtonText));

    public static readonly StyledProperty<string?> IgnoreButtonTextProperty =
        AvaloniaProperty.Register<DialogButtonBox, string?>(nameof(IgnoreButtonText));

    public string? OkButtonText
    {
        get => GetValue(OkButtonTextProperty);
        set => SetValue(OkButtonTextProperty, value);
    }

    public string? OpenButtonText
    {
        get => GetValue(OpenButtonTextProperty);
        set => SetValue(OpenButtonTextProperty, value);
    }

    public string? SaveButtonText
    {
        get => GetValue(SaveButtonTextProperty);
        set => SetValue(SaveButtonTextProperty, value);
    }

    public string? CancelButtonText
    {
        get => GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }

    public string? CloseButtonText
    {
        get => GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    public string? DiscardButtonText
    {
        get => GetValue(DiscardButtonTextProperty);
        set => SetValue(DiscardButtonTextProperty, value);
    }

    public string? ApplyButtonText
    {
        get => GetValue(ApplyButtonTextProperty);
        set => SetValue(ApplyButtonTextProperty, value);
    }

    public string? ResetButtonText
    {
        get => GetValue(ResetButtonTextProperty);
        set => SetValue(ResetButtonTextProperty, value);
    }

    public string? ReloadButtonText
    {
        get => GetValue(ReloadButtonTextProperty);
        set => SetValue(ReloadButtonTextProperty, value);
    }

    public string? RestoreDefaultsButtonText
    {
        get => GetValue(RestoreDefaultsButtonTextProperty);
        set => SetValue(RestoreDefaultsButtonTextProperty, value);
    }

    public string? HelpButtonText
    {
        get => GetValue(HelpButtonTextProperty);
        set => SetValue(HelpButtonTextProperty, value);
    }

    public string? SaveAllButtonText
    {
        get => GetValue(SaveAllButtonTextProperty);
        set => SetValue(SaveAllButtonTextProperty, value);
    }

    public string? YesButtonText
    {
        get => GetValue(YesButtonTextProperty);
        set => SetValue(YesButtonTextProperty, value);
    }

    public string? YesToAllButtonText
    {
        get => GetValue(YesToAllButtonTextProperty);
        set => SetValue(YesToAllButtonTextProperty, value);
    }

    public string? NoButtonText
    {
        get => GetValue(NoButtonTextProperty);
        set => SetValue(NoButtonTextProperty, value);
    }

    public string? NoToAllButtonText
    {
        get => GetValue(NoToAllButtonTextProperty);
        set => SetValue(NoToAllButtonTextProperty, value);
    }

    public string? AbortButtonText
    {
        get => GetValue(AbortButtonTextProperty);
        set => SetValue(AbortButtonTextProperty, value);
    }

    public string? RetryButtonText
    {
        get => GetValue(RetryButtonTextProperty);
        set => SetValue(RetryButtonTextProperty, value);
    }

    public string? IgnoreButtonText
    {
        get => GetValue(IgnoreButtonTextProperty);
        set => SetValue(IgnoreButtonTextProperty, value);
    }
    #endregion

    #region 内部协作 API

    internal event EventHandler? EffectiveButtonsChanged;

    internal IReadOnlyList<DialogButton> EffectiveButtons => _effectiveButtons;

    #endregion

    private static readonly (
        DialogStandardButton Type,
        DialogButtonRole Role,
        StyledProperty<string?> ContentProperty)[] StandardButtonDefinitions =
    {
        (DialogStandardButton.Ok, DialogButtonRole.AcceptRole, OkButtonTextProperty),
        (DialogStandardButton.Open, DialogButtonRole.AcceptRole, OpenButtonTextProperty),
        (DialogStandardButton.Save, DialogButtonRole.AcceptRole, SaveButtonTextProperty),
        (DialogStandardButton.SaveAll, DialogButtonRole.AcceptRole, SaveAllButtonTextProperty),
        (DialogStandardButton.Retry, DialogButtonRole.AcceptRole, RetryButtonTextProperty),
        (DialogStandardButton.Ignore, DialogButtonRole.AcceptRole, IgnoreButtonTextProperty),
        (DialogStandardButton.Yes, DialogButtonRole.YesRole, YesButtonTextProperty),
        (DialogStandardButton.YesToAll, DialogButtonRole.YesRole, YesToAllButtonTextProperty),
        (DialogStandardButton.Cancel, DialogButtonRole.RejectRole, CancelButtonTextProperty),
        (DialogStandardButton.Close, DialogButtonRole.RejectRole, CloseButtonTextProperty),
        (DialogStandardButton.Abort, DialogButtonRole.RejectRole, AbortButtonTextProperty),
        (DialogStandardButton.No, DialogButtonRole.NoRole, NoButtonTextProperty),
        (DialogStandardButton.NoToAll, DialogButtonRole.NoRole, NoToAllButtonTextProperty),
        (DialogStandardButton.Discard, DialogButtonRole.DestructiveRole, DiscardButtonTextProperty),
        (DialogStandardButton.Help, DialogButtonRole.HelpRole, HelpButtonTextProperty),
        (DialogStandardButton.Reset, DialogButtonRole.ResetRole, ResetButtonTextProperty),
        (DialogStandardButton.Reload, DialogButtonRole.ActionRole, ReloadButtonTextProperty),
        (DialogStandardButton.RestoreDefaults, DialogButtonRole.ResetRole, RestoreDefaultsButtonTextProperty),
        (DialogStandardButton.Apply, DialogButtonRole.ApplyRole, ApplyButtonTextProperty)
    };

    private DockPanel? _leftGroup;
    private DockPanel? _centerGroup;
    private DockPanel? _rightGroup;
    private readonly List<DialogButton> _standardButtons = new();
    private readonly List<IDisposable> _standardButtonBindings = new();
    private readonly HashSet<DialogButton> _subscribedCustomButtons = new();
    private readonly AvaloniaList<DialogButton> _effectiveButtons = new();

    static DialogButtonBox()
    {
        AffectsMeasure<DialogButtonBox>(StandardButtonsProperty);
    }

    public DialogButtonBox()
    {
        CustomButtons.CollectionChanged += new(HandleCustomButtonsChanged);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TemplateProperty)
        {
            ReleaseTemplateParts();
        }
        else if (change.Property == StandardButtonsProperty)
        {
            BuildStandardButtons();
            SynchronizeButtons();
        }
        else if (change.Property == DefaultStandardButtonProperty ||
                 change.Property == EscapeStandardButtonProperty)
        {
            UpdateStandardButtonDefaults();
            NotifyButtonsSynchronized();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseTemplateParts();
        _leftGroup = e.NameScope.Find<DockPanel>("PART_LeftGroup");
        _centerGroup = e.NameScope.Find<DockPanel>("PART_CenterGroup");
        _rightGroup = e.NameScope.Find<DockPanel>("PART_RightGroup");
        BuildStandardButtons();
        SynchronizeButtons();
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SynchronizeCustomButtonSubscriptions();
        SynchronizeButtons();
    }

    private void BuildStandardButtons()
    {
        ReleaseStandardButtons();

        foreach (var definition in StandardButtonDefinitions)
        {
            if (!StandardButtons.HasFlag(definition.Type))
            {
                continue;
            }

            var isDefault = definition.Type == DefaultStandardButton;
            var button = new DialogButton
            {
                Role = definition.Role,
                ButtonType = isDefault ? ButtonType.Primary : ButtonType.Default,
                IsDefaultConfirmButton = isDefault,
                IsDefaultEscapeButton = definition.Type == EscapeStandardButton,
                StandardButtonType = definition.Type
            };
            _standardButtonBindings.Add(
                button.Bind(
                    Button.ContentProperty,
                    this.GetObservable(definition.ContentProperty),
                    BindingPriority.Template));
            _standardButtonBindings.Add(
                button.Bind(Button.IsMotionEnabledProperty, this.GetObservable(IsMotionEnabledProperty)));
            button.Click += HandleButtonClicked;
            _standardButtons.Add(button);
        }
    }

    private void UpdateStandardButtonDefaults()
    {
        foreach (var button in _standardButtons)
        {
            var standardButton = button.StandardButtonType;
            var isDefault = standardButton == DefaultStandardButton;
            button.ButtonType = isDefault ? ButtonType.Primary : ButtonType.Default;
            button.IsDefaultConfirmButton = isDefault;
            button.IsDefaultEscapeButton = standardButton == EscapeStandardButton;
        }
    }

    private void ReleaseStandardButtons()
    {
        var retainedValues = _standardButtons
            .Select(button => (button.Content, button.IsMotionEnabled))
            .ToArray();
        foreach (var button in _standardButtons)
        {
            button.Click -= HandleButtonClicked;
        }

        foreach (var binding in _standardButtonBindings)
        {
            binding.Dispose();
        }

        for (var index = 0; index < _standardButtons.Count; index++)
        {
            var button = _standardButtons[index];
            var values = retainedValues[index];
            button.SetCurrentValue(Button.ContentProperty, values.Content);
            button.SetCurrentValue(Button.IsMotionEnabledProperty, values.IsMotionEnabled);
        }

        _standardButtonBindings.Clear();
        _standardButtons.Clear();
    }

    internal void ReleaseButtons()
    {
        ReleaseTemplateParts();
        ReleaseStandardButtons();
        ReleaseCustomButtonSubscriptions();
        _effectiveButtons.Clear();
    }

    private void SynchronizeCustomButtonSubscriptions()
    {
        var currentButtons = new HashSet<DialogButton>(CustomButtons);
        foreach (var button in new List<DialogButton>(_subscribedCustomButtons))
        {
            if (currentButtons.Contains(button))
            {
                continue;
            }

            button.Click -= HandleButtonClicked;
            _subscribedCustomButtons.Remove(button);
        }

        foreach (var button in currentButtons)
        {
            if (_subscribedCustomButtons.Add(button))
            {
                button.Click += HandleButtonClicked;
            }
        }
    }

    private void ReleaseCustomButtonSubscriptions()
    {
        foreach (var button in _subscribedCustomButtons)
        {
            button.Click -= HandleButtonClicked;
        }

        _subscribedCustomButtons.Clear();
    }

    private void ReleaseTemplateParts()
    {
        ClearButtonPanel(_leftGroup);
        ClearButtonPanel(_centerGroup);
        ClearButtonPanel(_rightGroup);
        _leftGroup = null;
        _centerGroup = null;
        _rightGroup = null;
    }

    private void SynchronizeButtons()
    {
        if (_leftGroup is null || _centerGroup is null || _rightGroup is null)
        {
            return;
        }

        ClearButtonPanel(_leftGroup);
        ClearButtonPanel(_centerGroup);
        ClearButtonPanel(_rightGroup);

        var customButtons = GetDistinctCustomButtons();
        var rightButtons = BuildRightGroupButtons(customButtons);
        var centerButtons = BuildCenterGroupButtons(customButtons);
        var leftButtons = BuildLeftGroupButtons(customButtons);

        PopulateButtonPanel(_rightGroup, rightButtons, Dock.Right);
        PopulateButtonPanel(_centerGroup, centerButtons, Dock.Left);
        PopulateButtonPanel(_leftGroup, leftButtons, Dock.Left);

        _effectiveButtons.Clear();
        _effectiveButtons.AddRange(rightButtons);
        _effectiveButtons.AddRange(centerButtons);
        _effectiveButtons.AddRange(leftButtons);
        NotifyButtonsSynchronized();
    }

    private List<DialogButton> BuildRightGroupButtons(IReadOnlyList<DialogButton> customButtons)
    {
        var standardAccept = GetRoleButtons(_standardButtons, DialogButtonRole.AcceptRole);
        var customAccept = GetRoleButtons(customButtons, DialogButtonRole.AcceptRole);
        var standardYes = GetRoleButtons(_standardButtons, DialogButtonRole.YesRole);
        var customYes = GetRoleButtons(customButtons, DialogButtonRole.YesRole);
        var buttons = new List<DialogButton>();

        var standardAcceptIndex = AddFirst(buttons, standardAccept);
        var customAcceptIndex = AddFirst(buttons, customAccept);
        var standardYesIndex = AddFirst(buttons, standardYes);
        var customYesIndex = AddFirst(buttons, customYes);

        buttons.AddRange(GetRoleButtons(_standardButtons, DialogButtonRole.RejectRole));
        buttons.AddRange(GetRoleButtons(customButtons, DialogButtonRole.RejectRole));
        buttons.AddRange(GetRoleButtons(_standardButtons, DialogButtonRole.NoRole));
        buttons.AddRange(GetRoleButtons(customButtons, DialogButtonRole.NoRole));
        AddRemaining(buttons, standardAccept, standardAcceptIndex);
        AddRemaining(buttons, customAccept, customAcceptIndex);
        AddRemaining(buttons, standardYes, standardYesIndex);
        AddRemaining(buttons, customYes, customYesIndex);
        buttons.AddRange(GetRoleButtons(customButtons, DialogButtonRole.CustomRole));
        return buttons;
    }

    private List<DialogButton> BuildCenterGroupButtons(IReadOnlyList<DialogButton> customButtons)
    {
        var buttons = new List<DialogButton>();
        buttons.AddRange(GetRoleButtons(_standardButtons, DialogButtonRole.DestructiveRole));
        buttons.AddRange(GetRoleButtons(customButtons, DialogButtonRole.DestructiveRole));
        return buttons;
    }

    private List<DialogButton> BuildLeftGroupButtons(IReadOnlyList<DialogButton> customButtons)
    {
        var buttons = new List<DialogButton>();
        AppendRoleButtons(buttons, customButtons, DialogButtonRole.HelpRole);
        AppendRoleButtons(buttons, customButtons, DialogButtonRole.ResetRole);
        AppendRoleButtons(buttons, customButtons, DialogButtonRole.ApplyRole);
        AppendRoleButtons(buttons, customButtons, DialogButtonRole.ActionRole);
        return buttons;
    }

    private void AppendRoleButtons(
        List<DialogButton> target,
        IReadOnlyList<DialogButton> customButtons,
        DialogButtonRole role)
    {
        target.AddRange(GetRoleButtons(_standardButtons, role));
        target.AddRange(GetRoleButtons(customButtons, role));
    }

    private List<DialogButton> GetDistinctCustomButtons()
    {
        var result = new List<DialogButton>(CustomButtons.Count);
        var seen = new HashSet<DialogButton>();
        foreach (var button in CustomButtons)
        {
            if (seen.Add(button))
            {
                result.Add(button);
            }
        }

        return result;
    }

    private static List<DialogButton> GetRoleButtons(
        IReadOnlyList<DialogButton> buttons,
        DialogButtonRole role)
    {
        var result = new List<DialogButton>();
        foreach (var button in buttons)
        {
            if (button.Role == role)
            {
                result.Add(button);
            }
        }

        return result;
    }

    private static int AddFirst(List<DialogButton> target, IReadOnlyList<DialogButton> source)
    {
        if (source.Count == 0)
        {
            return 0;
        }

        target.Add(source[0]);
        return 1;
    }

    private static void AddRemaining(
        List<DialogButton> target,
        IReadOnlyList<DialogButton> source,
        int startIndex)
    {
        for (var index = startIndex; index < source.Count; index++)
        {
            target.Add(source[index]);
        }
    }

    private static void PopulateButtonPanel(
        Panel panel,
        IReadOnlyList<DialogButton> buttons,
        Dock dock)
    {
        foreach (var button in buttons)
        {
            DockPanel.SetDock(button, dock);
            panel.Children.Add(button);
        }

        panel.Children.Add(new Control());
    }

    private void NotifyButtonsSynchronized()
    {
        if (_leftGroup is null || _centerGroup is null || _rightGroup is null)
        {
            return;
        }

        EffectiveButtonsChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void ClearButtonPanel(Panel? panel)
    {
        panel?.Children.Clear();
    }

    private void HandleButtonClicked(object? sender, EventArgs args)
    {
        if (sender is DialogButton button)
        {
            Clicked?.Invoke(this, new DialogButtonClickedEventArgs(button));
            if (button.Role == DialogButtonRole.AcceptRole)
            {
                Accepted?.Invoke(this, EventArgs.Empty);
            }
            else if (button.Role == DialogButtonRole.RejectRole)
            {
                Rejected?.Invoke(this, EventArgs.Empty);
            }
            else if (button.Role == DialogButtonRole.HelpRole)
            {
                HelpRequested?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.DialogLang;
using AtomUI.Controls.MessageBox;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class DialogButtonBox : TemplatedControl, 
                               IControlSharedTokenResourcesHost,
                               IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        AvaloniaProperty.Register<DialogButtonBox, DialogStandardButtons>(nameof (StandardButtons), DialogStandardButton.NoButton);
    
    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        AvaloniaProperty.Register<DialogButtonBox, DialogStandardButton>(nameof (DefaultStandardButton));
    
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
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public AvaloniaList<Button> CustomButtons { get; } = new ();

    public Dictionary<DialogButtonRole, List<Button>> ButtonGroup => _buttonGroup;
    public Dictionary<DialogButtonRole, List<Button>> StandardButtonGroup => _standardButtonGroup;
    
    #endregion
    
    #region 内部属性定义
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DialogToken.ID;
    Control IMotionAwareControl.PropertyBindTarget => this;
    #endregion

    private StackPanel? _leftGroup;
    private StackPanel? _centerGroup;
    private StackPanel? _rightGroup;
    private List<DialogBoxButton>? _standardButtons;
    private Dictionary<DialogButtonRole, List<Button>> _buttonGroup;
    private Dictionary<DialogButtonRole, List<Button>> _standardButtonGroup;
    private Dictionary<Button, CompositeDisposable> _bindingDisposables;
    
    static DialogButtonBox()
    {
        AffectsMeasure<DialogButtonBox>(StandardButtonsProperty);
    }
    
    public DialogButtonBox()
    {
        this.RegisterResources();
        CustomButtons.CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCustomButtonsChanged);
        _standardButtonGroup            =  new Dictionary<DialogButtonRole, List<Button>>();
        _buttonGroup                    =  new Dictionary<DialogButtonRole, List<Button>>();
        _bindingDisposables             =  new Dictionary<Button, CompositeDisposable>();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StandardButtonsProperty)
        {
            BuildStandardButtons();
            if (this.IsAttachedToVisualTree())
            {
                SyncButtonsToGroup();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _leftGroup = e.NameScope.Find<StackPanel>(DialogButtonBoxThemeConstants.LeftGroupPart);
        _centerGroup = e.NameScope.Find<StackPanel>(DialogButtonBoxThemeConstants.CenterGroupPart);
        _rightGroup = e.NameScope.Find<StackPanel>(DialogButtonBoxThemeConstants.RightGroupPart);
        BuildStandardButtons();
        foreach (var item in CustomButtons)
        {
            DialogButtonRole role = DialogButtonRole.NoRole;
            if (item.Tag is DialogButtonRole buttonRole)
            {
                role = buttonRole;
            }
            AddButtonToGroup(role, item, false);
        }
        SyncButtonsToGroup();
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newItems = e.NewItems!.OfType<Button>();
                foreach (var item in newItems)
                {
                    DialogButtonRole role = DialogButtonRole.NoRole;
                    if (item.Tag is DialogButtonRole buttonRole)
                    {
                        role = buttonRole;
                    }
                    AddButtonToGroup(role, item, false);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldItems = e.OldItems!.OfType<Button>();
                foreach (var item in oldItems)
                {
                    DialogButtonRole role = DialogButtonRole.NoRole;
                    if (item.Tag is DialogButtonRole buttonRole)
                    {
                        role = buttonRole;
                    }

                    RemoveButtonFromGroup(role, item, false);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }

        if (this.IsAttachedToVisualTree())
        {
            SyncButtonsToGroup();
        }
    }

    private void AddButtonToGroup(DialogButtonRole buttonRole, Button button, bool isStandard)
    {
        List<Button>? buttons;
        var targetGroup = isStandard ? _standardButtonGroup : _buttonGroup;
        if (!targetGroup.ContainsKey(buttonRole))
        {
            buttons = new List<Button>();
            targetGroup.Add(buttonRole, buttons);
        }
        else
        {
            buttons = targetGroup[buttonRole];
        }
        buttons.Add(button);
    }
    
    private void RemoveButtonFromGroup(DialogButtonRole buttonRole, Button button, bool isStandard)
    {
        var           targetGroup = isStandard ? _standardButtonGroup : _buttonGroup;
        List<Button>? buttons;
        if (!targetGroup.ContainsKey(buttonRole))
        {
            buttons = new List<Button>();
            targetGroup.Add(buttonRole, buttons);
        }
        else
        {
            buttons = targetGroup[buttonRole];
        }
        buttons.Remove(button);
    }

    private void BuildStandardButtons()
    {
        if (_standardButtons != null)
        {
            foreach (var button in _standardButtons)
            {
                var role = button.Tag as DialogButtonRole?;
                Debug.Assert(role != null);
                RemoveButtonFromGroup(role.Value, button, true);
                if (_bindingDisposables.TryGetValue(button, out var disposables))
                {
                    disposables.Dispose();
                    _bindingDisposables.Remove(button);
                }
            }
        }
        if (StandardButtons == DialogStandardButton.NoButton)
        {
            return;
        }
        
        _standardButtons = new List<DialogBoxButton>();
        
        if (StandardButtons.HasFlag(DialogStandardButton.Ok))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Ok ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Ok));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }

        if (StandardButtons.HasFlag(DialogStandardButton.Open))
        {
            var button = new DialogBoxButton
            {
                Tag        = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Open ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Open));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Save))
        {
            var button = new DialogBoxButton
            {
                Tag        = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Save ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Save));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.SaveAll))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.SaveAll ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.SaveAll));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Retry))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Retry ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Retry));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Ignore))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.AcceptRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Ignore ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Ignore));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.AcceptRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Yes))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.YesRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Yes ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Yes));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.YesRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.YesToAll))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.YesRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.YesToAll ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.YesToAll));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.YesRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Cancel))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.RejectRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Cancel ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Cancel));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.RejectRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Close))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.RejectRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Close ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Close));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.RejectRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Abort))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.RejectRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Abort ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Abort));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.RejectRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.No))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.NoRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.No ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.No));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.NoRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.NoToAll))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.NoRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.NoToAll ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.NoToAll));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.NoRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Discard))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.DestructiveRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Discard ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Discard));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.DestructiveRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Help))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.HelpRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Help ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Help));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.HelpRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Reset))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.ResetRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Reset ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Reset));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.ResetRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.RestoreDefaults))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.ResetRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.RestoreDefaults ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.RestoreDefaults));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.ResetRole, button, true);
        }
        
        if (StandardButtons.HasFlag(DialogStandardButton.Apply))
        {
            var button = new DialogBoxButton
            {
                Tag = DialogButtonRole.ApplyRole,
                ButtonType = DefaultStandardButton == DialogStandardButton.Apply ? ButtonType.Primary : ButtonType.Default
            };
            var disposables = new CompositeDisposable();
            disposables.Add(LanguageResourceBinder.CreateBinding(button, Button.ContentProperty, DialogLangResourceKey.Apply));
            _bindingDisposables.Add(button, disposables);
            _standardButtons.Add(button);
            AddButtonToGroup(DialogButtonRole.ApplyRole, button, true);
        }
    }

    private void SyncButtonsToGroup()
    {
        SyncRightGroupButtons();
        SyncCenterGroupButtons();
        SyncLeftGroupButtons();
    }
    
    private void SyncRightGroupButtons()
    {
        Debug.Assert(_rightGroup != null);
        var rightGroupButtons = new List<Button>();
        
        // 后面可以定义各种操作系统的风格，现在先写死
        // 标准
        var standardAcceptList  = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.AcceptRole) ?? new List<Button>();
        var standardAcceptIndex = 0;
        
        if (standardAcceptList.Count > 0)
        {
            var acceptButton = standardAcceptList[standardAcceptIndex++];
            rightGroupButtons.Add(acceptButton);
        }
        
        // 自定义
        var acceptList = _buttonGroup.GetValueOrDefault(DialogButtonRole.AcceptRole) ?? new List<Button>();
        var acceptIndex = 0;
        
        if (acceptList.Count > 0)
        {
            var acceptButton = acceptList[acceptIndex++];
            rightGroupButtons.Add(acceptButton);
        }
        
        // 标准
        var standardYesList  = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.YesRole) ?? new List<Button>();
        var standardYesIndex = 0;
        
        if (standardYesList.Count > 0)
        {
            var yesButton = standardYesList[standardYesIndex++];
            rightGroupButtons.Add(yesButton);
        }
        // 自定义
        
        var yesList  = _buttonGroup.GetValueOrDefault(DialogButtonRole.YesRole) ?? new List<Button>();
        var yesIndex = 0;
        
        if (yesList.Count > 0)
        {
            var yesButton = yesList[yesIndex++];
            rightGroupButtons.Add(yesButton);
        }
        
        // 输出 reject
        // 标准
        var standardRejectList  = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.RejectRole) ?? new List<Button>();
        foreach (var rejectButton in standardRejectList)
        {
            rightGroupButtons.Add(rejectButton);
        }
        
        // 自定义
        var rejectList = _buttonGroup.GetValueOrDefault(DialogButtonRole.RejectRole) ?? new List<Button>();
        foreach (var rejectButton in rejectList)
        {
            rightGroupButtons.Add(rejectButton);
        }
        
        // 输出 No
        var standardNoList  = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.NoRole) ?? new List<Button>();
        foreach (var button in standardNoList)
        {
            rightGroupButtons.Add(button);
        }
        
        // 自定义
        var noList = _buttonGroup.GetValueOrDefault(DialogButtonRole.NoRole) ?? new List<Button>();
        foreach (var button in noList)
        {
            rightGroupButtons.Add(button);
        }
        
        // 输出剩下的 Accept
        // 标准
        for (var i = standardAcceptIndex; i < standardAcceptList.Count; i++)
        {
            foreach (var acceptButton in standardAcceptList)
            {
                rightGroupButtons.Add(acceptButton);
            }
        }
        
        // 自定义

        for (var i = acceptIndex; i < acceptList.Count; i++)
        {
            foreach (var acceptButton in acceptList)
            {
                rightGroupButtons.Add(acceptButton);
            }
        }
        
        // 输出剩下的 Yes
        // 标准
        for (var i = standardYesIndex; i < standardYesList.Count; i++)
        {
            foreach (var button in standardYesList)
            {
                rightGroupButtons.Add(button);
            }
        }
        
        // 自定义

        for (var i = yesIndex; i < yesList.Count; i++)
        {
            foreach (var button in yesList)
            {
                rightGroupButtons.Add(button);
            }
        }
        
        _rightGroup.Children.AddRange(rightGroupButtons);
    }

    private void SyncCenterGroupButtons()
    {
        Debug.Assert(_centerGroup != null);
        var centerGroupButtons = new List<Button>();
        // 标准
        var standardDestructiveList  = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.DestructiveRole) ?? new List<Button>();
        
        foreach (var button in standardDestructiveList)
        {
            centerGroupButtons.Add(button);
        }
        
        // 自定义
        var destructiveList = _buttonGroup.GetValueOrDefault(DialogButtonRole.DestructiveRole) ?? new List<Button>();
        foreach (var button in destructiveList)
        {
            centerGroupButtons.Add(button);
        }
        _centerGroup.Children.AddRange(centerGroupButtons);
    }

    private void SyncLeftGroupButtons()
    {
        Debug.Assert(_leftGroup != null);
        var leftGroupButtons = new List<Button>();
        // 标准
        var standardHelpList = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.HelpRole) ?? new List<Button>();
        foreach (var button in standardHelpList)
        {
            leftGroupButtons.Add(button);
        }
        
        // 自定义
        var helpList = _buttonGroup.GetValueOrDefault(DialogButtonRole.HelpRole) ?? new List<Button>();
        foreach (var button in helpList)
        {
            leftGroupButtons.Add(button);
        }
        
        // 标准
        var standardResetList = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.ResetRole) ?? new List<Button>();
        foreach (var button in standardResetList)
        {
            leftGroupButtons.Add(button);
        }
        
        // 自定义
        var resetList = _buttonGroup.GetValueOrDefault(DialogButtonRole.ResetRole) ?? new List<Button>();
        foreach (var button in resetList)
        {
            leftGroupButtons.Add(button);
        }
        
        // 标准
        var standardApplyList = _standardButtonGroup.GetValueOrDefault(DialogButtonRole.ResetRole) ?? new List<Button>();
        foreach (var button in standardApplyList)
        {
            leftGroupButtons.Add(button);
        }
        
        // 自定义
        var applyList = _buttonGroup.GetValueOrDefault(DialogButtonRole.ApplyRole) ?? new List<Button>();
        foreach (var button in applyList)
        {
            leftGroupButtons.Add(button);
        }
        
        _leftGroup.Children.AddRange(leftGroupButtons);
    }
}
﻿using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

public class TextBox : AvaloniaTextBox,
                       IControlSharedTokenResourcesHost,
                       ITokenResourceConsumer
{
    public const string ErrorPC = ":error";
    public const string WarningPC = ":warning";

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<TextBox>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<TextBox>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<TextBox>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsEnableClearButton));

    public static readonly StyledProperty<bool> IsEnableRevealButtonProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsEnableRevealButton));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public bool IsEnableClearButton
    {
        get => GetValue(IsEnableClearButtonProperty);
        set => SetValue(IsEnableClearButtonProperty, value);
    }

    public bool IsEnableRevealButton
    {
        get => GetValue(IsEnableRevealButtonProperty);
        set => SetValue(IsEnableRevealButtonProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TextBox, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<TextBox, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);

    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }

    internal static readonly DirectProperty<TextBox, bool> EmbedModeProperty =
        AvaloniaProperty.RegisterDirect<TextBox, bool>(nameof(EmbedMode),
            o => o.EmbedMode,
            (o, v) => o.EmbedMode = v);

    private bool _embedMode;

    internal bool EmbedMode
    {
        get => _embedMode;
        set => SetAndRaise(EmbedModeProperty, ref _embedMode, value);
    }

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => LineEditToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;

    static TextBox()
    {
        AffectsRender<TextBox>(BorderBrushProperty, BackgroundProperty);
    }

    public TextBox()
    {
        this.RegisterResources();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AcceptsReturnProperty ||
            change.Property == IsReadOnlyProperty ||
            change.Property == TextProperty ||
            change.Property == IsEnableClearButtonProperty)
        {
            SetupEffectiveShowClearButton();
        }
        else if (change.Property == StatusProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == InnerLeftContentProperty ||
            change.Property == InnerRightContentProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                oldControl.SetTemplatedParent(null);
            }

            if (change.NewValue is Control newControl)
            {
                newControl.SetTemplatedParent(this);
            }
        }
    }

    private void SetupEffectiveShowClearButton()
    {
        if (!IsEnableClearButton)
        {
            IsEffectiveShowClearButton = false;
            return;
        }

        IsEffectiveShowClearButton = !IsReadOnly && !AcceptsReturn && !string.IsNullOrEmpty(Text);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ErrorPC, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(WarningPC, Status == AddOnDecoratedStatus.Warning);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupEffectiveShowClearButton();
    }
}
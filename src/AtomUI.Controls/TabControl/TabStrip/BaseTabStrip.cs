﻿using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTabStrip = Avalonia.Controls.Primitives.TabStrip;

public abstract class BaseTabStrip : AvaloniaTabStrip, ISizeTypeAware
{
    public const string TopPC = ":top";
    public const string RightPC = ":right";
    public const string BottomPC = ":bottom";
    public const string LeftPC = ":left";

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<BaseTabStrip, SizeType>(nameof(SizeType), SizeType.Middle);

    public static readonly StyledProperty<Dock> TabStripPlacementProperty =
        AvaloniaProperty.Register<BaseTabStrip, Dock>(nameof(TabStripPlacement), Dock.Top);

    public static readonly StyledProperty<bool> TabAlignmentCenterProperty =
        AvaloniaProperty.Register<BaseTabStrip, bool>(nameof(TabAlignmentCenter));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public Dock TabStripPlacement
    {
        get => GetValue(TabStripPlacementProperty);
        set => SetValue(TabStripPlacementProperty, value);
    }

    public bool TabAlignmentCenter
    {
        get => GetValue(TabAlignmentCenterProperty);
        set => SetValue(TabAlignmentCenterProperty, value);
    }

    #endregion

    private Border? _frameDecorator;

    static BaseTabStrip()
    {
        ItemsPanelProperty.OverrideDefaultValue<BaseTabStrip>(DefaultPanel);
        AffectsRender<BaseTabStrip>(TabStripPlacementProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frameDecorator = e.NameScope.Find<Border>(BaseTabStripTheme.FrameDecoratorPart);
        SetupBorderBinding();
    }

    private void SetupBorderBinding()
    {
        if (_frameDecorator is not null)
        {
            TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
                GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
                new RenderScaleAwareThicknessConfigure(this));
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabStripItem tabStripItem)
        {
            tabStripItem.TabStripPlacement = TabStripPlacement;
            BindUtils.RelayBind(this, SizeTypeProperty, tabStripItem, TabStripItem.SizeTypeProperty);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TabStripPlacementProperty)
        {
            UpdatePseudoClasses();
            for (var i = 0; i < ItemCount; ++i)
            {
                var itemContainer = ContainerFromIndex(i);
                if (itemContainer is TabStripItem tabStripItem)
                {
                    tabStripItem.TabStripPlacement = TabStripPlacement;
                }
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TopPC, TabStripPlacement == Dock.Top);
        PseudoClasses.Set(RightPC, TabStripPlacement == Dock.Right);
        PseudoClasses.Set(BottomPC, TabStripPlacement == Dock.Bottom);
        PseudoClasses.Set(LeftPC, TabStripPlacement == Dock.Left);
    }

    public override void Render(DrawingContext context)
    {
        Point startPoint      = default;
        Point endPoint        = default;
        var   borderThickness = BorderThickness.Left;
        var   offsetDelta     = borderThickness / 2;
        if (TabStripPlacement == Dock.Top)
        {
            startPoint = new Point(0, Bounds.Height - offsetDelta);
            endPoint   = new Point(Bounds.Width, Bounds.Height - offsetDelta);
        }
        else if (TabStripPlacement == Dock.Right)
        {
            startPoint = new Point(offsetDelta, 0);
            endPoint   = new Point(offsetDelta, Bounds.Height);
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            startPoint = new Point(0, offsetDelta);
            endPoint   = new Point(Bounds.Width, offsetDelta);
        }
        else
        {
            startPoint = new Point(Bounds.Width - offsetDelta, 0);
            endPoint   = new Point(Bounds.Width - offsetDelta, Bounds.Height);
        }

        using var optionState = context.PushRenderOptions(new RenderOptions
        {
            EdgeMode = EdgeMode.Aliased
        });
        context.DrawLine(new Pen(BorderBrush, borderThickness), startPoint, endPoint);
    }
}
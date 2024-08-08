using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class CollapseItem : HeaderedContentControl, ISelectable
{
   #region 公共属性定义
   public static readonly StyledProperty<bool> IsSelectedProperty =
      SelectingItemsControl.IsSelectedProperty.AddOwner<CollapseItem>();
   
   public static readonly StyledProperty<bool> IsShowExpandIconProperty =
      AvaloniaProperty.Register<CollapseItem, bool>(nameof(IsShowExpandIcon));

   public static readonly StyledProperty<PathIcon?> ExpandIconProperty =
      AvaloniaProperty.Register<CollapseItem, PathIcon?>(nameof(ExpandIcon));
   
   public static readonly StyledProperty<object?> AddOnContentProperty =
      AvaloniaProperty.Register<CollapseItem, object?>(nameof(AddOnContent));
   
   public bool IsSelected
   {
      get => GetValue(IsSelectedProperty);
      set => SetValue(IsSelectedProperty, value);
   }
   
   public bool IsShowExpandIcon
   {
      get => GetValue(IsShowExpandIconProperty);
      set => SetValue(IsShowExpandIconProperty, value);
   }
   
   public PathIcon? ExpandIcon
   {
      get => GetValue(ExpandIconProperty);
      set => SetValue(ExpandIconProperty, value);
   }
   
   public object? AddOnContent
   {
      get => GetValue(AddOnContentProperty);
      set => SetValue(AddOnContentProperty, value);
   }
   #endregion

   #region 内部属性定义
   internal static readonly DirectProperty<CollapseItem, SizeType> SizeTypeProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, SizeType>(nameof(SizeType),
                                                              o => o.SizeType,
                                                              (o, v) => o.SizeType = v);
   
   internal static readonly DirectProperty<CollapseItem, bool> IsGhostStyleProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, bool>(nameof(IsGhostStyle),
                                                          o => o.IsGhostStyle,
                                                          (o, v) => o.IsGhostStyle = v);
   
   internal static readonly DirectProperty<CollapseItem, CollapseTriggerType> TriggerTypeProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, CollapseTriggerType>(nameof(TriggerType),
                                                                         o => o.TriggerType,
                                                                         (o, v) => o.TriggerType = v);
   
   internal static readonly DirectProperty<CollapseItem, CollapseExpandIconPosition> ExpandIconPositionProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, CollapseExpandIconPosition>(nameof(ExpandIconPosition),
         o => o.ExpandIconPosition,
         (o, v) => o.ExpandIconPosition = v);
   
   internal static readonly DirectProperty<CollapseItem, Thickness> HeaderBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(HeaderBorderThickness),
                                                               o => o.HeaderBorderThickness,
                                                               (o, v) => o.HeaderBorderThickness = v);
   
   internal static readonly DirectProperty<CollapseItem, Thickness> ContentBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(ContentBorderThickness),
                                                               o => o. ContentBorderThickness,
                                                               (o, v) => o. ContentBorderThickness = v);

   private SizeType _sizeType;
   internal SizeType SizeType
   {
      get => _sizeType;
      set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
   }

   private bool _isGhostStyle = false;
   internal bool IsGhostStyle
   {
      get => _isGhostStyle;
      set => SetAndRaise(IsGhostStyleProperty, ref _isGhostStyle, value);
   }

   private CollapseTriggerType _triggerType = CollapseTriggerType.Header;
   internal CollapseTriggerType TriggerType
   {
      get => _triggerType;
      set => SetAndRaise(TriggerTypeProperty, ref _triggerType, value);
   }

   private CollapseExpandIconPosition _expandIconPosition = CollapseExpandIconPosition.Start;
   internal CollapseExpandIconPosition ExpandIconPosition
   {
      get => _expandIconPosition;
      set => SetAndRaise(ExpandIconPositionProperty, ref _expandIconPosition, value);
   }

   private Thickness _headerBorderThickness;
   internal Thickness HeaderBorderThickness
   {
      get => _headerBorderThickness;
      set => SetAndRaise(HeaderBorderThicknessProperty, ref _headerBorderThickness, value);
   }
   
   private Thickness _contentBorderThickness;
   internal Thickness ContentBorderThickness
   {
      get => _contentBorderThickness;
      set => SetAndRaise(ContentBorderThicknessProperty, ref _contentBorderThickness, value);
   }
   #endregion

   static CollapseItem()
   {
      SelectableMixin.Attach<CollapseItem>(IsSelectedProperty);
      PressedMixin.Attach<CollapseItem>();
      FocusableProperty.OverrideDefaultValue(typeof(CollapseItem), true);
      DataContextProperty.Changed.AddClassHandler<CollapseItem>((x, e) => x.UpdateHeader(e));
      AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabItem>(AutomationControlType.TabItem);
   }
   
   protected override AutomationPeer OnCreateAutomationPeer() => new ListItemAutomationPeer(this);
   
   private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
   {
      if (Header == null) {
         if (obj.NewValue is IHeadered headered) {
            if (Header != headered.Header) {
               SetCurrentValue(HeaderProperty, headered.Header);
            }
         } else {
            if (!(obj.NewValue is Control)) {
               SetCurrentValue(HeaderProperty, obj.NewValue);
            }
         }
      } else {
         if (Header == obj.OldValue) {
            SetCurrentValue(HeaderProperty, obj.NewValue);
         }
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupIconButton();
      HandleSelectedChanged();
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == ExpandIconProperty) {
         SetupIconButton();
      }

      if (VisualRoot is not null) {
         if (change.Property == IsSelectedProperty) {
            HandleSelectedChanged();
         }
      }
   }

   private void HandleSelectedChanged()
   {
      if (Presenter is not null) {
         Presenter.IsVisible = IsSelected;
      }
   }

   private void SetupIconButton()
   {
      if (ExpandIcon is null) {
         ExpandIcon = new PathIcon()
         {
            Kind = "RightOutlined"
         };
      }
      UIStructureUtils.SetTemplateParent(ExpandIcon, this);
   }
}
using AtomUI.Controls.Utils;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaTabStripItem = Avalonia.Controls.Primitives.TabStripItem;

public enum TabSharp
{
   Line,
   Card
}

public class TabStripItem : AvaloniaTabStripItem, IControlCustomStyle, ICustomHitTest
{
   #region 公共属性定义
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      BaseTabStrip.SizeTypeProperty.AddOwner<TabStripItem>();

   public static readonly StyledProperty<PathIcon?> IconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(Icon));
   
   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(CloseIcon));
   
   public static readonly StyledProperty<bool> IsClosableProperty =
      AvaloniaProperty.Register<TabStripItem, bool>(nameof(IsClosable));
   
   public static readonly DirectProperty<TabStripItem, Dock?> TabStripPlacementProperty =
      AvaloniaProperty.RegisterDirect<TabStripItem, Dock?>(nameof(TabStripPlacement), o => o.TabStripPlacement);

   internal static readonly StyledProperty<CornerRadius> CardBorderRadiusProperty =
      AvaloniaProperty.Register<TabStripItem, CornerRadius>(nameof(CardBorderRadius));
   
   internal static readonly DirectProperty<TabStripItem, CornerRadius> CardBorderRadiusSizeProperty =
      AvaloniaProperty.RegisterDirect<TabStripItem, CornerRadius>(nameof(CardBorderRadiusSize),
                                                                  o => o.CardBorderRadiusSize,
                                                                  (o, v) => o.CardBorderRadiusSize = v);
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   public PathIcon? CloseIcon
   {
      get => GetValue(CloseIconProperty);
      set => SetValue(CloseIconProperty, value);
   }
   
   public bool IsClosable
   {
      get => GetValue(IsClosableProperty);
      set => SetValue(IsClosableProperty, value);
   }
   
   private Dock? _tabStripPlacement;
   public Dock? TabStripPlacement
   {
      get => _tabStripPlacement;
      internal set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
   }

   internal CornerRadius CardBorderRadius
   {
      get => GetValue(CardBorderRadiusProperty);
      set => SetValue(CardBorderRadiusProperty, value);
   }
   
   private CornerRadius _cardBorderRadiusSize;
   public CornerRadius CardBorderRadiusSize
   {
      get => _cardBorderRadiusSize;
      internal set => SetAndRaise(CardBorderRadiusSizeProperty, ref _cardBorderRadiusSize, value);
   }
   
   #endregion
   
   #region 内部属性定义
   internal static readonly StyledProperty<TabSharp> ShapeProperty =
      AvaloniaProperty.Register<TabStripItem, TabSharp>(nameof(Shape));
   
   public TabSharp Shape
   {
      get => GetValue(ShapeProperty);
      set => SetValue(ShapeProperty, value);
   }
   #endregion

   private StackPanel? _contentLayout;
   private IControlCustomStyle _customStyle;
   private Border? _decorator;

   public TabStripItem()
   {
      _customStyle = this;
   }

   private void SetupItemIcon()
   {
      if (Icon is not null) {
         UIStructureUtils.SetTemplateParent(Icon, this);
         Icon.Name = BaseTabStripItemTheme.ItemIconPart;
         if (Icon.ThemeType != IconThemeType.TwoTone) {
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.NormalFilledBrushProperty, TabControlResourceKey.ItemColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, TabControlResourceKey.ItemHoverColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, TabControlResourceKey.ItemSelectedColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
         }
         if (_contentLayout is not null) {
            _contentLayout.Children.Insert(0, Icon);
         }
      }
   }

   private void SetupCloseIcon()
   {
      if (CloseIcon is null) {
         CloseIcon = new PathIcon
         {
            Kind = "CloseOutlined"
         };
         TokenResourceBinder.CreateGlobalResourceBinding(CloseIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         TokenResourceBinder.CreateGlobalResourceBinding(CloseIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
      }

      CloseIcon.SetValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      
      UIStructureUtils.SetTemplateParent(CloseIcon, this);
      if (CloseIcon.ThemeType != IconThemeType.TwoTone) {
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorIcon);
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   #region IControlCustomStyle 实现
   
   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _contentLayout = scope.Find<StackPanel>(BaseTabStripItemTheme.ContentLayoutPart);
      _decorator = scope.Find<Border>(BaseTabStripItemTheme.DecoratorPart);
      SetupItemIcon();
      SetupCloseIcon();
      if (Transitions is null) {
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
         Transitions = transitions;
      }

      if (_decorator is not null) {
         TokenResourceBinder.CreateTokenBinding(_decorator, BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template,
                                                new RenderScaleAwareThicknessConfigure(this));
      }

   }
   #endregion

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IconProperty) {
            var oldIcon = change.GetOldValue<PathIcon?>();
            if (oldIcon != null) {
               UIStructureUtils.SetTemplateParent(oldIcon, null);
            }
            SetupItemIcon();
         }
      } else if (change.Property == TabStripPlacementProperty ||
                 change.Property == SizeTypeProperty) {
         if (Shape == TabSharp.Card) {
            if (change.Property == SizeTypeProperty) {
               HandleSizeTypeChanged();
            }
            SetupCardBorderRadius();
         }
      } else if (change.Property == ShapeProperty) {
         if (Shape == TabSharp.Card) {
            SetupCardBorderRadius();
         }
      } else if (change.Property == CloseIconProperty) {
         var oldIcon = change.GetOldValue<PathIcon?>();
         if (oldIcon != null) {
            UIStructureUtils.SetTemplateParent(oldIcon, null);
         }
         SetupCloseIcon();
      }

      if (change.Property == ShapeProperty) {
         HandleShapeChanged();
      }
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      HandleShapeChanged();
      HandleSizeTypeChanged();
      if (Shape == TabSharp.Card) {
         SetupCardBorderRadius();
      }
   }

   private void HandleSizeTypeChanged()
   {
      if (SizeType == SizeType.Large) {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadiusLG);
      } else if (SizeType == SizeType.Middle) {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadius);
      } else {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadiusSM);
      }
   }

   private void HandleShapeChanged()
   {
      if (Shape == TabSharp.Line) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripItemTheme.ID);
      } else {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabStripItemTheme.ID);
      }
   }

   public bool HitTest(Point point)
   {
      return true;
   }

   private void SetupCardBorderRadius()
   {
      if (TabStripPlacement == Dock.Top) {
         CardBorderRadius = new CornerRadius(topLeft: _cardBorderRadiusSize.TopLeft, topRight:_cardBorderRadiusSize.TopRight, bottomLeft:0, bottomRight:0);
      }
   }
}
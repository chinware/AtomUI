using AtomUI.Controls.Utils;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaTabItem = Avalonia.Controls.TabItem;

public class TabItem : AvaloniaTabItem, IControlCustomStyle, ICustomHitTest
{
   #region 公共属性定义

   public static readonly StyledProperty<PathIcon?> IconProperty =
      AvaloniaProperty.Register<TabItem, PathIcon?>(nameof(Icon));

   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<TabItem, PathIcon?>(nameof(CloseIcon));

   public static readonly StyledProperty<bool> IsClosableProperty =
      AvaloniaProperty.Register<TabItem, bool>(nameof(IsClosable));

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

   #endregion

   #region 内部属性定义
   
   internal static readonly StyledProperty<SizeType> SizeTypeProperty =
      BaseTabControl.SizeTypeProperty.AddOwner<TabItem>();

   internal static readonly StyledProperty<TabSharp> ShapeProperty =
      AvaloniaProperty.Register<TabItem, TabSharp>(nameof(Shape));
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public TabSharp Shape
   {
      get => GetValue(ShapeProperty);
      set => SetValue(ShapeProperty, value);
   }

   #endregion

   private StackPanel? _contentLayout;
   private IControlCustomStyle _customStyle;
   private IconButton? _closeButton;

   public TabItem()
   {
      _customStyle = this;
   }

   private void SetupItemIcon()
   {
      if (Icon is not null) {
         UIStructureUtils.SetTemplateParent(Icon, this);
         Icon.Name = BaseTabItemTheme.ItemIconPart;
         if (Icon.ThemeType != IconThemeType.TwoTone) {
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.NormalFilledBrushProperty,
                                                   TabControlTokenResourceKey.ItemColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty,
                                                   TabControlTokenResourceKey.ItemHoverColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty,
                                                   TabControlTokenResourceKey.ItemSelectedColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.DisabledFilledBrushProperty,
                                                   GlobalTokenResourceKey.ColorTextDisabled);
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
         TokenResourceBinder.CreateGlobalResourceBinding(CloseIcon, PathIcon.WidthProperty,
                                                         GlobalTokenResourceKey.IconSizeSM);
         TokenResourceBinder.CreateGlobalResourceBinding(CloseIcon, PathIcon.HeightProperty,
                                                         GlobalTokenResourceKey.IconSizeSM);
      }

      CloseIcon.SetValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);

      UIStructureUtils.SetTemplateParent(CloseIcon, this);
      if (CloseIcon.ThemeType != IconThemeType.TwoTone) {
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.NormalFilledBrushProperty,
                                                GlobalTokenResourceKey.ColorIcon);
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty,
                                                GlobalTokenResourceKey.ColorIconHover);
         TokenResourceBinder.CreateTokenBinding(CloseIcon, PathIcon.DisabledFilledBrushProperty,
                                                GlobalTokenResourceKey.ColorTextDisabled);
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
      _contentLayout = scope.Find<StackPanel>(BaseTabItemTheme.ContentLayoutPart);
      _closeButton = scope.Find<IconButton>(BaseTabItemTheme.ItemCloseButtonPart);

      SetupItemIcon();
      SetupCloseIcon();
      if (Transitions is null) {
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
         Transitions = transitions;
      }

      if (_closeButton is not null) {
         _closeButton.Click += HandleCloseRequest;
      }
   }

   #endregion

   private void HandleCloseRequest(object? sender, RoutedEventArgs args)
   {
      if (Parent is BaseTabControl tabControl) {
         if (tabControl.SelectedItem is TabItem selectedItem) {
            if (selectedItem == this) {
               var selectedIndex = tabControl.SelectedIndex;
               object? newSelectedItem = null;
               if (selectedIndex != 0) {
                  newSelectedItem = tabControl.Items[--selectedIndex];
               }

               tabControl.Items.Remove(this);
               tabControl.SelectedItem = newSelectedItem;
            } else {
               tabControl.Items.Remove(this);
            }
         }
      }
   }

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
      }

      if (change.Property == CloseIconProperty) {
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
   }

   private void HandleShapeChanged()
   {
      if (Shape == TabSharp.Line) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabItemTheme.ID);
      } else {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabItemTheme.ID);
      }
   }

   public bool HitTest(Point point)
   {
      return true;
   }
}
﻿using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaTabStripItem = Avalonia.Controls.Primitives.TabStripItem;

public enum TabSharp
{
   Line,
   Card
}

public class TabStripItem : AvaloniaTabStripItem, IControlCustomStyle
{
   #region 公共属性定义
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      TabStrip.SizeTypeProperty.AddOwner<TabStrip>();

   public static readonly StyledProperty<PathIcon?> IconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(Icon));
   
   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(CloseIcon));
   
   public static readonly StyledProperty<bool> ClosableProperty =
      AvaloniaProperty.Register<TabStripItem, bool>(nameof(Closable));
   
   public static readonly DirectProperty<TabStripItem, Dock?> TabStripPlacementProperty =
      AvaloniaProperty.RegisterDirect<TabStripItem, Dock?>(nameof(TabStripPlacement), o => o.TabStripPlacement);
   
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
   
   public bool Closable
   {
      get => GetValue(ClosableProperty);
      set => SetValue(ClosableProperty, value);
   }
   
   private Dock? _tabStripPlacement;
   public Dock? TabStripPlacement
   {
      get => _tabStripPlacement;
      internal set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
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

   public TabStripItem()
   {
      _customStyle = this;
   }

   private void SetupItemIcon()
   {
      if (Icon is not null) {
         UIStructureUtils.SetTemplateParent(Icon, this);
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
      SetupItemIcon();
      if (Transitions is null) {
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
         Transitions = transitions;
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
            if (Icon is not null) {
               UIStructureUtils.SetTemplateParent(Icon, this);
            }
         }
      }
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (Shape == TabSharp.Line) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripItemTheme.ID);
      } else {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabStripItemTheme.ID);
      }
   }
}
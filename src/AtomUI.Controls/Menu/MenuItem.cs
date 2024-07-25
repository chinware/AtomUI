﻿using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(TopLevelPC)]
public class MenuItem : AvaloniaMenuItem, IControlCustomStyle
{
   public const string TopLevelPC = ":toplevel";
   
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      Menu.SizeTypeProperty.AddOwner<MenuItem>();

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   #endregion
   
   private readonly IControlCustomStyle _customStyle;
   private ContentPresenter? _topLevelContentPresenter;
   internal static PlatformKeyGestureConverter KeyGestureConverter = new PlatformKeyGestureConverter();

   static MenuItem()
   {
      AffectsRender<MenuItem>(BackgroundProperty);
   }
   
   public MenuItem()
   {
      _customStyle = this;
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      Cursor = new Cursor(StandardCursorType.Hand);
      HorizontalAlignment = HorizontalAlignment.Stretch;
      _customStyle.HandleTemplateApplied(e.NameScope);
   }
   
   #region IControlCustomStyle 实现
   void IControlCustomStyle.SetupTransitions()
   {
      if (_topLevelContentPresenter is not null && _topLevelContentPresenter.Transitions is null) {
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
         _topLevelContentPresenter.Transitions = transitions;
      }
   }
   
   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      if (IsTopLevel) {
         _topLevelContentPresenter = scope.Find<ContentPresenter>(TopLevelMenuItemTheme.HeaderPresenterPart);
      }
      _customStyle.SetupTransitions();
      UpdatePseudoClasses();
   }
   #endregion

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == ParentProperty) {
         UpdatePseudoClasses();
      } else if (e.Property == IconProperty) {
         if (Icon is not null && Icon is PathIcon pathIcon) {
            BindUtils.CreateTokenBinding(pathIcon, PathIcon.WidthProperty, MenuResourceKey.ItemIconSize);
            BindUtils.CreateTokenBinding(pathIcon, PathIcon.HeightProperty, MenuResourceKey.ItemIconSize);
            BindUtils.CreateTokenBinding(pathIcon, PathIcon.NormalFilledBrushProperty, MenuResourceKey.ItemColor);
         }
      }
   }

   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(TopLevelPC, IsTopLevel);
   }
}
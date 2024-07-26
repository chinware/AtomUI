using System.Reflection;
using AtomUI.ColorSystem;
using AtomUI.Controls.MotionScene;
using AtomUI.Controls.Utils;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Open)]
public class ToolTip : TemplatedControl, 
                               IShadowMaskInfoProvider,
                               IControlCustomStyle
{
   #region 公共属性定义

      /// <summary>
   /// Defines the <see cref="Content"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> ContentProperty =
      AvaloniaProperty.Register<ToolTip, object?>(nameof(Content));

   /// <summary>
   /// Defines the ToolTip.Tip attached property.
   /// </summary>
   public static readonly AttachedProperty<object?> TipProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, object?>("Tip");

   /// <summary>
   /// Defines the ToolTip.IsOpen attached property.
   /// </summary>
   public static readonly AttachedProperty<bool> IsOpenProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsOpen");

   /// <summary>
   /// Defines the ToolTip.PresetColor attached property.
   /// </summary>
   public static readonly AttachedProperty<PresetColorType?> PresetColorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, PresetColorType?>("PresetColor");

   /// <summary>
   /// Defines the ToolTip.PresetColor attached property.
   /// </summary>
   public static readonly AttachedProperty<Color?> ColorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, Color?>("Color");

   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly AttachedProperty<bool> IsShowArrowProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsShowArrow", true);

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly AttachedProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsPointAtCenter", false);

   /// <summary>
   /// Defines the ToolTip.Placement property.
   /// </summary>
   public static readonly AttachedProperty<PlacementMode> PlacementProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementMode>(
         "Placement", defaultValue: PlacementMode.Top);

   /// <summary>
   /// Defines the ToolTip.HorizontalOffset property.
   /// </summary>
   public static readonly AttachedProperty<double> HorizontalOffsetProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("HorizontalOffset");

   /// <summary>
   /// Defines the ToolTip.VerticalOffset property.
   /// </summary>
   public static readonly AttachedProperty<double> VerticalOffsetProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("VerticalOffset");

   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// </summary>
   public static readonly AttachedProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("MarginToAnchor", double.NaN);

   /// <summary>
   /// Defines the ToolTip.ShowDelay property.
   /// </summary>
   public static readonly AttachedProperty<int> ShowDelayProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("ShowDelay", 400);

   /// <summary>
   /// Defines the ToolTip.BetweenShowDelay property.
   /// </summary>
   public static readonly AttachedProperty<int> BetweenShowDelayProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("BetweenShowDelay", 100);

   /// <summary>
   /// Defines the ToolTip.ShowOnDisabled property.
   /// </summary>
   public static readonly AttachedProperty<bool> ShowOnDisabledProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ShowOnDisabled", defaultValue: false, inherits: true);

   /// <summary>
   /// Defines the ToolTip.ServiceEnabled property.
   /// </summary>
   public static readonly AttachedProperty<bool> ServiceEnabledProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ServiceEnabled", defaultValue: true, inherits: true);
   
   public object? Content
   {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
   }

   #endregion

   #region 内部属性定义

   /// <summary>
   /// Stores the current <see cref="ToolTip"/> instance in the control.
   /// </summary>
   internal static readonly AttachedProperty<ToolTip?> ToolTipProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip?>("ToolTip");
   
   internal static readonly StyledProperty<bool> IsShowArrowEffectiveProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<ToolTip>();
   
   /// <summary>
   /// 是否实际显示箭头
   /// </summary>
   internal bool IsShowArrowEffective
   {
      get => GetValue(IsShowArrowEffectiveProperty);
      set => SetValue(IsShowArrowEffectiveProperty, value);
   }

   internal static readonly StyledProperty<BoxShadows> ShadowsProperty
      = AvaloniaProperty.Register<ToolTip, BoxShadows>(nameof(Shadows));
   
   internal static readonly StyledProperty<TimeSpan> MotionDurationProperty 
      = AvaloniaProperty.Register<ToolTip, TimeSpan>(nameof(MotionDuration));
   
   internal static readonly StyledProperty<double> DefaultMarginToAnchorProperty
      = AvaloniaProperty.Register<ToolTip, double>(nameof(DefaultMarginToAnchor));
   
   internal BoxShadows Shadows
   {
      get => GetValue(ShadowsProperty);
      set => SetValue(ShadowsProperty, value);
   }
   
   internal TimeSpan MotionDuration
   {
      get => GetValue(MotionDurationProperty);
      set => SetValue(MotionDurationProperty, value);
   }
   
   internal double DefaultMarginToAnchor
   {
      get => GetValue(DefaultMarginToAnchorProperty);
      set => SetValue(DefaultMarginToAnchorProperty, value);
   }
   
   internal static readonly DirectProperty<ToolTip, PlacementMode?> FlipPlacementProperty =
      AvaloniaProperty.RegisterDirect<ToolTip, PlacementMode?>(nameof(FlipPlacement),
                                                               o => o.FlipPlacement,
                                                               (o, v) => o.FlipPlacement = v);
   
   private PlacementMode? _flipPlacement;
   
   internal PlacementMode? FlipPlacement
   {
      get => _flipPlacement;
      set => SetAndRaise(FlipPlacementProperty, ref _flipPlacement, value);
   }
   
   #endregion

   static ToolTip()
   {
      IsOpenProperty.Changed.Subscribe(IsOpenChanged);
      IsShowArrowProperty.Changed.Subscribe(IsShowArrowChanged);

      var requestedThemeVariantProperty =
         typeof(ThemeVariant).GetFieldInfoOrThrow("RequestedThemeVariantProperty",
                                                  BindingFlags.Static | BindingFlags.NonPublic);
      RequestedThemeVariantProperty = (StyledProperty<ThemeVariant?>)requestedThemeVariantProperty.GetValue(null)!;
      AffectsRender<ToolTip>(ForegroundProperty,
                             BackgroundProperty);
      AffectsArrange<ToolTip>(FlipPlacementProperty);
   }
   
   public ToolTip()
   {
      _customStyle = this;
   }
   
   #region 附加属性设置方法

    /// <summary>
   /// Gets the value of the ToolTipOld.Tip attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// The content to be displayed in the control's tooltip.
   /// </returns>
   public static object? GetTip(Control element)
   {
      return element.GetValue(TipProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTipOld.Tip attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">The content to be displayed in the control's tooltip.</param>
   public static void SetTip(Control element, object? value)
   {
      element.SetValue(TipProperty, value);
   }

   /// <summary>
   /// Gets the value of the ToolTipOld.IsOpen attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating whether the tool tip is visible.
   /// </returns>
   public static bool GetIsOpen(Control element)
   {
      return element.GetValue(IsOpenProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTipOld.IsOpen attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating whether the tool tip is visible.</param>
   public static void SetIsOpen(Control element, bool value)
   {
      element.SetValue(IsOpenProperty, value);
   }

   /// <summary>
   /// Gets the value of the ToolTipOld.Placement attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating how the tool tip is positioned.
   /// </returns>
   public static PlacementMode GetPlacement(Control element)
   {
      return element.GetValue(PlacementProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTipOld.Placement attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating how the tool tip is positioned.</param>
   public static void SetPlacement(Control element, PlacementMode value)
   {
      element.SetValue(PlacementProperty, value);
   }
   
   /// <summary>
   /// Gets the value of the ToolTip.HorizontalOffset attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating how the tool tip is positioned.
   /// </returns>
   public static double GetHorizontalOffset(Control element)
   {
      return element.GetValue(HorizontalOffsetProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTip.HorizontalOffset attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating how the tool tip is positioned.</param>
   public static void SetHorizontalOffset(Control element, double value)
   {
      element.SetValue(HorizontalOffsetProperty, value);
   }

   /// <summary>
   /// Gets the value of the ToolTip.VerticalOffset attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating how the tool tip is positioned.
   /// </returns>
   public static double GetVerticalOffset(Control element)
   {
      return element.GetValue(VerticalOffsetProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTip.VerticalOffset attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating how the tool tip is positioned.</param>
   public static void SetVerticalOffset(Control element, double value)
   {
      element.SetValue(VerticalOffsetProperty, value);
   }

   /// <summary>
   /// 是否显示箭头
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static bool GetIsShowArrow(Control element)
   {
      return element.GetValue(IsShowArrowProperty);
   }

   /// <summary>
   /// 设置是否显示箭头
   /// </summary>
   /// <param name="element"></param>
   /// <param name="flag"></param>
   public static void SetIsShowArrow(Control element, bool flag)
   {
      element.SetValue(IsShowArrowProperty, flag);
   }

   /// <summary>
   /// 箭头是否始终指向居中位置
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static bool GetIsPointAtCenter(Control element)
   {
      return element.GetValue(IsPointAtCenterProperty);
   }

   /// <summary>
   /// 设置箭头始终指向居中位置
   /// </summary>
   /// <param name="element"></param>
   /// <param name="flag"></param>
   public static void SetIsPointAtCenter(Control element, bool flag)
   {
      element.SetValue(IsPointAtCenterProperty, flag);
   }

   /// <summary>
   /// ToolTipOld Anchor 目标控件的边距
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static double GetMarginToAnchor(Control element)
   {
      return element.GetValue(MarginToAnchorProperty);
   }

   /// <summary>
   /// 设置 ToolTipOld Anchor 目标控件的边距
   /// </summary>
   /// <param name="element"></param>
   /// <param name="margin"></param>
   public static void SetMarginToAnchor(Control element, double margin)
   {
      element.SetValue(MarginToAnchorProperty, margin);
   }

   /// <summary>
   /// Gets the value of the ToolTipOld.ShowDelay attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating the time, in milliseconds, before a tool tip opens.
   /// </returns>
   public static int GetShowDelay(Control element)
   {
      return element.GetValue(ShowDelayProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTipOld.ShowDelay attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating the time, in milliseconds, before a tool tip opens.</param>
   public static void SetShowDelay(Control element, int value)
   {
      element.SetValue(ShowDelayProperty, value);
   }

   /// <summary>
   /// Gets the number of milliseconds since the last tooltip closed during which the tooltip of <paramref name="element"/> will open immediately,
   /// or a negative value indicating that the tooltip will always wait for <see cref="ShowDelayProperty"/> before opening.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   public static int GetBetweenShowDelay(Control element) => element.GetValue(BetweenShowDelayProperty);

   /// <summary>
   /// Sets the number of milliseconds since the last tooltip closed during which the tooltip of <paramref name="element"/> will open immediately.
   /// </summary>
   /// <remarks>
   /// Setting a negative value disables the immediate opening behaviour. The tooltip of <paramref name="element"/> will then always wait until 
   /// <see cref="ShowDelayProperty"/> elapses before showing.
   /// </remarks>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">The number of milliseconds to set, or a negative value to disable the behaviour.</param>
   public static void SetBetweenShowDelay(Control element, int value) =>
      element.SetValue(BetweenShowDelayProperty, value);

   /// <summary>
   /// Gets whether a control will display a tooltip even if it disabled.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   public static bool GetShowOnDisabled(Control element) =>
      element.GetValue(ShowOnDisabledProperty);

   /// <summary>
   /// Sets whether a control will display a tooltip even if it disabled.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">Whether the control is to display a tooltip even if it disabled.</param>
   public static void SetShowOnDisabled(Control element, bool value) =>
      element.SetValue(ShowOnDisabledProperty, value);

   /// <summary>
   /// Gets whether showing and hiding of a control's tooltip will be automatically controlled by Avalonia.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   public static bool GetServiceEnabled(Control element) =>
      element.GetValue(ServiceEnabledProperty);

   /// <summary>
   /// Sets whether showing and hiding of a control's tooltip will be automatically controlled by Avalonia.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">Whether the control is to display a tooltip even if it disabled.</param>
   public static void SetServiceEnabled(Control element, bool value) =>
      element.SetValue(ServiceEnabledProperty, value);

   /// <summary>
   /// 获取预设置的颜色
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static PresetColorType? GetPresetColor(Control element)
   {
      return element.GetValue(PresetColorProperty);
   }

   /// <summary>
   /// 设置预设颜色
   /// </summary>
   /// <param name="element"></param>
   /// <param name="color"></param>
   public static void SetPresetColor(Control element, PresetColorType color)
   {
      element.SetValue(PresetColorProperty, color);
   }

   /// <summary>
   /// 获取预设置的颜色
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static Color? GetColor(Control element)
   {
      return element.GetValue(ColorProperty);
   }

   /// <summary>
   /// 设置预设颜色
   /// </summary>
   /// <param name="element"></param>
   /// <param name="color"></param>
   public static void SetColor(Control element, Color color)
   {
      element.SetValue(ColorProperty, color);
   }

   #endregion
   
   private Popup? _popup;
   private Action<IPopupHost?>? _popupHostChangedHandler;
   private IControlCustomStyle _customStyle;
   private ArrowDecoratedBox? _arrowDecoratedBox;
   internal Control? AdornedControl { get; private set; }
   internal event EventHandler? Closed;
   private static StyledProperty<ThemeVariant?> RequestedThemeVariantProperty;
   
   private static void IsOpenChanged(AvaloniaPropertyChangedEventArgs e)
   {
      var control = (Control)e.Sender;
      var newValue = (bool)e.NewValue!;

      if (newValue) {
         var tip = GetTip(control);
         if (tip == null) return;

         var toolTip = control.GetValue(ToolTipProperty);
         if (toolTip == null || (tip != toolTip && tip != toolTip.Content)) {
            toolTip?.Close();
            toolTip = tip as ToolTip ?? new ToolTip
            {
               Content = tip
            };
            control.SetValue(ToolTipProperty, toolTip);
            toolTip.SetValue(RequestedThemeVariantProperty, control.ActualThemeVariant);
         }

         toolTip.AdornedControl = control;
         toolTip.Open(control);
         toolTip?.UpdatePseudoClasses(newValue);
      } else if (control.GetValue(ToolTipProperty) is { } toolTip) {
         toolTip.AdornedControl = null;
         toolTip.Close();
         toolTip.UpdatePseudoClasses(newValue);
      }
   }

   private static void IsShowArrowChanged(AvaloniaPropertyChangedEventArgs e)
   {
      var control = (Control)e.Sender;
      var toolTip = control.GetValue(ToolTipProperty);
      if (toolTip is not null) {
         // 重新配置
         toolTip.Close();
         control.SetValue(ToolTipProperty, null);
      }
   }

   internal IPopupHost? PopupHost => _popup?.Host;

   event Action<IPopupHost?>? PopupHostChanged
   {
      add => _popupHostChangedHandler += value;
      remove => _popupHostChangedHandler -= value;
   }

   private void SetToolTipColor(Control control)
   {
      // Preset 优先级高
      var presetColorType = GetPresetColor(control);
      var color = GetColor(control);
      if (presetColorType is not null) {
         var presetColor = PresetPrimaryColor.GetColor(presetColorType.Value);
         _arrowDecoratedBox?.SetValue(BackgroundProperty, new SolidColorBrush(presetColor.Color()), BindingPriority.LocalValue);
      } else if (color is not null) {
         _arrowDecoratedBox?.SetValue(BackgroundProperty, new SolidColorBrush(color.Value), BindingPriority.LocalValue);
      }
   }

   /// <summary>
   /// Helper method to set popup's styling and templated parent.
   /// </summary>
   internal void SetPopupParent(Popup popup, Control? newParent)
   {
      if (popup.Parent != null && popup.Parent != newParent) {
         ((ISetLogicalParent)popup).SetParent(null);
      }

      if (popup.Parent == null || popup.PlacementTarget != newParent) {
         ((ISetLogicalParent)popup).SetParent(newParent);
      }
   }

   private static bool TryGetAdorner(Visual target, out Visual? adorned, out Visual? adornerLayer)
   {
      var element = target;
      while (element != null) {
         if (AdornerLayer.GetAdornedElement(element) is { } adornedElement) {
            adorned = adornedElement;
            adornerLayer = AdornerLayer.GetAdornerLayer(adorned);
            return true;
         }

         element = element.GetPropertyOrThrow<Visual?>("VisualParent");
      }

      adorned = null;
      adornerLayer = null;
      return false;
   }

   // TODO 当设置跟随鼠标的时候，需要特殊处理
   private void Open(Control control)
   {
      Close();
      
      if (_popup is null) {
         _popup = new Popup();
         _popup.Child = this;
         _popup.WindowManagerAddShadowHint = false;

         _popup.Opened += OnPopupOpened;
         _popup.Closed += OnPopupClosed;
         _popup.PositionFlipped += OnPopupPositionFlipped;
      }
      
      SetPopupParent(_popup, control);

      BindUtils.CreateTokenBinding(_popup, Popup.MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
      SetToolTipColor(control);
      
      var marginToAnchor = GetMarginToAnchor(control);
      if (double.IsNaN(marginToAnchor)) {
         marginToAnchor = DefaultMarginToAnchor;
      }
      
      var placement = GetPlacement(control);
      
      var offsetX = GetHorizontalOffset(control);
      var offsetY = GetVerticalOffset(control);
      
      _popup.HorizontalOffset = offsetX;
      _popup.VerticalOffset = offsetY; 
      
      var anchorAndGravity = PopupUtils.GetAnchorAndGravity(placement);
      
      // TODO 可能是多余的，因为有那个对反转事件的处理
      SetupArrowPosition(placement);
      SetupPointCenterOffset(control, 
                             placement,
                             anchorAndGravity.Item1,
                             anchorAndGravity.Item2);
      
      // 后期看能不能检测对应字段的改变
      CalculateShowArrowEffective(control);
      
      _popup.MarginToAnchor = marginToAnchor;
      _popup.Placement = placement;
      _popup.PlacementTarget = control;

      Dispatcher.UIThread.Post(() =>
      { 
         _popup.OpenAnimation();
      });
   }

   private bool CalculateShowArrowEffective(Control control)
   {
      if (GetIsShowArrow(control) == false) {
         IsShowArrowEffective = false;
      } else {
         IsShowArrowEffective = PopupUtils.CanEnabledArrow(GetPlacement(control));
      }
      return IsShowArrowEffective;
   }

   private void Close()
   {
      if (_popup is null) {
         return;
      }

      Dispatcher.UIThread.Post(() =>
      { 
         _popup.CloseAnimation();
      });
   }
   
   private void OnPopupPositionFlipped(object? sender, PopupFlippedEventArgs e)
   {
      if (sender is Popup popup) {
         SetupArrowPosition(popup.Placement);
      }
   }
   
   private void OnPopupClosed(object? sender, EventArgs e)
   {
      // This condition is true, when Popup was closed by any other reason outside of ToolTipService/ToolTipOld, keeping IsOpen=true.
      if (AdornedControl is { } adornedControl
          && GetIsOpen(adornedControl)) {
         adornedControl.SetCurrentValue(IsOpenProperty, false);
      }

      _popupHostChangedHandler?.Invoke(null);
      Closed?.Invoke(this, EventArgs.Empty);
   }

   private void OnPopupOpened(object? sender, EventArgs e)
   {
      _popupHostChangedHandler?.Invoke(((Popup)sender!).Host);
   }

   private void UpdatePseudoClasses(bool newValue)
   {
      PseudoClasses.Set(StdPseudoClass.Open, newValue);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      // 手动提前应用模板，因为有某些计算需要
      ApplyStyling();
      ApplyTemplate();
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _arrowDecoratedBox = scope.Find<ArrowDecoratedBox>(ToolTipTheme.ToolTipContainerPart);
   }
   
   #region IControlCustomStyle 实现
   
   public CornerRadius GetMaskCornerRadius()
   {
      return _arrowDecoratedBox!.GetMaskCornerRadius();
   }

   public Rect GetMaskBounds()
   {
      return _arrowDecoratedBox!.GetMaskBounds();
   }
   
   private void SetupArrowPosition(PlacementMode placement, PopupAnchor? anchor = null, PopupGravity? gravity = null)
   {
      var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
      if (_arrowDecoratedBox is not null && arrowPosition is not null) {
         _arrowDecoratedBox.ArrowPosition = arrowPosition.Value;
      }
   }

   private void SetupPointCenterOffset(Control placementTarget, PlacementMode placement, PopupAnchor? anchor = null, PopupGravity? gravity = null)
   {
      var offset =
         CalculatePopupPositionDelta(placementTarget, placement, anchor, gravity);
      _popup!.HorizontalOffset += offset.X;
      _popup.VerticalOffset += offset.Y;
   }
   
   private Point CalculatePopupPositionDelta(Control anchorTarget, PlacementMode placement, PopupAnchor? anchor = null,
                                             PopupGravity? gravity = null)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (GetIsShowArrow(anchorTarget) && GetIsPointAtCenter(anchorTarget)) {
         if (PopupUtils.CanEnabledArrow(placement, anchor, gravity)) {
            var arrowVertexPoint = _arrowDecoratedBox!.ArrowVertexPoint;
            var anchorSize = anchorTarget.Bounds.Size;
            var centerX = anchorSize.Width / 2;
            var centerY = anchorSize.Height / 2;
            // 这里计算不需要全局坐标
            if (placement == PlacementMode.TopEdgeAlignedLeft ||
                placement == PlacementMode.BottomEdgeAlignedLeft) {
               offsetX += centerX - arrowVertexPoint.Item1;
            } else if (placement == PlacementMode.TopEdgeAlignedRight ||
                       placement == PlacementMode.BottomEdgeAlignedRight) {
               offsetX -= centerX - arrowVertexPoint.Item2;
            } else if (placement == PlacementMode.RightEdgeAlignedTop ||
                       placement == PlacementMode.LeftEdgeAlignedTop) {
               offsetY += centerY - arrowVertexPoint.Item1;
            } else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                       placement == PlacementMode.LeftEdgeAlignedBottom) {
               offsetY -= centerY - arrowVertexPoint.Item2;
            }
         }
      }
      return new Point(offsetX, offsetY);
   }
   #endregion
}
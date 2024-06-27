using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.ColorSystem;
using AtomUI.Data;
using AtomUI.Reflection;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaWin = Avalonia.Controls.Window;

/// <summary>
/// A control which pops up a hint when a control is hovered.
/// </summary>
/// <remarks>
/// You will probably not want to create a <see cref="ToolTip"/> control directly: if added to
/// the tree it will act as a simple <see cref="ContentControl"/> styled to look like a tooltip.
/// To add a tooltip to a control, use the <see cref="TipProperty"/> attached property,
/// assigning the content that you want displayed.
/// </remarks>
[PseudoClasses(":open")]
public partial class ToolTip : BorderedStyleControl, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => ToolTipToken.ID;

   /// <summary>
   /// Defines the <see cref="Content"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> ContentProperty =
      AvaloniaProperty.Register<ContentControl, object?>(nameof(Content));
   
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
   public static readonly AttachedProperty<PlacementType> PlacementProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementType>(
         "Placement", defaultValue: PlacementType.Top);

   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// </summary>
   public static readonly AttachedProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("MarginToAnchor", 0);

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

   /// <summary>
   /// Stores the current <see cref="ToolTip"/> instance in the control.
   /// </summary>
   internal static readonly AttachedProperty<ToolTip?> ToolTipProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip?>("ToolTip");
   
   public object? Content
   {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
   }

   private Popup? _popup;
   private Action<IPopupHost?>? _popupHostChangedHandler;
   private AvaloniaWin? _currentAnchorWindow;

   /// <summary>
   /// Initializes static members of the <see cref="ToolTip"/> class.
   /// </summary>
   static ToolTip()
   {
      IsOpenProperty.Changed.Subscribe(IsOpenChanged);

      var requestedThemeVariantProperty =
         typeof(ThemeVariant).GetFieldInfoOrThrow("RequestedThemeVariantProperty",
                                                  BindingFlags.Static | BindingFlags.NonPublic);
      RequestedThemeVariantProperty = (StyledProperty<ThemeVariant?>)requestedThemeVariantProperty.GetValue(null)!;
      AffectsRender<ToolTip>(DefaultBgTokenProperty,
                             ForegroundProperty,
                             BackgroundProperty);
      AffectsArrange<ToolTip>(FlipPlacementProperty);
   }

   public ToolTip()
   {
      _customStyle = this;
      _controlTokenBinder = new ControlTokenBinder(this);
   }

   internal Control? AdornedControl { get; private set; }
   internal event EventHandler? Closed;

   /// <summary>
   /// Gets the value of the ToolTip.Tip attached property.
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
   /// Sets the value of the ToolTip.Tip attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">The content to be displayed in the control's tooltip.</param>
   public static void SetTip(Control element, object? value)
   {
      element.SetValue(TipProperty, value);
   }

   /// <summary>
   /// Gets the value of the ToolTip.IsOpen attached property.
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
   /// Sets the value of the ToolTip.IsOpen attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating whether the tool tip is visible.</param>
   public static void SetIsOpen(Control element, bool value)
   {
      element.SetValue(IsOpenProperty, value);
   }

   /// <summary>
   /// Gets the value of the ToolTip.Placement attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <returns>
   /// A value indicating how the tool tip is positioned.
   /// </returns>
   public static PlacementType GetPlacement(Control element)
   {
      return element.GetValue(PlacementProperty);
   }

   /// <summary>
   /// Sets the value of the ToolTip.Placement attached property.
   /// </summary>
   /// <param name="element">The control to get the property from.</param>
   /// <param name="value">A value indicating how the tool tip is positioned.</param>
   public static void SetPlacement(Control element, PlacementType value)
   {
      element.SetValue(PlacementProperty, value);
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
   /// ToolTip Anchor 目标控件的边距
   /// </summary>
   /// <param name="element"></param>
   /// <returns></returns>
   public static double GetMarginToAnchor(Control element)
   {
      return element.GetValue(MarginToAnchorProperty);
   }

   /// <summary>
   /// 设置 ToolTip Anchor 目标控件的边距
   /// </summary>
   /// <param name="element"></param>
   /// <param name="margin"></param>
   public static void SetMarginToAnchor(Control element, double margin)
   {
      element.SetValue(MarginToAnchorProperty, margin);
   }

   /// <summary>
   /// Gets the value of the ToolTip.ShowDelay attached property.
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
   /// Sets the value of the ToolTip.ShowDelay attached property.
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
         toolTip?.UpdatePseudoClasses(newValue);
      }
   }

   internal IPopupHost? PopupHost => _popup?.Host;

   event Action<IPopupHost?>? PopupHostChanged
   {
      add => _popupHostChangedHandler += value;
      remove => _popupHostChangedHandler -= value;
   }

   private void Open(Control control)
   {
      Close();

      if (_popup is null) {
         _popup = new Popup();
         _popup.Child = this;
         _popup.WindowManagerAddShadowHint = false;

         _popup.Opened += OnPopupOpened;
         _popup.Closed += OnPopupClosed;
      }

      SetToolTipColor(control);
      
      _popup.Placement = PlacementMode.AnchorAndGravity;
      _popup.PlacementTarget = control;
      SetPopupParent(_popup, control);
      _currentAnchorWindow = (TopLevel.GetTopLevel(control) as AvaloniaWin)!;
      var scaling = _currentAnchorWindow.DesktopScaling;
      var anchorRectangle = GetAnchorRectangle(control);
      var anchorRect = new Rect(
         anchorRectangle.TopLeft * scaling,
         anchorRectangle.Size * scaling);
      var parentGeometry = GetParentClientAreaScreenGeometry();
      anchorRect = anchorRect.Translate(parentGeometry.TopLeft);
      var placement = GetPlacement(control);
      var anchorAndGravity = GetAnchorAndGravity(placement);
      _popup.PlacementAnchor = anchorAndGravity.Item1;
      _popup.PlacementGravity = anchorAndGravity.Item2;

      _popup.IsOpen = true;
      
      var offset = CalculateOffset(placement, control);

      _popup.HorizontalOffset = offset.X;
      _popup.VerticalOffset = offset.Y;
      
      var translatedSize = (_popup!.Host as WindowBase)!.ClientSize * scaling;
      var flipInfo = CalculateFlipInfo(translatedSize, anchorRect, anchorAndGravity.Item1, anchorAndGravity.Item2,
                                       offset);
      var effectPlacement = placement;
      if (flipInfo.Item1 || flipInfo.Item2) {
         var flipPlacement = FlipPlacementType(placement);
         var flipAnchorAndGravity = GetAnchorAndGravity(flipPlacement);
         var flipOffset = CalculateOffset(flipPlacement, control);
         _popup.Host!.ConfigurePosition(control,
                                        PlacementMode.AnchorAndGravity,
                                        offset: flipOffset,
                                        anchor: flipAnchorAndGravity.Item1,
                                        gravity: flipAnchorAndGravity.Item2);
         effectPlacement = flipPlacement;
         FlipPlacement = flipPlacement;
         _popup.HorizontalOffset = flipOffset.X;
         _popup.VerticalOffset = flipOffset.Y;
      } else {
         FlipPlacement = null;
      }
      BuildGeometry(GetDirection(effectPlacement));
   }

   private void SetToolTipColor(Control control)
   {
      // Preset 优先级高
      var presetColorType = GetPresetColor(control);
      var color = GetColor(control);
      if (presetColorType is not null) {
         var presetColor = new PresetPrimaryColor(presetColorType.Value);
         SetValue(BackgroundProperty, new SolidColorBrush(presetColor.Color()), BindingPriority.LocalValue);
      } else if (color is not null) {
         SetValue(BackgroundProperty, new SolidColorBrush(color.Value), BindingPriority.LocalValue);
      }
   }

   protected PlacementType FlipPlacementType(PlacementType placement)
   {
      return placement switch
      {
         PlacementType.Left => PlacementType.Right,
         PlacementType.LeftEdgeAlignedTop => PlacementType.RightEdgeAlignedTop,
         PlacementType.LeftEdgeAlignedBottom => PlacementType.RightEdgeAlignedBottom,

         PlacementType.Top => PlacementType.Bottom,
         PlacementType.TopEdgeAlignedLeft => PlacementType.BottomEdgeAlignedLeft,
         PlacementType.TopEdgeAlignedRight => PlacementType.BottomEdgeAlignedRight,

         PlacementType.Right => PlacementType.Left,
         PlacementType.RightEdgeAlignedTop => PlacementType.LeftEdgeAlignedTop,
         PlacementType.RightEdgeAlignedBottom => PlacementType.LeftEdgeAlignedBottom,

         PlacementType.Bottom => PlacementType.Top,
         PlacementType.BottomEdgeAlignedLeft => PlacementType.TopEdgeAlignedLeft,
         PlacementType.BottomEdgeAlignedRight => PlacementType.TopEdgeAlignedRight,
         
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement,
                                                    "Invalid value for PlacementType")
      };
   }

   private Point CalculateOffset(PlacementType placementType, Control control)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var direction = GetDirection(placementType);
      var margin = Math.Max(GetMarginToAnchor(AdornedControl!), _marginXXS);
      if (direction == Direction.Bottom) {
         offsetY += margin;
      } else if (direction == Direction.Top) {
         offsetY += -margin;
      } else if (direction == Direction.Left) {
         offsetX += -margin;
      } else {
         offsetX += margin;
      }

      if (GetIsShowArrow(control) && GetIsPointAtCenter(control)) {
         var anchorSize = control.Bounds.Size;
         var centerX = anchorSize.Width / 2;
         var centerY = anchorSize.Height / 2;
         // 这里计算不需要全局坐标
         if (placementType == PlacementType.TopEdgeAlignedLeft ||
             placementType == PlacementType.BottomEdgeAlignedLeft) {
            offsetX += centerX - ArrowPosition.Item1;
         } else if (placementType == PlacementType.TopEdgeAlignedRight ||
                    placementType == PlacementType.BottomEdgeAlignedRight) {
            offsetX -= centerX - ArrowPosition.Item2;
         } else if (placementType == PlacementType.RightEdgeAlignedTop ||
                    placementType == PlacementType.LeftEdgeAlignedTop) {
            offsetY += centerY - ArrowPosition.Item1;
         } else if (placementType == PlacementType.RightEdgeAlignedBottom ||
                    placementType == PlacementType.LeftEdgeAlignedBottom) {
            offsetY -= centerY - ArrowPosition.Item2;
         }
      }
      return new Point(offsetX, offsetY);
   }

   private Direction GetDirection(PlacementType placement)
   {
      return placement switch
      {
         PlacementType.Left => Direction.Left,
         PlacementType.LeftEdgeAlignedBottom => Direction.Left,
         PlacementType.LeftEdgeAlignedTop => Direction.Left,

         PlacementType.Top => Direction.Top,
         PlacementType.TopEdgeAlignedLeft => Direction.Top,
         PlacementType.TopEdgeAlignedRight => Direction.Top,

         PlacementType.Right => Direction.Right,
         PlacementType.RightEdgeAlignedBottom => Direction.Right,
         PlacementType.RightEdgeAlignedTop => Direction.Right,

         PlacementType.Bottom => Direction.Bottom,
         PlacementType.BottomEdgeAlignedLeft => Direction.Bottom,
         PlacementType.BottomEdgeAlignedRight => Direction.Bottom,
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement,
                                                    "Invalid value for PlacementType")
      };
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

   private (PopupAnchor, PopupGravity) GetAnchorAndGravity(PlacementType placement)
   {
      return placement switch
      {
         PlacementType.Bottom => (PopupAnchor.Bottom, PopupGravity.Bottom),
         PlacementType.Right => (PopupAnchor.Right, PopupGravity.Right),
         PlacementType.Left => (PopupAnchor.Left, PopupGravity.Left),
         PlacementType.Top => (PopupAnchor.Top, PopupGravity.Top),
         PlacementType.TopEdgeAlignedRight => (PopupAnchor.TopRight, PopupGravity.TopLeft),
         PlacementType.TopEdgeAlignedLeft => (PopupAnchor.TopLeft, PopupGravity.TopRight),
         PlacementType.BottomEdgeAlignedLeft => (PopupAnchor.BottomLeft, PopupGravity.BottomRight),
         PlacementType.BottomEdgeAlignedRight => (PopupAnchor.BottomRight, PopupGravity.BottomLeft),
         PlacementType.LeftEdgeAlignedTop => (PopupAnchor.TopLeft, PopupGravity.BottomLeft),
         PlacementType.LeftEdgeAlignedBottom => (PopupAnchor.BottomLeft, PopupGravity.TopLeft),
         PlacementType.RightEdgeAlignedTop => (PopupAnchor.TopRight, PopupGravity.BottomRight),
         PlacementType.RightEdgeAlignedBottom => (PopupAnchor.BottomRight, PopupGravity.TopRight),
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement,
                                                    "Invalid value for PlacementType")
      };
   }

   private Rect GetAnchorRectangle(Control anchor)
   {
      if (anchor == null) {
         throw new InvalidOperationException("Placement mode is not Pointer and PlacementTarget is null");
      }

      var topLevel = TopLevel.GetTopLevel(anchor)!;
      Matrix? matrix;
      if (TryGetAdorner(anchor, out var adorned, out var adornerLayer)) {
         matrix = adorned!.TransformToVisual(topLevel) * anchor.TransformToVisual(adornerLayer!);
      } else {
         matrix = anchor.TransformToVisual(topLevel);
      }

      if (matrix == null) {
         if (anchor.GetVisualRoot() == null) {
            throw new InvalidOperationException("Target control is not attached to the visual tree");
         }

         throw new InvalidOperationException("Target control is not in the same tree as the popup parent");
      }

      var anchorRect = new Rect(default, anchor.Bounds.Size);
      return anchorRect.TransformToAABB(matrix.Value);
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

   private (bool, bool) CalculateFlipInfo(Size translatedSize, Rect anchorRect, PopupAnchor anchor,
                                          PopupGravity gravity,
                                          Point offset)
   {
      var result = (false, false);
      var bounds = GetBounds(anchorRect);
      offset *= _currentAnchorWindow!.DesktopScaling;

      bool FitsInBounds(Rect rc, PopupAnchor edge = PopupAnchor.AllMask)
      {
         if (edge.HasFlag(PopupAnchor.Left) && rc.X < bounds.X ||
             edge.HasFlag(PopupAnchor.Top) && rc.Y < bounds.Y ||
             edge.HasFlag(PopupAnchor.Right) && rc.Right > bounds.Right ||
             edge.HasFlag(PopupAnchor.Bottom) && rc.Bottom > bounds.Bottom) {
            return false;
         }

         return true;
      }

      Rect GetUnconstrained(PopupAnchor a, PopupGravity g) =>
         new Rect(Gravitate(GetAnchorPoint(anchorRect, a), translatedSize, g) + offset, translatedSize);

      var geo = GetUnconstrained(anchor, gravity);
      // If flipping geometry and anchor is allowed and helps, use the flipped one,
      // otherwise leave it as is
      if (!FitsInBounds(geo, PopupAnchor.HorizontalMask)) {
         result.Item1 = true;
      }

      if (!FitsInBounds(geo, PopupAnchor.VerticalMask)) {
         result.Item2 = true;
      }

      return result;
   }

   private static Point GetAnchorPoint(Rect anchorRect, PopupAnchor edge)
   {
      double x, y;
      if (edge.HasFlag(PopupAnchor.Left)) {
         x = anchorRect.X;
      } else if (edge.HasFlag(PopupAnchor.Right)) {
         x = anchorRect.Right;
      } else {
         x = anchorRect.X + anchorRect.Width / 2;
      }

      if (edge.HasFlag(PopupAnchor.Top)) {
         y = anchorRect.Y;
      } else if (edge.HasFlag(PopupAnchor.Bottom)) {
         y = anchorRect.Bottom;
      } else {
         y = anchorRect.Y + anchorRect.Height / 2;
      }

      return new Point(x, y);
   }

   private static Point Gravitate(Point anchorPoint, Size size, PopupGravity gravity)
   {
      double x, y;
      if (gravity.HasFlag(PopupGravity.Left)) {
         x = -size.Width;
      } else if (gravity.HasFlag(PopupGravity.Right)) {
         x = 0;
      } else {
         x = -size.Width / 2;
      }

      if (gravity.HasFlag(PopupGravity.Top)) {
         y = -size.Height;
      } else if (gravity.HasFlag(PopupGravity.Bottom)) {
         y = 0;
      } else {
         y = -size.Height / 2;
      }

      return anchorPoint + new Point(x, y);
   }

   private Rect GetBounds(Rect anchorRect)
   {
      // 暂时只支持窗口的方式
      var parentGeometry = GetParentClientAreaScreenGeometry();
      var screens = GetScreens();

      var targetScreen = screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(anchorRect.TopLeft))
                         ?? screens.FirstOrDefault(s => s.Bounds.Intersects(anchorRect))
                         ?? screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(parentGeometry.TopLeft))
                         ?? screens.FirstOrDefault(s => s.Bounds.Intersects(parentGeometry))
                         ?? screens.FirstOrDefault();

      if (targetScreen != null &&
          (targetScreen.WorkingArea.Width == 0 && targetScreen.WorkingArea.Height == 0)) {
         return targetScreen.Bounds;
      }

      return targetScreen?.WorkingArea
             ?? new Rect(0, 0, double.MaxValue, double.MaxValue);
   }

   private Rect GetParentClientAreaScreenGeometry()
   {
      var point = _currentAnchorWindow!.PointToScreen(default);
      var size = _currentAnchorWindow!.ClientSize * _currentAnchorWindow.DesktopScaling;
      return new Rect(point.X, point.Y, size.Width, size.Height);
   }

   private IReadOnlyList<ManagedPopupPositionerScreenInfo> GetScreens()
   {
      return _currentAnchorWindow!.Screens.All
                                  .Select(s => new ManagedPopupPositionerScreenInfo(
                                             s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
                                  .ToArray();
   }

   private void Close()
   {
      if (_popup is not null) {
         _popup.IsOpen = false;
         SetPopupParent(_popup, null);
         _popup.PlacementTarget = null;
      }
   }

   private void OnPopupClosed(object? sender, EventArgs e)
   {
      // This condition is true, when Popup was closed by any other reason outside of ToolTipService/ToolTip, keeping IsOpen=true.
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
      PseudoClasses.Set(":open", newValue);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }
      
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }
}

internal class ManagedPopupPositionerScreenInfo
{
   public Rect Bounds { get; }
   public Rect WorkingArea { get; }

   public ManagedPopupPositionerScreenInfo(Rect bounds, Rect workingArea)
   {
      Bounds = bounds;
      WorkingArea = workingArea;
   }
}
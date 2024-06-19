using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class ToggleSwitch
{
   /// <summary>
   /// Defines the <see cref="GrooveBackground"/> property.
   /// </summary>
   private static readonly StyledProperty<IBrush?> GrooveBackgroundProperty 
      = AvaloniaProperty.Register<ToggleSwitch, IBrush?>(
      nameof(GrooveBackground));

   public IBrush? GrooveBackground
   {
      get => GetValue(GrooveBackgroundProperty);
      set => SetValue(GrooveBackgroundProperty, value);
   }

   /// <summary>
   /// Defines the <see cref="OffContent"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> OffContentProperty =
      AvaloniaProperty.Register<ToggleSwitch, object?>(nameof(OffContent));

   /// <summary>
   /// Defines the <see cref="OnContent"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> OnContentProperty =
      AvaloniaProperty.Register<ToggleSwitch, object?>(nameof(OnContent));

   /// <summary>
   /// 设置预置的大小类型
   /// </summary>
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Button, SizeType>(nameof(SizeType), SizeType.Middle);
   
   /// <summary>
   /// 是否处于加载状态
   /// </summary>
   public static readonly StyledProperty<bool> IsLoadingProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsLoading), false);

   /// <summary>
   /// Gets or Sets the Content that is displayed when in the On State.
   /// </summary>
   public object? OnContent
   {
      get => GetValue(OnContentProperty);
      set => SetValue(OnContentProperty, value);
   }

   /// <summary>
   /// Gets or Sets the Content that is displayed when in the Off State.
   /// </summary>
   public object? OffContent
   {
      get => GetValue(OffContentProperty);
      set => SetValue(OffContentProperty, value);
   }
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   /// <summary>
   /// 是否处于加载状态
   /// </summary>
   public bool IsLoading
   {
      get => GetValue(IsLoadingProperty);
      set => SetValue(IsLoadingProperty, value);
   }

   // 组件的 Token 绑定属性
   private double _handleSize;

   private static readonly DirectProperty<ToggleSwitch, double> HandleSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_handleSize),
         (o) => o._handleSize,
         (o, v) => o._handleSize = v);

   private double _handleSizeSM;

   private static readonly DirectProperty<ToggleSwitch, double> HandleSizeSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_handleSizeSM),
         (o) => o._handleSizeSM,
         (o, v) => o._handleSizeSM = v);

   private double _innerMaxMargin;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMaxMarginTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMaxMargin),
         (o) => o._innerMaxMargin,
         (o, v) => o._innerMaxMargin = v);

   private double _innerMaxMarginSM;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMaxMarginSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMaxMarginSM),
         (o) => o._innerMaxMarginSM,
         (o, v) => o._innerMaxMarginSM = v);

   private double _innerMinMargin;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMinMarginTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMinMargin),
         (o) => o._innerMinMargin,
         (o, v) => o._innerMinMargin = v);

   private double _innerMinMarginSM;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMinMarginSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMinMarginSM),
         (o) => o._innerMinMarginSM,
         (o, v) => o._innerMinMarginSM = v);

   private double _trackHeight;

   private static readonly DirectProperty<ToggleSwitch, double> TrackHeightTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackHeight),
         (o) => o._trackHeight,
         (o, v) => o._trackHeight = v);

   private double _trackHeightSM;

   private static readonly DirectProperty<ToggleSwitch, double> TrackHeightSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackHeightSM),
         (o) => o._trackHeightSM,
         (o, v) => o._trackHeightSM = v);

   private double _trackMinWidth;

   private static readonly DirectProperty<ToggleSwitch, double> TrackMinWidthTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackMinWidth),
         (o) => o._trackMinWidth,
         (o, v) => o._trackMinWidth = v);

   private double _trackMinWidthSM;

   private static readonly DirectProperty<ToggleSwitch, double> TrackMinWidthSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackMinWidthSM),
         (o) => o._trackMinWidthSM,
         (o, v) => o._trackMinWidthSM = v);

   private double _trackPadding;

   private static readonly DirectProperty<ToggleSwitch, double> TrackPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackPadding),
         (o) => o._trackPadding,
         (o, v) => o._trackPadding = v);
   
   private double _switchDisabledOpacity;

   private static readonly DirectProperty<ToggleSwitch, double> SwitchDisabledOpacityTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_switchDisabledOpacity),
         (o) => o._switchDisabledOpacity,
         (o, v) => o._switchDisabledOpacity = v);
   
   // 组件的 Token 绑定属性结束

   private double _iconSize;

   private static readonly DirectProperty<ToggleSwitch, double> IconSizeProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_iconSize),
         (o) => o._iconSize,
         (o, v) => o._iconSize = v);

   private static readonly StyledProperty<Point> KnobOffsetProperty
      = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(KnobOffset));

   private Point KnobOffset
   {
      get => GetValue(KnobOffsetProperty);
      set => SetValue(KnobOffsetProperty, value);
   }
   
   private static readonly StyledProperty<Point> OnContentOffsetProperty
      = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OnContentOffset));

   private Point OnContentOffset
   {
      get => GetValue(OnContentOffsetProperty);
      set => SetValue(OnContentOffsetProperty, value);
   }
   
   private static readonly StyledProperty<Point> OffContentOffsetProperty
      = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OffContentOffset));

   private Point OffContentOffset
   {
      get => GetValue(OffContentOffsetProperty);
      set => SetValue(OffContentOffsetProperty, value);
   }
   
   private static readonly StyledProperty<double> SwitchOpacityProperty
      = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(SwitchOpacity));

   private double SwitchOpacity
   {
      get => GetValue(SwitchOpacityProperty);
      set => SetValue(SwitchOpacityProperty, value);
   }
}
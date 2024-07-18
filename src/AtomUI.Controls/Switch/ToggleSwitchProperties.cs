using Avalonia;

namespace AtomUI.Controls;

public partial class ToggleSwitch
{
   // 组件的 Token 绑定属性

   private double _innerMaxMarginToken;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMaxMarginTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMaxMarginToken),
         (o) => o._innerMaxMarginToken,
         (o, v) => o._innerMaxMarginToken = v);

   private double _innerMaxMarginSMToken;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMaxMarginSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMaxMarginSMToken),
         (o) => o._innerMaxMarginSMToken,
         (o, v) => o._innerMaxMarginSMToken = v);

   private double _innerMinMarginToken;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMinMarginTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMinMarginToken),
         (o) => o._innerMinMarginToken,
         (o, v) => o._innerMinMarginToken = v);

   private double _innerMinMarginSMToken;

   private static readonly DirectProperty<ToggleSwitch, double> InnerMinMarginSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_innerMinMarginSMToken),
         (o) => o._innerMinMarginSMToken,
         (o, v) => o._innerMinMarginSMToken = v);

   private double _trackHeightToken;

   private static readonly DirectProperty<ToggleSwitch, double> TrackHeightTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackHeightToken),
         (o) => o._trackHeightToken,
         (o, v) => o._trackHeightToken = v);

   private double _trackHeightSMToken;

   private static readonly DirectProperty<ToggleSwitch, double> TrackHeightSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackHeightSMToken),
         (o) => o._trackHeightSMToken,
         (o, v) => o._trackHeightSMToken = v);

   private double _trackMinWidthToken;

   private static readonly DirectProperty<ToggleSwitch, double> TrackMinWidthTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackMinWidthToken),
         (o) => o._trackMinWidthToken,
         (o, v) => o._trackMinWidthToken = v);

   private double _trackMinWidthSMToken;

   private static readonly DirectProperty<ToggleSwitch, double> TrackMinWidthSMTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackMinWidthSMToken),
         (o) => o._trackMinWidthSMToken,
         (o, v) => o._trackMinWidthSMToken = v);

   private double _trackPaddingToken;

   private static readonly DirectProperty<ToggleSwitch, double> TrackPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<ToggleSwitch, double>(nameof(_trackPaddingToken),
         (o) => o._trackPaddingToken,
         (o, v) => o._trackPaddingToken = v);
   
   // 组件的 Token 绑定属性结束
   
   // 这几个属性跟动画相关
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
   
   internal static readonly StyledProperty<double> SwitchOpacityProperty
      = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(SwitchOpacity), 1d);

   internal double SwitchOpacity
   {
      get => GetValue(SwitchOpacityProperty);
      set => SetValue(SwitchOpacityProperty, value);
   }
}
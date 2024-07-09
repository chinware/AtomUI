using AtomUI.Data;
using Avalonia;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum SeparatorTitlePosition
{
   Left,
   Right,
   Center
}

public partial class Separator : StyledControl
{
   public static readonly StyledProperty<string?> TitleProperty =
      AvaloniaProperty.Register<Separator, string?>(nameof(Title));

   public static readonly StyledProperty<SeparatorTitlePosition> TitlePositionProperty =
      AvaloniaProperty.Register<Separator, SeparatorTitlePosition>(nameof(TitlePosition),
                                                                   SeparatorTitlePosition.Center);

   public static readonly StyledProperty<IBrush?> TitleColorProperty =
      AvaloniaProperty.Register<Separator, IBrush?>(nameof(TitleColor));
   
   public static readonly StyledProperty<IBrush?> LineColorProperty =
      AvaloniaProperty.Register<Separator, IBrush?>(nameof(LineColor));
   
   public static readonly StyledProperty<Orientation> OrientationProperty =
      AvaloniaProperty.Register<Separator, Orientation>(nameof(Orientation), Orientation.Horizontal);
   
   public static readonly StyledProperty<double> OrientationMarginProperty =
      AvaloniaProperty.Register<Separator, double>(nameof(OrientationMargin), double.NaN);
   
   public static readonly StyledProperty<bool> IsDashedLineProperty =
      AvaloniaProperty.Register<Separator, bool>(nameof(Orientation), false);
   
   public static readonly StyledProperty<double> LineWidthProperty =
      AvaloniaProperty.Register<Separator, double>(nameof(LineWidth), 1);

   /// <summary>
   /// 分割线的标题
   /// </summary>
   [Content]
   public string? Title
   {
      get => GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
   }

   /// <summary>
   /// 分割线标题的位置
   /// </summary>
   public SeparatorTitlePosition TitlePosition
   {
      get => GetValue(TitlePositionProperty);
      set => SetValue(TitlePositionProperty, value);
   }

   /// <summary>
   /// 分割线标题的颜色
   /// </summary>
   public IBrush? TitleColor
   {
      get => GetValue(TitleColorProperty);
      set => SetValue(TitleColorProperty, value);
   }
   
   /// <summary>
   /// 分割线标题的颜色
   /// </summary>
   public IBrush? LineColor
   {
      get => GetValue(LineColorProperty);
      set => SetValue(LineColorProperty, value);
   }
   
   /// <summary>
   /// 分割线的方向，垂直和水平分割线
   /// </summary>
   public Orientation Orientation
   {
      get => GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
   }

   /// <summary>
   /// The margin-left/right between the title and its closest border, while the orientation must be left or right,
   /// If a numeric value of type string is provided without a unit, it is assumed to be in pixels (px) by default.
   /// </summary>
   /// <returns></returns>
   public double OrientationMargin
   {
      get => GetValue(OrientationMarginProperty);
      set => SetValue(OrientationMarginProperty, value);
   }
   
   /// <summary>
   /// 是否为虚线
   /// </summary>
   public bool IsDashedLine
   {
      get => GetValue(IsDashedLineProperty);
      set => SetValue(IsDashedLineProperty, value);
   }

   /// <summary>
   /// 分割线的宽度，这里的宽度是 RenderScaling 中立的像素值
   /// </summary>
   public double LineWidth
   {
      get => GetValue(LineWidthProperty);
      set => SetValue(LineWidthProperty, value);
   }

   public Separator()
   {
      _controlTokenBinder = new ControlTokenBinder(this, SeparatorToken.ID);
      _customStyle = this;
      _customStyle.InitOnConstruct();
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
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }

   static Separator()
   {
      AffectsMeasure<Separator>(OrientationProperty, 
                                LineWidthProperty,
                                TitleProperty);
      AffectsArrange<Separator>(TitlePositionProperty);
      AffectsRender<Separator>(TitleColorProperty, 
                               LineColorProperty,
                               IsDashedLineProperty);
   }
}

public class VerticalSeparator : Separator
{
   static VerticalSeparator()
   {
      OrientationProperty.OverrideDefaultValue<VerticalSeparator>(Orientation.Vertical);
   }
}
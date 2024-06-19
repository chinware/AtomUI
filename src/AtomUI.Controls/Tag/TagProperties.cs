using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Tag
{
   public static readonly StyledProperty<string?> TagColorProperty 
      = AvaloniaProperty.Register<Tag, string?>(
         nameof(Color));
   
   public static readonly StyledProperty<bool> ClosableProperty
      = AvaloniaProperty.Register<Tag, bool>(nameof(Closable));
   
   public static readonly StyledProperty<bool> BorderedProperty
      = AvaloniaProperty.Register<Tag, bool>(nameof(Bordered), true);
   
   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<Tag, PathIcon?>(nameof(Icon));
   
   public static readonly StyledProperty<PathIcon?> CloseIconProperty
      = AvaloniaProperty.Register<Tag, PathIcon?>(nameof(CloseIcon));
   
   public string? TagColor
   {
      get => GetValue(TagColorProperty);
      set => SetValue(TagColorProperty, value);
   }
   
   public bool Closable
   {
      get => GetValue(ClosableProperty);
      set => SetValue(ClosableProperty, value);
   }
   
   public bool Bordered
   {
      get => GetValue(BorderedProperty);
      set => SetValue(BorderedProperty, value);
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

   // 组件的 Token 绑定属性
   private IBrush? _defaultBackground;
   private static readonly DirectProperty<Tag, IBrush?> DefaultBgTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, IBrush?>(nameof(_defaultBackground),
         (o) => o._defaultBackground,
         (o, v) => o._defaultBackground = v);
   
   private IBrush? _defaultForeground;
   private static readonly DirectProperty<Tag, IBrush?> DefaultForegroundTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, IBrush?>(nameof(_defaultForeground),
         (o) => o._defaultForeground,
         (o, v) => o._defaultForeground = v);
   
   private double _tagLineHeight;
   private static readonly DirectProperty<Tag, double> TagLineHeightTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, double>(nameof(_tagLineHeight),
         (o) => o._tagLineHeight,
         (o, v) => o._tagLineHeight = v);
      
   private double _tagIconSize;
   private static readonly DirectProperty<Tag, double> TagIconSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, double>(nameof(_tagIconSize),
         (o) => o._tagIconSize,
         (o, v) => o._tagIconSize = v);
   
   private double _tagCloseIconSize;
   private static readonly DirectProperty<Tag, double> TagCloseIconSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, double>(nameof(_tagCloseIconSize),
         (o) => o._tagCloseIconSize,
         (o, v) => o._tagCloseIconSize = v);
   
   private double _paddingXXS;
   private static readonly DirectProperty<Tag, double> PaddingXXSTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, double>(nameof(_paddingXXS),
         (o) => o._paddingXXS,
         (o, v) => o._paddingXXS = v);
   
   private IBrush? _tagBorderlessBg;
   private static readonly DirectProperty<Tag, IBrush?> TagBorderlessBgTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, IBrush?>(nameof(_tagBorderlessBg),
         (o) => o._tagBorderlessBg,
         (o, v) => o._tagBorderlessBg = v);
   
   private IBrush? _colorTextLightSolid;
   private static readonly DirectProperty<Tag, IBrush?> ColorTextLightSolidTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, IBrush?>(nameof(_colorTextLightSolid),
         (o) => o._colorTextLightSolid,
         (o, v) => o._colorTextLightSolid = v);
   
   // 组件的 Token 绑定属性结束
}
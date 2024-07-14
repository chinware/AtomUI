using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class EmptyIndicator
{
   private double _descriptionMargin;
   private static readonly DirectProperty<EmptyIndicator, double> DescriptionMarginProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, double>(
         nameof(_descriptionMargin),
         o => o._descriptionMargin,
         (o, v) => o._descriptionMargin = v);
   
   #region Control token 值绑定属性
   
   private double _emptyImgHeightToken;
   private static readonly DirectProperty<EmptyIndicator, double> EmptyImgHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, double>(
         nameof(_emptyImgHeightToken),
         o => o._emptyImgHeightToken,
         (o, v) => o._emptyImgHeightToken = v);
   
   private double _emptyImgHeightSMToken;
   private static readonly DirectProperty<EmptyIndicator, double> EmptyImgHeightSMTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, double>(
         nameof(_emptyImgHeightSMToken),
         o => o._emptyImgHeightSMToken,
         (o, v) => o._emptyImgHeightSMToken = v);
   
   private double _emptyImgHeightMDToken;
   private static readonly DirectProperty<EmptyIndicator, double> EmptyImgHeightMDTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, double>(
         nameof(_emptyImgHeightMDToken),
         o => o._emptyImgHeightMDToken,
         (o, v) => o._emptyImgHeightMDToken = v);
   
   private IBrush? _colorFillToken;
   private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
         nameof(_colorFillToken),
         o => o._colorFillToken,
         (o, v) => o._colorFillToken = v);
   
   private IBrush? _colorFillTertiary;
   private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillTertiaryTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
         nameof(_colorFillTertiary),
         o => o._colorFillTertiary,
         (o, v) => o._colorFillTertiary = v);
   
   private IBrush? _colorFillQuaternary;
   private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillQuaternaryTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
         nameof(_colorFillQuaternary),
         o => o._colorFillQuaternary,
         (o, v) => o._colorFillQuaternary = v);
   
   private IBrush? _colorBgContainer;
   private static readonly DirectProperty<EmptyIndicator, IBrush?> ColorBgContainerTokenProperty =
      AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
         nameof(_colorBgContainer),
         o => o._colorBgContainer,
         (o, v) => o._colorBgContainer = v);
   
   #endregion
   
}
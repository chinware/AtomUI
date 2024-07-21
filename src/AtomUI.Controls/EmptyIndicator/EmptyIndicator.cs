using AtomUI.ColorSystem;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public enum PresetEmptyImage
{
   Simple,
   Default,
}

public partial class EmptyIndicator : TemplatedControl,
                                      IControlCustomStyle

{
   public static readonly StyledProperty<PresetEmptyImage?> PresetEmptyImageProperty =
      AvaloniaProperty.Register<EmptyIndicator, PresetEmptyImage?>(nameof(PresetImage));

   public static readonly StyledProperty<string?> ImagePathProperty =
      AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(ImagePath));

   public static readonly StyledProperty<string?> ImageSourceProperty =
      AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(ImageSource));

   public static readonly StyledProperty<string?> DescriptionProperty =
      AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(Description));

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<EmptyIndicator, SizeType>(nameof(SizeType), SizeType.Large);

   public static readonly StyledProperty<bool> IsShowDescriptionProperty =
      AvaloniaProperty.Register<EmptyIndicator, bool>(nameof(IsShowDescription), true);

   public PresetEmptyImage? PresetImage
   {
      get => GetValue(PresetEmptyImageProperty);
      set => SetValue(PresetEmptyImageProperty, value);
   }

   public string? ImagePath
   {
      get => GetValue(ImagePathProperty);
      set => SetValue(ImagePathProperty, value);
   }

   public string? ImageSource
   {
      get => GetValue(ImageSourceProperty);
      set => SetValue(ImageSourceProperty, value);
   }

   public string? Description
   {
      get => GetValue(DescriptionProperty);
      set => SetValue(DescriptionProperty, value);
   }

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public bool IsShowDescription
   {
      get => GetValue(IsShowDescriptionProperty);
      set => SetValue(IsShowDescriptionProperty, value);
   }

   private IControlCustomStyle _customStyle;
   private Avalonia.Svg.Svg? _svg;

   static EmptyIndicator()
   {
      AffectsMeasure<EmptyIndicator>(PresetEmptyImageProperty,
                                     ImagePathProperty,
                                     ImageSourceProperty,
                                     DescriptionProperty,
                                     IsShowDescriptionProperty);
   }

   public EmptyIndicator()
   {
      _customStyle = this;
   }

   private void CheckImageSource()
   {
      var imageSettedCount = 0;
      if (PresetImage is not null) {
         imageSettedCount++;
      }

      if (ImagePath is not null) {
         imageSettedCount++;
      }

      if (ImageSource is not null) {
         imageSettedCount++;
      }

      if (imageSettedCount > 1) {
         throw new ApplicationException("ImagePath, ImageSource and PresetEmptyImage cannot be set at the same time.");
      }
   }

   #region IControlCustomStyle 实现

   void IControlCustomStyle.SetupTokenBindings()
   {
      BindUtils.CreateTokenBinding(this, ColorFillTokenProperty, GlobalResourceKey.ColorFill);
      BindUtils.CreateTokenBinding(this, ColorFillTertiaryTokenProperty, GlobalResourceKey.ColorFillTertiary);
      BindUtils.CreateTokenBinding(this, ColorFillQuaternaryTokenProperty, GlobalResourceKey.ColorFillQuaternary);
      BindUtils.CreateTokenBinding(this, ColorBgContainerTokenProperty, GlobalResourceKey.ColorBgContainer);
   }

   private void SetupImage()
   {
      if (_svg is null) {
         return;
      }

      if (PresetImage is not null) {
         if (PresetImage.Value == PresetEmptyImage.Default) {
            _svg.Source = BuiltInImageBuilder.BuildDefaultImage();
         } else {
            var colorFill = ((SolidColorBrush)_colorFillToken!).Color;
            var colorFillTertiary = ((SolidColorBrush)_colorFillTertiary!).Color;
            var colorFillQuaternary = ((SolidColorBrush)_colorFillQuaternary!).Color;
            var colorBgContainer = ((SolidColorBrush)_colorBgContainer!).Color;
            var borderColor = ColorUtils.OnBackground(colorFill, colorBgContainer);
            var shadowColor = ColorUtils.OnBackground(colorFillTertiary, colorBgContainer);
            var contentColor = ColorUtils.OnBackground(colorFillQuaternary, colorBgContainer);
            _svg.Source = BuiltInImageBuilder.BuildSimpleImage(contentColor, borderColor, shadowColor);
         }
      } else if (ImageSource is not null) {
         _svg.Source = ImageSource;
      } else if (ImagePath is not null) {
         _svg.Path = ImagePath;
      }
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.SetupTokenBindings();
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _svg = scope.Find<Avalonia.Svg.Svg>(EmptyIndicatorTheme.SvgImagePart);
      HorizontalAlignment = HorizontalAlignment.Center;
      VerticalAlignment = VerticalAlignment.Center;
      CheckImageSource();
      SetupImage();
   }

   #endregion
}
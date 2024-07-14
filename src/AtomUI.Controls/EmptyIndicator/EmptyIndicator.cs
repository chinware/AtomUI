using AtomUI.ColorSystem;
using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public enum PresetEmptyImage
{
   Simple,
   Default,
}

public partial class EmptyIndicator : Control,
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
   private ControlTokenBinder _controlTokenBinder;
   private bool _initialized = false;
   private StackPanel? _layout;
   private Avalonia.Svg.Svg? _svg;
   private TextBlock? _description;

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
      _controlTokenBinder = new ControlTokenBinder(this, EmptyIndicatorToken.ID);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (_initialized) {
         if (change.Property == SizeTypeProperty) {
            HandleSizeTypeChanged();
         } else if (change.Property == DescriptionMarginProperty) {
            _description!.Margin = new Thickness(0, _descriptionMargin, 0, 0);
         }
      }
   }

   private void HandleSizeTypeChanged()
   {
      _customStyle.ApplySizeTypeStyleConfig();
      if (_svg is not null) {
         if (SizeType == SizeType.Large) {
            _svg.Height = _emptyImgHeightToken;
         } else if (SizeType == SizeType.Middle) {
            _svg.Height = _emptyImgHeightMDToken;
         } else {
            _svg.Height = _emptyImgHeightSMToken;
         }
      }
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

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(EmptyImgHeightTokenProperty, EmptyIndicatorResourceKey.EmptyImgHeight);
      _controlTokenBinder.AddControlBinding(EmptyImgHeightSMTokenProperty, EmptyIndicatorResourceKey.EmptyImgHeightSM);
      _controlTokenBinder.AddControlBinding(EmptyImgHeightMDTokenProperty, EmptyIndicatorResourceKey.EmptyImgHeightMD);
      _controlTokenBinder.AddControlBinding(ColorFillTokenProperty, GlobalResourceKey.ColorFill);
      _controlTokenBinder.AddControlBinding(ColorFillTertiaryTokenProperty, GlobalResourceKey.ColorFillTertiary);
      _controlTokenBinder.AddControlBinding(ColorFillQuaternaryTokenProperty, GlobalResourceKey.ColorFillQuaternary);
      _controlTokenBinder.AddControlBinding(ColorBgContainerTokenProperty, GlobalResourceKey.ColorBgContainer);
      _controlTokenBinder.AddControlBinding(_description!, TextElement.ForegroundProperty,
                                            GlobalResourceKey.ColorTextDescription);
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Small ||
          SizeType == SizeType.Middle) {
         _controlTokenBinder.AddControlBinding(DescriptionMarginProperty, GlobalResourceKey.MarginXS);
      } else {
         _controlTokenBinder.AddControlBinding(DescriptionMarginProperty, GlobalResourceKey.MarginSM);
      }
   }

   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Center;
      VerticalAlignment = VerticalAlignment.Center;
      CheckImageSource();
      _layout = new StackPanel()
      {
         Orientation = Orientation.Vertical
      };

      LogicalChildren.Add(_layout);
      VisualChildren.Add(_layout);

      _svg = new Avalonia.Svg.Svg(new Uri("https://github.com/avaloniaui"));
      _layout.Children.Add(_svg);
      _description = new TextBlock()
      {
         HorizontalAlignment = HorizontalAlignment.Center,
         TextWrapping = TextWrapping.Wrap,
         Text = Description ?? "No data"
      };
      BindUtils.RelayBind(this, IsShowDescriptionProperty, _description, TextBlock.IsVisibleProperty);
      
      _layout.Children.Add(_description);
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplySizeTypeStyleConfig();

      _description.Margin = new Thickness(0, _descriptionMargin, 0, 0);
      SetupImage();
      HandleSizeTypeChanged();
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

   #endregion
}
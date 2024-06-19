using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class StyledControl : Control
{
   /// <summary>
   /// Defines the <see cref="Background"/> property.
   /// </summary>
   public static readonly StyledProperty<IBrush?> BackgroundProperty =
      AvaloniaProperty.Register<StyledControl, IBrush?>(nameof(Background));

   /// <summary>
   /// Defines the <see cref="BackgroundSizing"/> property.
   /// </summary>
   public static readonly StyledProperty<BackgroundSizing> BackgroundSizingProperty =
      AvaloniaProperty.Register<StyledControl, BackgroundSizing>(
         nameof(BackgroundSizing),
         BackgroundSizing.CenterBorder);
   
   /// <summary>
   /// Defines the <see cref="FontFamily"/> property.
   /// </summary>
   public static readonly StyledProperty<FontFamily> FontFamilyProperty =
      TextElement.FontFamilyProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="FontFeaturesProperty"/> property.
   /// </summary>
   public static readonly StyledProperty<FontFeatureCollection?> FontFeaturesProperty =
      TextElement.FontFeaturesProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="FontSize"/> property.
   /// </summary>
   public static readonly StyledProperty<double> FontSizeProperty =
      TextElement.FontSizeProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="FontStyle"/> property.
   /// </summary>
   public static readonly StyledProperty<FontStyle> FontStyleProperty =
      TextElement.FontStyleProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="FontWeight"/> property.
   /// </summary>
   public static readonly StyledProperty<FontWeight> FontWeightProperty =
      TextElement.FontWeightProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="FontWeight"/> property.
   /// </summary>
   public static readonly StyledProperty<FontStretch> FontStretchProperty =
      TextElement.FontStretchProperty.AddOwner<StyledControl>();

   /// <summary>
   /// Defines the <see cref="Foreground"/> property.
   /// </summary>
   public static readonly StyledProperty<IBrush?> ForegroundProperty =
      TextElement.ForegroundProperty.AddOwner<StyledControl>();
   
   /// <summary>
   /// Defines the <see cref="Padding"/> property.
   /// </summary>
   public static readonly StyledProperty<Thickness> PaddingProperty =
      AvaloniaProperty.Register<Decorator, Thickness>(nameof(Padding));
   
   /// <summary>
   /// Gets or sets a brush with which to paint the background.
   /// </summary>
   public IBrush? Background
   {
      get => GetValue(BackgroundProperty);
      set => SetValue(BackgroundProperty, value);
   }

   /// <summary>
   /// Gets or sets how the background is drawn relative to the border.
   /// </summary>
   public BackgroundSizing BackgroundSizing
   {
      get => GetValue(BackgroundSizingProperty);
      set => SetValue(BackgroundSizingProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the padding to place around the <see cref="Child"/> control.
   /// </summary>
   public Thickness Padding
   {
      get => GetValue(PaddingProperty);
      set => SetValue(PaddingProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the font family used to draw the control's text.
   /// </summary>
   public FontFamily FontFamily
   {
      get => GetValue(FontFamilyProperty);
      set => SetValue(FontFamilyProperty, value);
   }

   /// <summary>
   /// Gets or sets the font features turned on/off.
   /// </summary>
   public FontFeatureCollection? FontFeatures
   {
      get => GetValue(FontFeaturesProperty);
      set => SetValue(FontFeaturesProperty, value);
   }

   /// <summary>
   /// Gets or sets the size of the control's text in points.
   /// </summary>
   public double FontSize
   {
      get => GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
   }

   /// <summary>
   /// Gets or sets the font style used to draw the control's text.
   /// </summary>
   public FontStyle FontStyle
   {
      get => GetValue(FontStyleProperty);
      set => SetValue(FontStyleProperty, value);
   }

   /// <summary>
   /// Gets or sets the font weight used to draw the control's text.
   /// </summary>
   public FontWeight FontWeight
   {
      get => GetValue(FontWeightProperty);
      set => SetValue(FontWeightProperty, value);
   }

   /// <summary>
   /// Gets or sets the font stretch used to draw the control's text.
   /// </summary>
   public FontStretch FontStretch
   {
      get => GetValue(FontStretchProperty);
      set => SetValue(FontStretchProperty, value);
   }

   /// <summary>
   /// Gets or sets the brush used to draw the control's text and other foreground elements.
   /// </summary>
   public IBrush? Foreground
   {
      get => GetValue(ForegroundProperty);
      set => SetValue(ForegroundProperty, value);
   }
   
   /// <summary>
   /// Initializes static members of the <see cref="StyledControl"/> class.
   /// </summary>
   static StyledControl()
   {
      AffectsMeasure<Decorator>(PaddingProperty);
      ClipToBoundsProperty.OverrideDefaultValue<StyledControl>(true);
   }
   
   public StyledControl()
   {
      ResourcesChanged += (sender, args) =>
      {
         NotifyChildResourcesChanged(args);
      };
   }
   
   internal virtual void NotifyChildResourcesChanged(ResourcesChangedEventArgs e)
   {
      {
         var count = VisualChildren.Count;

         for (var i = 0; i < count; ++i) {
            if (VisualChildren[i] is ILogical logical) {
               logical.NotifyResourcesChanged(e);
            }
         }
      }

      {
         var count = LogicalChildren.Count;
         for (var i = 0; i < count; ++i) {
            var logical = LogicalChildren[i];
            logical.NotifyResourcesChanged(e);
         }
      }
   }
}
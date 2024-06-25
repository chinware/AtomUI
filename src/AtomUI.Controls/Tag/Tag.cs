using AtomUI.ColorSystem;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public enum TagPresetColor
{
   Red,
   Volcano,
   Orange,
   Gold,
   Yellow,
   Lime,
   Green,
   Cyan,
   Blue,
   GeekBlue,
   Purple,
   Pink,
   Magenta,
}

public enum TagStatus {
   // 状态
   Success,
   Info,
   Error,
   Warning
}

public partial class Tag : Label, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => TagToken.ID;
   
   static Tag()
   {
      _presetColorMap = new Dictionary<TagPresetColor, TagCalcColor>();
      _statusColorMap = new Dictionary<TagStatus, TagStatusCalcColor>();
      _colorCodeMap = new Dictionary<TagPresetColor, PresetPrimaryColor.ColorType>();
      SetupColorCodeMap();
      AffectsMeasure<Tag>(BorderedProperty,
         IconProperty,
         ClosableProperty,
         PaddingXXSTokenProperty,
         TagCloseIconSizeTokenProperty,
         TagIconSizeTokenProperty);
      AffectsRender<Tag>(ClosableProperty,
         DefaultBgTokenProperty,
         DefaultForegroundTokenProperty,
         TagBorderlessBgTokenProperty,
         ColorTextLightSolidTokenProperty);
   }
   
   public Tag()
   {
      _customStyle = this;
      _tokenResourceBinder = new TokenResourceBinder(this);
      SetupPresetColorMap();
      SetupStatusColorMap();
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
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var textBlock = _textBlock!;
      textBlock.Measure(availableSize);
      var targetWidth = textBlock.DesiredSize.Width;
      targetWidth += Padding.Left + Padding.Right;
      if (Icon is not null) {
         Icon.Measure(new Size(_tagIconSize, _tagIconSize));
         targetWidth += _tagIconSize;
         targetWidth += _paddingXXS;
      }

      if (Closable && CloseIcon is not null) {
         CloseIcon.Measure(new Size(_tagCloseIconSize, _tagCloseIconSize));
         targetWidth += _paddingXXS;
         targetWidth += _tagCloseIconSize;
      }

      // 高度写死，也就是强制风格统一，先看看效果
      var targetHeight = _tagLineHeight;
      _layoutPanel!.Height = targetHeight;
      _layoutPanel!.Width = targetWidth;
      textBlock.Height = targetHeight;
      textBlock.LineHeight = targetHeight;
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      base.ArrangeOverride(finalSize);
      _layoutPanel?.Arrange(BevelRect(finalSize));
      var textRect = TextRect(finalSize);
      _textBlock!.Arrange(textRect);
      if (Icon is not null) {
         Icon.Arrange(IconRect(finalSize));
      }
      if (CloseIcon is not null) {
         CloseIcon.Arrange(CloseIconRect(finalSize));
      }
      return finalSize;
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }
   
   protected override void OnPointerMoved(PointerEventArgs e)
   {
      base.OnPointerMoved(e);
      if (Closable) {
         var closeRect = new Rect(new Point(0, 0), CloseIcon!.DesiredSize);
         if (closeRect.Contains(e.GetPosition(CloseIcon!))) {
            CloseIcon.IconMode = IconMode.Active;
         } else {
            CloseIcon.IconMode = IconMode.Normal;
         }
      }
   }
}
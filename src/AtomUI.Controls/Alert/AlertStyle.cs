using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Alert : IControlCustomStyle
{
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private bool _initialized = false;
   private Grid? _mainLayout;
   private Label? _messageLabel;
   private MarqueeLabel? _messageMarqueeLabel;
   private Label? _descriptionLabel;
   private PathIcon? _icon;
   private IconButton? _closeButton;
   private StackPanel? _infoStack;

   void IControlCustomStyle.InitOnConstruct()
   {
      _mainLayout = new Grid()
      {
         RowDefinitions =
         {
            new RowDefinition(GridLength.Auto)
         },
         ColumnDefinitions =
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Auto)
         }
      };
   }

   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Stretch;
      _customStyle.ApplyFixedStyleConfig();
      CreateStructure();
      SetupPaddingStyleConfig();
      HandleDescriptionEnabled(!string.IsNullOrEmpty(_description));
      Child = _mainLayout;
   }

   private void CreateStructure()
   {
      _descriptionLabel = new Label
      {
         Content = Description,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         HorizontalContentAlignment = HorizontalAlignment.Left,
         VerticalContentAlignment = VerticalAlignment.Center,
         Padding = new Thickness(0)
      };
      TextBlock.SetTextWrapping(_descriptionLabel, TextWrapping.Wrap);
      _tokenResourceBinder.AddBinding(_descriptionLabel!, Label.MarginProperty, GlobalResourceKey.MarginXS);
      _descriptionLabel.IsVisible = !string.IsNullOrEmpty(Description);
      _infoStack = new StackPanel()
      {
         Orientation = Orientation.Vertical,
         VerticalAlignment = VerticalAlignment.Center
      };

      HandleMessageMarqueEnabled(_isMessageMarqueEnabled);
      SetupCloseButton();
      SetupTypeIcon();
      
      _infoStack.Children.Add(_descriptionLabel);

      Grid.SetColumn(_infoStack, 1);
      Grid.SetRow(_infoStack, 0);
      _mainLayout!.Children.Add(_infoStack);

      
      if (_extraAction is not null) {
         _extraAction.VerticalAlignment = VerticalAlignment.Top;
         Grid.SetColumn(_extraAction, 2);
         Grid.SetRow(_extraAction, 0);
         _tokenResourceBinder.AddBinding(_extraAction, MarginProperty, AlertResourceKey.ExtraElementMargin);
         _mainLayout!.Children.Add(_extraAction);
      }
   }

   private Control GetMessageControl()
   {
      if (_isMessageMarqueEnabled) {
         return _messageMarqueeLabel!;
      }

      return _messageLabel!;
   }

   public void SetupCloseButton()
   {
      if (_isClosable && _closeButton is null) {
         if (_closeIcon is null) {
            _closeIcon = new PathIcon
            {
               Kind = "CloseOutlined",
            };
            _tokenResourceBinder.AddBinding(_closeIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
            _tokenResourceBinder.AddBinding(_closeIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
         }

         _closeButton = new IconButton
         {
            VerticalAlignment = VerticalAlignment.Top,
         };
         _tokenResourceBinder.AddBinding(_closeButton, WidthProperty, GlobalResourceKey.IconSizeSM);
         _tokenResourceBinder.AddBinding(_closeButton, HeightProperty, GlobalResourceKey.IconSizeSM);
         _tokenResourceBinder.AddBinding(_closeButton, MarginProperty, AlertResourceKey.ExtraElementMargin);
         Grid.SetRow(_closeButton, 0);
         Grid.SetColumn(_closeButton, 3);
         _mainLayout!.Children.Add(_closeButton);
        BindUtils.RelayBind(this, "CloseIcon", _closeButton, "Icon");
      } else if (_closeButton is not null) {
         _closeButton.IsVisible = _isClosable;
      }
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      ApplyAlertTypeStyleConfig();
      _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
   }

   private void SetupPaddingStyleConfig()
   {
      if (_descriptionLabel!.IsVisible) {
         _tokenResourceBinder.AddBinding(PaddingProperty, AlertResourceKey.WithDescriptionPadding);
      } else {
         _tokenResourceBinder.AddBinding(PaddingProperty, AlertResourceKey.DefaultPadding);
      }
   }

   private void ApplyAlertTypeStyleConfig()
   {
      if (Type == AlertType.Success) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorSuccessBg);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorSuccessBorder);
      } else if (Type == AlertType.Info) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorInfoBg);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorInfoBorder);
      } else if (Type == AlertType.Warning) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorWarningBg);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorWarningBorder);
      } else if (Type == AlertType.Error) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorErrorBg);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorBorder);
      }
      _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
     _tokenResourceBinder.AddBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style, 
                                     new RenderScaleAwareThicknessConfigure(this));
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (_initialized) {
         if (e.Property == DescriptionProperty) {
            var desc = e.GetNewValue<string?>();
            var enabled = !string.IsNullOrEmpty(desc);
            HandleDescriptionEnabled(enabled);
            HandleIconForDescriptionEnabled(enabled);
            _descriptionLabel!.IsVisible = enabled;
         } else if (e.Property == IsMessageMarqueEnabledProperty) {
            var enabled = e.GetNewValue<bool>();
            HandleMessageMarqueEnabled(enabled);
         }
      }
   }

   private void HandleDescriptionEnabled(bool enabled)
   {
      var messageControl = GetMessageControl();
      if (enabled) {
         _tokenResourceBinder.AddBinding(messageControl, FontSizeProperty, GlobalResourceKey.FontSizeLG);
         _tokenResourceBinder.AddBinding(messageControl, MarginProperty, AlertResourceKey.MessageWithDescriptionMargin);
         if (_closeButton is not null) {
            _closeButton.VerticalAlignment = VerticalAlignment.Top;
         }
         messageControl.VerticalAlignment = VerticalAlignment.Top;
      } else {
         _tokenResourceBinder.AddBinding(messageControl, FontSizeProperty, GlobalResourceKey.FontSize);
         messageControl.Margin = new Thickness();
         if (_closeButton is not null) {
            _closeButton.VerticalAlignment = VerticalAlignment.Center;
         }
         messageControl.VerticalAlignment = VerticalAlignment.Stretch;
      }
   }

   private void HandleIconForDescriptionEnabled(bool enabled)
   {
      if (_icon is not null && _isShowIcon) {
         if (enabled) {
            _tokenResourceBinder.AddBinding(_icon, WidthProperty, AlertResourceKey.WithDescriptionIconSize);
            _tokenResourceBinder.AddBinding(_icon, HeightProperty, AlertResourceKey.WithDescriptionIconSize);
            _tokenResourceBinder.AddBinding(_icon, MarginProperty, AlertResourceKey.IconWithDescriptionMargin);
            _icon.VerticalAlignment = VerticalAlignment.Top;
         } else {
            _tokenResourceBinder.AddBinding(_icon, WidthProperty, AlertResourceKey.IconSize);
            _tokenResourceBinder.AddBinding(_icon, HeightProperty, AlertResourceKey.IconSize);
            _tokenResourceBinder.AddBinding(_icon, MarginProperty, AlertResourceKey.IconDefaultMargin);
            _icon.VerticalAlignment = VerticalAlignment.Center;
         }
      }
   }

   private void SetupTypeIcon()
   {
      if (_isShowIcon) {
         if (_icon is null) {
            var kind = string.Empty;
            var resourceKey = string.Empty;
            if (Type == AlertType.Success) {
               kind = "CheckCircleFilled";
               resourceKey = GlobalResourceKey.ColorSuccess;
            } else if (Type == AlertType.Info) {
               kind = "InfoCircleFilled";
               resourceKey = GlobalResourceKey.ColorPrimary;
            } else if (Type == AlertType.Warning) {
               kind = "ExclamationCircleFilled";
               resourceKey = GlobalResourceKey.ColorWarning;
            } else if (Type == AlertType.Error) {
               kind = "CloseCircleFilled";
               resourceKey = GlobalResourceKey.ColorError;
            }
  
            _icon = new PathIcon
            {
               Kind = kind,
            };
            
            _tokenResourceBinder.AddBinding(_icon, PathIcon.NormalFillBrushProperty, resourceKey);
            
            HandleIconForDescriptionEnabled(_descriptionLabel!.IsVisible);
            
            Grid.SetRow(_icon, 0);
            Grid.SetColumn(_icon, 0);
            _mainLayout!.Children.Add(_icon);
         }
      } else if (_icon is not null) {
         _icon.IsVisible = false;
      }
   }

   private void HandleMessageMarqueEnabled(bool enabled)
   {
      if (!enabled) {
         if (_messageLabel is null) {
            _messageLabel = new Label
            {
               Content = Message,
               HorizontalAlignment = HorizontalAlignment.Stretch,
               HorizontalContentAlignment = HorizontalAlignment.Left,
               VerticalContentAlignment = VerticalAlignment.Center,
               Padding = new Thickness(0)
            };
            TextBlock.SetTextWrapping(_messageLabel, TextWrapping.Wrap);
         }

         if (_messageMarqueeLabel is not null) {
            _infoStack!.Children.Remove(_messageMarqueeLabel);
         }
         _infoStack!.Children.Insert(0, _messageLabel);
      } else {
         if (_messageMarqueeLabel is null) {
            _messageMarqueeLabel = new MarqueeLabel
            {
               Text = Message,
               HorizontalAlignment = HorizontalAlignment.Stretch,
               Padding = new Thickness(0)
            };
         }
         if (_messageLabel is not null) {
            _infoStack!.Children.Remove(_messageLabel);
         }
         _infoStack!.Children.Insert(0, _messageMarqueeLabel);
      }
   }
}
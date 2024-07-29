using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AlertTheme : BaseControlTheme
{
   public const string CloseBtnPart = "PART_CloseBtn";
   public const string InfoIconPart = "PART_InfoIcon";
   public const string DescriptionLabelPart = "PART_DescriptionLabel";
   public const string MessageLabelPart = "PART_MessageLabel";
   public const string MarqueeLabelPart = "PART_MarqueeLabel";
   
   public AlertTheme()
      : base(typeof(Alert))
   {
   }

   protected override void BuildStyles()
   {
      BuildAlertTypeStyle();
      BuildMessageLabelStyle();
      BuildCloseBtnStyle();
      BuildInfoIconStyle();
   }

   private void BuildAlertTypeStyle()
   {
      this.Add(Alert.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      var successStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Success));
      successStyle.Add(Alert.BackgroundProperty, GlobalResourceKey.ColorSuccessBg);
      successStyle.Add(Alert.BorderBrushProperty, GlobalResourceKey.ColorSuccessBorder);
      Add(successStyle);
      
      var infoStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
      infoStyle.Add(Alert.BackgroundProperty, GlobalResourceKey.ColorInfoBg);
      infoStyle.Add(Alert.BorderBrushProperty, GlobalResourceKey.ColorInfoBorder);
      Add(infoStyle);
      
      var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
      warningStyle.Add(Alert.BackgroundProperty, GlobalResourceKey.ColorWarningBg);
      warningStyle.Add(Alert.BorderBrushProperty, GlobalResourceKey.ColorWarningBorder);
      Add(warningStyle);
      
      var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
      errorStyle.Add(Alert.BackgroundProperty, GlobalResourceKey.ColorErrorBg);
      errorStyle.Add(Alert.BorderBrushProperty, GlobalResourceKey.ColorErrorBorder);
      Add(errorStyle);
      
      // 根据是否显示 Description 设置 Padding
      {
         // 为空
         var paddingStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         paddingStyle.Add(TemplatedControl.PaddingProperty, AlertResourceKey.DefaultPadding);
         Add(paddingStyle);
      }
      {
         // 不为空
         var paddingStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         paddingStyle.Add(TemplatedControl.PaddingProperty, AlertResourceKey.WithDescriptionPadding);
         Add(paddingStyle);
      }
   }

   private void BuildMessageLabelStyle()
   {
      var normalLabel = default(Selector).Nesting().Template().OfType<Label>().Name(MessageLabelPart);
      var marqueeLabel = default(Selector).Nesting().Template().OfType<MarqueeLabel>().Name(MarqueeLabelPart);
      var commonLabelSelector = Selectors.Or(normalLabel, marqueeLabel);
      {
         // Description 不为空
         var descriptionStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         var commonLabelStyle = new Style(selector => commonLabelSelector);
         commonLabelStyle.Add(TemplatedControl.FontSizeProperty, GlobalResourceKey.FontSizeLG);
         commonLabelStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
         descriptionStyle.Add(commonLabelStyle);
         Add(descriptionStyle);
      }
      {
         var descriptionStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         var commonLabelStyle = new Style(selector => commonLabelSelector);
         commonLabelStyle.Add(TemplatedControl.FontSizeProperty, GlobalResourceKey.FontSize);
         commonLabelStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
         descriptionStyle.Add(commonLabelStyle);
         Add(descriptionStyle);
      }
      
      // 根据是否显示 MarqueeLabel 是指对应的样式
      {
         var wrapperStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.IsMessageMarqueEnabledProperty, true));
         var normalLabelStyle = new Style(selector => normalLabel);
         normalLabelStyle.Add(Label.IsVisibleProperty, false);
         wrapperStyle.Add(normalLabelStyle);
         Add(wrapperStyle);
      }
   }

   private void BuildCloseBtnStyle()
   {
      var closeBtnSelector = default(Selector).Nesting().Template().OfType<IconButton>().Name(CloseBtnPart);
      // 设置根据 Description 是否显示 Close 按钮的相关样式
      {
         // 为空
         var descriptionStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         var closeBtnStyle = new Style(selector=> closeBtnSelector);
         closeBtnStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
         descriptionStyle.Add(closeBtnStyle);
         Add(descriptionStyle);
      }
      {
         // 不为空
         var descriptionStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         var closeBtnStyle = new Style(selector=> closeBtnSelector);
         closeBtnStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
         descriptionStyle.Add(closeBtnStyle);
         Add(descriptionStyle);
      }
   }

   private void BuildInfoIconStyle()
   {
      var infoIconSelector = default(Selector).Nesting().Template().OfType<PathIcon>().Name(InfoIconPart);
      {
         var successStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Success));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Add(PathIcon.KindProperty, "CheckCircleFilled");
         infoIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorSuccess);
         successStyle.Add(infoIconStyle);
         Add(successStyle);
      }
      
      {
         var infoStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Add(PathIcon.KindProperty, "InfoCircleFilled");
         infoIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorPrimary);
         infoStyle.Add(infoIconStyle);
         Add(infoStyle);
      }
      
      {
         var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Add(PathIcon.KindProperty, "ExclamationCircleFilled");
         infoIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
         warningStyle.Add(infoIconStyle);
         Add(warningStyle);
      }
      
      {
         var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Add(PathIcon.KindProperty, "CloseCircleFilled");
         infoIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
         errorStyle.Add(infoIconStyle);
         Add(errorStyle);
      }
      
      // 设置根据 Description 设置 InfoIcon 样式
      {
         // 为空
         var wrapperStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         var infoIconStyle = new Style(selector=> infoIconSelector);
         infoIconStyle.Add(Layoutable.WidthProperty, AlertResourceKey.IconSize);
         infoIconStyle.Add(Layoutable.HeightProperty, AlertResourceKey.IconSize);
         infoIconStyle.Add(Layoutable.MarginProperty, AlertResourceKey.IconDefaultMargin);
         infoIconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
         wrapperStyle.Add(infoIconStyle);
         Add(wrapperStyle);
      }
      {
         // 不为空
         var wrapperStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         var infoIconStyle = new Style(selector=> infoIconSelector);
         infoIconStyle.Add(Layoutable.WidthProperty, AlertResourceKey.WithDescriptionIconSize);
         infoIconStyle.Add(Layoutable.HeightProperty, AlertResourceKey.WithDescriptionIconSize);
         infoIconStyle.Add(Layoutable.MarginProperty, AlertResourceKey.IconWithDescriptionMargin);
         infoIconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
         wrapperStyle.Add(infoIconStyle);
         Add(wrapperStyle);
      }
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      var controlTemplate = new FuncControlTemplate<Alert>((alert, scope) =>
      {
         var borderContainer = CreateBorderContainer(alert, scope);
         var infoStack = new StackPanel()
         {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
         };
       
         Grid.SetColumn(infoStack, 1);
         Grid.SetRow(infoStack, 0);
         
         var mainLayout = new Grid()
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
         
         mainLayout.Children.Add(infoStack);
         
         var closeBtn = CreateCloseButton(scope);
         mainLayout.Children.Add(closeBtn);

         var infoIcon = CreateInfoIcon(scope);
         mainLayout.Children.Add(infoIcon);
         
         var normalInfoLabel = CreateMessageLabel(alert, scope);
         infoStack.Children.Add(normalInfoLabel);

         var marqueeLabel = CreateMessageMarqueeLabel(alert, scope);
         infoStack.Children.Add(marqueeLabel);

         var descriptionLabel = CreateDescriptionLabel(alert, scope);
         infoStack.Children.Add(descriptionLabel);
         
         if (alert.ExtraAction is not null) {
            alert.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(alert.ExtraAction, 2);
            Grid.SetRow(alert.ExtraAction, 0);
            mainLayout.Children.Add(alert.ExtraAction);
         }
         
         borderContainer.Child = mainLayout;
         
         return borderContainer;
      });

      return controlTemplate;
   }

   private Border CreateBorderContainer(Alert alert, INameScope scope)
   {
      var borderContainer = new Border();
      
      BindUtils.RelayBind(alert, Alert.BackgroundProperty, borderContainer, ContentPresenter.BackgroundProperty);
      BindUtils.RelayBind(alert, Alert.BorderBrushProperty, borderContainer, ContentPresenter.BorderBrushProperty);
      BindUtils.RelayBind(alert, Alert.CornerRadiusProperty, borderContainer, ContentPresenter.CornerRadiusProperty);
      BindUtils.RelayBind(alert, Alert.BorderThicknessProperty, borderContainer, ContentPresenter.BorderThicknessProperty);
      BindUtils.RelayBind(alert, Alert.PaddingProperty, borderContainer, ContentPresenter.PaddingProperty);
      
      return borderContainer;
   }

   private IconButton CreateCloseButton(INameScope scope)
   {
      var closeBtn = new IconButton
      {
         Name = CloseBtnPart,
      };
      
      TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.WidthProperty, GlobalResourceKey.IconSizeSM);
      TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.HeightProperty, GlobalResourceKey.IconSizeSM);
      TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.MarginProperty, AlertResourceKey.ExtraElementMargin);

      CreateTemplateParentBinding(closeBtn, IconButton.IsVisibleProperty, Alert.IsClosableProperty);
      CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Alert.CloseIconProperty);
      
      Grid.SetRow(closeBtn, 0);
      Grid.SetColumn(closeBtn, 3);
      return closeBtn;
   }

   private PathIcon CreateInfoIcon(INameScope scope)
   {
      var infoIcon = new PathIcon()
      {
         Name = InfoIconPart,
      };
      infoIcon.Bind(PathIcon.IsVisibleProperty, new Binding("IsShowIcon")
      {
         RelativeSource = new RelativeSource()
         {
            Mode = RelativeSourceMode.TemplatedParent
         }
      });
      Grid.SetRow(infoIcon, 0);
      Grid.SetColumn(infoIcon, 0);
      return infoIcon;
   }

   private Label CreateMessageLabel(Alert alert, INameScope scope)
   {
      var label = new Label
      {
         Name = MessageLabelPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         HorizontalContentAlignment = HorizontalAlignment.Left,
         VerticalContentAlignment = VerticalAlignment.Center,
         Padding = new Thickness(0),
      };
      TextBlock.SetTextWrapping(label, TextWrapping.Wrap);
      BindUtils.RelayBind(alert, Alert.MessageProperty, label, Label.ContentProperty);
      return label;
   }

   private MarqueeLabel CreateMessageMarqueeLabel(Alert alert, INameScope scope)
   {
      var label = new MarqueeLabel
      {
         Name = MarqueeLabelPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         Padding = new Thickness(0)
      };
      label.Bind(MarqueeLabel.IsVisibleProperty, new Binding("IsMessageMarqueEnabled")
      {
         RelativeSource = new RelativeSource()
         {
            Mode = RelativeSourceMode.TemplatedParent
         }
      });
      BindUtils.RelayBind(alert, Alert.MessageProperty, label, MarqueeLabel.TextProperty);
      return label;
   }

   private Label CreateDescriptionLabel(Alert alert, INameScope scope)
   {
      var descriptionLabel = new Label
      {
         Name = DescriptionLabelPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         HorizontalContentAlignment = HorizontalAlignment.Left,
         VerticalContentAlignment = VerticalAlignment.Center,
         Padding = new Thickness(0),
         IsVisible = !string.IsNullOrEmpty(alert.Description)
      };
      TokenResourceBinder.CreateTokenBinding(descriptionLabel, Label.MarginProperty, GlobalResourceKey.MarginXS, BindingPriority.Template, 
                                      o =>
                                      {
                                         if (o is double value) {
                                            return new Thickness(0, value, 0, 0);
                                         }

                                         return o;
                                      });
      BindUtils.RelayBind(alert, Alert.DescriptionProperty, descriptionLabel, Label.ContentProperty);
      TextBlock.SetTextWrapping(descriptionLabel, TextWrapping.Wrap);
      return descriptionLabel;
   }
   
}
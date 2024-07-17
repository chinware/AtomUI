using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AlertTheme : ControlTheme
{
   public const string ContentPresenterPart = "PART_ContentPresenter";
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
      Add(new Setter(Alert.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusLG)));
      var successStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Success));
      successStyle.Setters.Add(new Setter(Alert.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorSuccessBg)));
      successStyle.Setters.Add(new Setter(Alert.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorSuccessBorder)));
      Add(successStyle);
      
      var infoStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
      infoStyle.Setters.Add(new Setter(Alert.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorInfoBg)));
      infoStyle.Setters.Add(new Setter(Alert.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorInfoBorder)));
      Add(infoStyle);
      
      var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
      warningStyle.Setters.Add(new Setter(Alert.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorWarningBg)));
      warningStyle.Setters.Add(new Setter(Alert.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorWarningBorder)));
      Add(warningStyle);
      
      var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
      errorStyle.Setters.Add(new Setter(Alert.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBg)));
      errorStyle.Setters.Add(new Setter(Alert.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorBorder)));
      Add(errorStyle);
      
      // 根据是否显示 Description 设置 Padding
      {
         // 为空
         var paddingStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         paddingStyle.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new DynamicResourceExtension(AlertResourceKey.DefaultPadding)));
         Add(paddingStyle);
      }
      {
         // 不为空
         var paddingStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         paddingStyle.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new DynamicResourceExtension(AlertResourceKey.WithDescriptionPadding)));
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
         commonLabelStyle.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, new DynamicResourceExtension(GlobalResourceKey.FontSizeLG)));
         commonLabelStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top));
         descriptionStyle.Add(commonLabelStyle);
         Add(descriptionStyle);
      }
      {
         var descriptionStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         var commonLabelStyle = new Style(selector => commonLabelSelector);
         commonLabelStyle.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, new DynamicResourceExtension(GlobalResourceKey.FontSize)));
         commonLabelStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch));
         descriptionStyle.Add(commonLabelStyle);
         Add(descriptionStyle);
      }
      
      // 根据是否显示 MarqueeLabel 是指对应的样式
      {
         var wrapperStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.IsMessageMarqueEnabledProperty, true));
         var normalLabelStyle = new Style(selector => normalLabel);
         normalLabelStyle.Setters.Add(new Setter(Label.IsVisibleProperty, false));
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
         closeBtnStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center));
         descriptionStyle.Add(closeBtnStyle);
         Add(descriptionStyle);
      }
      {
         // 不为空
         var descriptionStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         var closeBtnStyle = new Style(selector=> closeBtnSelector);
         closeBtnStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top));
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
         infoIconStyle.Setters.Add(new Setter(PathIcon.KindProperty, "CheckCircleFilled"));
         infoIconStyle.Setters.Add(new Setter(PathIcon.NormalFillBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorSuccess)));
         successStyle.Add(infoIconStyle);
         Add(successStyle);
      }

      {
         var infoStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Setters.Add(new Setter(PathIcon.KindProperty, "InfoCircleFilled"));
         infoIconStyle.Setters.Add(new Setter(PathIcon.NormalFillBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorPrimary)));
         infoStyle.Add(infoIconStyle);
         Add(infoStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Setters.Add(new Setter(PathIcon.KindProperty, "ExclamationCircleFilled"));
         infoIconStyle.Setters.Add(new Setter(PathIcon.NormalFillBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorWarning)));
         warningStyle.Add(infoIconStyle);
         Add(warningStyle);
      }

      {
         var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
         var infoIconStyle = new Style(selector => infoIconSelector);
         infoIconStyle.Setters.Add(new Setter(PathIcon.KindProperty, "CloseCircleFilled"));
         infoIconStyle.Setters.Add(new Setter(PathIcon.NormalFillBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError)));
         errorStyle.Add(infoIconStyle);
         Add(errorStyle);
      }
      
      // 设置根据 Description 设置 InfoIcon 样式
      {
         // 为空
         var wrapperStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
         var infoIconStyle = new Style(selector=> infoIconSelector);
         infoIconStyle.Setters.Add(new Setter(Layoutable.WidthProperty, new DynamicResourceExtension(AlertResourceKey.IconSize)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.HeightProperty, new DynamicResourceExtension(AlertResourceKey.IconSize)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.MarginProperty, new DynamicResourceExtension(AlertResourceKey.IconDefaultMargin)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center));
         wrapperStyle.Add(infoIconStyle);
         Add(wrapperStyle);
      }
      {
         // 不为空
         var wrapperStyle = new Style(selector => selector.Nesting().Not(x=> x.PropertyEquals(Alert.DescriptionProperty, null)));
         var infoIconStyle = new Style(selector=> infoIconSelector);
         infoIconStyle.Setters.Add(new Setter(Layoutable.WidthProperty, new DynamicResourceExtension(AlertResourceKey.WithDescriptionIconSize)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.HeightProperty, new DynamicResourceExtension(AlertResourceKey.WithDescriptionIconSize)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.MarginProperty, new DynamicResourceExtension(AlertResourceKey.IconWithDescriptionMargin)));
         infoIconStyle.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top));
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
      var borderContainer = new Border()
      {
         Name = ContentPresenterPart
      };
      
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
      
      BindUtils.CreateTokenBinding(closeBtn, IconButton.WidthProperty, GlobalResourceKey.IconSizeSM);
      BindUtils.CreateTokenBinding(closeBtn, IconButton.HeightProperty, GlobalResourceKey.IconSizeSM);
      BindUtils.CreateTokenBinding(closeBtn, IconButton.MarginProperty, AlertResourceKey.ExtraElementMargin);
      
      closeBtn.Bind(IconButton.IsVisibleProperty, new Binding("IsClosable")
      {
         RelativeSource = new RelativeSource()
         {
            Mode = RelativeSourceMode.TemplatedParent
         }
      });
      closeBtn.Bind(IconButton.IconProperty, new Binding("CloseIcon")
      {
         RelativeSource = new RelativeSource()
         {
            Mode = RelativeSourceMode.TemplatedParent
         }
      });
      
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
      BindUtils.CreateTokenBinding(descriptionLabel, Label.MarginProperty, GlobalResourceKey.MarginXS, BindingPriority.Template, 
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
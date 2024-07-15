using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class AlertTheme : ControlTheme
{
   public const string ContentPresenterPart = "PART_ContentPresenter";
   public const string CloseBtnPart = "PART_CloseBtn";
   public const string InfoIconPart = "PART_InfoIcon";
   public const string DescriptionLabelPart = "PART_DescriptionLabel";
   public const string MessageLabelPart = "PART_MessageLabel";
   public const string MarqueeLabelPart = "PART_MarqueeLabel";
   
   public AlertTheme()
      : base(typeof(TemplatedAlert))
   {
   }
   
   public override void BuildControlTemplate()
   {
      var controlTemplate = new FuncControlTemplate<TemplatedAlert>((alert, scope) =>
      {
         var contentPresenter = CreateContentPresenter(alert, scope);
         
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
         
         contentPresenter.Content = mainLayout;
         ((ISetLogicalParent)contentPresenter).SetParent(alert);
         
         if (alert.ExtraAction is not null) {
            alert.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(alert.ExtraAction, 2);
            Grid.SetRow(alert.ExtraAction, 0);
            mainLayout.Children.Add(alert.ExtraAction);
         }
         return contentPresenter;
      });
      
      var setter = new Setter(TemplatedControl.TemplateProperty, controlTemplate);
      Add(setter);
   }

   private ContentPresenter CreateContentPresenter(TemplatedAlert alert, INameScope scope)
   {
      var contentPresenter = new ContentPresenter()
      {
         Name = ContentPresenterPart
      };
      
      BindUtils.RelayBind(alert, TemplatedAlert.BackgroundProperty, contentPresenter, ContentPresenter.BackgroundProperty);
      BindUtils.RelayBind(alert, TemplatedAlert.BorderBrushProperty, contentPresenter, ContentPresenter.BorderBrushProperty);
      BindUtils.RelayBind(alert, TemplatedAlert.CornerRadiusProperty, contentPresenter, ContentPresenter.CornerRadiusProperty);
      BindUtils.RelayBind(alert, TemplatedAlert.BorderThicknessProperty, contentPresenter, ContentPresenter.BorderThicknessProperty);
      BindUtils.RelayBind(alert, TemplatedAlert.PaddingProperty, contentPresenter, ContentPresenter.PaddingProperty);
      
      contentPresenter.RegisterInNameScope(scope);
      return contentPresenter;
   }

   private IconButton CreateCloseButton(INameScope scope)
   {
      var closeBtn = new IconButton
      {
         Name = CloseBtnPart,
         VerticalAlignment = VerticalAlignment.Top,
      };
      
      BindUtils.CreateTokenBinding(closeBtn, IconButton.WidthProperty, GlobalResourceKey.IconSizeSM);
      BindUtils.CreateTokenBinding(closeBtn, IconButton.HeightProperty, GlobalResourceKey.IconSizeSM);
      BindUtils.CreateTokenBinding(closeBtn, IconButton.MarginProperty, AlertResourceKey.ExtraElementMargin);
      
      Grid.SetRow(closeBtn, 0);
      Grid.SetColumn(closeBtn, 3);
      closeBtn.RegisterInNameScope(scope);
      return closeBtn;
   }

   private PathIcon CreateInfoIcon(INameScope scope)
   {
      var infoIcon = new PathIcon()
      {
         Name = InfoIconPart
      };
      Grid.SetRow(infoIcon, 0);
      Grid.SetColumn(infoIcon, 0);
      infoIcon.RegisterInNameScope(scope);
      return infoIcon;
   }

   private Label CreateMessageLabel(TemplatedAlert alert, INameScope scope)
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
      BindUtils.RelayBind(alert, TemplatedAlert.MessageProperty, label, Label.ContentProperty);
      label.RegisterInNameScope(scope);
      return label;
   }

   private MarqueeLabel CreateMessageMarqueeLabel(TemplatedAlert alert, INameScope scope)
   {
      var label = new MarqueeLabel
      {
         Name = MarqueeLabelPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         Padding = new Thickness(0)
      };
      BindUtils.RelayBind(alert, TemplatedAlert.MessageProperty, label, MarqueeLabel.TextProperty);
      label.RegisterInNameScope(scope);
      return label;
   }

   private Label CreateDescriptionLabel(TemplatedAlert alert, INameScope scope)
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
         
      BindUtils.RelayBind(alert, TemplatedAlert.DescriptionProperty, descriptionLabel, Label.ContentProperty);
      TextBlock.SetTextWrapping(descriptionLabel, TextWrapping.Wrap);
      descriptionLabel.RegisterInNameScope(scope);
      return descriptionLabel;
   }
   
}
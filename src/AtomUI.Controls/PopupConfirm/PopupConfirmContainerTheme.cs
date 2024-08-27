using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PopupConfirmContainerTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_MainLayout";
   public const string OkButtonPart = "PART_OkButton";
   public const string TitlePart = "PART_Title";
   public const string ContentPart = "PART_Content";
   public const string IconContentPart = "PART_IconContent";

   public PopupConfirmContainerTheme()
      : base(typeof(PopupConfirmContainer)) { }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<PopupConfirmContainer>((popupConfirmContainer, scope) =>
      {
         var mainLayout = new DockPanel()
         {
            Name = MainLayoutPart,
            LastChildFill = true
         };
         var buttons = BuildButtons(popupConfirmContainer, scope);
         mainLayout.Children.Add(buttons);
         var content = BuildContent(popupConfirmContainer, scope);
         mainLayout.Children.Add(content);
         TokenResourceBinder.CreateTokenBinding(mainLayout, DockPanel.MinWidthProperty, PopupConfirmTokenResourceKey.PopupMinWidth);
         TokenResourceBinder.CreateTokenBinding(mainLayout, DockPanel.MinHeightProperty, PopupConfirmTokenResourceKey.PopupMinHeight);
         BuildInstanceStyles(popupConfirmContainer);
         return mainLayout;
      });
   }

   private DockPanel BuildContent(PopupConfirmContainer popupConfirmContainer, INameScope scope)
   {
      var wrapperLayout = new DockPanel()
      {
         LastChildFill = true
      };
      
      var contentLayout = new DockPanel()
      {
         LastChildFill = true
      };
      
      var iconContentPresenter = new ContentPresenter()
      {
         Name = IconContentPart
      };

      CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty, PopupConfirmContainer.IconProperty);
      CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.IsVisibleProperty, PopupConfirmContainer.IconProperty,
         BindingMode.Default,
         ObjectConverters.IsNotNull);
      
      DockPanel.SetDock(iconContentPresenter, Dock.Left);
      wrapperLayout.Children.Add(iconContentPresenter);
      wrapperLayout.Children.Add(contentLayout);

      var titleTextBlock = new TextBlock()
      {
         Name = TitlePart,
         TextWrapping = TextWrapping.NoWrap
      };
      CreateTemplateParentBinding(titleTextBlock, TextBlock.TextProperty, PopupConfirmContainer.TitleProperty);
      DockPanel.SetDock(titleTextBlock, Dock.Top);
      contentLayout.Children.Add(titleTextBlock);
      
      var contentPresenter = new ContentPresenter()
      {
         Name = ContentPart
      };
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, PopupConfirmContainer.ConfirmContentProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, PopupConfirmContainer.ConfirmContentTemplateProperty);
      
      contentLayout.Children.Add(contentPresenter);
      
      return wrapperLayout;
   }

   private StackPanel BuildButtons(PopupConfirmContainer popupConfirmContainer, INameScope scope)
   {
      var buttonLayout = new StackPanel()
      {
         Orientation = Orientation.Horizontal,
         HorizontalAlignment = HorizontalAlignment.Right,
         VerticalAlignment = VerticalAlignment.Bottom
      };
      DockPanel.SetDock(buttonLayout, Dock.Bottom);
      var cancelButton = new Button()
      {
         SizeType = SizeType.Small,
         HorizontalAlignment = HorizontalAlignment.Right
      };
      TokenResourceBinder.CreateTokenBinding(cancelButton, Button.MarginProperty, PopupConfirmTokenResourceKey.ButtonMargin);
      CreateTemplateParentBinding(cancelButton, Button.TextProperty, PopupConfirmContainer.CancelTextProperty);
      CreateTemplateParentBinding(cancelButton, Button.IsVisibleProperty, PopupConfirmContainer.IsShowCancelButtonProperty);
      buttonLayout.Children.Add(cancelButton);
      
      var okButton = new Button()
      {
         Name = OkButtonPart,
         SizeType = SizeType.Small,
         HorizontalAlignment = HorizontalAlignment.Right
      };
      okButton.RegisterInNameScope(scope);
      TokenResourceBinder.CreateTokenBinding(okButton, Button.MarginProperty, PopupConfirmTokenResourceKey.ButtonMargin);
      CreateTemplateParentBinding(okButton, Button.TextProperty, PopupConfirmContainer.OkTextProperty);
      CreateTemplateParentBinding(okButton, Button.ButtonTypeProperty, PopupConfirmContainer.OkButtonTypeProperty);
      buttonLayout.Children.Add(okButton);
      return buttonLayout;
   }

   protected override void BuildInstanceStyles(Control control)
   {
      {
         var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSizeLG);
         iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSizeLG);
         iconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Top);
         control.Styles.Add(iconStyle);
      }
      var infoStatusStyle = new Style(selector => selector.PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty, PopupConfirmStatus.Info)
                                                           .Descendant().Name(IconContentPart).Child().OfType<PathIcon>());
      infoStatusStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      control.Styles.Add(infoStatusStyle);
      
      var warningStatusStyle = new Style(selector => selector.PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty, PopupConfirmStatus.Warning)
                                                           .Descendant().Name(IconContentPart).Child().OfType<PathIcon>());
      warningStatusStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorWarning);
      control.Styles.Add(warningStatusStyle);
      
      var errorStatusStyle = new Style(selector => selector.PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty, PopupConfirmStatus.Error)
                                                           .Descendant().Name(IconContentPart).Child().OfType<PathIcon>());
      errorStatusStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorError);
      control.Styles.Add(errorStatusStyle);
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      var iconContentPresenter = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
      iconContentPresenter.Add(ContentPresenter.MarginProperty, PopupConfirmTokenResourceKey.IconMargin);

      var titleStyle = new Style(selector => selector.Nesting().Template().Name(TitlePart));
      titleStyle.Add(TextBlock.MarginProperty, PopupConfirmTokenResourceKey.TitleMargin);
      titleStyle.Add(TextBlock.ForegroundProperty, GlobalTokenResourceKey.ColorTextHeading);
      titleStyle.Add(TextBlock.FontWeightProperty, FontWeight.SemiBold);
      commonStyle.Add(titleStyle);
      
      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.MarginProperty, PopupConfirmTokenResourceKey.ContentContainerMargin);
      commonStyle.Add(contentStyle);
      
      commonStyle.Add(iconContentPresenter);
      Add(commonStyle);
   }
}
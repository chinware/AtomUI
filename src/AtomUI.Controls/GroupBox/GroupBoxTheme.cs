using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class GroupBoxTheme : BaseControlTheme
{
   public const string HeaderPresenterPart = "PART_HeaderPresenter";
   public const string HeaderContainerPart = "PART_HeaderContainer";
   public const string HeaderContentPart = "PART_HeaderContentLayout";
   public const string ContentPresenterPart = "PART_ContentPresenter";
   public const string HeaderIconPart = "PART_HeaderIcon";
   public const string FrameDecoratorPart = "PART_FrameDecorator";

   public GroupBoxTheme()
      : base(typeof(GroupBox)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<GroupBox>((groupBox, scope) =>
      {
         BuildInstanceStyles(groupBox);
         var frameDecorator = new Border()
         {
            Name = FrameDecoratorPart
         };
         CreateTemplateParentBinding(frameDecorator, Border.CornerRadiusProperty, GroupBox.CornerRadiusProperty);
         frameDecorator.RegisterInNameScope(scope);
         var mainLayout = new DockPanel()
         {
            LastChildFill = true
         };

         var headerContainer = new Panel()
         {
            Name = HeaderContainerPart
         };
         headerContainer.RegisterInNameScope(scope);
         DockPanel.SetDock(headerContainer, Dock.Top);

         var headerContentContainer = new Decorator()
         {
            Name = HeaderContentPart,
            VerticalAlignment = VerticalAlignment.Center
         };
         
         var headerContentLayout = new StackPanel()
         {
            Orientation = Orientation.Horizontal,
         };
         headerContentContainer.Child = headerContentLayout;
         headerContentContainer.RegisterInNameScope(scope);
         headerContainer.Children.Add(headerContentContainer);

         var headerIconContentPresenter = new ContentPresenter()
         {
            Name = HeaderIconPart
         };
         CreateTemplateParentBinding(headerIconContentPresenter, ContentPresenter.IsVisibleProperty, GroupBox.HeaderIconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
         CreateTemplateParentBinding(headerIconContentPresenter, ContentPresenter.ContentProperty, GroupBox.HeaderIconProperty);
         headerContentLayout.Children.Add(headerIconContentPresenter);
         
         var headerTextBlock = new TextBlock()
         {
            Name = HeaderPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
         };
         
         CreateTemplateParentBinding(headerTextBlock, TextBlock.TextProperty, GroupBox.HeaderTitleProperty);
         CreateTemplateParentBinding(headerTextBlock, TextBlock.FontSizeProperty, GroupBox.HeaderFontSizeProperty);
         CreateTemplateParentBinding(headerTextBlock, TextBlock.ForegroundProperty, GroupBox.HeaderTitleColorProperty);
         CreateTemplateParentBinding(headerTextBlock, TextBlock.FontStyleProperty, GroupBox.HeaderFontStyleProperty);
         CreateTemplateParentBinding(headerTextBlock, TextBlock.FontWeightProperty, GroupBox.HeaderFontWeightProperty);
         
         headerContentLayout.Children.Add(headerTextBlock);
     
         mainLayout.Children.Add(headerContainer);

         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPresenterPart
         };
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, GroupBox.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                                     GroupBox.ContentTemplateProperty);

         mainLayout.Children.Add(contentPresenter);
         frameDecorator.Child = mainLayout;
         return frameDecorator;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(GroupBox.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(GroupBox.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      commonStyle.Add(GroupBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      commonStyle.Add(GroupBox.HeaderTitleColorProperty, GlobalTokenResourceKey.ColorText);
      commonStyle.Add(GroupBox.HeaderFontSizeProperty, GlobalTokenResourceKey.FontSize);
      
      var headerContainerStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContainerPart));
      headerContainerStyle.Add(Panel.MarginProperty, GroupBoxTokenResourceKey.HeaderContainerMargin);
      commonStyle.Add(headerContainerStyle);
      
      var headerContentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
      headerContentStyle.Add(ContentPresenter.PaddingProperty, GroupBoxTokenResourceKey.HeaderContentPadding);
      commonStyle.Add(headerContentStyle);
      
      var headerIconStyle = new Style(selector => selector.Nesting().Template().Name(HeaderIconPart));
      headerIconStyle.Add(ContentPresenter.MarginProperty, GroupBoxTokenResourceKey.HeaderIconMargin);
      commonStyle.Add(headerIconStyle);
      
      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
      contentStyle.Add(ContentPresenter.PaddingProperty, GroupBoxTokenResourceKey.ContentPadding);
      commonStyle.Add(contentStyle);
      Add(commonStyle);

      BuildHeaderPositionStyle();
   }

   private void BuildHeaderPositionStyle()
   {
      var leftStyle = new Style(selector => selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Left));
      {
         var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
         contentStyle.Add(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         leftStyle.Add(contentStyle);
      }
      Add(leftStyle);
      
      var centerStyle = new Style(selector => selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Center));
      {
         var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
         contentStyle.Add(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         centerStyle.Add(contentStyle);
      }
      Add(centerStyle);
      
      var rightStyle = new Style(selector => selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Right));
      {
         var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
         contentStyle.Add(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Right);
         rightStyle.Add(contentStyle);
      }
      Add(rightStyle);
   }

   protected override void BuildInstanceStyles(Control control)
   {
      var iconStyle = new Style(selector => selector.Name(HeaderIconPart).Descendant().OfType<PathIcon>());
      iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSizeLG);
      iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSizeLG);
      iconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      control.Styles.Add(iconStyle);
   }

}
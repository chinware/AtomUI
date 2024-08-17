using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AddOnDecoratedBoxTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_MainLayout";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string InnerBoxContentPart = "PART_InnerBoxContent";

   public const int NormalZIndex = 1000;
   public const int ActivatedZIndex = 2000;

   public AddOnDecoratedBoxTheme() : base(typeof(AddOnDecoratedBox)) { }
   protected AddOnDecoratedBoxTheme(Type targetType) : base(targetType) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<AddOnDecoratedBox>((decoratedBox, scope) =>
      {
         var mainLayout = new Grid()
         {
            Name = MainLayoutPart,
            ColumnDefinitions = new ColumnDefinitions()
            {
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Star),
               new ColumnDefinition(GridLength.Auto)
            }
         };
         BuildGridChildren(decoratedBox, mainLayout, scope);
         return mainLayout;
      });
   }

   protected override void BuildStyles()
   {
      BuildFixedStyle();
      BuildCommonStyle();
      BuildDisabledStyle();
   }

   protected virtual void BuildGridChildren(AddOnDecoratedBox decoratedBox, Grid mainLayout, INameScope scope)
   {
      BuildLeftAddOn(mainLayout, scope);
      BuildInnerBox(decoratedBox, mainLayout, scope);
      BuildRightAddOn(mainLayout, scope);
   }

   protected virtual void BuildLeftAddOn(Grid layout, INameScope scope)
   {
      var leftAddOnContentPresenter = new ContentPresenter()
      {
         Name = LeftAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
         Focusable = false
      };

      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedBox.LeftAddOnProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  AddOnDecoratedBox.LeftAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  AddOnDecoratedBox.LeftAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  AddOnDecoratedBox.LeftAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      leftAddOnContentPresenter.RegisterInNameScope(scope);

      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             AddOnDecoratedBoxResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      Grid.SetColumn(leftAddOnContentPresenter, 0);
      layout.Children.Add(leftAddOnContentPresenter);
   }

   protected virtual void BuildInnerBox(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
   {
      var innerBox = new ContentPresenter()
      {
         Name = InnerBoxContentPart,
      };
      
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, AddOnDecoratedBox.ContentProperty);
      
      layout.Children.Add(innerBox);
      Grid.SetColumn(innerBox, 1);
   }

   protected virtual void BuildRightAddOn(Grid layout, INameScope scope)
   {
      var rightAddOnContentPresenter = new ContentPresenter()
      {
         Name = RightAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Right,
         Focusable = false
      };
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedBox.RightAddOnProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  AddOnDecoratedBox.RightAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  AddOnDecoratedBox.RightAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  AddOnDecoratedBox.RightAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);

      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             AddOnDecoratedBoxResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      rightAddOnContentPresenter.RegisterInNameScope(scope);
      layout.Children.Add(rightAddOnContentPresenter);
      Grid.SetColumn(rightAddOnContentPresenter, 2);
   }
   
    private void BuildFixedStyle()
   {
      this.Add(AddOnDecoratedBox.VerticalAlignmentProperty, VerticalAlignment.Center);
      this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(AddOnDecoratedBox.ForegroundProperty, GlobalResourceKey.ColorText);

      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxResourceKey.PaddingLG);
         largeStyle.Add(addOnStyle);
      }

      largeStyle.Add(AddOnDecoratedBox.FontSizeProperty, AddOnDecoratedBoxResourceKey.FontSizeLG);
      // largeStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeightLG);
      largeStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxResourceKey.Padding);
         middleStyle.Add(addOnStyle);
      }
      
      middleStyle.Add(AddOnDecoratedBox.FontSizeProperty, AddOnDecoratedBoxResourceKey.FontSize);
      // middleStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeight);
      middleStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, AddOnDecoratedBoxResourceKey.PaddingSM);
         smallStyle.Add(addOnStyle);
      }
      
      smallStyle.Add(AddOnDecoratedBox.FontSizeProperty, AddOnDecoratedBoxResourceKey.FontSizeSM);
      // smallStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeightSM);
      smallStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      commonStyle.Add(smallStyle);

      Add(commonStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(AddOnDecoratedBox.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      // TODO 暂时这么简单处理吧
      var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
      addOnStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(addOnStyle);
      Add(disabledStyle);
   }
}
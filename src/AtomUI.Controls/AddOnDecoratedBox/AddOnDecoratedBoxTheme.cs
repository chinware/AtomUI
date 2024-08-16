using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace AtomUI.Controls.AddOnDecoratedBox;

internal class AddOnDecoratedBoxTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_FrameDecorator";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string InnerBoxContentPart = "PART_InnerBoxContent";
   public const string InnerBoxDecoratorPart = "PART_InnerBoxDecorator";

   public const int NormalZIndex = 1000;
   public const int ActivatedZIndex = 2000;

   public AddOnDecoratedBoxTheme(Type targetType) : base(targetType) { }

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

   protected override void BuildStyles() { }

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
      var innerBoxDecorator = new Border()
      {
         Name = InnerBoxDecoratorPart,
         Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
         }
      };
      CreateTemplateParentBinding(innerBoxDecorator, Border.BorderThicknessProperty, AddOnDecoratedBox.BorderThicknessProperty);
      CreateTemplateParentBinding(innerBoxDecorator, Border.CornerRadiusProperty, AddOnDecoratedBox.InnerBoxCornerRadiusProperty);

      innerBoxDecorator.RegisterInNameScope(scope);

      var innerBox = new ContentPresenter()
      {
         Name = InnerBoxContentPart,
      };
      
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, AddOnDecoratedBox.ContentProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentTemplateProperty, AddOnDecoratedBox.ContentTemplateProperty);
      
      innerBoxDecorator.Child = innerBox;
      layout.Children.Add(innerBoxDecorator);
      Grid.SetColumn(innerBoxDecorator, 1);
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
}
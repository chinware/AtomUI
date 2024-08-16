using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class AddOnDecoratedInnerBoxTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_MainLayout";
   public const string ContentPresenterPart = "PART_ContentPresenter";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string LeftAddOnLayoutPart = "PART_LeftAddOnLayout";
   public const string RightAddOnLayoutPart = "PART_RightAddOnLayout";
   public const string ClearButtonPart = "PART_ClearButton";
   public const string RevealButtonPart = "PART_RevealButton";

   public AddOnDecoratedInnerBoxTheme(Type targetType) : base(targetType) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<AddOnDecoratedInnerBox>((decoratedBox, scope) =>
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

   protected virtual void BuildGridChildren(AddOnDecoratedInnerBox decoratedBox, Grid mainLayout, INameScope scope)
   {
      BuildLeftAddOn(mainLayout, scope);
      BuildContent(decoratedBox, mainLayout, scope);
      BuildRightAddOn(mainLayout, scope);
   }

   protected virtual void BuildLeftAddOn(Grid layout, INameScope scope)
   {
      // 理论上可以支持多个，暂时先支持一个
      var addLayout = new StackPanel()
      {
         Name = LeftAddOnLayoutPart,
         Orientation = Orientation.Horizontal,
      };
      TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty, GlobalResourceKey.PaddingXXS);
      addLayout.RegisterInNameScope(scope);
      
      var leftAddOnContentPresenter = new ContentPresenter()
      {
         Name = LeftAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
         Focusable = false,
      };

      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedInnerBox.LeftAddOnContentProperty);
      leftAddOnContentPresenter.RegisterInNameScope(scope);

      addLayout.Children.Add(leftAddOnContentPresenter);

      Grid.SetColumn(addLayout, 0);
      layout.Children.Add(addLayout);
   }

   protected virtual void BuildContent(AddOnDecoratedInnerBox decoratedBox, Grid layout, INameScope scope)
   {
      var innerBox = new ContentPresenter()
      {
         Name = ContentPresenterPart,
      };
      innerBox.RegisterInNameScope(scope);

      CreateTemplateParentBinding(innerBox, ContentPresenter.MarginProperty, AddOnDecoratedInnerBox.ContentPresenterMarginProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, AddOnDecoratedInnerBox.ContentProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentTemplateProperty,
                                  AddOnDecoratedInnerBox.ContentTemplateProperty);

      layout.Children.Add(innerBox);
      Grid.SetColumn(innerBox, 1);
   }

   private void BuildRightAddOn(Grid layout, INameScope scope)
   {
      var addLayout = new StackPanel()
      {
         Name = RightAddOnLayoutPart,
         Orientation = Orientation.Horizontal
      };
      TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty, GlobalResourceKey.PaddingXXS);
      addLayout.RegisterInNameScope(scope);
      var rightAddOnContentPresenter = new ContentPresenter()
      {
         Name = RightAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Right,
         Focusable = false
      };
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedInnerBox.RightAddOnContentProperty);

      rightAddOnContentPresenter.RegisterInNameScope(scope);
      addLayout.Children.Add(rightAddOnContentPresenter);
      
      BuildRightAddOnItems(addLayout, scope);

      layout.Children.Add(addLayout);
      Grid.SetColumn(addLayout, 2);
   }

   protected virtual void BuildRightAddOnItems(StackPanel layout, INameScope scope)
   {
      BuildClearButton(layout, scope);
   }

   protected virtual void BuildClearButton(StackPanel addOnLayout, INameScope scope)
   {
      var closeIcon = new PathIcon()
      {
         Kind = "CloseCircleFilled"
      };
      var clearButton = new IconButton()
      {
         Name = ClearButtonPart,
         Icon = closeIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalResourceKey.ColorText);

      clearButton.RegisterInNameScope(scope);
      CreateTemplateParentBinding(clearButton, IconButton.IsVisibleProperty,
                                  AddOnDecoratedInnerBox.IsClearButtonVisibleProperty);
      addOnLayout.Children.Add(clearButton);
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
   }
   
   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Large));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeLG);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeLG);
         largeStyle.Add(iconStyle);
      }
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Middle));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         middleStyle.Add(iconStyle);
      }
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Small));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
         smallStyle.Add(iconStyle);
      }
      commonStyle.Add(smallStyle);
      
      Add(commonStyle);
   }
}
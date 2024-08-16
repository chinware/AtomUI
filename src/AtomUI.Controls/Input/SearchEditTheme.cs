using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;
//
// [ControlThemeProvider]
// internal class SearchEditTheme : LineEditTheme
// {
//    public SearchEditTheme() : base(typeof(SearchEdit)) { }
//    
//    protected override void BuildRightAddOn(Grid layout, INameScope scope)
//    {
//       var searchIcon = new PathIcon()
//       {
//          Kind = "SearchOutlined"
//       };
//       TokenResourceBinder.CreateGlobalTokenBinding(searchIcon, PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorIcon);
//       var rightAddOnContentPresenter = new Button()
//       {
//          Name = RightAddOnPart,
//          Focusable = false,
//          Icon = searchIcon
//       };
//       CreateTemplateParentBinding(rightAddOnContentPresenter, Button.ContentProperty,
//                                   SearchEdit.RightAddOnProperty);
//       CreateTemplateParentBinding(rightAddOnContentPresenter, Button.BorderThicknessProperty,
//                                   SearchEdit.RightAddOnBorderThicknessProperty);
//       CreateTemplateParentBinding(rightAddOnContentPresenter, Button.CornerRadiusProperty,
//                                   SearchEdit.RightAddOnCornerRadiusProperty);
//       CreateTemplateParentBinding(rightAddOnContentPresenter, Button.TextProperty,
//                                   SearchEdit.SearchButtonTextProperty);
//       CreateTemplateParentBinding(rightAddOnContentPresenter, Button.SizeTypeProperty,
//                                   SearchEdit.SizeTypeProperty);
//
//       rightAddOnContentPresenter.RegisterInNameScope(scope);
//       layout.Children.Add(rightAddOnContentPresenter);
//       Grid.SetColumn(rightAddOnContentPresenter, 2);
//    }
//
//    protected override void BuildStyles()
//    {
//       base.BuildStyles();
//
//       var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
//       decoratorStyle.Add(Border.ZIndexProperty, NormalZIndex);
//       Add(decoratorStyle);
//          
//       var decoratorHoverOrFocusStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LineEditKernelDecoratorPart).Class(StdPseudoClass.FocusWithIn),
//                                                                           selector.Nesting().Template().Name(LineEditKernelDecoratorPart).Class(StdPseudoClass.PointerOver)));
//       decoratorHoverOrFocusStyle.Add(Border.ZIndexProperty, ActivatedZIndex);
//       Add(decoratorHoverOrFocusStyle);
//       
//       var searchButtonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
//       searchButtonStyle.Add(Border.ZIndexProperty, NormalZIndex);
//       Add(searchButtonStyle);
//          
//       var searchButtonStyleHoverOrFocusStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(RightAddOnPart).Class(StdPseudoClass.Pressed),
//                                                             selector.Nesting().Template().Name(RightAddOnPart).Class(StdPseudoClass.PointerOver)));
//       searchButtonStyleHoverOrFocusStyle.Add(Border.ZIndexProperty, ActivatedZIndex);
//       Add(searchButtonStyleHoverOrFocusStyle);
//       
//       // Icon button
//       var iconSearchButtonStyle = new Style(selector => selector.Nesting().PropertyEquals(SearchEdit.SearchButtonStyleProperty, SearchEditButtonStyle.Default));
//       {
//          var buttonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
//          buttonStyle.Add(Button.IsIconVisibleProperty, true);
//          buttonStyle.Add(Button.ButtonTypeProperty, ButtonType.Default);
//          iconSearchButtonStyle.Add(buttonStyle);
//       }
//       Add(iconSearchButtonStyle);
//       
//       // primary button
//       var primarySearchButtonStyle = new Style(selector => selector.Nesting().PropertyEquals(SearchEdit.SearchButtonStyleProperty, SearchEditButtonStyle.Primary));
//       {
//          var buttonStyle = new Style(selector => selector.Nesting().Template().Name(RightAddOnPart));
//          buttonStyle.Add(Button.IsIconVisibleProperty, false);
//          buttonStyle.Add(Button.ButtonTypeProperty, ButtonType.Primary);
//          primarySearchButtonStyle.Add(buttonStyle);
//       }
//       Add(primarySearchButtonStyle);
//    }
// }
using Avalonia;

namespace AtomUI.Controls;

public class SearchEditDecoratedBox : AddOnDecoratedBox
{
   #region 公共属性定义
   
   public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
      SearchEdit.SearchButtonStyleProperty.AddOwner<SearchEditDecoratedBox>();
   
   public static readonly StyledProperty<string> SearchButtonTextProperty =
      SearchEdit.SearchButtonTextProperty.AddOwner<SearchEditDecoratedBox>();
   
   public SearchEditButtonStyle SearchButtonStyle
   {
      get => GetValue(SearchButtonStyleProperty);
      set => SetValue(SearchButtonStyleProperty, value);
   }
   
   public object? SearchButtonText
   {
      get => GetValue(SearchButtonTextProperty);
      set => SetValue(SearchButtonTextProperty, value);
   }
   #endregion

}
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public enum SearchEditButtonStyle
{
   Default,
   Primary
}

public class SearchEdit : LineEdit
{
   #region 公共属性定义
   
   public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
      AvaloniaProperty.Register<SearchEdit, SearchEditButtonStyle>(nameof(SearchButtonStyle), SearchEditButtonStyle.Default);
   
   public static readonly StyledProperty<string> SearchButtonTextProperty =
      AvaloniaProperty.Register<SearchEdit, string>(nameof(SearchButtonText));
   
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

   private Rect? _originRect;
   
   protected override void NotifyAddOnBorderInfoCalculated()
   {
      RightAddOnBorderThickness = BorderThickness;
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var size = base.ArrangeOverride(finalSize);
      if (_originRect is null) {
         _originRect = _rightAddOnPresenter?.Bounds;
      }
      if (_rightAddOnPresenter is not null && _originRect.HasValue) {
         _rightAddOnPresenter.Arrange(_originRect.Value.Inflate(new Thickness(1, 0, 0, 0)));
      }

      return size;
   }
}
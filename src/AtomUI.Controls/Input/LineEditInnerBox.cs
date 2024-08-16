using Avalonia;

namespace AtomUI.Controls;

public class LineEditInnerBox : AddOnDecoratedInnerBox
{
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsRevealButtonVisibleProperty =
      AvaloniaProperty.Register<LineEditInnerBox, bool>(nameof(IsRevealButtonVisible));
   
   public static readonly StyledProperty<bool> IsRevealButtonCheckedProperty =
      AvaloniaProperty.Register<LineEditInnerBox, bool>(nameof(IsRevealButtonChecked));
   
   public bool IsRevealButtonVisible
   {
      get => GetValue(IsRevealButtonVisibleProperty);
      set => SetValue(IsRevealButtonVisibleProperty, value);
   }
   
   public bool IsRevealButtonChecked
   {
      get => GetValue(IsRevealButtonCheckedProperty);
      set => SetValue(IsRevealButtonCheckedProperty, value);
   }

   #endregion
   
   private WeakReference<LineEdit> _lineEdit;

   public LineEditInnerBox(LineEdit edit)
   {
      _lineEdit = new WeakReference<LineEdit>(edit);
   }
   
   protected override void NotifyClearButtonClicked()
   {
      if (_lineEdit.TryGetTarget(out var lineEdit)) {
         lineEdit.Clear();
      }
   }
}
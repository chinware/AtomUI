using Avalonia;

namespace AtomUI.Controls;

public class LineEditInnerBox : AddOnDecoratedInnerBox
{
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsRevealButtonVisibleProperty =
      AvaloniaProperty.Register<LineEditInnerBox, bool>(nameof(IsRevealButtonVisible));
   
   public static readonly StyledProperty<bool> IsRevealButtonCheckedProperty =
      AvaloniaProperty.Register<LineEditInnerBox, bool>(nameof(IsRevealButtonChecked));
   
   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AddOnDecoratedBox.StatusProperty.AddOwner<LineEditInnerBox>();
   
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
   
   public AddOnDecoratedStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
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
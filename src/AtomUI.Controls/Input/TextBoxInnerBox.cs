using Avalonia;

namespace AtomUI.Controls;

public class TextBoxInnerBox : AddOnDecoratedInnerBox
{
   protected override Type StyleKeyOverride => typeof(AddOnDecoratedInnerBox);
   
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsRevealButtonVisibleProperty =
      AvaloniaProperty.Register<TextBoxInnerBox, bool>(nameof(IsRevealButtonVisible));
   
   public static readonly StyledProperty<bool> IsRevealButtonCheckedProperty =
      AvaloniaProperty.Register<TextBoxInnerBox, bool>(nameof(IsRevealButtonChecked));
   
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

   #region 内部属性定义

   internal static readonly DirectProperty<TextBoxInnerBox, bool> DisabledInnerBoxPaddingProperty =
      AvaloniaProperty.RegisterDirect<TextBoxInnerBox, bool>(nameof(DisabledInnerBoxPadding),
                                                             o => o.DisabledInnerBoxPadding,
                                                             (o, v) => o.DisabledInnerBoxPadding = v);
   
   private bool _disabledInnerBoxPadding;
   internal bool DisabledInnerBoxPadding
   {
      get => _disabledInnerBoxPadding;
      set => SetAndRaise(DisabledInnerBoxPaddingProperty, ref _disabledInnerBoxPadding, value);
   }

   #endregion
   
   private WeakReference<TextBox> _textBox;

   public TextBoxInnerBox(TextBox textBox)
   {
      _textBox = new WeakReference<TextBox>(textBox);
   }
   
   protected override void NotifyClearButtonClicked()
   {
      if (_textBox.TryGetTarget(out var textBox)) {
         textBox.Clear();
      }
   }
   
   protected override void BuildEffectiveInnerBoxPadding()
   {
      if (!_disabledInnerBoxPadding) {
         base.BuildEffectiveInnerBoxPadding();
      }
   }
}
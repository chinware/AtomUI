using Avalonia;

namespace AtomUI.Controls;

internal class TextBoxInnerBox : AddOnDecoratedInnerBox
{
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
   
   internal static readonly DirectProperty<TextBoxInnerBox, bool> EmbedModeProperty =
      AvaloniaProperty.RegisterDirect<TextBoxInnerBox, bool>(nameof(EmbedMode),
                                                             o => o.EmbedMode,
                                                             (o, v) => o.EmbedMode = v);
   
   private bool _embedMode = false;
   internal bool EmbedMode
   {
      get => _embedMode;
      set => SetAndRaise(EmbedModeProperty, ref _embedMode, value);
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
      if (!_embedMode) {
         base.BuildEffectiveInnerBoxPadding();
      }
   }
}
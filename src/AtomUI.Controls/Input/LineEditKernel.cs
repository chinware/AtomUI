using AtomUI.Media;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace AtomUI.Controls;

internal class LineEditKernel : Border
{
   #region 公共属性定义
   public static readonly DirectProperty<LineEditKernel, ContentPresenter?> LeftInnerContentProperty =
      AvaloniaProperty.RegisterDirect<LineEditKernel, ContentPresenter?>(nameof(LeftInnerContent),
                                                                               o => o.LeftInnerContent,
                                                                               (o, v) => o.LeftInnerContent = v);
   
   public static readonly DirectProperty<LineEditKernel, ContentPresenter?> RightInnerContentProperty =
      AvaloniaProperty.RegisterDirect<LineEditKernel, ContentPresenter?>(nameof(RightInnerContent),
                                                                               o => o.RightInnerContent,
                                                                               (o, v) => o.RightInnerContent = v);
   
   public static readonly DirectProperty<LineEditKernel, IconButton?> ClearButtonProperty =
      AvaloniaProperty.RegisterDirect<LineEditKernel, IconButton?>(nameof(ClearButton),
                                                                         o => o.ClearButton,
                                                                         (o, v) => o.ClearButton = v);
   
   public static readonly DirectProperty<LineEditKernel, ToggleIconButton?> RevealButtonProperty =
      AvaloniaProperty.RegisterDirect<LineEditKernel, ToggleIconButton?>(nameof(RevealButton),
                                                                               o => o.RevealButton,
                                                                               (o, v) => o.RevealButton = v);
   
   public static readonly DirectProperty<LineEditKernel, ScrollViewer?> TextPresenterProperty =
      AvaloniaProperty.RegisterDirect<LineEditKernel, ScrollViewer?>(nameof(TextPresenter),
                                                                           o => o.TextPresenter,
                                                                           (o, v) => o.TextPresenter = v);
   
   private ContentPresenter? _leftInnerContent;
   public ContentPresenter? LeftInnerContent
   {
      get => _leftInnerContent;
      set => SetAndRaise(LeftInnerContentProperty, ref _leftInnerContent, value);
   }
   
   private ContentPresenter? _rightInnerContent;
   public ContentPresenter? RightInnerContent
   {
      get => _rightInnerContent;
      set => SetAndRaise(RightInnerContentProperty, ref _rightInnerContent, value);
   }
   
   private IconButton? _clearButton;
   public IconButton? ClearButton
   {
      get => _clearButton;
      set => SetAndRaise(ClearButtonProperty, ref _clearButton, value);
   }
   
   private ToggleIconButton? _revealButton;
   public ToggleIconButton? RevealButton
   {
      get => _revealButton;
      set => SetAndRaise(RevealButtonProperty, ref _revealButton, value);
   }
   
   private ScrollViewer? _textPresenter;
   public ScrollViewer? TextPresenter
   {
      get => _textPresenter;
      set => SetAndRaise(TextPresenterProperty, ref _textPresenter, value);
   }
   
   #endregion

   static LineEditKernel()
   {
      AffectsMeasure<TabsContainerPanel>(LeftInnerContentProperty,
                                         RightInnerContentProperty,
                                         ClearButtonProperty,
                                         RevealButtonProperty,
                                         TextPresenterProperty);
   }

   private Panel _layout;

   public LineEditKernel()
   {
      _layout = new Panel();
      Child = _layout;
      Focusable = true;
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == LeftInnerContentProperty) {
         var oldLeftInnerContent = change.GetOldValue<ContentPresenter?>();
         if (oldLeftInnerContent is not null) {
            _layout.Children.Remove(oldLeftInnerContent);
         }

         if (LeftInnerContent is not null) {
            _layout.Children.Add(LeftInnerContent);
         }
      } else if (change.Property == RightInnerContentProperty) {
         var oldRightInnerContent = change.GetOldValue<ContentPresenter?>();
         if (oldRightInnerContent is not null) {
            _layout.Children.Remove(oldRightInnerContent);
         }

         if (RightInnerContent is not null) {
            _layout.Children.Add(RightInnerContent);
         }
      } else if (change.Property == ClearButtonProperty) {
         var oldClearButton = change.GetOldValue<IconButton?>();
         if (oldClearButton is not null) {
            _layout.Children.Remove(oldClearButton);
         }

         if (ClearButton is not null) {
            _layout.Children.Add(ClearButton);
         }
      } else if (change.Property == RevealButtonProperty) {
         var oldRevealButton = change.GetOldValue<ToggleIconButton?>();
         if (oldRevealButton is not null) {
            _layout.Children.Remove(oldRevealButton);
         }

         if (RevealButton is not null) {
            _layout.Children.Add(RevealButton);
         }
      } else if (change.Property == TextPresenterProperty) {
         var oldTextPresenter = change.GetOldValue<ScrollViewer?>();
         if (oldTextPresenter is not null) {
            _layout.Children.Remove(oldTextPresenter);
         }

         if (TextPresenter is not null) {
            _layout.Children.Add(TextPresenter);
         }
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      return new Size(availableSize.Width, size.Height);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var offsetLeft = Padding.Left;
      if (LeftInnerContent is not null) {
         offsetLeft += LeftInnerContent.DesiredSize.Width;
         LeftInnerContent.Arrange(new Rect(new Point(offsetLeft, (finalSize.Height - LeftInnerContent.DesiredSize.Height) / 2), LeftInnerContent.DesiredSize));
      }

      var offsetRight = finalSize.Width - Padding.Right;
      if (RightInnerContent is not null) {
         offsetRight -= RightInnerContent.DesiredSize.Width;
         RightInnerContent.Arrange(new Rect(new Point(offsetRight, (finalSize.Height - RightInnerContent.DesiredSize.Height) / 2), RightInnerContent.DesiredSize));
      }
      
      if (RevealButton is not null) {
         offsetRight -= RevealButton.DesiredSize.Width;
         RevealButton.Arrange(new Rect(new Point(offsetRight, (finalSize.Height - RevealButton.DesiredSize.Height) / 2), RevealButton.DesiredSize));
      }
      
      if (ClearButton is not null) {
         offsetRight -= ClearButton.DesiredSize.Width;
         ClearButton.Arrange(new Rect(new Point(offsetRight, (finalSize.Height - ClearButton.DesiredSize.Height) / 2), ClearButton.DesiredSize));
      }

      if (TextPresenter is not null) {
         TextPresenter.Arrange(new Rect(new Point(offsetLeft, 0), new Size(offsetRight - offsetLeft, finalSize.Height)));
      }
      
      return new Size(finalSize.Width - BorderThickness.Left - BorderThickness.Right, finalSize.Height);
   }

   public override void ApplyTemplate()
   {
      base.ApplyTemplate();
      if (Transitions is null) {
         Transitions = new Transitions();
         Transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
         Transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
      }
   }
}
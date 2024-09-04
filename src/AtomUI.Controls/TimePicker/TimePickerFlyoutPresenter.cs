using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class TimePickerFlyoutPresenter : FlyoutPresenter
{
   protected override Type StyleKeyOverride => typeof(TimePickerFlyoutPresenter);
   
   internal TimePicker TimePickerRef { get; set; }

   private TimePickerPresenter? _timePickerPresenter;
   private IDisposable? _disposable;
   private Button? _confirmButton;

   public TimePickerFlyoutPresenter(TimePicker timePicker)
   {
      TimePickerRef = timePicker;
      HorizontalAlignment = HorizontalAlignment.Left;
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _timePickerPresenter = e.NameScope.Get<TimePickerPresenter>(TimePickerFlyoutPresenterTheme.ContentPresenterPart);
      _confirmButton = e.NameScope.Get<Button>(TimePickerFlyoutPresenterTheme.ConfirmButtonPart);
      if (_timePickerPresenter is not null) {
         _timePickerPresenter.Confirmed += (sender, args) =>
         {
            TimePickerRef.NotifyConfirmed(_timePickerPresenter.Time);
         };
      }

      if (_confirmButton is not null) {
         _confirmButton.Click += HandleConfirmButtonClicked;
      }
   }

   private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
   {
      _timePickerPresenter?.Confirm();
      TimePickerRef.ClosePickerFlyout();
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (_timePickerPresenter is not null) {
         _disposable = TimePickerPresenter.TemporaryTimeProperty.Changed.Subscribe(args =>
         {
            TimePickerRef.NotifyTemporaryTimeSelected(args.GetNewValue<TimeSpan>());
         });
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _disposable?.Dispose();
   }
}
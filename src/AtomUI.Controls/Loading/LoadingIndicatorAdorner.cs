using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class LoadingIndicatorAdorner : Control, IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private LoadingIndicator? _loadingIndicator;

   public EventHandler<LoadingIndicatorCreatedEventArgs>? IndicatorCreated;

   public LoadingIndicatorAdorner()
   {
      _customStyle = this;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUI();
         _initialized = true;
      }
   }

   void IControlCustomStyle.SetupUI()
   {
      _loadingIndicator = new LoadingIndicator();
      IndicatorCreated?.Invoke(this, new LoadingIndicatorCreatedEventArgs(_loadingIndicator));
      LogicalChildren.Add(_loadingIndicator);
      VisualChildren.Add(_loadingIndicator);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var offsetX = (finalSize.Width - _loadingIndicator!.DesiredSize.Width) / 2;
      var offsetY = (finalSize.Height - _loadingIndicator.DesiredSize.Height) / 2;
      _loadingIndicator.Arrange(new Rect(new Point(offsetX, offsetY), _loadingIndicator.DesiredSize));
      return finalSize;
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      base.MeasureOverride(availableSize);
      return availableSize;
   }
}

public class LoadingIndicatorCreatedEventArgs : EventArgs
{
   public LoadingIndicator LoadingIndicator { get; set; }

   public LoadingIndicatorCreatedEventArgs(LoadingIndicator indicator)
   {
      LoadingIndicator = indicator;
   }
}
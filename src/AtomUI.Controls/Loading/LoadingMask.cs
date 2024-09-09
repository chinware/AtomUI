using AtomUI.Data;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class LoadingMask : AvaloniaObject, IDisposable
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      LoadingIndicator.SizeTypeProperty.AddOwner<LoadingMask>();

   public static readonly StyledProperty<string?> LoadingMsgProperty =
      LoadingIndicator.LoadingMsgProperty.AddOwner<LoadingMask>();
   
   public static readonly StyledProperty<bool> IsShowLoadingMsgProperty =
      LoadingIndicator.IsShowLoadingMsgProperty.AddOwner<LoadingMask>();
   
   public static readonly StyledProperty<PathIcon?> CustomIndicatorIconProperty =
      LoadingIndicator.CustomIndicatorIconProperty.AddOwner<LoadingMask>();
   
   public static readonly StyledProperty<TimeSpan?> MotionDurationProperty =
      LoadingIndicator.MotionDurationProperty.AddOwner<LoadingMask>();
   
   public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
      LoadingIndicator.MotionEasingCurveProperty.AddOwner<LoadingMask>();
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public string? LoadingMsg
   {
      get => GetValue(LoadingMsgProperty);
      set => SetValue(LoadingMsgProperty, value);
   }
   
   public bool IsShowLoadingMsg
   {
      get => GetValue(IsShowLoadingMsgProperty);
      set => SetValue(IsShowLoadingMsgProperty, value);
   }
   
   public PathIcon? CustomIndicatorIcon
   {
      get => GetValue(CustomIndicatorIconProperty);
      set => SetValue(CustomIndicatorIconProperty, value);
   }
      
   public TimeSpan? MotionDuration
   {
      get => GetValue(MotionDurationProperty);
      set => SetValue(MotionDurationProperty, value);
   }
   
   public Easing? MotionEasingCurve
   {
      get => GetValue(MotionEasingCurveProperty);
      set => SetValue(MotionEasingCurveProperty, value);
   }
   
   public bool IsLoading
   {
      get;
      private set;
   }
   
   #endregion
   
   private Control? _attachTarget;
   private LoadingIndicatorAdorner? _loadingIndicatorAdorner;
   private AdornerLayer? _adornerLayer;
   
   public LoadingMask(Control? attachTarget = null)
   {
      if (attachTarget is not null) {
         Attach(attachTarget);
      }
   }

   public void Attach(Control attachTarget)
   {
      if (_attachTarget is not null) {
         Detach();
      }

      attachTarget.DetachedFromLogicalTree += HandleTargetDetachedFromVisualTree;
      _attachTarget = attachTarget;
   }

   public void Detach()
   {
      if (_attachTarget is null) {
         return;
      }
      Hide();
      _attachTarget.DetachedFromLogicalTree -= HandleTargetDetachedFromVisualTree;
      _attachTarget = null;
   }

   public void Show()
   {
      if (_attachTarget is null || IsLoading) {
         return;
      }

      if (_adornerLayer is null) {
         _adornerLayer = AdornerLayer.GetAdornerLayer(_attachTarget);
         if (_adornerLayer == null) {
            throw new SystemException("No Adorner Layer found.");
         }
      }

      _loadingIndicatorAdorner ??= new LoadingIndicatorAdorner();
      _loadingIndicatorAdorner.IndicatorCreated += HandleLoadingIndicatorCreated;
      
     _attachTarget.Effect = new BlurEffect()
      {
         Radius = 5
      };
      AdornerLayer.SetAdornedElement(_loadingIndicatorAdorner, _attachTarget);
      AdornerLayer.SetIsClipEnabled(_loadingIndicatorAdorner, true);
      _adornerLayer.Children.Add(_loadingIndicatorAdorner);
    
      IsLoading = true;
   }

   // 在这里进行配置
   private void HandleLoadingIndicatorCreated(object? sender, LoadingIndicatorCreatedEventArgs args)
   {
      var indicator = args.LoadingIndicator;
      BindUtils.RelayBind(this, SizeTypeProperty, indicator, SizeTypeProperty);
      BindUtils.RelayBind(this, LoadingMsgProperty, indicator, LoadingMsgProperty);
      BindUtils.RelayBind(this, IsShowLoadingMsgProperty, indicator, IsShowLoadingMsgProperty);
      BindUtils.RelayBind(this, CustomIndicatorIconProperty, indicator, CustomIndicatorIconProperty);
      BindUtils.RelayBind(this, MotionDurationProperty, indicator, MotionDurationProperty);
      BindUtils.RelayBind(this, MotionEasingCurveProperty, indicator, MotionEasingCurveProperty);
   }

   public void Hide()
   {
      if (!IsLoading || _attachTarget is null) {
         return;
      }
      if (_adornerLayer is not null && _loadingIndicatorAdorner is not null) {
         _adornerLayer.Children.Remove(_loadingIndicatorAdorner);
      }
      
      _attachTarget.Effect = null;
      IsLoading = false;
   }

   public void Dispose()
   {
      if (_attachTarget is not null) {
         Detach();
      }

      _adornerLayer = null;
      _loadingIndicatorAdorner = null;
   }
   
   private void HandleTargetDetachedFromVisualTree(object? sender, LogicalTreeAttachmentEventArgs args)
   {
      Dispose();
   }
}
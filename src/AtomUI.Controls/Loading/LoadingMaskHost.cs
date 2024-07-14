using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class LoadingMaskHost : Control, IControlCustomStyle
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      LoadingIndicator.SizeTypeProperty.AddOwner<LoadingMaskHost>();

   public static readonly StyledProperty<string?> LoadingMsgProperty =
      LoadingIndicator.LoadingMsgProperty.AddOwner<LoadingMaskHost>();
   
   public static readonly StyledProperty<bool> IsShowLoadingMsgProperty =
      LoadingIndicator.IsShowLoadingMsgProperty.AddOwner<LoadingMaskHost>();
   
   public static readonly StyledProperty<PathIcon?> CustomIndicatorIconProperty =
      LoadingIndicator.CustomIndicatorIconProperty.AddOwner<LoadingMaskHost>();
   
   public static readonly StyledProperty<TimeSpan?> MotionDurationProperty =
      LoadingIndicator.MotionDurationProperty.AddOwner<LoadingMaskHost>();
   
   public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
      LoadingIndicator.MotionEasingCurveProperty.AddOwner<LoadingMaskHost>();

   public static readonly StyledProperty<bool> IsLoadingProperty =
      AvaloniaProperty.Register<LoadingMaskHost, bool>(nameof(IsLoading), false);
   
   public static readonly StyledProperty<Control?> MaskTargetProperty =
      AvaloniaProperty.Register<CountBadge, Control?>(nameof(MaskTarget));
   
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
      get => GetValue(IsLoadingProperty);
      set => SetValue(IsLoadingProperty, value);
   }
   
   [Content]
   public Control? MaskTarget
   {
      get => GetValue(MaskTargetProperty);
      set => SetValue(MaskTargetProperty, value);
   }

   #endregion

   private LoadingMask? _loadingMask;
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;

   public LoadingMaskHost()
   {
      _customStyle = this;
   }

   void IControlCustomStyle.SetupUi()
   {
      if (MaskTarget is not null) {
         LogicalChildren.Add(MaskTarget);
         VisualChildren.Add(MaskTarget);
      }
   }
   
   public void ShowLoading()
   {
      if (_loadingMask is not null && _loadingMask.IsLoading) {
         return;
      }
      if (_loadingMask is null && MaskTarget is not null) {
         _loadingMask = new LoadingMask();
         _loadingMask.Attach(this);
         BindUtils.RelayBind(this, SizeTypeProperty, _loadingMask, SizeTypeProperty);
         BindUtils.RelayBind(this, LoadingMsgProperty, _loadingMask, LoadingMsgProperty);
         BindUtils.RelayBind(this, IsShowLoadingMsgProperty, _loadingMask, IsShowLoadingMsgProperty);
         BindUtils.RelayBind(this, CustomIndicatorIconProperty, _loadingMask, CustomIndicatorIconProperty);
         BindUtils.RelayBind(this, MotionDurationProperty, _loadingMask, MotionDurationProperty);
         BindUtils.RelayBind(this, MotionEasingCurveProperty, _loadingMask, MotionEasingCurveProperty);
      }
      _loadingMask!.Show();
   }

   public void HideLoading()
   {
      if (_loadingMask is null || 
          (_loadingMask is not null && !_loadingMask.IsLoading)) {
         return;
      }
      _loadingMask?.Hide();
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (IsLoading && MaskTarget is not null) {
         ShowLoading();
      }
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      HideLoading();
      _loadingMask?.Dispose();
      _loadingMask = null;
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (_initialized && VisualRoot is not null) {
         if (IsLoadingProperty == change.Property) {
            if (IsLoading) {
               ShowLoading();
            } else {
               HideLoading();
            }
         }
      }
   }
}
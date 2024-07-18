using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Media;
using AtomUI.Platform.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class PopupShadowLayer : LiteWindow, IShadowDecorator
{
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<PopupShadowLayer>();

   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }
   
   private static readonly FieldInfo ManagedPopupPositionerPopupInfo;

   static PopupShadowLayer()
   {
      ManagedPopupPositionerPopupInfo = typeof(ManagedPopupPositioner).GetField("_popup",
         BindingFlags.Instance | BindingFlags.NonPublic)!;
   }

   private Popup? _target;
   private Canvas? _layout;
   private ShadowRenderer? _shadowRenderer;
   private CompositeDisposable? _compositeDisposable;
   private IManagedPopupPositionerPopup? _managedPopupPositionerPopup;
   private TopLevel? _topLevel;
   private bool _isOpened = false;

   public PopupShadowLayer(TopLevel topLevel)
      : base(topLevel, topLevel.PlatformImpl?.CreatePopup()!)
   {
      _topLevel = topLevel;
      Background = new SolidColorBrush(Colors.Transparent);
      if (this is WindowBase window) {
         window.SetTransparentForMouseEvents(true);
      }

      if (PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner) {
         _managedPopupPositionerPopup =
            ManagedPopupPositionerPopupInfo.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
      }
   }

   public void AttachToTarget(Popup popup)
   {
      _target = popup;
      ConfigureShadowPopup();
   }

   public void DetachedFromTarget(Popup popup) { }

   private void ConfigureShadowPopup()
   {
      if (_target is not null) {
         _target.Opened += HandleTargetOpened;
         _target.Closed += HandleTargetClosed;
      }

      if (_shadowRenderer is null) {
         _shadowRenderer ??= new ShadowRenderer();
         _layout = new Canvas();
         _layout.Children.Add(_shadowRenderer);
         SetChild(_layout);
      }
   }

   private void HandleTargetOpened(object? sender, EventArgs args)
   {
      SetupShadowRenderer();
      Open();
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var content = _target?.Child;
      Size size = default;
      if (content is not null) {
         size = content.DesiredSize;
      }
      return CalculateShadowRendererSize(size);
   }

   private void Open()
   {
      if (_isOpened) {
         return;
      }
      
      _compositeDisposable = new CompositeDisposable();

      var popupRoot = _target?.Host as PopupRoot;
      if (popupRoot is not null) {
         popupRoot.PositionChanged += TargetPopupPositionChanged;
      }

      _compositeDisposable.Add(Disposable.Create(this, state =>
      {
         state.SetChild(null);
         Hide();
         ((ISetLogicalParent)state).SetParent(null);
         Dispose();
      }));
      ((ISetLogicalParent)this).SetParent(_target);
      SetupPositionAndSize();
      _isOpened = true;
      Show();
      if (popupRoot is not null) {
         popupRoot.PlatformImpl!.SetTopmost(true);
      }
   }

   private void TargetPopupPositionChanged(object? sender, PixelPointEventArgs e)
   {
      SetupPositionAndSize();
   }

   private void HandleTargetClosed(object? sender, EventArgs args)
   {
      if (_target is not null) {
         _target.Opened -= HandleTargetOpened;
         _target.Closed -= HandleTargetClosed;
      }

      _compositeDisposable?.Dispose();
      _compositeDisposable = null;
   }

   private void SetupShadowRenderer()
   {
      if (_target?.Child is not null && _shadowRenderer is not null) {
         // 理论上现在已经有大小了
         var content = _target?.Child!;
         _shadowRenderer.Shadows = MaskShadows;
   
         CornerRadius cornerRadius = default;
         if (content is IShadowMaskInfoProvider shadowMaskInfoProvider) {
            cornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
            var maskBounds = shadowMaskInfoProvider.GetMaskBounds();
            var rendererSize = CalculateShadowRendererSize(maskBounds.Size);
            Canvas.SetLeft(_shadowRenderer, maskBounds.Left);
            Canvas.SetTop(_shadowRenderer, maskBounds.Top);
            _shadowRenderer.Width = rendererSize.Width;
            _shadowRenderer.Height = rendererSize.Height;
         } else if (content is Border bordered) {
            cornerRadius = bordered.CornerRadius;
            var rendererSize = CalculateShadowRendererSize(content.DesiredSize);
            _shadowRenderer.Width = rendererSize.Width;
            _shadowRenderer.Height = rendererSize.Height;
         } else if (content is TemplatedControl templatedControl) {
            cornerRadius = templatedControl.CornerRadius;
            var rendererSize = CalculateShadowRendererSize(templatedControl.DesiredSize);
            _shadowRenderer.Width = rendererSize.Width;
            _shadowRenderer.Height = rendererSize.Height;
         }
         _shadowRenderer.MaskCornerRadius = cornerRadius;
      }
   }

   private Size CalculateShadowRendererSize(Size content)
   {
      var shadowThickness = MaskShadows.Thickness();
      var targetWidth = shadowThickness.Left + shadowThickness.Right;
      var targetHeight = shadowThickness.Top + shadowThickness.Bottom;
      targetWidth += content.Width;
      targetHeight += content.Height;
      return new Size(targetWidth, targetHeight);
   }

   private void SetupPositionAndSize()
   {
      if (_target?.Host is PopupRoot popupRoot) {
         var impl = popupRoot.PlatformImpl!;
         var targetPosition = impl.Position;
         double offsetX = targetPosition.X;
         double offsetY = targetPosition.Y;
         double scaling = _managedPopupPositionerPopup!.Scaling;
         var shadowThickness = MaskShadows.Thickness();
         offsetX -= shadowThickness.Left * scaling;
         offsetY -= shadowThickness.Top * scaling;

         offsetX = Math.Round(offsetX);
         offsetY = Math.Floor(offsetY + 0.5);
         
         _managedPopupPositionerPopup?.MoveAndResize(new Point(offsetX, offsetY),
                                                     new Size(_shadowRenderer!.Width, _shadowRenderer.Height));
      }
   }
}
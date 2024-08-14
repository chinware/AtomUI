using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

/// <summary>
/// 如果 checked icon 和 unchecked icon 都设置的时候，就直接切换
/// 否则就直接改变 render transform
/// </summary>
internal class NodeSwitcherButton : ToggleIconButton
{
   #region 公共属性

   public static readonly StyledProperty<PathIcon?> LoadingIconProperty
      = AvaloniaProperty.Register<NodeSwitcherButton, PathIcon?>(nameof(LoadingIcon));

   public static readonly StyledProperty<PathIcon?> LeafIconProperty
      = AvaloniaProperty.Register<NodeSwitcherButton, PathIcon?>(nameof(LeafIcon));

   public static readonly StyledProperty<bool> IsLeafProperty
      = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsLeaf));

   public PathIcon? LoadingIcon
   {
      get => GetValue(LoadingIconProperty);
      set => SetValue(LoadingIconProperty, value);
   }

   public PathIcon? LeafIcon
   {
      get => GetValue(LeafIconProperty);
      set => SetValue(LeafIconProperty, value);
   }

   public bool IsLeaf
   {
      get => GetValue(IsLeafProperty);
      set => SetValue(IsLeafProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly StyledProperty<bool> IsIconVisibleProperty
      = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsIconVisible), true);

   internal bool IsIconVisible
   {
      get => GetValue(IsIconVisibleProperty);
      set => SetValue(IsIconVisibleProperty, value);
   }
   
   #endregion
   
   private readonly BorderRenderHelper _borderRenderHelper;
   
   static NodeSwitcherButton()
   {
      AffectsMeasure<NodeSwitcherButton>(LoadingIconProperty, LeafIconProperty);
      AffectsRender<NodeSwitcherButton>(BackgroundProperty);
   }

   public NodeSwitcherButton()
   {
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      if (LoadingIcon is null) {
         LoadingIcon = new PathIcon()
         {
            Kind = "LoadingOutlined"
         };
      }

      ConfigureFixedSizeIcon(LoadingIcon);

      LoadingIcon.LoadingAnimation = IconAnimation.Spin;

      if (LeafIcon is null) {
         LeafIcon = new PathIcon()
         {
            Kind = "FileOutlined"
         };
      }

      ConfigureFixedSizeIcon(LeafIcon);
      base.OnApplyTemplate(e);
      ApplyIconToContent();
   }

   private void ConfigureFixedSizeIcon(PathIcon icon)
   {
      icon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      icon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      UIStructureUtils.SetTemplateParent(icon, this);
      TokenResourceBinder.CreateGlobalResourceBinding(icon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalResourceBinding(icon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      BindUtils.RelayBind(this, IsIconVisibleProperty, icon, PathIcon.IsVisibleProperty);
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IsLeafProperty) {
            ApplyIconToContent();
         }
      }
      if (change.Property == LoadingIconProperty ||
          change.Property == LeafIconProperty) {
         if (change.NewValue is PathIcon newIcon) {
            ConfigureFixedSizeIcon(newIcon);
            ApplyIconToContent();
         }
      }
   }

   internal override void ApplyIconToContent()
   {
      if (!IsLeaf) {
         if (IsChecked.HasValue) {
            if (CheckedIcon is not null && UnCheckedIcon is not null) {
               // 直接切换模式
               if (IsChecked.Value) {
                  Content = CheckedIcon;
               } else {
                  Content = UnCheckedIcon;
               }
            } else if (UnCheckedIcon is not null) {
               // 通过 render transform 进行设置
               Content = UnCheckedIcon;
               if (IsChecked.Value) {
                  RenderTransform = new RotateTransform(90);
               } else {
                  RenderTransform = null;
               }
            }
         } else {
            Content = LoadingIcon;
         }
      } else {
         RenderTransform = null;
         Content = LeafIcon;
      }
   }

   public override void Render(DrawingContext context)
   {
      if (IsIconVisible && !IsLeaf) {
         _borderRenderHelper.Render(context,
                                    Bounds.Size,
                                    new Thickness(),
                                    CornerRadius,
                                    BackgroundSizing.InnerBorderEdge,
                                    Background,
                                    null,
                                    default);
      }

   }
}
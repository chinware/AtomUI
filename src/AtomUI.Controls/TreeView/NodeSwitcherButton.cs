using AtomUI.Controls.Utils;
using AtomUI.Icon;
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
      = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(LoadingIcon));
   
   public static readonly StyledProperty<PathIcon?> LeafIconProperty
      = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(LeafIcon));
   
   public static readonly StyledProperty<bool> IsLeafProperty
      = AvaloniaProperty.Register<ToggleIconButton, bool>(nameof(IsLeaf));

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

   static NodeSwitcherButton()
   {
      AffectsMeasure<NodeSwitcherButton>(LoadingIconProperty, IsLeafProperty);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      if (LoadingIcon is null) {
         LoadingIcon = new PathIcon()
         {
            Kind = "LoadingOutlined"
         };
      }
      
      LoadingIcon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      LoadingIcon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      LoadingIcon.LoadingAnimation = IconAnimation.Spin;
      UIStructureUtils.SetTemplateParent(LoadingIcon, this);
      
      if (LeafIcon is null) {
         LeafIcon = new PathIcon()
         {
            Kind = "FileOutlined"
         };
      }
      
      LeafIcon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      LeafIcon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      LeafIcon.LoadingAnimation = IconAnimation.Spin;
      UIStructureUtils.SetTemplateParent(LeafIcon, this);
      
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IsLeafProperty) {
            SetupStatusIcon();
         }
      }
   }

   internal override void SetupStatusIcon()
   {
      if (Presenter is not null) {
         if (!IsLeaf) {
            if (IsChecked.HasValue) {
               if (CheckedIcon is not null && UnCheckedIcon is not null) {
                  // 直接切换模式
                  if (IsChecked.Value) {
                     Presenter.Content = CheckedIcon;
                  } else {
                     Presenter.Content = UnCheckedIcon;
                  }
               } else if (CheckedIcon is not null) {
                  // 通过 render transform 进行设置
                  Presenter.Content = CheckedIcon;
                  if (IsChecked.Value) {
                     RenderTransform = new RotateTransform(90);
                  } else {
                     RenderTransform = null;
                  }
               }
            } else {
               Presenter.Content = LoadingIcon;
            }
         } else {
            RenderTransform = null;
            Presenter.Content = LoadingIcon;
         }
      }
   }

   public static NodeSwitcherButton BuildShowLineLeafSwitcherButton()
   {
      return new NodeSwitcherButton()
      {
         IsLeaf = true,
         IsChecked = null,
         LeafIcon = new PathIcon()
         {
            Kind = "FileOutlined"
         }
      };
   }

   public static NodeSwitcherButton BuildShowLineNoLeafSwitcherButton()
   {
      return new NodeSwitcherButton()
      {
         IsLeaf = false,
         CheckedIcon = new PathIcon()
         {
            Kind = "MinusSquareOutlined"
         },
         UnCheckedIcon = new PathIcon()
         {
            Kind = "PlusSquareOutlined"
         }
      };
   }

   public static NodeSwitcherButton BuildDirectoryNoLeafSwitcherButton()
   {
      return new NodeSwitcherButton()
      {
         IsLeaf = false,
         CheckedIcon = new PathIcon()
         {
            Kind = "FolderOpenOutlined"
         },
         UnCheckedIcon = new PathIcon()
         {
            Kind = "FolderOutlined"
         }
      };
   }
}
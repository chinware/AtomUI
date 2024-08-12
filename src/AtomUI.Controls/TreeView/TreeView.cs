using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace AtomUI.Controls;

using AvaloniaTreeView = Avalonia.Controls.TreeView;

[PseudoClasses(DraggablePC)]
public class TreeView : AvaloniaTreeView
{
   public const string DraggablePC = ":draggable";

   #region 公共属性定义

   public static readonly StyledProperty<bool> IsDraggableProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsDraggable));
   
   public static readonly StyledProperty<bool> IsCheckableProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsCheckable));
   
   public static readonly StyledProperty<bool> IsShowIconProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowIcon));
   
   public static readonly StyledProperty<bool> IsShowLineProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowLine));

   public bool IsDraggable
   {
      get => GetValue(IsDraggableProperty);
      set => SetValue(IsDraggableProperty, value);
   }
   
   public bool IsCheckable
   {
      get => GetValue(IsCheckableProperty);
      set => SetValue(IsCheckableProperty, value);
   }
   
   public bool IsShowIcon
   {
      get => GetValue(IsShowIconProperty);
      set => SetValue(IsShowIconProperty, value);
   }
   
   public bool IsShowLine
   {
      get => GetValue(IsShowLineProperty);
      set => SetValue(IsShowLineProperty, value);
   }

   #endregion

   public TreeView()
   {
      UpdatePseudoClasses();
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(DraggablePC, IsDraggable);
   }
}
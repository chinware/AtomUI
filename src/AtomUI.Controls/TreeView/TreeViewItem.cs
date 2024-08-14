using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTreeItem = Avalonia.Controls.TreeViewItem;

public class TreeViewItem : AvaloniaTreeItem
{
   public const string TreeNodeHoverPC = ":treenode-hover";
   
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsCheckableProperty =
      AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsCheckable), true);

   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<TreeViewItem, PathIcon?>(nameof(Icon));

   public static readonly StyledProperty<bool?> IsCheckedProperty
      = AvaloniaProperty.Register<TreeViewItem, bool?>(nameof(IsChecked), false);
   
   public static readonly StyledProperty<PathIcon?> SwitcherExpandIconProperty
      = AvaloniaProperty.Register<TreeViewItem, PathIcon?>(nameof(SwitcherExpandIcon));
   
   public static readonly StyledProperty<PathIcon?> SwitcherCollapseIconProperty
      = AvaloniaProperty.Register<TreeViewItem, PathIcon?>(nameof(SwitcherCollapseIcon));
   
   public static readonly DirectProperty<TreeViewItem, bool> IsLeafProperty
      = AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsLeaf),
                                                            o => o.IsLeaf,
                                                            (o, v) => o.IsLeaf = v);
   public bool IsCheckable
   {
      get => GetValue(IsCheckableProperty);
      set => SetValue(IsCheckableProperty, value);
   }

   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   public PathIcon? SwitcherExpandIcon
   {
      get => GetValue(SwitcherExpandIconProperty);
      set => SetValue(SwitcherExpandIconProperty, value);
   }
   
   public PathIcon? SwitcherCollapseIcon
   {
      get => GetValue(SwitcherCollapseIconProperty);
      set => SetValue(SwitcherCollapseIconProperty, value);
   }

   public bool? IsChecked
   {
      get => GetValue(IsCheckedProperty);
      set => SetValue(IsCheckedProperty, value);
   }

   private bool _isLeaf;
   public bool IsLeaf
   {
      get => _isLeaf;
      internal set => SetAndRaise(IsLeafProperty, ref _isLeaf, value);
   }

   public TreeNodeKey? Key { get; set; }
   
   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<TreeViewItem, double> TitleHeightProperty
      = AvaloniaProperty.RegisterDirect<TreeViewItem, double>(nameof(TitleHeight),
                                                              o => o.TitleHeight,
                                                              (o, v) => o.TitleHeight = v);

   internal static readonly DirectProperty<TreeViewItem, TreeItemHoverMode> NodeHoverModeProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, TreeItemHoverMode>(nameof(NodeHoverMode),
                                                                       o => o.NodeHoverMode,
                                                                       (o, v) => o.NodeHoverMode = v);
   
   internal static readonly DirectProperty<TreeViewItem, bool> IsShowLineProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsShowLine),
                                                                       o => o.IsShowLine,
                                                                       (o, v) => o.IsShowLine = v);
   
   internal static readonly DirectProperty<TreeViewItem, bool> IsShowLeafSwitcherProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsShowLeafSwitcher),
                                                          o => o.IsShowLeafSwitcher,
                                                          (o, v) => o.IsShowLeafSwitcher = v);
   
   internal static readonly DirectProperty<TreeViewItem, bool> IsCheckboxVisibleProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsCheckboxVisible),
                                                          o => o.IsCheckboxVisible,
                                                          (o, v) => o.IsCheckboxVisible = v);
   
   internal static readonly DirectProperty<TreeViewItem, bool> IsCheckboxEnableProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsCheckboxEnable),
                                                          o => o.IsCheckboxEnable,
                                                          (o, v) => o.IsCheckboxEnable = v);

   internal static readonly StyledProperty<IBrush?> EffectiveNodeBgProperty
      = AvaloniaProperty.Register<TreeViewItem, IBrush?>(nameof(EffectiveNodeBg));

   internal static readonly StyledProperty<CornerRadius> EffectiveNodeCornerRadiusProperty
      = AvaloniaProperty.Register<TreeViewItem, CornerRadius>(nameof(EffectiveNodeCornerRadius));
   
   private double _titleHeight;

   internal double TitleHeight
   {
      get => _titleHeight;
      set => SetAndRaise(TitleHeightProperty, ref _titleHeight, value);
   }

   private TreeItemHoverMode _nodeHoverMode;

   internal TreeItemHoverMode NodeHoverMode
   {
      get => _nodeHoverMode;
      set => SetAndRaise(NodeHoverModeProperty, ref _nodeHoverMode, value);
   }

   private bool _isShowLine;

   internal bool IsShowLine
   {
      get => _isShowLine;
      set => SetAndRaise(IsShowLineProperty, ref _isShowLine, value);
   }

   private bool _isShowLeafSwitcher;
   internal bool IsShowLeafSwitcher
   {
      get => _isShowLeafSwitcher;
      set => SetAndRaise(IsShowLeafSwitcherProperty, ref _isShowLeafSwitcher, value);
   }

   private bool _isCheckboxVisible;
   internal bool IsCheckboxVisible
   {
      get => _isCheckboxVisible;
      set => SetAndRaise(IsCheckboxVisibleProperty, ref _isCheckboxVisible, value);
   }
   
   private bool _isCheckboxEnable;
   internal bool IsCheckboxEnable
   {
      get => _isCheckboxEnable;
      set => SetAndRaise(IsCheckboxEnableProperty, ref _isCheckboxEnable, value);
   }

   internal IBrush? EffectiveNodeBg
   {
      get => GetValue(EffectiveNodeBgProperty);
      set => SetValue(EffectiveNodeBgProperty, value);
   }

   internal CornerRadius EffectiveNodeCornerRadius
   {
      get => GetValue(EffectiveNodeCornerRadiusProperty);
      set => SetValue(EffectiveNodeCornerRadiusProperty, value);
   }
   
   internal TreeView? OwnerTreeView { get; set; }

   #endregion

   private bool _initialized;
   private ContentPresenter? _headerPresenter;
   private Border? _frameDecorator;
   private NodeSwitcherButton? _switcherButton;
   private Rect _effectiveBgRect;
   private readonly BorderRenderHelper _borderRenderHelper;

   static TreeViewItem()
   {
      AffectsRender<TreeViewItem>(EffectiveNodeCornerRadiusProperty,
                                  EffectiveNodeBgProperty);
   }

   public TreeViewItem()
   {
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         TokenResourceBinder.CreateTokenBinding(this, TitleHeightProperty, TreeViewResourceKey.TitleHeight);
         if (IsChecked.HasValue && IsChecked.Value) {
            OwnerTreeView = this.FindLogicalAncestorOfType<TreeView>();
            // 注册到 TreeView
            OwnerTreeView?.DefaultCheckedItems.Add(this);
         }
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == NodeHoverModeProperty) {
            CalculateEffectiveBgRect();
         } else if (change.Property == IsShowLineProperty ||
                    change.Property == SwitcherExpandIconProperty ||
                    change.Property == SwitcherCollapseIconProperty) {
            SetNodeSwitcherIcons();
         } else if (change.Property == IsEnabledProperty ||
                    change.Property == IsCheckableProperty) {
            SetupCheckBoxEnabled();
         } else if (change.Property == IsCheckedProperty) {
            // 我们处理某个节点的点击只有 true 或者 false
            if (IsChecked.HasValue) {
               if (IsChecked.Value) {
                  OwnerTreeView?.CheckedSubTree(this);
               } else {
                  OwnerTreeView?.UnCheckedSubTree(this);
               }
            } 
         }
      }

      if (change.Property == ItemCountProperty) {
         IsLeaf = ItemCount == 0;
      }
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var size = base.ArrangeOverride(finalSize);
      CalculateEffectiveBgRect();
      return size;
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _headerPresenter = e.NameScope.Find<ContentPresenter>(TreeViewItemTheme.HeaderPresenterPart);
      _frameDecorator = e.NameScope.Find<Border>(TreeViewItemTheme.FrameDecoratorPart);
      _switcherButton = e.NameScope.Find<NodeSwitcherButton>(TreeViewItemTheme.NodeSwitcherButtonPart);
      
      if (_frameDecorator is not null) {
         _frameDecorator.PointerEntered += HandleFrameDecoratorEntered;
         _frameDecorator.PointerExited += HandleFrameDecoratorExited;
      }
      if (_headerPresenter is not null) {
         _headerPresenter.PointerEntered += HandleHeaderPresenterEntered;
         _headerPresenter.PointerExited += HandleHeaderPresenterExited;
      }
      IsLeaf = ItemCount == 0;
      SetNodeSwitcherIcons();
      SetupCheckBoxEnabled();
   }

   private void CalculateEffectiveBgRect()
   {
      if (_frameDecorator is null) {
         return;
      }

      Point offset = default;
      var targetWidth = 0d;
      var targetHeight = 0d;
      if (NodeHoverMode == TreeItemHoverMode.Default) {
         if (_headerPresenter is not null) {
            offset = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
            targetWidth = _headerPresenter.Bounds.Width;
            targetHeight = _frameDecorator.Bounds.Height;
         }
      }

      _effectiveBgRect = new Rect(offset, new Size(targetWidth, targetHeight));
   }

   public override void Render(DrawingContext context)
   {
      using var state = context.PushTransform(Matrix.CreateTranslation(_effectiveBgRect.X, 0));
      _borderRenderHelper.Render(context,
                                 _effectiveBgRect.Size,
                                 new Thickness(),
                                 EffectiveNodeCornerRadius,
                                 BackgroundSizing.InnerBorderEdge,
                                 EffectiveNodeBg,
                                 null,
                                 default);
   }

   private void SetNodeSwitcherIcons()
   {
      if (_switcherButton is null) {
         return;
      }
      if (SwitcherExpandIcon is not null || SwitcherCollapseIcon is not null) {
         _switcherButton.UnCheckedIcon = SwitcherExpandIcon;
         _switcherButton.CheckedIcon = SwitcherCollapseIcon;
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty, GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty, GlobalResourceKey.IconSize);
         return;
      }

      if (IsShowLine) {
         _switcherButton.UnCheckedIcon = new PathIcon()
         {
            Kind = "PlusSquareOutlined"
         };
         _switcherButton.CheckedIcon = new PathIcon()
         {
            Kind = "MinusSquareOutlined"
         };
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty, GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty, GlobalResourceKey.IconSize);
      } else {
         _switcherButton.UnCheckedIcon = new PathIcon()
         {
            Kind = "CaretRightOutlined"
         };
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty, GlobalResourceKey.IconSizeXS);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty, GlobalResourceKey.IconSizeXS);
         _switcherButton.CheckedIcon = null;
      }
   }

   private void SetupCheckBoxEnabled()
   {
      if (!IsEnabled) {
         IsCheckboxEnable = false;
      } else {
         IsCheckboxEnable = IsCheckable;
      }
   }
   
   private void HandleFrameDecoratorEntered(object? sender, PointerEventArgs? args)
   {
      if (NodeHoverMode != TreeItemHoverMode.WholeLine) {
         return;
      }
      PseudoClasses.Set(TreeNodeHoverPC, true);
   }
   
   private void HandleFrameDecoratorExited(object? sender, PointerEventArgs args)
   {
      if (NodeHoverMode != TreeItemHoverMode.WholeLine) {
         return;
      }
      PseudoClasses.Set(TreeNodeHoverPC, false);
   }
   
   private void HandleHeaderPresenterEntered(object? sender, PointerEventArgs? args)
   {
      if (NodeHoverMode == TreeItemHoverMode.WholeLine) {
         return;
      }
      PseudoClasses.Set(TreeNodeHoverPC, true);
   }
   
   private void HandleHeaderPresenterExited(object? sender, PointerEventArgs args)
   {
      if (NodeHoverMode == TreeItemHoverMode.WholeLine) {
         return;
      }
      PseudoClasses.Set(TreeNodeHoverPC, false);
   }
   
}
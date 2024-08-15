using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

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

   internal static readonly DirectProperty<TreeViewItem, bool> IsShowIconProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsShowIcon),
                                                          o => o.IsShowIcon,
                                                          (o, v) => o.IsShowIcon = v);

   internal static readonly DirectProperty<TreeViewItem, bool> IconEffectiveVisibleProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IconEffectiveVisible),
                                                          o => o.IconEffectiveVisible,
                                                          (o, v) => o.IconEffectiveVisible = v);

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

   internal static readonly DirectProperty<TreeViewItem, bool> IsDraggableProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDraggable),
                                                          o => o.IsDraggable,
                                                          (o, v) => o.IsDraggable = v);

   internal static readonly DirectProperty<TreeViewItem, bool> IsDraggingProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragging),
                                                          o => o.IsDragging,
                                                          (o, v) => o.IsDragging = v);
   
   internal static readonly DirectProperty<TreeViewItem, bool> IsDragOverProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragOver),
                                                          o => o.IsDragOver,
                                                          (o, v) => o.IsDragOver = v);
   
   internal static readonly DirectProperty<TreeViewItem, Point> DragOverPositionProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, Point>(nameof(DragOverPosition),
                                                           o => o.DragOverPosition,
                                                           (o, v) => o.DragOverPosition = v);

   internal static readonly DirectProperty<TreeViewItem, Thickness> DragFrameBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, Thickness>(nameof(DragFrameBorderThickness),
                                                               o => o.DragFrameBorderThickness,
                                                               (o, v) => o.DragFrameBorderThickness = v);
   
   internal static readonly DirectProperty<TreeViewItem, double> DragIndicatorLineWidthProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, double>(nameof(DragIndicatorLineWidth),
                                                            o => o.DragIndicatorLineWidth,
                                                            (o, v) => o.DragIndicatorLineWidth = v);
   
   internal static readonly DirectProperty<TreeViewItem, IBrush?> DragIndicatorBrushProperty =
      AvaloniaProperty.RegisterDirect<TreeViewItem, IBrush?>(nameof(DragIndicatorBrush),
                                                             o => o.DragIndicatorBrush,
                                                             (o, v) => o.DragIndicatorBrush = v);

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

   private bool _isShowIcon = true;

   internal bool IsShowIcon
   {
      get => _isShowIcon;
      set => SetAndRaise(IsShowIconProperty, ref _isShowIcon, value);
   }

   private bool _iconEffectiveVisible = true;

   internal bool IconEffectiveVisible
   {
      get => _iconEffectiveVisible;
      set => SetAndRaise(IconEffectiveVisibleProperty, ref _iconEffectiveVisible, value);
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

   private bool _isDraggable;

   internal bool IsDraggable
   {
      get => _isDraggable;
      set => SetAndRaise(IsDraggableProperty, ref _isDraggable, value);
   }

   private bool _isDragging;

   internal bool IsDragging
   {
      get => _isDragging;
      set => SetAndRaise(IsDraggingProperty, ref _isDragging, value);
   }
   
   private bool _isDragOver;

   internal bool IsDragOver
   {
      get => _isDragOver;
      set => SetAndRaise(IsDragOverProperty, ref _isDragOver, value);
   }
   
   private Point _dragOverPosition;

   internal Point DragOverPosition
   {
      get => _dragOverPosition;
      set => SetAndRaise(DragOverPositionProperty, ref _dragOverPosition, value);
   }
   
   private Thickness _dragFrameBorderThickness;

   internal Thickness DragFrameBorderThickness
   {
      get => _dragFrameBorderThickness;
      set => SetAndRaise(DragFrameBorderThicknessProperty, ref _dragFrameBorderThickness, value);
   }
   
   private double _dragIndicatorLineWidth;

   internal double DragIndicatorLineWidth
   {
      get => _dragIndicatorLineWidth;
      set => SetAndRaise(DragIndicatorLineWidthProperty, ref _dragIndicatorLineWidth, value);
   }
   
   private IBrush? _dragIndicatorBrush;

   internal IBrush? DragIndicatorBrush
   {
      get => _dragIndicatorBrush;
      set => SetAndRaise(DragIndicatorBrushProperty, ref _dragIndicatorBrush, value);
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
   private ContentPresenter? _iconPresenter;
   private NodeSwitcherButton? _switcherButton;
   private Rect _effectiveBgRect;
   private readonly BorderRenderHelper _borderRenderHelper;
   private Point? _lastPoint;
   private DragPreviewAdorner? _dragPreview;
   private TreeViewItem? _currentDragOver; // 这个不是目标节点，有可能是在父节点上拖动
   private TreeViewItem? _dropTargetNode; // 目标释放节点

   static TreeViewItem()
   {
      AffectsRender<TreeViewItem>(EffectiveNodeCornerRadiusProperty,
                                  EffectiveNodeBgProperty,
                                  IsShowLineProperty,
                                  IsShowLeafSwitcherProperty,
                                  IsDraggingProperty,
                                  IsDragOverProperty,
                                  DragOverPositionProperty);
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

      if (change.Property == IsShowIconProperty || change.Property == IconProperty) {
         IconEffectiveVisible = IsShowIcon && Icon is not null;
      }

      if (change.Property == ItemCountProperty) {
         IsLeaf = ItemCount == 0;
      } else if (change.Property == IconProperty) {
         if (change.OldValue is PathIcon oldIcon) {
            UIStructureUtils.SetTemplateParent(oldIcon, null);
         }

         if (change.NewValue is PathIcon newIcon) {
            UIStructureUtils.SetTemplateParent(newIcon, this);
         }
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
      TokenResourceBinder.CreateGlobalResourceBinding(this, DragFrameBorderThicknessProperty,
                                                      GlobalResourceKey.BorderThickness,
                                                      BindingPriority.Template,
                                                      new RenderScaleAwareThicknessConfigure(this));
      TokenResourceBinder.CreateGlobalResourceBinding(this, DragIndicatorLineWidthProperty,
                                                      TreeViewResourceKey.DragIndicatorLineWidth);
      TokenResourceBinder.CreateGlobalResourceBinding(this, DragIndicatorBrushProperty,
                                                      GlobalResourceKey.ColorPrimary);
      
      base.OnApplyTemplate(e);
      _headerPresenter = e.NameScope.Find<ContentPresenter>(TreeViewItemTheme.HeaderPresenterPart);
      _iconPresenter = e.NameScope.Find<ContentPresenter>(TreeViewItemTheme.IconPresenterPart);
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

      if (_iconPresenter is not null) {
         _iconPresenter.PointerEntered += HandleHeaderPresenterEntered;
         _iconPresenter.PointerExited += HandleHeaderPresenterExited;
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
      if (NodeHoverMode == TreeItemHoverMode.Default || NodeHoverMode == TreeItemHoverMode.Block) {
         if (_iconPresenter is not null && _iconPresenter.IsVisible) {
            offset = _iconPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
            targetWidth = _iconPresenter.Bounds.Width + _headerPresenter?.Bounds.Width ?? 0d;
            targetHeight = _frameDecorator.Bounds.Height;
         } else if (_headerPresenter is not null) {
            offset = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
            targetWidth = _headerPresenter.Bounds.Width;
            targetHeight = _frameDecorator.Bounds.Height;
         }
      }

      _effectiveBgRect = new Rect(offset, new Size(targetWidth, targetHeight));
   }

   public override void Render(DrawingContext context)
   {
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
      if (IsShowLine && (IsExpanded || IsLeaf)) {
         RenderTreeNodeLine(context);
      }
      if (IsDragOver) {
         RenderDraggingIndicator(context);
      }
   }

   private void RenderDraggingIndicator(DrawingContext context)
   {
      if (OwnerTreeView is null || _dropTargetNode is null) {
         return;
      }
      
      var localPosition = OwnerTreeView.TranslatePoint(DragOverPosition, this) ?? default;
      var dropTargetOffset = _dropTargetNode.TranslatePoint(new Point(0, 0), this) ?? default;
      var dropTargetHalfOffsetY = dropTargetOffset.Y + _dropTargetNode._frameDecorator?.DesiredSize.Height / 2 ?? default;
      var offsetY = localPosition.Y;
      Point startPoint = default;
      Point endPoint = default;
      
      var indicatorOffsetX = 0d;
      
      if (this != _dropTargetNode) {
         if (_dropTargetNode._iconPresenter is not null && _dropTargetNode._iconPresenter.IsVisible) {
            var offset = _dropTargetNode._iconPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
            indicatorOffsetX = offset.X;
         } else if (_dropTargetNode._headerPresenter is not null) {
            var offset = _dropTargetNode._headerPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
            indicatorOffsetX = offset.X;
         }

         var frameHeight = _dropTargetNode._frameDecorator?.Bounds.Height ?? default;
         var frameMargin = _dropTargetNode._frameDecorator?.Margin.Bottom ?? default;

         var indicatorOffsetY = Math.Min(dropTargetOffset.Y + frameHeight + frameMargin + DragIndicatorLineWidth / 2, Bounds.Height - DragIndicatorLineWidth / 2);
         
         if (offsetY > dropTargetHalfOffsetY) {
            startPoint = new Point(indicatorOffsetX, indicatorOffsetY);
            endPoint = new Point(indicatorOffsetX + _dropTargetNode.Bounds.Width, indicatorOffsetY);
         } else {
            startPoint = new Point(indicatorOffsetX, dropTargetOffset.Y + DragIndicatorLineWidth / 2);
            endPoint = new Point(indicatorOffsetX + _dropTargetNode.Bounds.Width, dropTargetOffset.Y + DragIndicatorLineWidth / 2);
         }
      } else {
         if (_iconPresenter is not null && _iconPresenter.IsVisible) {
            var offset = _iconPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
            indicatorOffsetX = offset.X;
         } else if (_headerPresenter is not null) {
            var offset = _headerPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
            indicatorOffsetX = offset.X;
         }

         var isFirstChild = false;

         var frameHeight = _frameDecorator?.Bounds.Height ?? default;
         
         if (Parent is TreeViewItem parentItem) {
            isFirstChild = parentItem.ContainerFromIndex(0) == this;
         }
         
         if (isFirstChild || Level == 0) {
            if (offsetY > dropTargetHalfOffsetY) {
               indicatorOffsetX += 20;
               startPoint = new Point(indicatorOffsetX, dropTargetOffset.Y + frameHeight);
               endPoint = new Point(indicatorOffsetX + Bounds.Width, dropTargetOffset.Y + frameHeight);
            } else {
               startPoint = new Point(indicatorOffsetX, dropTargetOffset.Y + DragIndicatorLineWidth / 2);
               endPoint = new Point(indicatorOffsetX + _dropTargetNode.Bounds.Width, dropTargetOffset.Y + DragIndicatorLineWidth / 2);
            }
         } else {
            indicatorOffsetX += 20;
            startPoint = new Point(indicatorOffsetX, dropTargetOffset.Y + frameHeight);
            endPoint = new Point(indicatorOffsetX + Bounds.Width, dropTargetOffset.Y + frameHeight);
         }
      }

      var pen = new Pen(DragIndicatorBrush, DragIndicatorLineWidth);
      {
         using var state = context.PushRenderOptions(new RenderOptions
         {
            EdgeMode = EdgeMode.Aliased
         });
         context.DrawLine(pen, startPoint, endPoint);
      }
   }

   private void RenderTreeNodeLine(DrawingContext context)
   {
      if (_switcherButton is null) {
         return;
      }

      var penWidth = BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0).Top;
      using var state = context.PushRenderOptions(new RenderOptions
      {
         EdgeMode = EdgeMode.Aliased
      });

      if (!IsLeaf) {
         var switcherMiddleBottom =
            _switcherButton.TranslatePoint(
               new Point(_switcherButton.DesiredSize.Width / 2, _switcherButton.DesiredSize.Height), this) ?? default;
         var blockStartPoint = new Point(switcherMiddleBottom.X, switcherMiddleBottom.Y);
         var blockEndPoint = new Point(blockStartPoint.X, DesiredSize.Height);

         context.DrawLine(new Pen(BorderBrush, penWidth), blockStartPoint, blockEndPoint);
      }

      // 画孩子线条
      if (!IsShowLeafSwitcher && IsLeaf) {
         var isLastChild = false;
         if (Parent is TreeViewItem parentTreeItem) {
            if (parentTreeItem.ContainerFromIndex(parentTreeItem.ItemCount - 1) == this) {
               isLastChild = true;
            }
         }

         {
            // 纵向
            var childStartPoint =
               _switcherButton.TranslatePoint(new Point(_switcherButton.DesiredSize.Width / 2, 0), this) ?? default;
            var childEndPoint =
               _switcherButton.TranslatePoint(
                  new Point(_switcherButton.DesiredSize.Width / 2,
                            isLastChild ? _switcherButton.DesiredSize.Height : DesiredSize.Height), this) ?? default;

            if (isLastChild) {
               childEndPoint = childEndPoint.WithY(childEndPoint.Y / 2);
            }

            context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint, childEndPoint);
         }

         {
            // 横向
            var childStartPoint =
               _switcherButton.TranslatePoint(
                  new Point(_switcherButton.DesiredSize.Width / 2, _switcherButton.DesiredSize.Height / 2), this) ??
               default;
            var childEndPoint =
               _switcherButton.TranslatePoint(
                  new Point(_switcherButton.DesiredSize.Width, _switcherButton.DesiredSize.Height / 2),
                  this) ?? default;

            context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint, childEndPoint);
         }
      }
   }

   private void SetNodeSwitcherIcons()
   {
      if (_switcherButton is null) {
         return;
      }

      if (SwitcherExpandIcon is not null || SwitcherCollapseIcon is not null) {
         _switcherButton.UnCheckedIcon = SwitcherExpandIcon;
         _switcherButton.CheckedIcon = SwitcherCollapseIcon;
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty,
                                                GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty,
                                                GlobalResourceKey.IconSize);
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
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty,
                                                GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty,
                                                GlobalResourceKey.IconSize);
      } else {
         _switcherButton.CheckedIcon = null;
         _switcherButton.UnCheckedIcon = new PathIcon()
         {
            Kind = "CaretRightOutlined"
         };
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconWidthProperty,
                                                GlobalResourceKey.IconSizeXS);
         TokenResourceBinder.CreateTokenBinding(_switcherButton, NodeSwitcherButton.IconHeightProperty,
                                                GlobalResourceKey.IconSizeXS);
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

   #region 拖动相关处理

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);
      if (IsDraggable) {
         e.Handled = true;
         _lastPoint = e.GetPosition(this);
         e.PreventGestureRecognition();
      }
   }

   protected override void OnPointerMoved(PointerEventArgs e)
   {
      if (_lastPoint.HasValue) {
         var delta = e.GetPosition(this) - _lastPoint.Value;
         var manhattanDistance = Math.Abs(delta.X) + Math.Abs(delta.Y);
         // 先写死
         if (manhattanDistance > 5) {
            if (!IsDragging) {
               HandlePrepareDrag();
               Dispatcher.UIThread.Post(() => { IsDragging = true; });
            }

            HandleDragging(e.GetPosition(OwnerTreeView), delta);
         }
      }
   }

   protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
   {
      if (_lastPoint.HasValue) {
         HandleDragCompleted(_lastPoint.Value);
         _lastPoint = null;
         IsDragging = false;
      }

      base.OnPointerCaptureLost(e);
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      if (_lastPoint.HasValue) {
         e.Handled = true;
         HandleDragCompleted(e.GetPosition(this));
         _lastPoint = null;
         IsDragging = false;
      }
   }

   private void HandlePrepareDrag()
   {
      var adornerLayer = AdornerLayer.GetAdornerLayer(this);
      if (adornerLayer == null || _frameDecorator is null) {
         return;
      }

      _dragPreview = new DragPreviewAdorner(_frameDecorator);
      AdornerLayer.SetAdornedElement(_dragPreview, TopLevel.GetTopLevel(this));
      AdornerLayer.SetIsClipEnabled(_dragPreview, false);
      adornerLayer.Children.Add(_dragPreview);
   }

   private void HandleDragCompleted(Point point)
   {
      var dropInfo = FindDropTargetPosition();
      Console.WriteLine($"{dropInfo.Item1?.Header}|{dropInfo.Item2}|{dropInfo.Item3}");
      if (dropInfo.Item1 is not null) {
         PerformDropOperation(this, dropInfo.Item1, dropInfo.Item2);
      }
      if (_dragPreview is not null) {
         AdornerLayer layer = AdornerLayer.GetAdornerLayer(this)!;
         layer.Children.Remove(_dragPreview);
         if (_currentDragOver is not null) {
            _currentDragOver.IsDragOver = false;
            _currentDragOver.DragOverPosition = default;
            _currentDragOver = null;
         }

         DragOverPosition = default;
         _dropTargetNode = null;
      }
   }

   private (TreeViewItem?, int, bool) FindDropTargetPosition()
   {
      Console.WriteLine($"{_currentDragOver?.Header}-{_dropTargetNode?.Header}");
      if (_currentDragOver is null || OwnerTreeView is null || _dropTargetNode is null) {
         return (null, -1, false);
      }

      bool isRoot = false;
      var index = 0;
      TreeViewItem? targetTreeItem = null;
      
      var localPosition = OwnerTreeView.TranslatePoint(DragOverPosition, _currentDragOver) ?? default;
      var dropTargetOffset = _dropTargetNode.TranslatePoint(new Point(0, 0), _currentDragOver) ?? default;
      var dropTargetHalfOffsetY = dropTargetOffset.Y + _dropTargetNode._frameDecorator?.DesiredSize.Height / 2 ?? default;
      var offsetY = localPosition.Y;
      
      if (_currentDragOver == _dropTargetNode) {
   
         // 有可能是自己，也可能是自己前面，因为第一个元素我们区别对待
         var isFirstChild = false;
         
         if (isFirstChild || Level == 0) {
            if (offsetY > dropTargetHalfOffsetY) {
               index = _dropTargetNode.ItemCount;
               targetTreeItem = _dropTargetNode;
            } else {
               if (_dropTargetNode.Parent is TreeViewItem parentItem) {
                  index = parentItem.IndexFromContainer(_dropTargetNode);
                  targetTreeItem = parentItem;
               } else {
                  isRoot = true;
                  index = OwnerTreeView.IndexFromContainer(_dropTargetNode);
               }
            }
         } else {
            index = _dropTargetNode.ItemCount;
         }
      } else {
         var dropTargetNodeIndex = _currentDragOver.IndexFromContainer(_dropTargetNode);
         targetTreeItem = _dropTargetNode;
         if (offsetY > dropTargetHalfOffsetY) {
            index = dropTargetNodeIndex;
         } else {
            index = dropTargetNodeIndex - 1;
         }
      }
      return (targetTreeItem, index, isRoot);
   }

   private void PerformDropOperation(TreeViewItem sourceItem, TreeViewItem targetItem, int index)
   {
      
   }

   private void HandleDragging(Point treePosition, Point delta)
   {
      if (_dragPreview is not null && OwnerTreeView is not null) {
         var basePosition = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!) ?? default;
         _dragPreview.OffsetX = basePosition.X + delta.X;
         _dragPreview.OffsetY = basePosition.Y + delta.Y;
         SetupDragOver(treePosition);
         if (_currentDragOver is not null) {
            _currentDragOver._dropTargetNode = OwnerTreeView?.GetNodeByOffsetY(treePosition);
         }
      }
   }

   protected void SetupDragOver(Point treePosition)
   {
      if (OwnerTreeView is null) {
         return;
      }
      var treeViewItem = OwnerTreeView.GetNodeByPosition(treePosition);
      if (_currentDragOver is not null) {
         if (_currentDragOver != treeViewItem) {
            _currentDragOver.IsDragOver = false;
            _currentDragOver.DragOverPosition = default;
         } else {
            _currentDragOver.IsDragOver = true;
            _currentDragOver.DragOverPosition = treePosition;
            return;
         }
      }

      if (treeViewItem is not null) {
         _currentDragOver = treeViewItem;
         _currentDragOver.IsDragOver = true;
         _currentDragOver.DragOverPosition = treePosition;
      } else {
         _currentDragOver = null;
      }
   }

   internal bool IsDragOverForPoint(Point treePosition)
   {
      if (OwnerTreeView is null) {
         return false;
      }

      var offsetX = 0d;
      var offsetY = 0d;

      if (_iconPresenter is not null && _iconPresenter.IsVisible) {
         var offset = _iconPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
         offsetX = offset.X;
         offsetY = offset.Y;
      } else if (_headerPresenter is not null) {
         var offset = _headerPresenter.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
         offsetX = offset.X;
         offsetY = offset.Y;
      }

      if (_switcherButton is not null && _switcherButton.IsIconVisible) {
         offsetX -= _switcherButton.Bounds.Width;
      }

      var treeViewBound = new Rect(new Point(offsetX, offsetY), Bounds.Size);
      return treeViewBound.Contains(treePosition);
   }

   internal bool IsDragOverForOffsetY(Point treePosition)
   {
      if (OwnerTreeView is null) {
         return false;
      }

      var offsetX = 0d;
      var offsetY = 0d;

      var offset = this.TranslatePoint(new Point(0, 0), OwnerTreeView) ?? default;
      offsetX = offset.X;
      offsetY = offset.Y;

      var treeViewBound = new Rect(new Point(offsetX, offsetY), Bounds.Size);
      return treeViewBound.Contains(treePosition);
   }

   #endregion
}
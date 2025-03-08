using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NodeSwitcherButtonTheme : BaseControlTheme
{
    public const string RootPart = "PART_Root";
    public const string RotationIconPresenterPart = "PART_RotationIconPresenter";
    public const string ExpandIconPresenterPart = "PART_ExpandIconPresenter";
    public const string CollapseIconPresenterPart = "PART_CollapseIconPresenter";
    public const string LoadingIconPresenterPart = "PART_LoadingIconPresenter";
    public const string LeafIconPresenterPart = "PART_LeafIconPresenter";
    
    public NodeSwitcherButtonTheme()
        : base(typeof(NodeSwitcherButton))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NodeSwitcherButton>((button, scope) =>
        {
            var rootContainer = new Panel
            {
                Name = RootPart,
                IsHitTestVisible = false
            };

            rootContainer.RegisterInNameScope(scope);
            
            // 我们需要在这个切换按钮显示几种状态的图标
            // 1、展开和收缩两种状态的图标
            // 2、只设置一个图标，展开和收缩通过旋转开表示
            // 3、当节点是叶子节点的时候，没有展开和收缩的概念，直接显示叶子节点的图标
            // 4、加载状态的图标
            var expandIconPresenter = new ContentPresenter
            {
                Name = ExpandIconPresenterPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            expandIconPresenter.RegisterInNameScope(scope);

            CreateTemplateParentBinding(expandIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.ExpandIconProperty);

            var collapseIconPresenter = new ContentPresenter
            {
                Name = CollapseIconPresenterPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            collapseIconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(collapseIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.CollapseIconProperty);
            
            var rotationIconPresenter = new ContentPresenter
            {
                Name = RotationIconPresenterPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            rotationIconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(rotationIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.RotationIconProperty);
            CreateTemplateParentBinding(rotationIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.IconModeProperty, BindingMode.Default, 
                new FuncValueConverter<NodeSwitcherButtonIconMode, bool>(iconMode => iconMode == NodeSwitcherButtonIconMode.Rotation));

            var loadingIconPresenter = new ContentPresenter
            {
                Name = LoadingIconPresenterPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            loadingIconPresenter.RegisterInNameScope(scope);
            
            CreateTemplateParentBinding(loadingIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.LoadingIconProperty);
            CreateTemplateParentBinding(loadingIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.IconModeProperty, 
                BindingMode.Default, 
                new FuncValueConverter<NodeSwitcherButtonIconMode, bool>(iconMode => iconMode == NodeSwitcherButtonIconMode.Loading));

            var leafIconPresenter = new ContentPresenter
            {
                Name = LeafIconPresenterPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            leafIconPresenter.RegisterInNameScope(scope);
            
            CreateTemplateParentBinding(leafIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.LeafIconProperty);
 
            rootContainer.Children.Add(expandIconPresenter);
            rootContainer.Children.Add(collapseIconPresenter);
            rootContainer.Children.Add(rotationIconPresenter);
            rootContainer.Children.Add(loadingIconPresenter);
            rootContainer.Children.Add(leafIconPresenter);
            
            return rootContainer;
        });
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(NodeSwitcherButton.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        
        Add(commonStyle);
        
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        {
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
                enabledStyle.Add(iconStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
                hoverStyle.Add(iconStyle);
            }
            hoverStyle.Add(TemplatedControl.BackgroundProperty, TreeViewTokenKey.NodeHoverBg);
            enabledStyle.Add(hoverStyle);

            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
                checkedStyle.Add(iconStyle);
            }
            enabledStyle.Add(checkedStyle);
        }
        Add(enabledStyle);

        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
            disabledStyle.Add(iconStyle);
        }
        Add(disabledStyle);
        BuildIconsStyle();
        BuildModeStyle();
    }

    private void BuildIconsStyle()
    {
        var iconsStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<Icon>());
        iconsStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSize);
        iconsStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSize);
        Add(iconsStyle);

        {
            var rotationIconStyle = new Style(selector => selector.Nesting().Template().Name(RotationIconPresenterPart).Descendant().OfType<Icon>());
            rotationIconStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSizeXS);
            rotationIconStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSizeXS);
        
            Add(rotationIconStyle);
        }
        
        {
            // 打开关闭指示按钮的动画
            var isMotionEnabledStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(NodeSwitcherButton.IsMotionEnabledProperty, true));
            var rotationIconStyle =
                new Style(selector => selector.Nesting().Template().Name(RotationIconPresenterPart));
            rotationIconStyle.Add(ContentPresenter.TransitionsProperty, new SetterValueFactory<Transitions>(() =>
                new Transitions
                {
                    AnimationUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
                }));
            isMotionEnabledStyle.Add(rotationIconStyle);
            Add(isMotionEnabledStyle);
        }
        
        var checkStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked)
                                                       .PropertyEquals(NodeSwitcherButton.IconModeProperty, NodeSwitcherButtonIconMode.Rotation));
        {
            var rotationIconStyle = new Style(selector => selector.Nesting().Template().Name(RotationIconPresenterPart));
            rotationIconStyle.Add(ContentPresenter.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
                return transformOptions.Build();
            }));
            checkStyle.Add(rotationIconStyle);
        }
        Add(checkStyle);
    }

    private void BuildModeStyle()
    {
        {
            var expandIconPresenter = new Style(selector => selector.Nesting().Template().Name(ExpandIconPresenterPart));
            expandIconPresenter.Add(Icon.IsVisibleProperty, false);
            Add(expandIconPresenter);

            var collapseIconPresenter = new Style(selector => selector.Nesting().Template().Name(CollapseIconPresenterPart));
            collapseIconPresenter.Add(Icon.IsVisibleProperty, false);
            Add(collapseIconPresenter);
            
            var leafIconPresenter = new Style(selector => selector.Nesting().Template().Name(LeafIconPresenterPart));
            leafIconPresenter.Add(Icon.IsVisibleProperty, false);
            Add(leafIconPresenter);
            
        }
        var defaultStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NodeSwitcherButton.IconModeProperty, NodeSwitcherButtonIconMode.Default));
        {
            {
                var expandIconPresenter = new Style(selector => selector.Nesting().Template().Name(ExpandIconPresenterPart));
                expandIconPresenter.Add(Icon.IsVisibleProperty, true);
                defaultStyle.Add(expandIconPresenter);

                var collapseIconPresenter = new Style(selector => selector.Nesting().Template().Name(CollapseIconPresenterPart));
                collapseIconPresenter.Add(Icon.IsVisibleProperty, false);
                defaultStyle.Add(collapseIconPresenter);
            }
            var isChecked = new Style(selector =>
                selector.Nesting().PropertyEquals(NodeSwitcherButton.IsCheckedProperty, true));
            {
                {
                    var expandIconPresenter = new Style(selector => selector.Nesting().Template().Name(ExpandIconPresenterPart));
                    expandIconPresenter.Add(Icon.IsVisibleProperty, false);
                    isChecked.Add(expandIconPresenter);

                    var collapseIconPresenter = new Style(selector => selector.Nesting().Template().Name(CollapseIconPresenterPart));
                    collapseIconPresenter.Add(Icon.IsVisibleProperty, true);
                    isChecked.Add(collapseIconPresenter);
                }
            }
            defaultStyle.Add(isChecked);
        }
        Add(defaultStyle);
        
        var leafModeStyle = new Style(selector => selector.Nesting().PropertyEquals(NodeSwitcherButton.IconModeProperty, NodeSwitcherButtonIconMode.Leaf)
                                                          .PropertyEquals(NodeSwitcherButton.IsLeafIconVisibleProperty, true));
        {
            var leafIconPresenter = new Style(selector => selector.Nesting().Template().Name(LeafIconPresenterPart));
            leafIconPresenter.Add(Icon.IsVisibleProperty, true);
            leafModeStyle.Add(leafIconPresenter);
        }
        Add(leafModeStyle);
    }
}
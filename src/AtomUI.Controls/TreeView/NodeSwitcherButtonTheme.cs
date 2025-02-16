using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NodeSwitcherButtonTheme : BaseControlTheme
{
    public const string RootPart = "PART_Root";
    public const string RotationIconPart = "PART_RotationIcon";
    public const string ExpandIconPart = "PART_ExpandIcon";
    public const string CollapseIconPart = "PART_CollapseIcon";
    public const string LoadingIconPart = "PART_LoadingIcon";
    public const string LeafIconPart = "PART_LeafIcon";
    
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
                Name = ExpandIconPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            expandIconPresenter.RegisterInNameScope(scope);

            CreateTemplateParentBinding(expandIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.ExpandIconProperty);
            CreateTemplateParentBinding(expandIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.ExpandIconVisibleProperty);

            var collapseIconPresenter = new ContentPresenter
            {
                Name = CollapseIconPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            collapseIconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(collapseIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.CollapseIconProperty);
            CreateTemplateParentBinding(collapseIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.CollapseIconVisibleProperty);

            var rotationIconPresenter = new ContentPresenter
            {
                Name = RotationIconPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            rotationIconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(rotationIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.RotationIconProperty);
            CreateTemplateParentBinding(rotationIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.IconModeProperty, BindingMode.Default, new FuncValueConverter<NodeSwitcherButtonIconMode, bool>(iconMode =>
                {
                    return iconMode == NodeSwitcherButtonIconMode.Rotation;
                }));

            var loadingIconPresenter = new ContentPresenter
            {
                Name = LoadingIconPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            loadingIconPresenter.RegisterInNameScope(scope);
            
            CreateTemplateParentBinding(loadingIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.LoadingIconProperty);
            CreateTemplateParentBinding(loadingIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.IconModeProperty, BindingMode.Default, new FuncValueConverter<NodeSwitcherButtonIconMode, bool>(iconMode =>
                {
                    return iconMode == NodeSwitcherButtonIconMode.Loading;
                }));

            var leafIconPresenter = new ContentPresenter
            {
                Name = LeafIconPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            leafIconPresenter.RegisterInNameScope(scope);
            
            CreateTemplateParentBinding(leafIconPresenter, ContentPresenter.ContentProperty,
                NodeSwitcherButton.LeafIconProperty);
            CreateTemplateParentBinding(leafIconPresenter, ContentPresenter.IsVisibleProperty,
                NodeSwitcherButton.IsLeafIconVisibleProperty);
            
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
        
        var checkStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked)
            .PropertyEquals(NodeSwitcherButton.IconModeProperty, NodeSwitcherButtonIconMode.Rotation));
        checkStyle.Add(NodeSwitcherButton.RenderTransformProperty, new SetterValueFactory<ITransform>(() => new RotateTransform(90)));
        commonStyle.Add(checkStyle);
        
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
    }
}
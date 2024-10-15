using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MessageCardTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string IconContentPart = "PART_IconContent";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string MessagePart = "PART_Message";
    public const string MotionActorPart = "PART_MotionActor";
    public const string MarginGhostDecoratorPart = "PART_MarginGhostDecorator";

    public MessageCardTheme()
        : base(typeof(MessageCard))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MessageCard>((card, scope) =>
        {
            BuildInstanceStyles(card);
            var motionActor = new MotionActorControl()
            {
                Name         = MotionActorPart,
                ClipToBounds = false
            };
            motionActor.RegisterInNameScope(scope);

            // 防止关闭的时候抖动，如果直接设置到 MessageCard，layoutTransformControl没有办法平滑处理
            var marginGhostDecorator = new Border
            {
                Name         = MarginGhostDecoratorPart,
                ClipToBounds = false
            };

            var frameDecorator = new Border
            {
                Name = FrameDecoratorPart
            };

            marginGhostDecorator.Child = frameDecorator;

            var header = BuildContent(scope);
            frameDecorator.Child = header;

            frameDecorator.RegisterInNameScope(scope);

            motionActor.Child = marginGhostDecorator;
            return motionActor;
        });
    }

    private DockPanel BuildContent(INameScope scope)
    {
        var headerLayout = new DockPanel
        {
            Name          = HeaderContainerPart,
            LastChildFill = true
        };
        var iconContent = new ContentPresenter
        {
            Name = IconContentPart
        };
        DockPanel.SetDock(iconContent, Dock.Left);
        CreateTemplateParentBinding(iconContent, Visual.IsVisibleProperty, MessageCard.IconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
        CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, MessageCard.IconProperty);
        TokenResourceBinder.CreateTokenBinding(iconContent, Layoutable.MarginProperty,
            MessageTokenResourceKey.MessageIconMargin);

        headerLayout.Children.Add(iconContent);

        var message = new SelectableTextBlock
        {
            Name = MessagePart
        };
        TokenResourceBinder.CreateGlobalTokenBinding(message, SelectableTextBlock.SelectionBrushProperty,
            GlobalTokenResourceKey.SelectionBackground);
        TokenResourceBinder.CreateGlobalTokenBinding(message, SelectableTextBlock.SelectionForegroundBrushProperty,
            GlobalTokenResourceKey.SelectionForeground);

        CreateTemplateParentBinding(message, TextBlock.TextProperty, MessageCard.MessageProperty);
        headerLayout.Children.Add(message);
        return headerLayout;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildContentStyle();
        BuildContentStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);

        var marginGhostDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
        marginGhostDecoratorStyle.Add(Layoutable.MarginProperty, MessageTokenResourceKey.MessageTopMargin);
        commonStyle.Add(marginGhostDecoratorStyle);

        var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
        frameDecoratorStyle.Add(Decorator.PaddingProperty, MessageTokenResourceKey.ContentPadding);
        frameDecoratorStyle.Add(Border.BoxShadowProperty, GlobalTokenResourceKey.BoxShadows);
        frameDecoratorStyle.Add(Border.BackgroundProperty, MessageTokenResourceKey.ContentBg);
        frameDecoratorStyle.Add(Border.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
        commonStyle.Add(frameDecoratorStyle);

        var closedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(MessageCard.IsClosedProperty, true));
        closedStyle.Add(Layoutable.MarginProperty, new Thickness(0));
        commonStyle.Add(closedStyle);

        Add(commonStyle);
    }

    private void BuildContentStyle()
    {
        var titleStyle = new Style(selector => selector.Nesting().Template().Name(MessagePart));
        titleStyle.Add(ContentPresenter.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
        titleStyle.Add(ContentPresenter.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        titleStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorText);
        Add(titleStyle);
    }

    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<Icon>());
        iconStyle.Add(Layoutable.WidthProperty, MessageTokenResourceKey.MessageIconSize);
        iconStyle.Add(Layoutable.HeightProperty, MessageTokenResourceKey.MessageIconSize);
        control.Styles.Add(iconStyle);
    }
}
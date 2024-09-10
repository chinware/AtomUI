using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
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
    public const string LayoutTransformControlPart = "PART_LayoutTransformControl";
    public const string MarginGhostDecoratorPart = "PART_MarginGhostDecorator";

    public const double AnimationMaxOffsetY = 100d;
    public const int AnimationDuration = 400;
    private readonly Easing _quadraticEaseIn = new QuadraticEaseIn();

    private readonly Easing _quadraticEaseOut = new QuadraticEaseOut();

    public MessageCardTheme()
        : base(typeof(MessageCard))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MessageCard>((card, scope) =>
        {
            BuildInstanceStyles(card);
            var layoutTransformControl = new LayoutTransformControl
            {
                Name         = LayoutTransformControlPart,
                ClipToBounds = false
            };
            layoutTransformControl.RegisterInNameScope(scope);

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

            layoutTransformControl.Child = marginGhostDecorator;
            return layoutTransformControl;
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
        BuildAnimationStyle();
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
        var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<PathIcon>());
        iconStyle.Add(Layoutable.WidthProperty, MessageTokenResourceKey.MessageIconSize);
        iconStyle.Add(Layoutable.HeightProperty, MessageTokenResourceKey.MessageIconSize);
        control.Styles.Add(iconStyle);
    }

    private void BuildAnimationStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        {
            var layoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(LayoutTransformControlPart));
            var moveRightInMotionConfig = MotionFactory.BuildMoveUpInMotion(
                AnimationMaxOffsetY, TimeSpan.FromMilliseconds(AnimationDuration), _quadraticEaseOut,
                FillMode.Forward);
            foreach (var animation in moveRightInMotionConfig.Animations)
            {
                layoutTransformStyle.Animations.Add(animation);
            }

            commonStyle.Add(layoutTransformStyle);
        }

        var isClosingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(MessageCard.IsClosingProperty, true));
        {
            var layoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(LayoutTransformControlPart));

            var moveRightOutMotionConfig = MotionFactory.BuildMoveUpOutMotion(
                AnimationMaxOffsetY, TimeSpan.FromMilliseconds(AnimationDuration), _quadraticEaseIn, FillMode.Forward);

            foreach (var animation in moveRightOutMotionConfig.Animations)
            {
                layoutTransformStyle.Animations.Add(animation);
            }

            isClosingStyle.Animations.Add(new Animation
            {
                Duration = TimeSpan.FromMilliseconds(AnimationDuration * 1.2),
                Easing   = _quadraticEaseIn,
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters =
                        {
                            new Setter(MessageCard.IsClosedProperty, true)
                        }
                    }
                }
            });

            isClosingStyle.Add(layoutTransformStyle);
        }
        commonStyle.Add(isClosingStyle);

        Add(commonStyle);
    }
}
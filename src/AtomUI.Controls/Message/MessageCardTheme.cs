using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme;
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
    public const string FramePart = "PART_Frame";
    public const string IconContentPart = "PART_IconContent";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string MessagePart = "PART_Message";
    public const string MotionActorPart = "PART_MotionActor";

    public MessageCardTheme()
        : base(typeof(MessageCard))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MessageCard>((messageCard, scope) =>
        {
            var motionActor = new MotionActorControl()
            {
                Name         = MotionActorPart,
                ClipToBounds = false
            };
            motionActor.RegisterInNameScope(scope);

            var frame = new Border
            {
                Name = FramePart
            };

            var header = BuildContent(messageCard, scope);
            frame.Child = header;

            frame.RegisterInNameScope(scope);

            motionActor.Child = frame;
            return motionActor;
        });
    }

    private DockPanel BuildContent(MessageCard messageCard, INameScope scope)
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
        headerLayout.Children.Add(iconContent);

        var message = new SelectableTextBlock
        {
            Name = MessagePart
        };

        CreateTemplateParentBinding(message, SelectableTextBlock.TextProperty, MessageCard.MessageProperty);
        headerLayout.Children.Add(message);
        
        return headerLayout;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildContentStyle();
        BuildIconStyle();
    }

    private void BuildIconStyle()
    {
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.WidthProperty, MessageTokenKey.MessageIconSize);
            iconStyle.Add(Icon.HeightProperty, MessageTokenKey.MessageIconSize);
            Add(iconStyle);
        }
        
        var infoOrLoadingTypeStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(MessageCard.MessageTypeProperty, MessageType.Information),
            selector.Nesting().PropertyEquals(MessageCard.MessageTypeProperty, MessageType.Loading)));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorPrimary);
            infoOrLoadingTypeStyle.Add(iconStyle);
        }
        Add(infoOrLoadingTypeStyle);
        
        var errorTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(MessageCard.MessageTypeProperty, MessageType.Error));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
            errorTypeStyle.Add(iconStyle);
        }
        Add(errorTypeStyle);
        
        var successTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(MessageCard.MessageTypeProperty, MessageType.Success));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorSuccess);
            successTypeStyle.Add(iconStyle);
        }
        Add(successTypeStyle);
        
        var warningTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(MessageCard.MessageTypeProperty, MessageType.Warning));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorWarning);
            warningTypeStyle.Add(iconStyle);
        }
        Add(warningTypeStyle);
 
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(MessageCard.OpenCloseMotionDurationProperty, SharedTokenKey.MotionDurationMid);
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);

        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Layoutable.MarginProperty, MessageTokenKey.MessageTopMargin);
        frameStyle.Add(Decorator.PaddingProperty, MessageTokenKey.ContentPadding);
        frameStyle.Add(Border.BoxShadowProperty, SharedTokenKey.BoxShadows);
        frameStyle.Add(Border.BackgroundProperty, MessageTokenKey.ContentBg);
        frameStyle.Add(Border.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        commonStyle.Add(frameStyle);

        var closedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(MessageCard.IsClosedProperty, true));
        closedStyle.Add(Layoutable.MarginProperty, new Thickness(0));
        commonStyle.Add(closedStyle);

        Add(commonStyle);
    }

    private void BuildContentStyle()
    {
        var messageStyle = new Style(selector => selector.Nesting().Template().Name(MessagePart));
        messageStyle.Add(SelectableTextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
        messageStyle.Add(SelectableTextBlock.FontSizeProperty, SharedTokenKey.FontSize);
        messageStyle.Add(SelectableTextBlock.ForegroundProperty, SharedTokenKey.ColorText);
        messageStyle.Add(SelectableTextBlock.SelectionBrushProperty, SharedTokenKey.SelectionBackground);
        messageStyle.Add(SelectableTextBlock.SelectionForegroundBrushProperty, SharedTokenKey.SelectionForeground);
        Add(messageStyle);

        var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
        iconStyle.Add(Layoutable.MarginProperty, MessageTokenKey.MessageIconMargin);
        Add(iconStyle);
    }

}
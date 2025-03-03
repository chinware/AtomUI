using AtomUI.IconPkg;
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
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PopupConfirmContainerTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string ButtonLayoutPart = "PART_ButtonLayout";
    public const string OkButtonPart = "PART_OkButton";
    public const string CancelButtonPart = "PART_CancelButton";
    public const string TitlePart = "PART_Title";
    public const string ContentPart = "PART_Content";
    public const string IconContentPart = "PART_IconContent";

    public PopupConfirmContainerTheme()
        : base(typeof(PopupConfirmContainer))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<PopupConfirmContainer>((popupConfirmContainer, scope) =>
        {
            var mainLayout = new DockPanel
            {
                Name          = MainLayoutPart,
                LastChildFill = true
            };
            var buttons = BuildButtons(popupConfirmContainer, scope);
            mainLayout.Children.Add(buttons);
            var content = BuildContent(popupConfirmContainer, scope);
            mainLayout.Children.Add(content);
            popupConfirmContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(mainLayout,
                Layoutable.MinWidthProperty,
                PopupConfirmTokenKey.PopupMinWidth));
            popupConfirmContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(mainLayout,
                Layoutable.MinHeightProperty,
                PopupConfirmTokenKey.PopupMinHeight));
            BuildInstanceStyles(popupConfirmContainer);
            return mainLayout;
        });
    }

    private DockPanel BuildContent(PopupConfirmContainer popupConfirmContainer, INameScope scope)
    {
        var wrapperLayout = new DockPanel
        {
            LastChildFill = true
        };

        var contentLayout = new DockPanel
        {
            LastChildFill = true
        };

        var iconContentPresenter = new ContentPresenter
        {
            Name = IconContentPart
        };

        CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty,
            PopupConfirmContainer.IconProperty);
        CreateTemplateParentBinding(iconContentPresenter, Visual.IsVisibleProperty,
            PopupConfirmContainer.IconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);

        DockPanel.SetDock(iconContentPresenter, Dock.Left);
        wrapperLayout.Children.Add(iconContentPresenter);
        wrapperLayout.Children.Add(contentLayout);

        var titleTextBlock = new SingleLineText
        {
            Name              = TitlePart,
            VerticalAlignment = VerticalAlignment.Center,
        };
        CreateTemplateParentBinding(titleTextBlock, SingleLineText.TextProperty, PopupConfirmContainer.TitleProperty);
        DockPanel.SetDock(titleTextBlock, Dock.Top);
        contentLayout.Children.Add(titleTextBlock);

        var contentPresenter = new ContentPresenter
        {
            Name = ContentPart
        };
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            PopupConfirmContainer.ConfirmContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            PopupConfirmContainer.ConfirmContentTemplateProperty);

        contentLayout.Children.Add(contentPresenter);

        return wrapperLayout;
    }

    private Panel BuildButtons(PopupConfirmContainer popupConfirmContainer, INameScope scope)
    {
        var buttonLayout = new StackPanel
        {
            Name                = ButtonLayoutPart,
            Orientation         = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Bottom,
        };
        DockPanel.SetDock(buttonLayout, Dock.Bottom);

        var cancelButton = new Button
        {
            Name                = CancelButtonPart,
            SizeType            = SizeType.Small,
        };
        
        cancelButton.RegisterInNameScope(scope);
        popupConfirmContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(cancelButton, Layoutable.MarginProperty,
            PopupConfirmTokenKey.ButtonMargin));
        CreateTemplateParentBinding(cancelButton, Button.TextProperty, PopupConfirmContainer.CancelTextProperty);
        CreateTemplateParentBinding(cancelButton, Visual.IsVisibleProperty,
            PopupConfirmContainer.IsShowCancelButtonProperty);
        buttonLayout.Children.Add(cancelButton);

        var okButton = new Button
        {
            Name     = OkButtonPart,
            SizeType = SizeType.Small,
        };

        okButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(okButton, Button.TextProperty, PopupConfirmContainer.OkTextProperty);
        CreateTemplateParentBinding(okButton, Button.ButtonTypeProperty, PopupConfirmContainer.OkButtonTypeProperty);
        buttonLayout.Children.Add(okButton);
        return buttonLayout;
    }

    protected override void BuildInstanceStyles(Control control)
    {
        {
            var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeLG);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeLG);
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            control.Styles.Add(iconStyle);
        }
        var infoStatusStyle = new Style(selector => selector
                                                    .PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty,
                                                        PopupConfirmStatus.Info)
                                                    .Descendant().Name(IconContentPart).Child().OfType<Icon>());
        infoStatusStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorPrimary);
        control.Styles.Add(infoStatusStyle);

        var warningStatusStyle = new Style(selector => selector
                                                       .PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty,
                                                           PopupConfirmStatus.Warning)
                                                       .Descendant().Name(IconContentPart).Child().OfType<Icon>());
        warningStatusStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorWarning);
        control.Styles.Add(warningStatusStyle);

        var errorStatusStyle = new Style(selector => selector
                                                     .PropertyEquals(PopupConfirmContainer.ConfirmStatusProperty,
                                                         PopupConfirmStatus.Error)
                                                     .Descendant().Name(IconContentPart).Child().OfType<Icon>());
        errorStatusStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
        control.Styles.Add(errorStatusStyle);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var iconContentPresenter = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
        iconContentPresenter.Add(Layoutable.MarginProperty, PopupConfirmTokenKey.IconMargin);
        commonStyle.Add(iconContentPresenter);

        var titleStyle = new Style(selector => selector.Nesting().Template().Name(TitlePart));
        titleStyle.Add(Layoutable.MarginProperty, PopupConfirmTokenKey.TitleMargin);
        titleStyle.Add(SingleLineText.ForegroundProperty, SharedTokenKey.ColorTextHeading);
        titleStyle.Add(SingleLineText.FontWeightProperty, FontWeight.SemiBold);
        commonStyle.Add(titleStyle);

        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
        contentStyle.Add(Layoutable.MarginProperty, PopupConfirmTokenKey.ContentContainerMargin);
        commonStyle.Add(contentStyle);
        
        var okButtonStyle = new Style(selector => selector.Nesting().Template().Name(OkButtonPart));
        okButtonStyle.Add(Layoutable.MarginProperty, PopupConfirmTokenKey.ButtonMargin);
        commonStyle.Add(okButtonStyle);
        
        var cancelButtonStyle = new Style(selector => selector.Nesting().Template().Name(CancelButtonPart));
        cancelButtonStyle.Add(Layoutable.MarginProperty, PopupConfirmTokenKey.ButtonMargin);
        commonStyle.Add(cancelButtonStyle);

        Add(commonStyle);
    }
}
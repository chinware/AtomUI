using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AlertTheme : BaseControlTheme
{
    public const string CloseBtnPart = "PART_CloseBtn";
    public const string InfoIconPart = "PART_InfoIcon";
    public const string DescriptionLabelPart = "PART_DescriptionLabel";
    public const string MessageLabelPart = "PART_MessageLabel";
    public const string MarqueeLabelPart = "PART_MarqueeLabel";

    public AlertTheme()
        : base(typeof(Alert))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        var controlTemplate = new FuncControlTemplate<Alert>((alert, scope) =>
        {
            var borderContainer = CreateBorderContainer(alert, scope);
            var infoStack = new StackPanel
            {
                Orientation       = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(infoStack, 1);
            Grid.SetRow(infoStack, 0);

            var mainLayout = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto)
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            mainLayout.Children.Add(infoStack);

            var closeBtn = CreateCloseButton(alert, scope);
            mainLayout.Children.Add(closeBtn);

            var infoIcon = CreateInfoIcon(scope);
            mainLayout.Children.Add(infoIcon);

            var normalInfoLabel = CreateMessageLabel(alert, scope);
            infoStack.Children.Add(normalInfoLabel);

            var marqueeLabel = CreateMessageMarqueeLabel(alert, scope);
            infoStack.Children.Add(marqueeLabel);

            var descriptionLabel = CreateDescriptionLabel(alert, scope);
            infoStack.Children.Add(descriptionLabel);

            if (alert.ExtraAction is not null)
            {
                alert.VerticalAlignment = VerticalAlignment.Top;
                Grid.SetColumn(alert.ExtraAction, 2);
                Grid.SetRow(alert.ExtraAction, 0);
                mainLayout.Children.Add(alert.ExtraAction);
            }

            borderContainer.Child = mainLayout;

            return borderContainer;
        });

        return controlTemplate;
    }

    private Border CreateBorderContainer(Alert alert, INameScope scope)
    {
        var borderContainer = new Border();

        BindUtils.RelayBind(alert, TemplatedControl.BackgroundProperty, borderContainer,
            ContentPresenter.BackgroundProperty);
        BindUtils.RelayBind(alert, TemplatedControl.BorderBrushProperty, borderContainer,
            ContentPresenter.BorderBrushProperty);
        BindUtils.RelayBind(alert, TemplatedControl.CornerRadiusProperty, borderContainer,
            ContentPresenter.CornerRadiusProperty);
        BindUtils.RelayBind(alert, TemplatedControl.BorderThicknessProperty, borderContainer,
            ContentPresenter.BorderThicknessProperty);
        BindUtils.RelayBind(alert, TemplatedControl.PaddingProperty, borderContainer, ContentPresenter.PaddingProperty);

        return borderContainer;
    }

    private IconButton CreateCloseButton(Alert alert, INameScope scope)
    {
        var closeBtn = new IconButton
        {
            Name = CloseBtnPart
        };

        RegisterTokenResourceBindings(alert, () =>
        {
            alert.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconWidthProperty,
                SharedTokenKey.IconSizeSM));
            alert.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconHeightProperty,
                SharedTokenKey.IconSizeSM));
            alert.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeBtn, Layoutable.MarginProperty,
                AlertTokenKey.ExtraElementMargin));
        });

        CreateTemplateParentBinding(closeBtn, Visual.IsVisibleProperty, Alert.IsClosableProperty);
        CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Alert.CloseIconProperty);

        Grid.SetRow(closeBtn, 0);
        Grid.SetColumn(closeBtn, 3);
        return closeBtn;
    }

    private Icon CreateInfoIcon(INameScope scope)
    {
        var infoIcon = new Icon
        {
            Name = InfoIconPart
        };
        CreateTemplateParentBinding(infoIcon, Visual.IsVisibleProperty, Alert.IsShowIconProperty);
        Grid.SetRow(infoIcon, 0);
        Grid.SetColumn(infoIcon, 0);
        return infoIcon;
    }

    private Label CreateMessageLabel(Alert alert, INameScope scope)
    {
        var label = new Label
        {
            Name                       = MessageLabelPart,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment   = VerticalAlignment.Center,
            Padding                    = new Thickness(0)
        };
        TextBlock.SetTextWrapping(label, TextWrapping.Wrap);
        BindUtils.RelayBind(alert, Alert.MessageProperty, label, ContentControl.ContentProperty);
        return label;
    }

    private MarqueeLabel CreateMessageMarqueeLabel(Alert alert, INameScope scope)
    {
        var label = new MarqueeLabel
        {
            Name                = MarqueeLabelPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Padding             = new Thickness(0)
        };
        CreateTemplateParentBinding(label, Visual.IsVisibleProperty, Alert.IsMessageMarqueEnabledProperty);
        BindUtils.RelayBind(alert, Alert.MessageProperty, label, MarqueeLabel.TextProperty);
        return label;
    }

    private Label CreateDescriptionLabel(Alert alert, INameScope scope)
    {
        var descriptionLabel = new Label
        {
            Name                       = DescriptionLabelPart,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment   = VerticalAlignment.Center,
            Padding                    = new Thickness(0),
            IsVisible                  = !string.IsNullOrEmpty(alert.Description)
        };
        RegisterTokenResourceBindings(alert, () =>
        {
            alert.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(descriptionLabel, Layoutable.MarginProperty,
                SharedTokenKey.MarginXS, BindingPriority.Template,
                o =>
                {
                    if (o is double value)
                    {
                        return new Thickness(0, value, 0, 0);
                    }
                    return o;
                }));
        });
        BindUtils.RelayBind(alert, Alert.DescriptionProperty, descriptionLabel, ContentControl.ContentProperty);
        TextBlock.SetTextWrapping(descriptionLabel, TextWrapping.Wrap);
        return descriptionLabel;
    }
    
     protected override void BuildStyles()
    {
        BuildAlertTypeStyle();
        BuildMessageLabelStyle();
        BuildDescriptionLabelStyle();
        BuildCloseBtnStyle();
        BuildInfoIconStyle();
    }

    private void BuildAlertTypeStyle()
    {
        this.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        var successStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Success));
        successStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorSuccessBg);
        successStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorSuccessBorder);
        Add(successStyle);

        var infoStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
        infoStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorInfoBg);
        infoStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorInfoBorder);
        Add(infoStyle);

        var warningStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
        warningStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorWarningBg);
        warningStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorWarningBorder);
        Add(warningStyle);

        var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
        errorStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorErrorBg);
        errorStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorBorder);
        Add(errorStyle);

        // 根据是否显示 Description 设置 Padding
        {
            // 为空
            var paddingStyle =
                new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
            paddingStyle.Add(TemplatedControl.PaddingProperty, AlertTokenKey.DefaultPadding);
            Add(paddingStyle);
        }
        {
            // 不为空
            var paddingStyle = new Style(selector =>
                selector.Nesting().Not(x => x.PropertyEquals(Alert.DescriptionProperty, null)));
            paddingStyle.Add(TemplatedControl.PaddingProperty, AlertTokenKey.WithDescriptionPadding);
            Add(paddingStyle);
        }
    }

    private void BuildMessageLabelStyle()
    {
        var normalLabel         = default(Selector).Nesting().Template().OfType<Label>().Name(MessageLabelPart);
        var marqueeLabel        = default(Selector).Nesting().Template().OfType<MarqueeLabel>().Name(MarqueeLabelPart);
        var commonLabelSelector = Selectors.Or(normalLabel, marqueeLabel);
        {
            // Description 不为空
            var descriptionStyle = new Style(selector =>
                selector.Nesting().Not(x => x.PropertyEquals(Alert.DescriptionProperty, null)));
            var commonLabelStyle = new Style(selector => commonLabelSelector);
            commonLabelStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeLG);
            commonLabelStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            descriptionStyle.Add(commonLabelStyle);
            Add(descriptionStyle);
        }
        {
            var descriptionStyle =
                new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
            var commonLabelStyle = new Style(selector => commonLabelSelector);
            commonLabelStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
            commonLabelStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            descriptionStyle.Add(commonLabelStyle);
            Add(descriptionStyle);
        }

        {
            var normalLabelStyle = new Style(selector => selector.Nesting().Template().Name(MessageLabelPart));
            normalLabelStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
            Add(normalLabelStyle);
        }

        // 根据是否显示 MarqueeLabel 是指对应的样式
        {
            var wrapperStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Alert.IsMessageMarqueEnabledProperty, true));
            var normalLabelStyle = new Style(selector => normalLabel);
            normalLabelStyle.Add(Visual.IsVisibleProperty, false);
            wrapperStyle.Add(normalLabelStyle);
            Add(wrapperStyle);
        }
    }

    private void BuildDescriptionLabelStyle()
    {
        var normalLabelStyle = new Style(selector => selector.Nesting().Template().Name(DescriptionLabelPart));
        normalLabelStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
        Add(normalLabelStyle);
    }

    private void BuildCloseBtnStyle()
    {
        var closeBtnSelector = default(Selector).Nesting().Template().OfType<IconButton>().Name(CloseBtnPart);
        // 设置根据 Description 是否显示 Close 按钮的相关样式
        {
            // 为空
            var descriptionStyle =
                new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
            var closeBtnStyle = new Style(selector => closeBtnSelector);
            closeBtnStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            descriptionStyle.Add(closeBtnStyle);
            Add(descriptionStyle);
        }
        {
            // 不为空
            var descriptionStyle = new Style(selector =>
                selector.Nesting().Not(x => x.PropertyEquals(Alert.DescriptionProperty, null)));
            var closeBtnStyle = new Style(selector => closeBtnSelector);
            closeBtnStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            descriptionStyle.Add(closeBtnStyle);
            Add(descriptionStyle);
        }
    }

    private void BuildInfoIconStyle()
    {
        var infoIconSelector = default(Selector).Nesting().Template().OfType<Icon>().Name(InfoIconPart);
        {
            var successStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Success));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Icon.IconInfoProperty, new SetterValueFactory<IconInfo>(() => AntDesignIconPackage.Current.GetIconInfo(AntDesignIconKind.CheckCircleFilled)));
            infoIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorSuccess);
            successStyle.Add(infoIconStyle);
            Add(successStyle);
        }

        {
            var infoStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Info));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Icon.IconInfoProperty, new SetterValueFactory<IconInfo>(() => AntDesignIconPackage.Current.GetIconInfo(AntDesignIconKind.InfoCircleFilled)));
            infoIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorPrimary);
            infoStyle.Add(infoIconStyle);
            Add(infoStyle);
        }

        {
            var warningStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Warning));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Icon.IconInfoProperty, new SetterValueFactory<IconInfo>(() => AntDesignIconPackage.Current.GetIconInfo(AntDesignIconKind.ExclamationCircleFilled)));
            infoIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorWarning);
            warningStyle.Add(infoIconStyle);
            Add(warningStyle);
        }

        {
            var errorStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Alert.TypeProperty, AlertType.Error));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Icon.IconInfoProperty, new SetterValueFactory<IconInfo>(() => AntDesignIconPackage.Current.GetIconInfo(AntDesignIconKind.CloseCircleFilled)));
            infoIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
            errorStyle.Add(infoIconStyle);
            Add(errorStyle);
        }

        // 设置根据 Description 设置 InfoIcon 样式
        {
            // 为空
            var wrapperStyle =
                new Style(selector => selector.Nesting().PropertyEquals(Alert.DescriptionProperty, null));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Layoutable.WidthProperty, AlertTokenKey.IconSize);
            infoIconStyle.Add(Layoutable.HeightProperty, AlertTokenKey.IconSize);
            infoIconStyle.Add(Layoutable.MarginProperty, AlertTokenKey.IconDefaultMargin);
            infoIconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            wrapperStyle.Add(infoIconStyle);
            Add(wrapperStyle);
        }
        {
            // 不为空
            var wrapperStyle = new Style(selector =>
                selector.Nesting().Not(x => x.PropertyEquals(Alert.DescriptionProperty, null)));
            var infoIconStyle = new Style(selector => infoIconSelector);
            infoIconStyle.Add(Layoutable.WidthProperty, AlertTokenKey.WithDescriptionIconSize);
            infoIconStyle.Add(Layoutable.HeightProperty, AlertTokenKey.WithDescriptionIconSize);
            infoIconStyle.Add(Layoutable.MarginProperty, AlertTokenKey.IconWithDescriptionMargin);
            infoIconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            wrapperStyle.Add(infoIconStyle);
            Add(wrapperStyle);
        }
    }
}
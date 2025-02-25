using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AvatarTheme : BaseControlTheme
{
    public const string IconPart = "PART_Icon";
    public const string ImagePart = "PART_imagePart";
    public const string AltLabelPart = "PART_AltLabel";

    public AvatarTheme()
        : base(typeof(Avatar))
    {
    }

    protected override void BuildStyles()
    {
        Console.WriteLine("cc BuildStyles");
        BuildAvatarTypeStyle();
        BuildContainerStyle();
        BuildImageStyle();
        BuildIconSizeStyle();
    }

    private void BuildAvatarTypeStyle()
    {
        this.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        this.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        //this.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
    }

    private void BuildContainerStyle()
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Avatar.SizeTypeProperty, AvatarSizeType.Large));
        largeSizeStyle.Add(Layoutable.WidthProperty, AvatarTokenKey.ContainerSizeLG);
        largeSizeStyle.Add(Layoutable.HeightProperty, AvatarTokenKey.ContainerSizeLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, AvatarTokenKey.TextFontSizeLG);
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);

        Add(largeSizeStyle);
        
        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Avatar.SizeTypeProperty, AvatarSizeType.Small));
        smallSizeStyle.Add(Layoutable.WidthProperty, AvatarTokenKey.ContainerSizeSM);
        smallSizeStyle.Add(Layoutable.HeightProperty, AvatarTokenKey.ContainerSizeSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, AvatarTokenKey.TextFontSizeSM);
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);

        Add(smallSizeStyle);
        
        var defaultSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Avatar.SizeTypeProperty, AvatarSizeType.Default));
        defaultSizeStyle.Add(Layoutable.WidthProperty, AvatarTokenKey.ContainerSize);
        defaultSizeStyle.Add(Layoutable.HeightProperty, AvatarTokenKey.ContainerSize);
        defaultSizeStyle.Add(TemplatedControl.FontSizeProperty, AvatarTokenKey.TextFontSize);
        defaultSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);

        Add(defaultSizeStyle);

        var numSizeStyle = new Style(selector =>
            selector.Nesting().Class(Avatar.SizeNumber));
        //numSizeStyle.Add(Label.FontSizeProperty, AvatarTokenKey.ContainerSize);
        Add(numSizeStyle);
    }

    private void BuildImageStyle()
    {
        var imageSelector = default(Selector).Nesting().Template().OfType<Image>().Name(ImagePart);
        {
           
            var imageStyle = new Style(selector => imageSelector);
            imageStyle.Add(Image.StretchProperty, Stretch.UniformToFill);
            Add(imageStyle);
        }
    }
    
    private void BuildIconSizeStyle()
    {
        //this.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTextPlaceholder);
        //this.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
        var style = new Style(selector =>
            selector.Nesting().PropertyEquals(Avatar.IsIconProperty, true));
        style.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTextPlaceholder);
        style.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
        Add(style);
        
        style = new Style(selector =>
            selector.Nesting().PropertyEquals(Avatar.IsTxtProperty, true));
        style.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTextPlaceholder);
        style.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
        Add(style);
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        var controlTemplate = new FuncControlTemplate<Avatar>((avatar, scope) =>
        {
            
            var borderContainer = CreateBorderContainer(avatar, scope);

            var mainLayout = new Panel();
            
            var icon = CreateIcon(avatar, scope);
            mainLayout.Children.Add(icon);
            
            var image = CreateImage(avatar, scope);
            mainLayout.Children.Add(image);

            var svg = CreateSvg(avatar, scope);
            mainLayout.Children.Add(svg);
            
            var label = CreateTxt(avatar, scope);
            label.RegisterInNameScope(scope);
            mainLayout.Children.Add(label);
            
            borderContainer.Child = mainLayout;

            return borderContainer;
        });

        return controlTemplate;
    }

    private Border CreateBorderContainer(Avatar avatar, INameScope scope)
    {
        var borderContainer = new Border();
        borderContainer.ClipToBounds = true;

        BindUtils.RelayBind(avatar, TemplatedControl.BackgroundProperty, borderContainer,
            ContentPresenter.BackgroundProperty);
        BindUtils.RelayBind(avatar, TemplatedControl.BorderBrushProperty, borderContainer,
            ContentPresenter.BorderBrushProperty);
        BindUtils.RelayBind(avatar, TemplatedControl.CornerRadiusProperty, borderContainer,
            ContentPresenter.CornerRadiusProperty);
        BindUtils.RelayBind(avatar, TemplatedControl.BorderThicknessProperty, borderContainer,
            ContentPresenter.BorderThicknessProperty);
        BindUtils.RelayBind(avatar, TemplatedControl.PaddingProperty, borderContainer, ContentPresenter.PaddingProperty);

        return borderContainer;
    }

    private Image CreateImage(Avatar avatar, INameScope scope)
    {
        var image = new Image
        {
            Name = ImagePart
        };
        BindUtils.RelayBind(avatar, Avatar.CurrentImageProperty, image, Image.SourceProperty);
                BindUtils.RelayBind(avatar, Avatar.CurrentImageProperty, image, Image.SourceProperty);
        CreateTemplateParentBinding(image, Visual.IsVisibleProperty,
            Avatar.CurrentImageProperty, BindingMode.Default,
            ObjectConverters.IsNotNull);
        return image;
    }
    
    private Avalonia.Svg.Svg CreateSvg(Avatar avatar, INameScope scope)
    {
        var svg = new Avalonia.Svg.Svg(new Uri("https://github.com/avaloniaui"));
        BindUtils.RelayBind(avatar, Avatar.SvgProperty, svg, Avalonia.Svg.Svg.PathProperty);
        CreateTemplateParentBinding(svg, Visual.IsVisibleProperty,
            Avatar.SvgProperty, BindingMode.Default,
            ObjectConverters.IsNotNull);
        return svg;
    }

    private ContentPresenter CreateIcon(Avatar avatar, INameScope scope)
    {
        var iconPresenter = new ContentPresenter()
        {
            Name = IconPart
        };

        CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, Avatar.IconProperty);
        CreateTemplateParentBinding(iconPresenter, Layoutable.WidthProperty, Avatar.IconSizeProperty);
        CreateTemplateParentBinding(iconPresenter, Layoutable.HeightProperty, Avatar.IconSizeProperty);
        CreateTemplateParentBinding(iconPresenter, Visual.IsVisibleProperty,
            Avatar.IsIconProperty);
        return iconPresenter;
    }
    
    private Label CreateTxt(Avatar avatar, INameScope scope)
    {
       
        var label = new Label
        {
            Name                       = AltLabelPart,
            Padding                    = new Thickness(0),
            VerticalContentAlignment   = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment          = VerticalAlignment.Center,
        };
        CreateTemplateParentBinding(label, ContentControl.ContentProperty, Avatar.AltProperty);
        CreateTemplateParentBinding(label, Visual.IsVisibleProperty,
            Avatar.IsTxtProperty);
        BindUtils.RelayBind(avatar, Avatar.LabelFontSizeProperty, label,
            TemplatedControl.FontSizeProperty);
        BindUtils.RelayBind(avatar, Avatar.GapProperty, label, Layoutable.MarginProperty, 
            d => new Thickness(d, 0));
        /*BindUtils.RelayBind(avatar, Avatar.TxtScaleProperty, label, Visual.RenderTransformProperty, 
            d => new ScaleTransform(d, d));*/
        
        return label;
    }

    
}
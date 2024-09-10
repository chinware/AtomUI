using Avalonia;
using Avalonia.Media;

namespace AtomUI.Icon;

public class IconImage : DrawingImage, IImage
{
    public static readonly StyledProperty<IconInfo> DataProperty = AvaloniaProperty.Register<
        IconImage,
        IconInfo
    >(nameof(Data), new IconInfo());

    public static readonly StyledProperty<IconMode> IconModeProperty = AvaloniaProperty.Register<
        IconImage,
        IconMode>(nameof(IconMode));

    public static readonly StyledProperty<Size> SizeProperty = AvaloniaProperty.Register<IconImage, Size>(
        nameof(Size), new Size(16, 16));

    public IconInfo Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
    }

    public new Size Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <inheritdoc />
    Size IImage.Size => GetValue(SizeProperty);

    public IconImage(IconInfo? data = null)
    {
        Data = data ?? new IconInfo();
    }

    /// <inheritdoc />
    void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        var drawing = Drawing;
        if (drawing is null)
        {
            return;
        }

        var scale = Matrix.CreateScale(
            destRect.Width / sourceRect.Width,
            destRect.Height / sourceRect.Height
        );
        var translate = Matrix.CreateTranslation(
            -sourceRect.X + destRect.X,
            -sourceRect.Y + destRect.Y
        );
        using (context.PushClip(destRect))
        using (context.PushTransform(translate * scale))
        {
            var geometriesData  = Data.Data;
            var themeType       = Data.ThemeType;
            var geometryDrawing = (Drawing as GeometryDrawing)!;
            if (themeType != IconThemeType.TwoTone)
            {
                var brush     = new SolidColorBrush();
                var colorInfo = Data.ColorInfo;
                if (colorInfo is not null)
                {
                    if (IconMode == IconMode.Normal)
                    {
                        brush.Color = colorInfo.Value.NormalColor;
                    }
                    else if (IconMode == IconMode.Active)
                    {
                        brush.Color = colorInfo.Value.ActiveColor;
                    }
                    else if (IconMode == IconMode.Selected)
                    {
                        brush.Color = colorInfo.Value.SelectedColor;
                    }
                    else if (IconMode == IconMode.Disabled)
                    {
                        brush.Color = colorInfo.Value.DisabledColor;
                    }
                }

                brush.Color = Colors.Black;

                geometryDrawing.Brush = brush;
                // foreach (var geometryData in geometriesData) {
                //    geometryDrawing.Geometry = StreamGeometry.Parse(geometryData.PathData);
                //    geometryDrawing.Draw(context);
                // }
                geometryDrawing.Geometry = StreamGeometry.Parse(
                    "M880 184H712v-64c0-4.4-3.6-8-8-8h-56c-4.4 0-8 3.6-8 8v64H384v-64c0-4.4-3.6-8-8-8h-56c-4.4 0-8 3.6-8 8v64H144c-17.7 0-32 14.3-32 32v664c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V216c0-17.7-14.3-32-32-32zM648.3 426.8l-87.7 161.1h45.7c5.5 0 10 4.5 10 10v21.3c0 5.5-4.5 10-10 10h-63.4v29.7h63.4c5.5 0 10 4.5 10 10v21.3c0 5.5-4.5 10-10 10h-63.4V752c0 5.5-4.5 10-10 10h-41.3c-5.5 0-10-4.5-10-10v-51.8h-63.1c-5.5 0-10-4.5-10-10v-21.3c0-5.5 4.5-10 10-10h63.1v-29.7h-63.1c-5.5 0-10-4.5-10-10v-21.3c0-5.5 4.5-10 10-10h45.2l-88-161.1c-2.6-4.8-.9-10.9 4-13.6 1.5-.8 3.1-1.2 4.8-1.2h46c3.8 0 7.2 2.1 8.9 5.5l72.9 144.3 73.2-144.3a10 10 0 0 1 8.9-5.5h45c5.5 0 10 4.5 10 10 .1 1.7-.3 3.3-1.1 4.8z");
                geometryDrawing.Draw(context);
            }
            else
            {
                var twoToneColor = Data.TwoToneColorInfo;
                foreach (var geometryData in geometriesData)
                {
                    var brush = new SolidColorBrush();
                    if (twoToneColor is not null)
                    {
                        if (geometryData.IsPrimary)
                        {
                            brush.Color = twoToneColor.Value.PrimaryColor;
                        }
                        else
                        {
                            brush.Color = twoToneColor.Value.SecondaryColor;
                        }
                    }

                    geometryDrawing.Brush = brush;

                    geometryDrawing.Geometry = StreamGeometry.Parse(
                        "M880 184H712v-64c0-4.4-3.6-8-8-8h-56c-4.4 0-8 3.6-8 8v64H384v-64c0-4.4-3.6-8-8-8h-56c-4.4 0-8 3.6-8 8v64H144c-17.7 0-32 14.3-32 32v664c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V216c0-17.7-14.3-32-32-32zM648.3 426.8l-87.7 161.1h45.7c5.5 0 10 4.5 10 10v21.3c0 5.5-4.5 10-10 10h-63.4v29.7h63.4c5.5 0 10 4.5 10 10v21.3c0 5.5-4.5 10-10 10h-63.4V752c0 5.5-4.5 10-10 10h-41.3c-5.5 0-10-4.5-10-10v-51.8h-63.1c-5.5 0-10-4.5-10-10v-21.3c0-5.5 4.5-10 10-10h63.1v-29.7h-63.1c-5.5 0-10-4.5-10-10v-21.3c0-5.5 4.5-10 10-10h45.2l-88-161.1c-2.6-4.8-.9-10.9 4-13.6 1.5-.8 3.1-1.2 4.8-1.2h46c3.8 0 7.2 2.1 8.9 5.5l72.9 144.3 73.2-144.3a10 10 0 0 1 8.9-5.5h45c5.5 0 10 4.5 10 10 .1 1.7-.3 3.3-1.1 4.8z");
                    geometryDrawing.Draw(context);
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DataProperty ||
            (change.Property == IconModeProperty && Data.ThemeType != IconThemeType.TwoTone) ||
            change.Property == SizeProperty)
        {
            Drawing ??= new GeometryDrawing();
            RaiseInvalidated(EventArgs.Empty);
        }
    }
}
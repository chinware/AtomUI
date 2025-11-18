using AtomUI.Desktop.Controls.Themes;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class SearchButton : Button
{
    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<SearchButton>();
    
    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<SearchButton>();
    
    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }
    
    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    protected override string GetThemeResourceKey()
    {
        string? resourceKey = null;
        if (ButtonType == ButtonType.Default)
        {
            resourceKey = DefaultButtonTheme.ID;
        }
        else if (ButtonType == ButtonType.Primary)
        {
            resourceKey = PrimaryButtonTheme.ID;
        }
        else if (ButtonType == ButtonType.Dashed)
        {
            resourceKey = DashedButtonTheme.ID;
        }
        else if (ButtonType == ButtonType.Text)
        {
            resourceKey = SearchButtonTheme.ID;
        }
        else if (ButtonType == ButtonType.Link)
        {
            resourceKey = LinkButtonTheme.ID;
        }

        resourceKey ??= DefaultButtonTheme.ID;
        return resourceKey;
    }
}
using System.Reflection;
using System.Xml.Linq;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Buttons;

public class ButtonIconOnlyVisualContractTests
{
    static ButtonIconOnlyVisualContractTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(typeof(AtomUIButton))]
    [InlineData(typeof(DropdownButton))]
    public void Button_Exposes_Icon_Width_And_Height_Properties(Type controlType)
    {
        const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        var iconWidth = controlType.GetProperty("IconWidth");
        var iconHeight = controlType.GetProperty("IconHeight");
        var iconWidthProperty = controlType.GetField("IconWidthProperty", propertyFlags);
        var iconHeightProperty = controlType.GetField("IconHeightProperty", propertyFlags);

        iconWidth.ShouldNotBeNull();
        iconHeight.ShouldNotBeNull();
        iconWidthProperty.ShouldNotBeNull();
        iconHeightProperty.ShouldNotBeNull();
        iconWidth.PropertyType.ShouldBe(typeof(double));
        iconHeight.PropertyType.ShouldBe(typeof(double));
        iconWidthProperty.FieldType.ShouldBe(typeof(StyledProperty<double>));
        iconHeightProperty.FieldType.ShouldBe(typeof(StyledProperty<double>));
    }

    [Fact]
    public void SplitButton_Does_Not_Expose_Button_Icon_Size_Contract()
    {
        const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        typeof(SplitButton).GetProperty("IconWidth").ShouldBeNull();
        typeof(SplitButton).GetProperty("IconHeight").ShouldBeNull();
        typeof(SplitButton).GetField("IconWidthProperty", propertyFlags).ShouldBeNull();
        typeof(SplitButton).GetField("IconHeightProperty", propertyFlags).ShouldBeNull();
    }

    [Theory]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/ButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonBaseTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/DropdownButtonTheme.axaml")]
    public void Button_Template_Icon_Parts_Bind_Size_To_Control_Properties(string relativePath)
    {
        var document = XDocument.Load(GetRepoFile(relativePath));

        foreach (var partName in new[] { "PART_ButtonIcon", "PART_LoadingIcon" })
        {
            var parts = document.Descendants()
                                .Where(element => element.Attribute("Name")?.Value == partName)
                                .ToArray();

            parts.ShouldNotBeEmpty($"{relativePath} must define {partName}");
            parts.All(part => part.Attribute("Width")?.Value == "{TemplateBinding IconWidth}")
                 .ShouldBeTrue($"every {partName} in {relativePath} must bind IconWidth");
            parts.All(part => part.Attribute("Height")?.Value == "{TemplateBinding IconHeight}")
                 .ShouldBeTrue($"every {partName} in {relativePath} must bind IconHeight");
        }
    }

    [Theory]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/ButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonBaseTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/DropdownButtonTheme.axaml")]
    [InlineData("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml")]
    public void Button_Icon_Dimensions_Are_Not_Set_Through_Template_Selectors(string relativePath)
    {
        var document = XDocument.Load(GetRepoFile(relativePath));
        var violatingSelectors = document.Descendants()
                                           .Where(element => element.Name.LocalName == "Style")
                                           .Where(style =>
                                               style.Attribute("Selector")?.Value.Contains(
                                                   "PART_ButtonIcon",
                                                   StringComparison.Ordinal) == true ||
                                               style.Attribute("Selector")?.Value.Contains(
                                                   "PART_LoadingIcon",
                                                   StringComparison.Ordinal) == true)
                                           .Where(style => style.Elements().Any(setter =>
                                               setter.Name.LocalName == "Setter" &&
                                               setter.Attribute("Property")?.Value is "Width" or "Height"))
                                           .Select(style => style.Attribute("Selector")!.Value)
                                           .ToArray();

        violatingSelectors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Small, SharedTokenKind.IconSizeSM)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Middle, SharedTokenKind.IconSize)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Large, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Custom, SharedTokenKind.IconSize)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Small, SharedTokenKind.IconSizeSM)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Middle, SharedTokenKind.IconSize)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Large, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Custom, SharedTokenKind.IconSize)]
    public void Button_IconOnly_Icons_Preserve_Current_Effective_Size(
        Type buttonType,
        CustomizableSizeType sizeType,
        SharedTokenKind expectedSizeToken)
    {
        var pathIconButton = CreateButton(
            buttonType,
            sizeType,
            new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") });
        var atomIconButton = CreateButton(buttonType, sizeType, new SearchOutlined());
        var host = new StackPanel
        {
            Children =
            {
                pathIconButton,
                atomIconButton
            }
        };

        ShowInWindow(host, () =>
        {
            var expectedSize = GetThemeResource<double>(expectedSizeToken);
            AssertIconSize(pathIconButton, expectedSize);
            AssertIconSize(atomIconButton, expectedSize);
        });
    }

    [Theory]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Small, SharedTokenKind.IconSize)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Middle, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Large, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(AtomUIButton), CustomizableSizeType.Custom, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Small, SharedTokenKind.IconSize)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Middle, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Large, SharedTokenKind.IconSizeLG)]
    [InlineData(typeof(DropdownButton), CustomizableSizeType.Custom, SharedTokenKind.IconSizeLG)]
    public void Button_IconOnly_Loading_Uses_OnlyIconSize_Default(
        Type buttonType,
        CustomizableSizeType sizeType,
        SharedTokenKind expectedSizeToken)
    {
        var button = CreateButton(
            buttonType,
            sizeType,
            new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") });
        button.IsLoading = true;

        ShowInWindow(button, () =>
        {
            var expectedSize = GetThemeResource<double>(expectedSizeToken);
            var loadingIcon = button.GetVisualDescendants()
                                    .OfType<Icon>()
                                    .Single(control => control.Name == "PART_LoadingIcon");

            loadingIcon.Width.ShouldBe(expectedSize);
            loadingIcon.Height.ShouldBe(expectedSize);
        });
    }

    [Theory]
    [InlineData(typeof(AtomUIButton))]
    [InlineData(typeof(DropdownButton))]
    public void Button_Local_Icon_Size_Applies_To_User_And_Loading_Icons(Type buttonType)
    {
        var button = CreateButton(
            buttonType,
            CustomizableSizeType.Custom,
            new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") });

        buttonType.GetProperty("IconWidth")!.SetValue(button, 22d);
        buttonType.GetProperty("IconHeight")!.SetValue(button, 24d);

        ShowInWindow(button, () =>
        {
            var userIcon = button.GetVisualDescendants()
                                 .OfType<IconPresenter>()
                                 .Single(control => control.Name == "PART_ButtonIcon");
            var loadingIcon = button.GetVisualDescendants()
                                    .OfType<Icon>()
                                    .Single(control => control.Name == "PART_LoadingIcon");

            userIcon.Width.ShouldBe(22d);
            userIcon.Height.ShouldBe(24d);

            button.IsLoading = true;
            Dispatcher.UIThread.RunJobs();

            loadingIcon.Width.ShouldBe(22d);
            loadingIcon.Height.ShouldBe(24d);
        });
    }

    [Fact]
    public void DropdownButton_OpenIndicator_Size_Remains_Independent()
    {
        var button = new DropdownButton
        {
            Content         = "Dropdown",
            Icon            = new SearchOutlined(),
            IsMotionEnabled = false
        };

        typeof(DropdownButton).GetProperty("IconWidth")!.SetValue(button, 22d);
        typeof(DropdownButton).GetProperty("IconHeight")!.SetValue(button, 24d);

        ShowInWindow(button, () =>
        {
            var indicator = button.GetVisualDescendants()
                                  .OfType<IconPresenter>()
                                  .Single(control => control.Name == "PART_DropdownIndicator");

            indicator.Width.ShouldBe(12d);
            indicator.Height.ShouldBe(12d);
        });
    }

    private static AtomUIButton CreateButton(
        Type buttonType,
        CustomizableSizeType sizeType,
        PathIcon icon)
    {
        var button = (AtomUIButton)Activator.CreateInstance(buttonType)!;
        button.Icon            = icon;
        button.SizeType        = sizeType;
        button.IsMotionEnabled = false;
        return button;
    }

    private static void AssertIconSize(AtomUIButton button, double expectedSize)
    {
        var presenter = button.GetVisualDescendants()
                              .OfType<IconPresenter>()
                              .Single(control => control.Name == "PART_ButtonIcon");

        presenter.Width.ShouldBe(expectedSize);
        presenter.Height.ShouldBe(expectedSize);
        presenter.Icon.ShouldNotBeNull();
        presenter.Icon!.Width.ShouldBe(expectedSize);
        presenter.Icon.Height.ShouldBe(expectedSize);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}

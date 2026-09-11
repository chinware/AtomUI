using System.Reflection;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Popups;

public class PopupPinnedOpenContractTests
{
    private const string PropertyName = "IsPopupPinnedOpen";
    private const string PropertyFieldName = "IsPopupPinnedOpenProperty";

    private static readonly Assembly DesktopAssembly = typeof(global::AtomUI.Desktop.Controls.Popup).Assembly;
    private static readonly Assembly ColorPickerAssembly =
        typeof(global::AtomUI.Desktop.Controls.AbstractColorPicker).Assembly;
    private static readonly Assembly DataGridAssembly = typeof(global::AtomUI.Desktop.Controls.DataGrid).Assembly;

    public static IEnumerable<object[]> DirectContractOwners()
    {
        foreach (var type in new[]
                 {
                     DesktopType("AtomUI.Desktop.Controls.Popup"),
                     DesktopType("AtomUI.Desktop.Controls.Flyout"),
                     DesktopType("AtomUI.Desktop.Controls.FlyoutHost"),
                     DesktopType("AtomUI.Desktop.Controls.AbstractSelect"),
                     DesktopType("AtomUI.Desktop.Controls.InfoPickerInput"),
                     DesktopType("AtomUI.Desktop.Controls.AbstractAutoComplete"),
                     DesktopType("AtomUI.Desktop.Controls.ComboBox"),
                     ColorPickerType("AtomUI.Desktop.Controls.AbstractColorPicker"),
                     DesktopType("AtomUI.Desktop.Controls.Mentions"),
                     DesktopType("AtomUI.Desktop.Controls.Menu"),
                     DesktopType("AtomUI.Desktop.Controls.MenuItem"),
                     DesktopType("AtomUI.Desktop.Controls.ContextMenu"),
                     DesktopType("AtomUI.Desktop.Controls.NavMenu"),
                     DesktopType("AtomUI.Desktop.Controls.NavMenuItem"),
                     DesktopType("AtomUI.Desktop.Controls.Tour"),
                     DesktopType("AtomUI.Desktop.Controls.TreeView"),
                     DesktopType("AtomUI.Desktop.Controls.AbstractTransfer"),
                     DesktopType("AtomUI.Desktop.Controls.TransferSelectDropdown"),
                     DesktopType("AtomUI.Desktop.Controls.BaseTabControl"),
                     DesktopType("AtomUI.Desktop.Controls.BaseTabStrip"),
                     DesktopType("AtomUI.Desktop.Controls.BaseTabScrollViewer"),
                     DesktopType("AtomUI.Desktop.Controls.DropdownButton"),
                     DesktopType("AtomUI.Desktop.Controls.SplitButton"),
                     DesktopType("AtomUI.Desktop.Controls.AvatarGroup"),
                     DataGridType("AtomUI.Desktop.Controls.DataGrid"),
                     DataGridType("AtomUI.Desktop.Controls.DataGridColumnHeader"),
                     DataGridType("AtomUI.Desktop.Controls.DataGridFilterIndicator")
                 })
        {
            yield return [type];
        }
    }

    public static IEnumerable<object[]> InheritedContractOwners()
    {
        yield return [DesktopType("AtomUI.Desktop.Controls.Select"), DesktopType("AtomUI.Desktop.Controls.AbstractSelect")];
        yield return [DesktopType("AtomUI.Desktop.Controls.TreeSelect"), DesktopType("AtomUI.Desktop.Controls.AbstractSelect")];
        yield return [DesktopType("AtomUI.Desktop.Controls.Cascader"), DesktopType("AtomUI.Desktop.Controls.AbstractSelect")];
        yield return [DesktopType("AtomUI.Desktop.Controls.DatePicker"), DesktopType("AtomUI.Desktop.Controls.InfoPickerInput")];
        yield return [DesktopType("AtomUI.Desktop.Controls.RangeDatePicker"), DesktopType("AtomUI.Desktop.Controls.InfoPickerInput")];
        yield return [DesktopType("AtomUI.Desktop.Controls.TimePicker"), DesktopType("AtomUI.Desktop.Controls.InfoPickerInput")];
        yield return [DesktopType("AtomUI.Desktop.Controls.RangeTimePicker"), DesktopType("AtomUI.Desktop.Controls.InfoPickerInput")];
        yield return [DesktopType("AtomUI.Desktop.Controls.MenuFlyout"), DesktopType("AtomUI.Desktop.Controls.Flyout")];
        yield return [DesktopType("AtomUI.Desktop.Controls.TreeViewFlyout"), DesktopType("AtomUI.Desktop.Controls.Flyout")];
        yield return [DesktopType("AtomUI.Desktop.Controls.FloatableTreeView"), DesktopType("AtomUI.Desktop.Controls.TreeView")];
        yield return [DesktopType("AtomUI.Desktop.Controls.TabControl"), DesktopType("AtomUI.Desktop.Controls.BaseTabControl")];
        yield return [DesktopType("AtomUI.Desktop.Controls.TabStrip"), DesktopType("AtomUI.Desktop.Controls.BaseTabStrip")];
        yield return [DesktopType("AtomUI.Desktop.Controls.PopupConfirm"), DesktopType("AtomUI.Desktop.Controls.FlyoutHost")];
    }

    [Theory]
    [MemberData(nameof(DirectContractOwners))]
    public void Direct_Popup_Pinned_Open_Contracts_Are_Internal(Type ownerType)
    {
        const BindingFlags propertyFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        const BindingFlags fieldFlags =
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        var property = ownerType.GetProperty(PropertyName, propertyFlags);
        property.ShouldNotBeNull($"{ownerType.FullName} must declare the popup pin CLR property.");
        property.PropertyType.ShouldBe(typeof(bool));
        AssertInternal(property.GetMethod.ShouldNotBeNull());
        AssertInternal(property.SetMethod.ShouldNotBeNull());

        var propertyField = ownerType.GetField(PropertyFieldName, fieldFlags);
        propertyField.ShouldNotBeNull($"{ownerType.FullName} must declare the popup pin Avalonia property field.");
        propertyField.IsAssembly.ShouldBeTrue();
        propertyField.IsInitOnly.ShouldBeTrue();
        typeof(AvaloniaProperty).IsAssignableFrom(propertyField.FieldType).ShouldBeTrue();

        ownerType.GetProperty(
            PropertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ShouldBeNull();
        ownerType.GetField(
            PropertyFieldName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(InheritedContractOwners))]
    public void Leaf_Controls_Inherit_The_Contract_From_Their_Semantic_Owner(Type leafType, Type expectedOwnerType)
    {
        var property = FindDeclaredProperty(leafType, PropertyName);
        property.ShouldNotBeNull($"{leafType.FullName} must inherit the popup pin contract.");
        property.DeclaringType.ShouldBe(expectedOwnerType);

        leafType.GetProperty(
            PropertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ShouldBeNull();
        leafType.GetField(
            PropertyFieldName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ShouldBeNull();
    }

    [Fact]
    public void ToolTip_Popup_Pinned_Open_Attached_Contract_Is_Internal()
    {
        var toolTipType = DesktopType("AtomUI.Desktop.Controls.ToolTip");
        var propertyField = toolTipType.GetField(
            PropertyFieldName,
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        propertyField.ShouldNotBeNull();
        propertyField.IsAssembly.ShouldBeTrue();
        propertyField.FieldType.ShouldBe(typeof(AttachedProperty<bool>));

        foreach (var accessorName in new[] { "GetIsPopupPinnedOpen", "SetIsPopupPinnedOpen" })
        {
            var accessor = toolTipType.GetMethod(
                accessorName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            accessor.ShouldNotBeNull();
            AssertInternal(accessor);
            toolTipType.GetMethod(
                accessorName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).ShouldBeNull();
        }
    }

    [Theory]
    [InlineData("AtomUI.Desktop.Controls.Dialog")]
    [InlineData("AtomUI.Desktop.Controls.Drawer")]
    [InlineData("AtomUI.Desktop.Controls.ImagePreviewer")]
    public void Non_Popup_Session_Controls_Do_Not_Expose_The_Contract(string typeName)
    {
        var controlType = DesktopType(typeName);
        FindDeclaredProperty(controlType, PropertyName).ShouldBeNull();
        FindDeclaredField(controlType, PropertyFieldName).ShouldBeNull();
    }

    [Fact]
    public void PopupConfirm_Does_Not_Duplicate_The_FlyoutHost_Registration()
    {
        var popupConfirmType = DesktopType("AtomUI.Desktop.Controls.PopupConfirm");
        popupConfirmType.GetProperty(
            PropertyName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ShouldBeNull();
        popupConfirmType.GetField(
            PropertyFieldName,
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ShouldBeNull();

        FindDeclaredProperty(popupConfirmType, PropertyName)
            .ShouldNotBeNull()
            .DeclaringType.ShouldBe(DesktopType("AtomUI.Desktop.Controls.FlyoutHost"));
    }

    private static void AssertInternal(MethodBase accessor)
    {
        accessor.IsAssembly.ShouldBeTrue();
        accessor.IsPublic.ShouldBeFalse();
        accessor.IsFamily.ShouldBeFalse();
        accessor.IsFamilyOrAssembly.ShouldBeFalse();
    }

    private static PropertyInfo? FindDeclaredProperty(Type type, string propertyName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var property = current.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (property is not null)
            {
                return property;
            }
        }

        return null;
    }

    private static FieldInfo? FindDeclaredField(Type type, string fieldName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var field = current.GetField(
                fieldName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field is not null)
            {
                return field;
            }
        }

        return null;
    }

    private static Type DesktopType(string typeName) => DesktopAssembly.GetType(typeName, throwOnError: true)!;

    private static Type ColorPickerType(string typeName) =>
        ColorPickerAssembly.GetType(typeName, throwOnError: true)!;

    private static Type DataGridType(string typeName) => DataGridAssembly.GetType(typeName, throwOnError: true)!;
}

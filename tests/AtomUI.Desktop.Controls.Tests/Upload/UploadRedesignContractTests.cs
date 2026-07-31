using System.Reflection;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadRedesignContractTests
{
    static UploadRedesignContractTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Public_Redesign_Properties_Are_Registered_With_Expected_Metadata()
    {
        var upload = new Desktop.Controls.Upload();

        var filesProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "FilesProperty");
        filesProperty.PropertyType.ShouldBe(typeof(IList<>).MakeGenericType(GetUploadType("UploadFileItem")));
        filesProperty.GetMetadata(typeof(Desktop.Controls.Upload)).DefaultBindingMode.ShouldBe(BindingMode.TwoWay);

        var listMaxHeightProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "ListMaxHeightProperty");
        listMaxHeightProperty.PropertyType.ShouldBe(typeof(double));
        GetPropertyValue(upload, "ListMaxHeight").ShouldBe(double.PositiveInfinity);

        var listScrollBarVisibilityProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "ListScrollBarVisibilityProperty");
        listScrollBarVisibilityProperty.PropertyType.ShouldBe(typeof(ScrollBarVisibility));
        GetPropertyValue(upload, "ListScrollBarVisibility").ShouldBe(ScrollBarVisibility.Disabled);

        var successAutoRemoveDelayProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "SuccessAutoRemoveDelayProperty");
        successAutoRemoveDelayProperty.PropertyType.ShouldBe(typeof(TimeSpan?));
        GetPropertyValue(upload, "SuccessAutoRemoveDelay").ShouldBeNull();

        var pendingTextProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "PendingTextProperty");
        pendingTextProperty.PropertyType.ShouldBe(typeof(string));
        GetPropertyValue(upload, "PendingText").ShouldBeNull();

        var fileValueModeType = GetUploadType("UploadFileValueMode");
        var fileValueModeProperty = GetAvaloniaProperty(typeof(Desktop.Controls.Upload), "FileValueModeProperty");
        fileValueModeProperty.PropertyType.ShouldBe(fileValueModeType);
        GetPropertyValue(upload, "FileValueMode").ShouldBe(Enum.Parse(fileValueModeType, "SuccessfulFiles"));
    }

    [Fact]
    public void UploadTrigger_SourceKind_Is_A_Public_Composable_Trigger_Contract()
    {
        var uploadTriggerType = GetUploadType("UploadTrigger");
        uploadTriggerType.IsPublic.ShouldBeTrue();
        var uploadTrigger = Activator.CreateInstance(uploadTriggerType);
        uploadTrigger.ShouldNotBeNull();

        var sourceKindType = GetUploadType("UploadSourceKind");
        var sourceKindProperty = GetAvaloniaProperty(uploadTriggerType, "SourceKindProperty");
        sourceKindProperty.PropertyType.ShouldBe(sourceKindType);
        GetPropertyValue(uploadTrigger, "SourceKind").ShouldBe(Enum.Parse(sourceKindType, "Files"));
    }

    [Fact]
    public void Legacy_Task_State_And_Fake_Trigger_Contracts_Are_Removed()
    {
        var uploadSource = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Upload.cs");
        var uploadTheme  = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml");

        uploadSource.ShouldNotContain("TaskInfoList");
        uploadSource.ShouldNotContain("DefaultTaskList");
        uploadSource.ShouldNotContain("CurrentTaskList");
        uploadSource.ShouldNotContain("IsUploadDirectoryEnabled");
        uploadSource.ShouldNotContain("IsShowUploadTrigger");
        uploadSource.ShouldNotContain("IsPictureTriggerTask");
        uploadTheme.ShouldNotContain("CurrentTaskList");
        uploadTheme.ShouldNotContain("IsShowUploadTrigger");
        uploadTheme.ShouldNotContain("UploadTriggerContent");
    }

    [Fact]
    public void Upload_Template_Binds_List_To_Effective_File_Source()
    {
        var uploadTheme = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml");

        uploadTheme.ShouldContain("ItemsSource=\"{TemplateBinding EffectiveFiles}\"");
        uploadTheme.ShouldNotContain("ItemsSource=\"{TemplateBinding Files}\"");
    }

    [Fact]
    public void Picture_Shape_Template_Binds_List_To_Display_Source_With_Append_Slot()
    {
        var uploadSource = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Upload.cs");
        var uploadTheme  = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml");
        var listSource   = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/PictureShapeList/UploadPictureShapeList.cs");

        uploadSource.ShouldContain("EffectivePictureItems");
        uploadSource.ShouldContain("UploadAppendContentItem");
        uploadSource.ShouldContain("SyncEffectivePictureItems()");
        uploadTheme.ShouldContain("ItemsSource=\"{TemplateBinding EffectivePictureItems}\"");
        uploadTheme.ShouldNotContain("Name=\"PART_AppendContent\"");
        listSource.ShouldContain("UploadAppendContentItem");
        listSource.ShouldContain("NeedsContainerOverride");
    }

    [Fact]
    public void Picture_Shape_List_Does_Not_Own_Trigger_Content_State()
    {
        var listSource = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/PictureShapeList/UploadPictureShapeList.cs");
        var uploadTheme = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml");

        listSource.ShouldNotContain("TriggerContentProperty");
        listSource.ShouldNotContain("TriggerContentTemplateProperty");
        uploadTheme.ShouldNotContain("TriggerContent=\"{TemplateBinding TriggerContent}\"");
        uploadTheme.ShouldNotContain("TriggerContentTemplate=\"{TemplateBinding TriggerContentTemplate}\"");
        uploadTheme.ShouldContain("EffectivePictureItems");
    }

    private static AvaloniaProperty GetAvaloniaProperty(Type ownerType, string fieldName)
    {
        var field = ownerType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        field.ShouldNotBeNull($"{ownerType.Name}.{fieldName} should be part of the Upload redesign contract.");
        var value = field.GetValue(null);
        value.ShouldBeAssignableTo<AvaloniaProperty>();
        return (AvaloniaProperty)value!;
    }

    private static Type GetUploadType(string typeName)
    {
        var type = typeof(Desktop.Controls.Upload).Assembly.GetType($"AtomUI.Desktop.Controls.{typeName}");
        type.ShouldNotBeNull($"AtomUI.Desktop.Controls.{typeName} should exist.");
        return type!;
    }

    private static object? GetPropertyValue(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.ShouldNotBeNull($"{propertyName} should be part of the Upload redesign public contract.");
        return property.GetValue(target);
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Unable to locate repository file '{relativePath}'.");
    }
}

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using AtomUI.Controls;
using Avalonia.Interactivity;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadFileStateTests
{
    static UploadFileStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task EnqueueFilesAsync_Appends_To_Bound_Files_Collection()
    {
        var upload = CreateUploadWithBoundFiles(out var files);

        await EnqueueFilesAsync(upload, CreateUploadFile("first.txt"));

        files.Count.ShouldBe(1);
        GetPropertyValue(files[0]!, "Name").ShouldBe("first.txt");
    }

    [Fact]
    public async Task RemoveFileAsync_Removes_Exactly_One_File_By_Id()
    {
        var upload = CreateUploadWithBoundFiles(out var files);
        await EnqueueFilesAsync(upload, CreateUploadFile("first.txt"), CreateUploadFile("second.txt"));
        var firstId = GetPropertyValue<Guid>(files[0]!, "Id");

        await InvokeUploadTaskAsync(upload, "RemoveFileAsync", firstId, CancellationToken.None);

        files.Count.ShouldBe(1);
        GetPropertyValue(files[0]!, "Name").ShouldBe("second.txt");
    }

    [Fact]
    public async Task TaskRemoveRequest_Removes_File_From_Owning_Upload()
    {
        var upload = CreateUploadWithBoundFiles(out var files);
        await EnqueueFilesAsync(upload, CreateUploadFile("first.txt"));
        var fileId = GetPropertyValue<Guid>(files[0]!, "Id");

        upload.RaiseEvent(new TaskRemoveRequestEventArgs(fileId)
        {
            RoutedEvent = AbstractUploadListItem.TaskRemoveRequestEvent,
            Source      = upload
        });
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();

        files.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Removing_Picture_Item_Does_Not_Reset_Display_Source()
    {
        var upload = CreateUploadWithBoundFiles(out var files);
        SetPropertyValue(upload, "ListType", UploadListType.PictureCard);
        await EnqueueFilesAsync(upload, CreateUploadFile("first.png"), CreateUploadFile("second.png"), CreateUploadFile("third.png"));
        var displaySource = upload.EffectivePictureItems;
        var actions       = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)displaySource).CollectionChanged += (_, args) => actions.Add(args.Action);
        var thirdId = GetPropertyValue<Guid>(files[2]!, "Id");

        await InvokeUploadTaskAsync(upload, "RemoveFileAsync", thirdId, CancellationToken.None);

        actions.ShouldNotContain(NotifyCollectionChangedAction.Reset);
        actions.ShouldContain(NotifyCollectionChangedAction.Remove);
        displaySource[0].ShouldBeSameAs(files[0]);
        displaySource[1].ShouldBeSameAs(files[1]);
    }

    [Fact]
    public async Task Removing_Observable_Picture_Item_Does_Not_Reset_Display_Source()
    {
        var upload = new Desktop.Controls.Upload
        {
            AutoUpload = false,
            ListType   = UploadListType.PictureCard,
            Files      = new ObservableCollection<UploadFileItem>()
        };
        await upload.EnqueueFilesAsync(
            [
                CreateUploadFile("first.png"),
                CreateUploadFile("second.png"),
                CreateUploadFile("third.png")
            ],
            TestContext.Current.CancellationToken);
        var files         = upload.Files!;
        var displaySource = upload.EffectivePictureItems;
        var actions       = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)displaySource).CollectionChanged += (_, args) => actions.Add(args.Action);

        await upload.RemoveFileAsync(files[2].Id, TestContext.Current.CancellationToken);

        actions.ShouldNotContain(NotifyCollectionChangedAction.Reset);
        actions.ShouldContain(NotifyCollectionChangedAction.Remove);
        displaySource[0].ShouldBeSameAs(files[0]);
        displaySource[1].ShouldBeSameAs(files[1]);
    }

    [Fact]
    public async Task ResetAsync_Clears_Files_Without_Replacing_Bound_Collection()
    {
        var upload = CreateUploadWithBoundFiles(out var files);
        await EnqueueFilesAsync(upload, CreateUploadFile("first.txt"));

        await InvokeUploadTaskAsync(upload, "ResetAsync", CancellationToken.None);

        GetPropertyValue(upload, "Files").ShouldBeSameAs(files);
        files.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Replacing_Files_Uses_New_Collection_Without_Mutating_Old_Collection()
    {
        var upload   = CreateUploadWithBoundFiles(out var oldFiles);
        var newFiles = CreateUploadFileCollection();

        await EnqueueFilesAsync(upload, CreateUploadFile("old.txt"));
        SetPropertyValue(upload, "Files", newFiles);
        await EnqueueFilesAsync(upload, CreateUploadFile("new.txt"));

        oldFiles.Count.ShouldBe(1);
        newFiles.Count.ShouldBe(1);
        GetPropertyValue(newFiles[0]!, "Name").ShouldBe("new.txt");
    }

    private static Desktop.Controls.Upload CreateUploadWithBoundFiles(out IList files)
    {
        var upload = new Desktop.Controls.Upload();
        files = CreateUploadFileCollection();
        SetPropertyValue(upload, "Files", files);
        SetPropertyValueIfExists(upload, "AutoUpload", false);
        return upload;
    }

    private static IList CreateUploadFileCollection()
    {
        var itemType       = GetUploadType("UploadFileItem");
        var collectionType = typeof(List<>).MakeGenericType(itemType);
        var collection = Activator.CreateInstance(collectionType);
        collection.ShouldBeAssignableTo<IList>();
        return (IList)collection!;
    }

    private static async Task EnqueueFilesAsync(Desktop.Controls.Upload upload, params UploadFileInfo[] files)
    {
        await InvokeUploadTaskAsync(upload, "EnqueueFilesAsync", files, CancellationToken.None);
    }

    private static async Task InvokeUploadTaskAsync(object target, string methodName, params object?[] args)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
        method.ShouldNotBeNull($"{methodName} should be part of the Upload redesign public contract.");
        var result = method.Invoke(target, args);
        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    private static UploadFileInfo CreateUploadFile(string name)
    {
        return new UploadFileInfo(name, new Uri($"file:///tmp/{name}"), 12);
    }

    private static Type GetUploadType(string typeName)
    {
        var type = typeof(Desktop.Controls.Upload).Assembly.GetType($"AtomUI.Desktop.Controls.{typeName}");
        type.ShouldNotBeNull($"AtomUI.Desktop.Controls.{typeName} should exist.");
        return type!;
    }

    private static void SetPropertyValue(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.ShouldNotBeNull($"{propertyName} should be part of the Upload redesign public contract.");
        property.SetValue(target, value);
    }

    private static void SetPropertyValueIfExists(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(target, value);
    }

    private static object? GetPropertyValue(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.ShouldNotBeNull($"{propertyName} should be part of the Upload redesign public contract.");
        return property!.GetValue(target);
    }

    private static TValue GetPropertyValue<TValue>(object target, string propertyName)
    {
        return Assert.IsAssignableFrom<TValue>(GetPropertyValue(target, propertyName));
    }
}

using System.Xml.Linq;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomIconPresenter = AtomUI.Controls.IconPresenter;
using AtomUIMessageCard = AtomUI.Desktop.Controls.MessageCard;
using AtomUIMessageManager = AtomUI.Desktop.Controls.WindowMessageManager;
using AtomUIMessageType = AtomUI.Desktop.Controls.MessageType;
using AvaloniaSelectableTextBlock = Avalonia.Controls.SelectableTextBlock;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Feedback;

public class MessageSemanticPartTests
{
    private const string MessageCardThemePath =
        "src/AtomUI.Desktop.Controls/Message/Themes/MessageCardTheme.axaml";

    private const string MessageManagerThemePath =
        "src/AtomUI.Desktop.Controls/Message/Themes/WindowMessageManagerTheme.axaml";

    // manifest 顺序：隐式 root 在前，其余按 path 字典序。
    private static readonly string[] ApprovedCardPartNames = ["root", "icon", "title", "wrapper"];
    private static readonly string[] ApprovedManagerPartNames = ["root", "listContent"];

    static MessageSemanticPartTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Registered_Descriptors_Expose_Only_The_Approved_Message_Parts()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;

        registry.TryGetControl(typeof(AtomUIMessageCard), out var cardDescriptor).ShouldBeTrue();
        cardDescriptor.ShouldNotBeNull();
        cardDescriptor.Parts.Select(static part => part.Name).ShouldBe(ApprovedCardPartNames);
        AssertRoot(cardDescriptor, typeof(AtomUIMessageCard));
        AssertPart(cardDescriptor, "wrapper", "semantic-wrapper", typeof(DockPanel));
        AssertPart(cardDescriptor, "icon", "semantic-icon", typeof(AtomIconPresenter));
        AssertPart(cardDescriptor, "title", "semantic-title", typeof(AvaloniaSelectableTextBlock));

        registry.TryGetControl(typeof(AtomUIMessageManager), out var managerDescriptor).ShouldBeTrue();
        managerDescriptor.ShouldNotBeNull();
        managerDescriptor.Parts.Select(static part => part.Name).ShouldBe(ApprovedManagerPartNames);
        AssertRoot(managerDescriptor, typeof(AtomUIMessageManager));
        AssertPart(managerDescriptor, "listContent", "semantic-list-content", typeof(ReversibleStackPanel));
    }

    [Fact]
    public void Built_In_Themes_Carry_The_Approved_Static_Markers()
    {
        var cardDocument = XDocument.Load(GetRepoFile(MessageCardThemePath), LoadOptions.SetLineInfo);
        CollectMarkers(cardDocument).ShouldBe(
        [
            "semantic-icon:IconPresenter",
            "semantic-title:SelectableTextBlock",
            "semantic-wrapper:DockPanel"
        ]);
        cardDocument.Descendants().Any(static element => HasMarker(element, "semantic-root")).ShouldBeFalse();

        var managerDocument = XDocument.Load(GetRepoFile(MessageManagerThemePath), LoadOptions.SetLineInfo);
        CollectMarkers(managerDocument).ShouldBe(["semantic-list-content:ReversibleStackPanel"]);
        managerDocument.Descendants().Any(static element => HasMarker(element, "semantic-root")).ShouldBeFalse();
    }

    [Fact]
    public void Built_In_Themes_Do_Not_Consume_Semantic_Selectors()
    {
        foreach (var path in new[] { MessageCardThemePath, MessageManagerThemePath })
        {
            var document = XDocument.Load(GetRepoFile(path), LoadOptions.SetLineInfo);
            document.Descendants()
                    .Where(static element => element.Name.LocalName == "Style")
                    .Select(static element => (string?)element.Attribute("Selector"))
                    .Where(static selector => selector?.Contains(".semantic-", StringComparison.Ordinal) == true)
                    .ShouldBeEmpty(path);
        }
    }

    [Fact]
    public void MessageCard_Template_Exposes_Markers_And_Root_Surface_Projection()
    {
        var card = CreateCard(AtomUIMessageType.Success, "Deployment completed");
        var window = ShowInWindow(card);
        try
        {
            FindSemanticControl<DockPanel>(card, "semantic-wrapper").Name.ShouldBe("PART_HeaderContainer");
            FindSemanticControl<AtomIconPresenter>(card, "semantic-icon").Name.ShouldBe("PART_IconContent");
            FindSemanticControl<AvaloniaSelectableTextBlock>(card, "semantic-title").Name.ShouldBe("PART_Message");

            // root 表面投影到 Border#PART_Frame：背景 / 圆角 / 内边距来自 owner 属性与 Token。
            var frame = card.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "PART_Frame");
            frame.Background.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void WindowMessageManager_Template_Exposes_The_ListContent_Marker()
    {
        var manager = CreateManager();
        var window = ShowInWindow(manager);
        try
        {
            var listContent = FindSemanticControl<ReversibleStackPanel>(manager, "semantic-list-content");
            listContent.Name.ShouldBe("PART_Items");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Parameterless_Manager_Renders_Inline_Without_Taking_Over_The_Host_Layer()
    {
        // 无参构造是可声明式（XAML）使用的入口：不安装到 TopLevel 反馈层，而是作为普通子控件
        // 渲染在调用方给定的位置。对齐上游把消息列表改为内联容器的用法。
        var manager = new AtomUIMessageManager { IsMotionEnabled = false };
        var window  = ShowInWindow(manager);
        try
        {
            // 未被反馈层收养：父链仍是窗口内容，而不是 AdornerLayer 之类的宿主层。
            manager.GetVisualAncestors().ShouldNotContain(static ancestor => ancestor is AdornerLayer);

            var listContent = FindSemanticControl<ReversibleStackPanel>(manager, "semantic-list-content");
            listContent.Name.ShouldBe("PART_Items");

            manager.Show(new AtomUI.Desktop.Controls.Message("Inline", expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();
            listContent.Children.Count.ShouldBe(1);
            manager.GetVisualDescendants().OfType<AtomUIMessageCard>().Count().ShouldBe(1);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void MessageType_Changes_Preserve_Static_Marker_Identity()
    {
        var card = CreateCard(AtomUIMessageType.Information, "Info text");
        var window = ShowInWindow(card);
        try
        {
            var wrapper = FindSemanticControl<DockPanel>(card, "semantic-wrapper");
            var icon    = FindSemanticControl<AtomIconPresenter>(card, "semantic-icon");
            var title   = FindSemanticControl<AvaloniaSelectableTextBlock>(card, "semantic-title");

            foreach (var type in Enum.GetValues<AtomUIMessageType>())
            {
                card.MessageType = type;
                Dispatcher.UIThread.RunJobs();

                FindSemanticControl<DockPanel>(card, "semantic-wrapper").ShouldBeSameAs(wrapper);
                FindSemanticControl<AtomIconPresenter>(card, "semantic-icon").ShouldBeSameAs(icon);
                FindSemanticControl<AvaloniaSelectableTextBlock>(card, "semantic-title").ShouldBeSameAs(title);
                icon.IsVisible.ShouldBeTrue();
            }
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Generated_Semantic_Styles_Apply_To_The_Card_Targets()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIMessageCard), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();

        var card = CreateCard(AtomUIMessageType.Success, "Styled text");
        card.Classes.Add("semantic-owner");
        var ownerStyle = new Style(selector => selector.OfType<AtomUIMessageCard>().Class("semantic-owner"));
        foreach (var part in descriptor.Parts.Where(static part => part.StyleType != null))
        {
            var partStyle = (Style)Activator.CreateInstance(part.StyleType.ShouldNotBeNull()).ShouldNotBeNull();
            partStyle.Setters.Add(new Setter(Control.TagProperty, part.Name));
            ownerStyle.Children.Add(partStyle);
        }
        card.Styles.Add(ownerStyle);

        var window = ShowInWindow(card);
        try
        {
            FindSemanticControl<DockPanel>(card, "semantic-wrapper").Tag.ShouldBe("wrapper");
            FindSemanticControl<AtomIconPresenter>(card, "semantic-icon").Tag.ShouldBe("icon");
            FindSemanticControl<AvaloniaSelectableTextBlock>(card, "semantic-title").Tag.ShouldBe("title");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Generated_Semantic_Styles_Apply_To_The_Manager_ListContent_Target()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIMessageManager), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();

        var manager = CreateManager();
        manager.Classes.Add("semantic-owner");
        var ownerStyle = new Style(selector => selector.OfType<AtomUIMessageManager>().Class("semantic-owner"));
        foreach (var part in descriptor.Parts.Where(static part => part.StyleType != null))
        {
            var partStyle = (Style)Activator.CreateInstance(part.StyleType.ShouldNotBeNull()).ShouldNotBeNull();
            partStyle.Setters.Add(new Setter(Control.TagProperty, part.Name));
            ownerStyle.Children.Add(partStyle);
        }
        manager.Styles.Add(ownerStyle);

        var window = ShowInWindow(manager);
        try
        {
            FindSemanticControl<ReversibleStackPanel>(manager, "semantic-list-content").Tag
                .ShouldBe("listContent");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Manager_Shows_Cards_With_Markers_And_Removes_Them_On_Close()
    {
        var manager = CreateManager();
        manager.MaxItems = 3;
        var window = ShowInWindow(manager);
        try
        {
            manager.Show(new AtomUI.Desktop.Controls.Message("First", expiration: TimeSpan.Zero));
            manager.Show(new AtomUI.Desktop.Controls.Message("Second", expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();

            var cards = manager.GetVisualDescendants().OfType<AtomUIMessageCard>().ToArray();
            cards.Length.ShouldBe(2);
            // marker 静态声明在卡片模板节点上，不注入到卡片实例本身。
            cards.ShouldAllBe(static card => !card.Classes.Contains("semantic-wrapper") &&
                                             !card.Classes.Contains("semantic-icon") &&
                                             !card.Classes.Contains("semantic-title"));

            foreach (var card in cards)
            {
                FindSemanticControl<DockPanel>(card, "semantic-wrapper").ShouldNotBeNull();
                FindSemanticControl<AtomIconPresenter>(card, "semantic-icon").ShouldNotBeNull();
                FindSemanticControl<AvaloniaSelectableTextBlock>(card, "semantic-title").ShouldNotBeNull();
            }

            var listContent = FindSemanticControl<ReversibleStackPanel>(manager, "semantic-list-content");
            listContent.Children.Count.ShouldBe(2);

            cards[0].Close();
            Dispatcher.UIThread.RunJobs();
            listContent.Children.Count.ShouldBe(1);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Host_Detach_Clears_Cards_And_Releases_The_Host_Layer()
    {
        var manager = CreateManager();
        var window = ShowInWindow(manager);
        try
        {
            manager.Show(new AtomUI.Desktop.Controls.Message("Transient", expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();
            window.GetVisualDescendants().OfType<AtomUIMessageCard>().Count().ShouldBe(1);
        }
        finally
        {
            window.Close();
        }

        Dispatcher.UIThread.RunJobs();
        manager.GetVisualDescendants().OfType<AtomUIMessageCard>().ShouldBeEmpty();
    }

    private static AtomUIMessageCard CreateCard(AtomUIMessageType type, string message)
    {
        return new AtomUIMessageCard
        {
            MessageType       = type,
            Message           = message,
            IsMotionEnabled   = false
        };
    }

    private static AtomUIMessageManager CreateManager()
    {
        // 以 null 宿主构造：不安装到 TopLevel 反馈层，作为普通子控件参与预览与测试。
        return new AtomUIMessageManager(null) { IsMotionEnabled = false };
    }

    private static AvaloniaWindow ShowInWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 320,
            Content = content
        };
        window.Show();
        content.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void AssertRoot(ControlSemanticDescriptor descriptor, Type controlType)
    {
        var root = descriptor.Parts.Single(static part => part.Name == "root");
        root.Path.ShouldBe("root");
        root.SelectorClass.ShouldBeNull();
        root.SelectorRoute.ShouldBeNull();
        root.ContractType.ShouldBe(controlType);
        root.Cardinality.ShouldBe(SemanticPartCardinality.Single);
        root.Customization.ShouldBe(SemanticPartCustomization.Root);
        root.StyleType.ShouldBeNull();
        root.CrossVisualRoot.ShouldBeFalse();
        root.RuntimeCreated.ShouldBeFalse();
        root.Since.ShouldBe("6.0");
    }

    private static void AssertPart(
        ControlSemanticDescriptor descriptor,
        string name,
        string selectorClass,
        Type contractType,
        SemanticPartCardinality cardinality = SemanticPartCardinality.Single)
    {
        var part = descriptor.Parts.Single(candidate => candidate.Name == name);
        part.Path.ShouldBe(name);
        part.SelectorClass.ShouldBe(selectorClass);
        part.SelectorRoute.ShouldBe($"/template/ .{selectorClass}");
        part.ContractType.ShouldBe(contractType);
        part.Cardinality.ShouldBe(cardinality);
        part.Customization.ShouldBe(SemanticPartCustomization.Selector);
        part.CrossVisualRoot.ShouldBeFalse();
        part.RuntimeCreated.ShouldBeFalse();
        part.Since.ShouldBe("6.0");
        part.StyleType.ShouldNotBeNull();
        part.StyleType.Name.ShouldBe($"{descriptor.ControlType.Name}{Pascal(name)}Style");
    }

    private static string Pascal(string partName)
    {
        return char.ToUpperInvariant(partName[0]) + partName[1..];
    }

    private static T FindSemanticControl<T>(Control owner, string marker)
        where T : Control
    {
        return owner.GetVisualDescendants()
                    .OfType<T>()
                    .Single(control => control.Classes.Contains(marker));
    }

    private static string[] CollectMarkers(XDocument document)
    {
        return document.Descendants()
                       .SelectMany(static element => element.Attributes()
                           .Where(static attribute => attribute.Name.LocalName.StartsWith(
                               "Classes.semantic-", StringComparison.Ordinal))
                           .Select(attribute =>
                               $"{attribute.Name.LocalName["Classes.".Length..]}:{attribute.Parent!.Name.LocalName}"))
                       .OrderBy(static marker => marker, StringComparer.Ordinal)
                       .ToArray();
    }

    private static bool HasMarker(XElement element, string marker)
    {
        var classProperty = element.Attribute($"Classes.{marker}");
        return classProperty is not null &&
               bool.TryParse(classProperty.Value, out var isEnabled) &&
               isEnabled;
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

        throw new FileNotFoundException($"Could not locate repository file '{relativePath}'.");
    }
}

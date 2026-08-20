using AtomUI.Theme;
using AtomUI.Theme.Schema;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class SemanticPartTargetResolverTests
{
    public SemanticPartTargetResolverTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Resolves_Button_Root_Icon_And_Content_Within_The_Owner_Template()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIButton), out var descriptor).ShouldBeTrue();
        var button = new AtomUIButton
        {
            Content   = "Semantic Button",
            IsLoading = true
        };

        ShowInWindow(button, () =>
        {
            var root = Resolve(button, descriptor, "root", registry);
            root.TotalMatchCount.ShouldBe(1);
            root.Targets.ShouldBe([button]);

            var icon = Resolve(button, descriptor, "icon", registry);
            icon.TotalMatchCount.ShouldBe(1);
            icon.Targets.Single().Classes.ShouldContain("semantic-icon");
            icon.Targets.Single().TemplatedParent.ShouldBe(button);

            var content = Resolve(button, descriptor, "content", registry);
            content.TotalMatchCount.ShouldBe(1);
            content.Targets.Single().ShouldBeOfType<ContentPresenter>();
            content.Targets.Single().TemplatedParent.ShouldBe(button);
        });
    }

    [Fact]
    public void Static_Resolution_Does_Not_Enter_A_Nested_Semantic_Control_Template()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIButton), out var descriptor).ShouldBeTrue();
        var nestedButton = new AtomUIButton
        {
            Content   = "Nested",
            IsLoading = true
        };
        var owner = new AtomUIButton
        {
            Content   = nestedButton,
            IsLoading = true
        };

        ShowInWindow(owner, () =>
        {
            var result = Resolve(owner, descriptor, "icon", registry);

            result.TotalMatchCount.ShouldBe(1);
            result.Targets.Single().TemplatedParent.ShouldBe(owner);
        });
    }

    [Fact]
    public void Runtime_Created_Resolution_Stops_Before_Another_Registered_Owner()
    {
        var directTarget = Marker();
        var nestedTarget = Marker();
        var nestedOwner = new StackPanel
        {
            Width    = 80,
            Height   = 40,
            Children = { nestedTarget }
        };
        var owner = new Grid
        {
            Width    = 180,
            Height   = 100,
            Children = { directTarget, nestedOwner }
        };
        var ownerDescriptor = RuntimeDescriptor(typeof(Grid), "RuntimeGrid");
        var registry = new SemanticPartRegistry([
            ownerDescriptor,
            RootOnlyDescriptor(typeof(StackPanel), "NestedStackPanel")
        ]);

        ShowInWindow(owner, () =>
        {
            var result = Resolve(owner, ownerDescriptor, "item", registry);

            result.TotalMatchCount.ShouldBe(1);
            result.Targets.ShouldBe([directTarget]);
        });
    }

    [Fact]
    public void Runtime_Created_Resolution_Matches_A_Nested_Registered_Owner_Carrying_The_Part_Marker()
    {
        var nestedOwner = new StackPanel
        {
            Width  = 80,
            Height = 40
        };
        nestedOwner.Classes.Add("semantic-item");
        var owner = new Grid
        {
            Width    = 180,
            Height   = 100,
            Children = { nestedOwner }
        };
        var ownerDescriptor = RuntimeDescriptor(typeof(Grid), "RuntimeGrid");
        var registry = new SemanticPartRegistry([
            ownerDescriptor,
            RootOnlyDescriptor(typeof(StackPanel), "NestedStackPanel")
        ]);

        ShowInWindow(owner, () =>
        {
            var result = Resolve(owner, ownerDescriptor, "item", registry);

            result.TotalMatchCount.ShouldBe(1);
            result.Targets.ShouldBe([nestedOwner]);
        });
    }

    [Fact]
    public void Runtime_Created_Resolution_Matches_The_Owner_When_It_Carries_The_Part_Marker()
    {
        var owner = new Grid
        {
            Width  = 180,
            Height = 100
        };
        owner.Classes.Add("semantic-item");
        var ownerDescriptor = RuntimeDescriptor(typeof(Grid), "RuntimeGrid");
        var registry = new SemanticPartRegistry([ownerDescriptor]);

        ShowInWindow(owner, () =>
        {
            var result = Resolve(owner, ownerDescriptor, "item", registry);

            result.TotalMatchCount.ShouldBe(1);
            result.Targets.ShouldBe([owner]);
        });
    }

    [Fact]
    public void Runtime_Created_Resolution_Matches_The_Owner_And_Nested_Owner_When_Both_Carry_The_Marker()
    {
        var nestedOwner = new StackPanel
        {
            Width  = 80,
            Height = 40
        };
        nestedOwner.Classes.Add("semantic-item");
        var owner = new Grid
        {
            Width    = 180,
            Height   = 100,
            Children = { nestedOwner }
        };
        owner.Classes.Add("semantic-item");
        var ownerDescriptor = RuntimeDescriptor(typeof(Grid), "RuntimeGrid");
        var registry = new SemanticPartRegistry([
            ownerDescriptor,
            RootOnlyDescriptor(typeof(StackPanel), "NestedStackPanel")
        ]);

        ShowInWindow(owner, () =>
        {
            var result = Resolve(owner, ownerDescriptor, "item", registry);

            result.TotalMatchCount.ShouldBe(2);
            result.Targets.ShouldBe([owner, nestedOwner]);
        });
    }

    [Fact]
    public void Resolution_Reports_All_Visible_Matches_But_Returns_Only_The_Budget()
    {
        var owner = new Grid
        {
            Width  = 400,
            Height = 400
        };
        for (var index = 0; index < 40; index++)
        {
            owner.Children.Add(Marker());
        }

        var descriptor = RuntimeDescriptor(typeof(Grid), "BudgetGrid");
        var registry = new SemanticPartRegistry([descriptor]);

        ShowInWindow(owner, () =>
        {
            var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");
            var result = SemanticPartTargetResolver.Resolve(owner, part, registry, targetBudget: 32);

            result.TotalMatchCount.ShouldBe(40);
            result.Targets.Count.ShouldBe(32);
            result.IsTruncated.ShouldBeTrue();
        });
    }

    private static SemanticPartTargetResolution Resolve(
        Control owner,
        ControlSemanticDescriptor descriptor,
        string path,
        SemanticPartRegistry registry)
    {
        var part = descriptor.Parts.Single(candidate => candidate.Path == path);
        return SemanticPartTargetResolver.Resolve(owner, part, registry);
    }

    private static Border Marker()
    {
        var target = new Border
        {
            Width  = 20,
            Height = 20
        };
        target.Classes.Add("semantic-item");
        return target;
    }

    private static ControlSemanticDescriptor RuntimeDescriptor(Type controlType, string id)
    {
        return new ControlSemanticDescriptor(
            controlType,
            new ControlTokenIdentity("GalleryTests", id),
            [
                Root(controlType),
                new SemanticPartDescriptor(
                    "item",
                    "item",
                    "semantic-item",
                    typeof(Control),
                    SemanticPartCardinality.Multiple,
                    SemanticPartCustomization.Selector,
                    null,
                    false,
                    null,
                    true,
                    "> .semantic-item")
            ]);
    }

    private static ControlSemanticDescriptor RootOnlyDescriptor(Type controlType, string id)
    {
        return new ControlSemanticDescriptor(
            controlType,
            new ControlTokenIdentity("GalleryTests", id),
            [Root(controlType)]);
    }

    private static SemanticPartDescriptor Root(Type controlType)
    {
        return new SemanticPartDescriptor(
            "root",
            "root",
            null,
            controlType,
            SemanticPartCardinality.Single,
            SemanticPartCustomization.Root,
            null,
            false,
            null,
            false);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Window
        {
            Width   = 500,
            Height  = 500,
            Content = content
        };

        try
        {
            window.Show();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}

using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Controls.Primitives;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.SelectControl;

public class SelectBehaviorTests
{
    static SelectBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void OptionsSource_Replacement_Preserves_Single_Selection_By_Content()
    {
        var firstLucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var select = new Desktop.Controls.Select
        {
            OptionsSource  = [new SelectOption { Header = "Jack", Content = "jack" }, firstLucy],
            SelectedOption = firstLucy
        };
        var secondLucy = new SelectOption
        {
            Header  = "Lucy Updated",
            Content = "lucy"
        };

        select.OptionsSource =
        [
            new SelectOption { Header = "Jack Updated", Content = "jack" },
            secondLucy
        ];

        select.SelectedOption.ShouldBeSameAs(secondLucy);
    }

    [Fact]
    public void DefaultValues_Selects_Single_Option_On_Load()
    {
        var lucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var select = new Desktop.Controls.Select
        {
            OptionsSource =
            [
                new SelectOption { Header = "Jack", Content = "jack" },
                lucy
            ],
            DefaultValues = ["lucy"]
        };

        ShowInWindow(select, () => select.SelectedOption.ShouldBeSameAs(lucy));
    }

    [Fact]
    public void Form_Value_Uses_Mode_Specific_Selection_Property()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var lucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var select = new Desktop.Controls.Select();
        var formItem = (IFormItemAware)select;

        formItem.SetFormValue(jack);
        formItem.GetFormValue().ShouldBeSameAs(jack);
        select.SelectedOption.ShouldBeSameAs(jack);

        select.Mode = SelectMode.Multiple;
        IList<ISelectOption> selectedOptions = [jack, lucy];

        formItem.SetFormValue(selectedOptions);
        formItem.GetFormValue().ShouldBeSameAs(selectedOptions);
        select.SelectedOptions.ShouldBeSameAs(selectedOptions);

        formItem.ClearFormValue();

        select.SelectedOptions.ShouldBeNull();
    }

    [Fact]
    public void Template_Binds_Right_AddOn_Count_And_Handle_State_From_Axaml()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var lucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var rightAddOn  = new TextBlock { Text = "extra" };
        var suffixIcon  = new DownOutlined();
        var loadingIcon = new LoadingOutlined();
        var select = new Desktop.Controls.Select
        {
            Width                   = 240,
            Mode                    = SelectMode.Multiple,
            IsFilterEnabled         = true,
            IsAllowClear            = true,
            IsMotionEnabled         = false,
            MaxCount                = 3,
            IsShowMaxCountIndicator = true,
            SelectedOptions         = [jack, lucy],
            ContentRightAddOn       = rightAddOn,
            SuffixIcon              = suffixIcon,
            SuffixLoadingIcon       = loadingIcon
        };

        ShowInWindow(select, () =>
        {
            var indicator = GetVisualDescendant<SelectMaxCountIndicator>(select, "PART_SelectMaxCountIndicator");
            indicator.MaxCount.ShouldBe(3);
            indicator.SelectedCount.ShouldBe(2);
            indicator.IsVisible.ShouldBeTrue();

            var contentPresenter = GetVisualDescendant<ContentPresenter>(select, "PART_ContentRightAddOnPresenter");
            contentPresenter.Content.ShouldBeSameAs(rightAddOn);
            contentPresenter.IsVisible.ShouldBeTrue();

            var handle = GetVisualDescendant<SelectHandle>(select, "PART_SelectHandle");
            handle.OpenIndicator.ShouldBeSameAs(suffixIcon);
            handle.LoadingIcon.ShouldBeSameAs(loadingIcon);
            handle.IsFilterEnabled.ShouldBeTrue();
            handle.IsMotionEnabled.ShouldBeFalse();
            handle.IsAllowClear.ShouldBeTrue();
            handle.IsSelectionEmpty.ShouldBeFalse();
        });
    }

    [Fact]
    public void Template_Relays_AddOnDecoratedBox_Input_State_To_Handle()
    {
        var select = new Desktop.Controls.Select
        {
            Width = 240
        };

        ShowInWindow(select, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                select,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var handle = GetVisualDescendant<SelectHandle>(select, "PART_SelectHandle");

            addOnBox.IsInnerBoxHover = true;
            addOnBox.IsInnerBoxPressed = true;

            handle.IsInputHover.ShouldBeTrue();
            handle.IsInputPressed.ShouldBeTrue();
        });
    }

    [Fact]
    public void Custom_Size_Single_Selected_Text_Is_Vertically_Centered()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var select = new Desktop.Controls.Select
        {
            Width           = 240,
            Height          = 38,
            FontSize        = 15,
            SizeType        = CustomizableSizeType.Custom,
            IsAllowClear    = true,
            OptionsSource   = [jack],
            SelectedOption  = jack,
            PlaceholderText = "Please select"
        };

        ShowInWindow(select, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                select,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var textPresenter = GetVisualDescendant<TextPresenter>(select, "PART_TextPresenter");

            var textTop = textPresenter.TranslatePoint(default, addOnBox);
            textTop.ShouldNotBeNull();

            var textCenterY = textTop.Value.Y + textPresenter.Bounds.Height / 2;
            var boxCenterY  = addOnBox.Bounds.Height / 2;

            Math.Abs(textCenterY - boxCenterY).ShouldBeLessThanOrEqualTo(1.0);
        });
    }

    [Fact]
    public void Custom_Size_Multiple_Tag_Uses_Input_Content_Height()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var lucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var select = new Desktop.Controls.Select
        {
            Width           = 240,
            Height          = 38,
            Mode            = SelectMode.Multiple,
            SizeType        = CustomizableSizeType.Custom,
            IsFilterEnabled = false,
            SelectedOptions = [jack, lucy]
        };

        ShowInWindow(select, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                select,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var tag = select.GetVisualDescendants()
                            .OfType<SelectTag>()
                            .First(item => item.Text == "Jack");

            addOnBox.ContentFrame.ShouldNotBeNull();
            var contentPadding = addOnBox.ContentFrame!.Padding;
            var border         = addOnBox.InnerBoxBorderThickness;
            var expectedHeight = select.Height -
                                 contentPadding.Top -
                                 contentPadding.Bottom -
                                 border.Top -
                                 border.Bottom;

            tag.Bounds.Height.ShouldBe(expectedHeight, 0.5);
        });
    }

    [Fact]
    public void Custom_Size_Multiple_Tag_Keeps_Minimum_Height_And_Uses_Small_Vertical_Padding_When_Input_Is_Low()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var select = new Desktop.Controls.Select
        {
            Width           = 240,
            Height          = 28,
            Mode            = SelectMode.Multiple,
            SizeType        = CustomizableSizeType.Custom,
            IsFilterEnabled = false,
            SelectedOptions = [jack]
        };

        ShowInWindow(select, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                select,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var tag = select.GetVisualDescendants()
                            .OfType<SelectTag>()
                            .Single(item => item.Text == "Jack");
            var minimumTagHeight = GetThemeResource<double>(SelectTokenKind.MultipleItemHeight);
            var smallPadding     = GetThemeResource<Thickness>(SelectTokenKind.MultiModePaddingSM);

            addOnBox.ContentFrame.ShouldNotBeNull();
            addOnBox.ContentFrame!.Padding.Top.ShouldBe(smallPadding.Top);
            addOnBox.ContentFrame.Padding.Bottom.ShouldBe(smallPadding.Bottom);
            tag.Bounds.Height.ShouldBe(minimumTagHeight, 0.5);
        });
    }

    [Fact]
    public void Committing_Current_Single_Selection_Does_Not_Suppress_Next_External_Selection_Sync()
    {
        var jack = new SelectOption
        {
            Header  = "Jack",
            Content = "jack"
        };
        var lucy = new SelectOption
        {
            Header  = "Lucy",
            Content = "lucy"
        };
        var select = new Desktop.Controls.Select
        {
            Width          = 240,
            OptionsSource  = [jack, lucy],
            SelectedOption = jack
        };

        ShowInWindow(select, () =>
        {
            select.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var candidateList = GetCandidateList(select);
            candidateList.SelectedItem.ShouldBeSameAs(jack);

            candidateList.RaiseEvent(new RoutedEventArgs(SelectCandidateList.CommitEvent));
            Dispatcher.UIThread.RunJobs();

            select.IsDropDownOpen.ShouldBeFalse();

            select.SelectedOption = lucy;

            candidateList.SelectedItem.ShouldBeSameAs(lucy);
        });
    }

    [Fact]
    public void Detaching_Select_Cancels_Pending_Async_Options_Load()
    {
        var loader = new PendingSelectOptionsLoader();
        var select = new Desktop.Controls.Select
        {
            Width         = 240,
            OptionsLoader = loader
        };
        var window = CreateWindow(select);
        var cancellationObserved = false;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            InvokeHandleOpenDropRequest(select);

            loader.Started.Wait(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken).ShouldBeTrue();

            window.Close();
            Dispatcher.UIThread.RunJobs();

            cancellationObserved =
                loader.Cancelled.Wait(TimeSpan.FromMilliseconds(500), TestContext.Current.CancellationToken);
        }
        finally
        {
            loader.Complete();
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }

        cancellationObserved.ShouldBeTrue();
    }

    private static T GetVisualDescendant<T>(Control control, string name)
        where T : Control
    {
        var descendant = control.GetVisualDescendants()
                                .OfType<T>()
                                .SingleOrDefault(x => x.Name == name);
        descendant.ShouldNotBeNull();
        return descendant;
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = CreateWindow(content);

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

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 420,
            Height = 320
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
        };

        return window;
    }

    private static SelectCandidateList GetCandidateList(Desktop.Controls.Select select)
    {
        var field = typeof(Desktop.Controls.Select).GetField(
            "_candidateList",
            BindingFlags.Instance | BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        var candidateList = field.GetValue(select) as SelectCandidateList;
        candidateList.ShouldNotBeNull();
        return candidateList;
    }

    private static void InvokeHandleOpenDropRequest(Desktop.Controls.Select select)
    {
        var method = typeof(Desktop.Controls.Select).GetMethod(
            "HandleOpenDropRequest",
            BindingFlags.Instance | BindingFlags.NonPublic);

        method.ShouldNotBeNull();
        method.Invoke(select, null);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private sealed class PendingSelectOptionsLoader : ISelectOptionsAsyncLoader
    {
        private readonly TaskCompletionSource<SelectOptionsLoadResult> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ManualResetEventSlim Started { get; } = new();
        public ManualResetEventSlim Cancelled { get; } = new();

        public Task<SelectOptionsLoadResult> LoadAsync(object? context, CancellationToken token)
        {
            Started.Set();
            token.Register(static state => ((ManualResetEventSlim)state!).Set(), Cancelled);
            return _completion.Task;
        }

        public void Complete()
        {
            _completion.TrySetResult(new SelectOptionsLoadResult
            {
                Data = []
            });
        }
    }
}

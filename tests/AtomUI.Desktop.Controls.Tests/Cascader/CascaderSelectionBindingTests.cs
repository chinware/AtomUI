using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Cascader;

public class CascaderSelectionBindingTests
{
    static CascaderSelectionBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectedOption_DefaultBinding_Updates_ViewModel_When_Control_Value_Changes()
    {
        var westLake = new CascaderOption
        {
            Header = "West Lake",
            Value  = "west-lake"
        };
        var lingyin = new CascaderOption
        {
            Header = "Lingyin",
            Value  = "lingyin"
        };
        var viewModel = new CascaderBindingViewModel
        {
            SelectedOption = westLake
        };
        var cascader = new Desktop.Controls.Cascader
        {
            OptionsSource = new[] { westLake, lingyin }
        };
        cascader.Bind(
            Desktop.Controls.Cascader.SelectedOptionProperty,
            new Binding(nameof(CascaderBindingViewModel.SelectedOption))
            {
                Source = viewModel
            });

        ShowInWindow(cascader, () =>
        {
            cascader.SelectedOption.ShouldBeSameAs(westLake);

            cascader.SelectedOption = lingyin;
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedOption.ShouldBeSameAs(lingyin);
        });
    }

    [Fact]
    public void SelectedOptions_DefaultBinding_Updates_ViewModel_When_Control_Value_Changes()
    {
        var westLake = new CascaderOption
        {
            Header = "West Lake",
            Value  = "west-lake"
        };
        var lingyin = new CascaderOption
        {
            Header = "Lingyin",
            Value  = "lingyin"
        };
        var viewModel = new CascaderBindingViewModel();
        var cascader = new Desktop.Controls.Cascader
        {
            IsMultiple    = true,
            OptionsSource = new[] { westLake, lingyin }
        };
        cascader.Bind(
            Desktop.Controls.Cascader.SelectedOptionsProperty,
            new Binding(nameof(CascaderBindingViewModel.SelectedOptions))
            {
                Source = viewModel
            });

        ShowInWindow(cascader, () =>
        {
            cascader.SelectedOptions = [westLake, lingyin];
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedOptions.ShouldNotBeNull();
            viewModel.SelectedOptions.ShouldBe([westLake, lingyin]);
        });
    }

    [Fact]
    public void SelectedOptions_ObservableCollection_Mutation_Refreshes_Result_And_CascaderView_Selection()
    {
        var westLake = new CascaderOption
        {
            Header = "West Lake",
            Value  = "west-lake"
        };
        var lingyin = new CascaderOption
        {
            Header = "Lingyin",
            Value  = "lingyin"
        };
        var viewModel = new CascaderBindingViewModel
        {
            SelectedOptions = new ObservableCollection<ICascaderOption>
            {
                westLake
            }
        };
        var cascader = new Desktop.Controls.Cascader
        {
            Width                   = 240,
            IsMultiple              = true,
            IsAllowClear            = true,
            IsMotionEnabled         = false,
            IsShowMaxCountIndicator = true,
            MaxCount                = 5,
            OptionsSource           = new[] { westLake, lingyin }
        };
        cascader.Bind(
            Desktop.Controls.Cascader.SelectedOptionsProperty,
            new Binding(nameof(CascaderBindingViewModel.SelectedOptions))
            {
                Source = viewModel
            });

        ShowInWindow(cascader, () =>
        {
            FindSelectTag(cascader, "West Lake").ShouldNotBeNull();

            cascader.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var cascaderView = GetCascaderView(cascader);
            cascaderView.SelectedOptions.ShouldNotBeNull();
            cascaderView.SelectedOptions.ShouldContain(westLake);

            viewModel.SelectedOptions!.Add(lingyin);
            Dispatcher.UIThread.RunJobs();

            cascader.EffectiveSelectedOptions.ShouldNotBeNull();
            cascader.EffectiveSelectedOptions.ShouldBe([westLake, lingyin]);
            cascader.SelectedCount.ShouldBe(2);
            FindSelectTag(cascader, "Lingyin").ShouldNotBeNull();
            cascaderView.SelectedOptions.ShouldNotBeNull();
            cascaderView.SelectedOptions.ShouldContain(lingyin);

            viewModel.SelectedOptions.Remove(westLake);
            Dispatcher.UIThread.RunJobs();

            cascader.EffectiveSelectedOptions.ShouldNotBeNull();
            cascader.EffectiveSelectedOptions.ShouldBe([lingyin]);
            cascader.SelectedCount.ShouldBe(1);
            cascader.GetVisualDescendants()
                    .OfType<SelectTag>()
                    .ShouldNotContain(tag => tag.Text == "West Lake");
            cascaderView.SelectedOptions.ShouldNotBeNull();
            cascaderView.SelectedOptions.ShouldNotContain(westLake);

            viewModel.SelectedOptions.Clear();
            Dispatcher.UIThread.RunJobs();

            cascader.EffectiveSelectedOptions.ShouldNotBeNull();
            cascader.EffectiveSelectedOptions.ShouldBeEmpty();
            cascader.SelectedCount.ShouldBe(0);
            cascader.IsSelectionEmpty.ShouldBeTrue();
            cascaderView.SelectedOptions.ShouldNotBeNull();
            cascaderView.SelectedOptions.ShouldBeEmpty();
        });
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

        return new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
        };
    }

    private static CascaderView GetCascaderView(Desktop.Controls.Cascader cascader)
    {
        var field = typeof(Desktop.Controls.Cascader).GetField(
            "_cascaderView",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        var cascaderView = field.GetValue(cascader) as CascaderView;
        cascaderView.ShouldNotBeNull();
        return cascaderView!;
    }

    private static SelectTag? FindSelectTag(Visual root, string text)
    {
        return root.GetVisualDescendants()
                   .OfType<SelectTag>()
                   .FirstOrDefault(tag => tag.Text == text);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private sealed class CascaderBindingViewModel : INotifyPropertyChanged
    {
        private ICascaderOption? _selectedOption;
        private IList<ICascaderOption>? _selectedOptions;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICascaderOption? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (ReferenceEquals(_selectedOption, value))
                {
                    return;
                }
                _selectedOption = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOption)));
            }
        }

        public IList<ICascaderOption>? SelectedOptions
        {
            get => _selectedOptions;
            set
            {
                if (ReferenceEquals(_selectedOptions, value))
                {
                    return;
                }
                _selectedOptions = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOptions)));
            }
        }
    }
}

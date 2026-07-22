using System.ComponentModel;
using System.IO;
using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomDialog = AtomUI.Desktop.Controls.Dialog;
using AtomFloatButtonGroup = AtomUI.Desktop.Controls.FloatButtonGroup;
using AtomFloatButtonGroupHost = AtomUI.Desktop.Controls.FloatButtonGroupHost;
using AtomImagePreviewer = AtomUI.Desktop.Controls.ImagePreviewer;
using AtomListView = AtomUI.Desktop.Controls.ListView;
using AtomPagination = AtomUI.Desktop.Controls.Pagination;
using AtomTour = AtomUI.Desktop.Controls.Tour;

namespace AtomUI.Desktop.Controls.Tests.SelectionSemantics;

public class ControlledStateBindingTests
{
    static ControlledStateBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void P2_Controlled_State_Properties_Expose_Selection_Projection_As_OneWay()
    {
        var selectedItemsMetadata = AtomListView.SelectedItemsProperty.GetMetadata(typeof(AtomListView));
        selectedItemsMetadata.DefaultBindingMode.ShouldBe(BindingMode.OneWay);
        selectedItemsMetadata.EnableDataValidation.ShouldBe(false);

        AssertTwoWay(AtomPagination.CurrentPageProperty, typeof(AtomPagination));
        AssertTwoWay(AtomPagination.PageSizeProperty, typeof(AtomPagination));
        AssertTwoWay(AtomImagePreviewer.IsOpenProperty, typeof(AtomImagePreviewer));
        AssertTwoWay(AtomImagePreviewer.CurrentIndexProperty, typeof(AtomImagePreviewer));
        AssertTwoWay(AtomDialog.IsOpenProperty, typeof(AtomDialog));
        AssertTwoWay(AtomFloatButtonGroup.IsOpenProperty, typeof(AtomFloatButtonGroup));
        AssertTwoWay(AtomFloatButtonGroupHost.IsOpenProperty, typeof(AtomFloatButtonGroupHost));
        AssertTwoWay(AtomTour.IsOpenProperty, typeof(AtomTour));
        AssertTwoWay(AtomTour.CurrentIndexProperty, typeof(AtomTour));
    }

    [Fact]
    public void Pagination_CurrentPage_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            CurrentPage = 1
        };
        var pagination = new AtomPagination
        {
            Total = 100
        };
        pagination.Bind(
            AtomPagination.CurrentPageProperty,
            new Binding(nameof(ControlledStateBindingViewModel.CurrentPage))
            {
                Source = viewModel
            });

        pagination.CurrentPage.ShouldBe(1);

        pagination.SetCurrentValue(AtomPagination.CurrentPageProperty, 2);
        Dispatcher.UIThread.RunJobs();

        viewModel.CurrentPage.ShouldBe(2);
    }

    [Fact]
    public void Pagination_PageSize_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            PageSize = 10
        };
        var pagination = new AtomPagination
        {
            Total = 100
        };
        pagination.Bind(
            AtomPagination.PageSizeProperty,
            new Binding(nameof(ControlledStateBindingViewModel.PageSize))
            {
                Source = viewModel
            });

        pagination.PageSize.ShouldBe(10);

        pagination.SetCurrentValue(AtomPagination.PageSizeProperty, 20);
        Dispatcher.UIThread.RunJobs();

        viewModel.PageSize.ShouldBe(20);
    }

    [Fact]
    public void Open_State_DefaultBindingMode_Updates_ViewModel()
    {
        AssertBooleanPropertyUpdatesViewModel(new AtomImagePreviewer(), AtomImagePreviewer.IsOpenProperty);
        AssertBooleanPropertyUpdatesViewModel(new AtomDialog(), AtomDialog.IsOpenProperty);
        AssertBooleanPropertyUpdatesViewModel(new AtomFloatButtonGroup(), AtomFloatButtonGroup.IsOpenProperty);
        AssertBooleanPropertyUpdatesViewModel(new AtomFloatButtonGroupHost(), AtomFloatButtonGroupHost.IsOpenProperty);
        AssertBooleanPropertyUpdatesViewModel(new AtomTour(), AtomTour.IsOpenProperty);
    }

    [Fact]
    public void CurrentIndex_DefaultBindingMode_Updates_ViewModel()
    {
        AssertIntegerPropertyUpdatesViewModel(new AtomImagePreviewer(), AtomImagePreviewer.CurrentIndexProperty);
        AssertIntegerPropertyUpdatesViewModel(new AtomTour(), AtomTour.CurrentIndexProperty);
    }

    [Fact]
    public void ImagePreviewer_CurrentIndex_Relay_Uses_TwoWay_Binding()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/AbstractImagePreviewer.cs");

        source.ShouldContain("ImagePreviewerDialog.CurrentIndexProperty,\n            BindingMode.TwoWay)");
        source.ShouldContain("ImagePreviewerOverlayHost.CurrentIndexProperty,\n            BindingMode.TwoWay)");
    }

    [Fact]
    public void FloatButtonGroup_Open_Interactions_Use_CurrentValue_For_Controlled_State()
    {
        var groupSource = ReadRepoFile("src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroup.cs");
        var hostSource  = ReadRepoFile("src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroupHost.cs");

        groupSource.ShouldNotContain("IsOpenProperty, true, BindingPriority.Style");
        groupSource.ShouldNotContain("IsOpenProperty, false, BindingPriority.Style");
        groupSource.ShouldNotContain("IsOpenProperty, !IsOpen, BindingPriority.Style");
        hostSource.ShouldNotContain("IsOpenProperty, true, BindingPriority.Style");
        hostSource.ShouldNotContain("IsOpenProperty, false, BindingPriority.Style");
    }

    [Fact]
    public void FloatButtonGroup_Click_Toggle_Uses_Next_Open_State_For_Request_Direction()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroup.cs");

        source.ShouldContain("var nextIsOpen = !IsOpen;");
        source.ShouldContain("SetCurrentValue(IsOpenProperty, nextIsOpen);");
        source.ShouldContain("if (nextIsOpen)");
    }

    private static void AssertBooleanPropertyUpdatesViewModel(
        AvaloniaObject control,
        StyledProperty<bool> property)
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            IsOpen = false
        };
        control.Bind(
            property,
            new Binding(nameof(ControlledStateBindingViewModel.IsOpen))
            {
                Source = viewModel
            });

        control.GetValue(property).ShouldBeFalse();

        control.SetCurrentValue(property, true);
        Dispatcher.UIThread.RunJobs();

        viewModel.IsOpen.ShouldBeTrue();
    }

    private static void AssertIntegerPropertyUpdatesViewModel(
        AvaloniaObject control,
        AvaloniaProperty<int> property)
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            CurrentIndex = 0
        };
        control.Bind(
            property,
            new Binding(nameof(ControlledStateBindingViewModel.CurrentIndex))
            {
                Source = viewModel
            });

        control.GetValue(property).ShouldBe(0);

        control.SetCurrentValue(property, 2);
        Dispatcher.UIThread.RunJobs();

        viewModel.CurrentIndex.ShouldBe(2);
    }

    private static void AssertTwoWay(AvaloniaProperty property, Type ownerType)
    {
        var metadata = property.GetMetadata(ownerType);

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
    }

    private static string ReadRepoFile(string relativePath)
    {
        var current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            var candidate = Path.Combine(current, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException($"Could not locate repository file: {relativePath}");
    }

    private sealed class ControlledStateBindingViewModel : INotifyPropertyChanged
    {
        private int _currentPage;
        private int _pageSize;
        private int _currentIndex;
        private bool _isOpen;
        private System.Collections.IList? _selectedItems;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize == value)
                {
                    return;
                }

                _pageSize = value;
                RaisePropertyChanged(nameof(PageSize));
            }
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value)
                {
                    return;
                }

                _currentIndex = value;
                RaisePropertyChanged(nameof(CurrentIndex));
            }
        }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen == value)
                {
                    return;
                }

                _isOpen = value;
                RaisePropertyChanged(nameof(IsOpen));
            }
        }

        public System.Collections.IList? SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (ReferenceEquals(_selectedItems, value))
                {
                    return;
                }

                _selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

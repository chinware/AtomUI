using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class BoxPanelShowCase : ReactiveUserControl<BoxPanelViewModel>
{
    private Border? _addedSpacing; // 跟踪添加的间距
    private readonly List<Border> _addedPlaceholders = new(); // 跟踪添加的占位符

    public BoxPanelShowCase()
    {
        InitializeComponent();
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        // Basic - Orientation switch
        Vertical.IsCheckedChanged += (sender, args) =>
        {
            if (Vertical.IsChecked == true)
            {
                BasicBoxPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
            }
        };
        Horizontal.IsCheckedChanged += (s, e) =>
        {
            if (Horizontal.IsChecked == true)
            {
                BasicBoxPanel.Orientation = Avalonia.Layout.Orientation.Horizontal;
            }
        };

        // Flex Ratio - Orientation switch
        Vertical1.IsCheckedChanged += (s, e) =>
        {
            if (Vertical1.IsChecked == true)
            {
                FlexBoxPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
            }
        };
        Horizontal1.IsCheckedChanged += (s, e) =>
        {
            if (Horizontal1.IsChecked == true)
            {
                FlexBoxPanel.Orientation = Avalonia.Layout.Orientation.Horizontal;
            }
        };

        // JustifyContent
        JustifyFlexStart.IsCheckedChanged += (s, e) =>
        {
            if (JustifyFlexStart.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent =
                    JustifyContent.FlexStart;
            }
        };
        JustifyFlexEnd.IsCheckedChanged += (s, e) =>
        {
            if (JustifyFlexEnd.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent = JustifyContent.FlexEnd;
            }
        };
        JustifyCenter.IsCheckedChanged += (s, e) =>
        {
            if (JustifyCenter.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent = JustifyContent.Center;
            }
        };
        JustifySpaceBetween.IsCheckedChanged += (s, e) =>
        {
            if (JustifySpaceBetween.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent =
                    JustifyContent.SpaceBetween;
            }
        };
        JustifySpaceAround.IsCheckedChanged += (s, e) =>
        {
            if (JustifySpaceAround.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent =
                    JustifyContent.SpaceAround;
            }
        };
        JustifySpaceEvenly.IsCheckedChanged += (s, e) =>
        {
            if (JustifySpaceEvenly.IsChecked == true)
            {
                JustifyContentBoxPanel.JustifyContent =
                    JustifyContent.SpaceEvenly;
            }
        };

        // AlignItems
        AlignFlexStart.IsCheckedChanged += (s, e) =>
        {
            if (AlignFlexStart.IsChecked == true)
            {
                AlignItemsBoxPanel.AlignItems = AlignItems.FlexStart;
            }
        };
        AlignFlexEnd.IsCheckedChanged += (s, e) =>
        {
            if (AlignFlexEnd.IsChecked == true)
            {
                AlignItemsBoxPanel.AlignItems = AlignItems.FlexEnd;
            }
        };
        AlignCenter.IsCheckedChanged += (s, e) =>
        {
            if (AlignCenter.IsChecked == true)
            {
                AlignItemsBoxPanel.AlignItems = AlignItems.Center;
            }
        };
        AlignStretch.IsCheckedChanged += (s, e) =>
        {
            if (AlignStretch.IsChecked == true)
            {
                AlignItemsBoxPanel.AlignItems = AlignItems.Stretch;
            }
        };

        // FlexWrap
        NoWrap.IsCheckedChanged += (s, e) =>
        {
            if (NoWrap.IsChecked == true)
            {
                WrapBoxPanel.Wrap = FlexWrap.NoWrap;
            }
        };
        Wrap.IsCheckedChanged += (s, e) =>
        {
            if (Wrap.IsChecked == true)
            {
                WrapBoxPanel.Wrap = FlexWrap.Wrap;
            }
        };
        WrapReverse.IsCheckedChanged += (s, e) =>
        {
            if (WrapReverse.IsChecked == true)
            {
                WrapBoxPanel.Wrap = FlexWrap.WrapReverse;
            }
        };

        // AlignContent
        ContentFlexStart.IsCheckedChanged += (s, e) =>
        {
            if (ContentFlexStart.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent = AlignContent.FlexStart;
            }
        };
        ContentFlexEnd.IsCheckedChanged += (s, e) =>
        {
            if (ContentFlexEnd.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent = AlignContent.FlexEnd;
            }
        };
        ContentCenter.IsCheckedChanged += (s, e) =>
        {
            if (ContentCenter.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent = AlignContent.Center;
            }
        };
        ContentStretch.IsCheckedChanged += (s, e) =>
        {
            if (ContentStretch.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent = AlignContent.Stretch;
            }
        };
        ContentSpaceBetween.IsCheckedChanged += (s, e) =>
        {
            if (ContentSpaceBetween.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent =
                    AlignContent.SpaceBetween;
            }
        };
        ContentSpaceAround.IsCheckedChanged += (s, e) =>
        {
            if (ContentSpaceAround.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent =
                    AlignContent.SpaceAround;
            }
        };
        ContentSpaceEvenly.IsCheckedChanged += (s, e) =>
        {
            if (ContentSpaceEvenly.IsChecked == true)
            {
                AlignContentBoxPanel.AlignContent =
                    AlignContent.SpaceEvenly;
            }
        };
    }

    private void HandleSpaceSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (ChangeSpaceBoxPanel != null)
        {
            ChangeSpaceBoxPanel.Spacing = e.NewValue;
        }
    }

    private void HandleAddSpaceButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (ChangeSpaceBoxPanel == null || AddSpaceButton == null)
            return;

        if (_addedSpacing == null)
        {
            // 添加固定间距
            _addedSpacing = new Border
            {
                Width      = ChangeSpaceBoxPanel.Orientation == Avalonia.Layout.Orientation.Horizontal ? 40 : 0,
                Height     = ChangeSpaceBoxPanel.Orientation == Avalonia.Layout.Orientation.Vertical ? 40 : 0,
                Background = Avalonia.Media.Brushes.Transparent
            };
            ChangeSpaceBoxPanel.Children.Add(_addedSpacing);
            AddSpaceButton.Content = "Remove space (40px)";
        }
        else
        {
            // 移除固定间距
            ChangeSpaceBoxPanel.Children.Remove(_addedSpacing);
            _addedSpacing          = null;
            AddSpaceButton.Content = "Add a space of size 40";
        }
    }

    private int _flexToggle = 0;

    private void HandleChangFlexButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (ChangeSpaceBoxPanel?.Children.Count >= 3)
        {
            var flexItem = ChangeSpaceBoxPanel.Children[2];
            _flexToggle = (_flexToggle + 1) % 4; // Cycle through 1, 2, 3, 0
            BoxPanel.SetFlex(flexItem, _flexToggle == 0 ? 1 : _flexToggle);
        }
    }

    private void HandleAddFlexButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (AddPlaceholderBoxPanel == null || AddFlexButton == null)
            return;

        if (_addedPlaceholders.Count == 0)
        {
            // 添加 Flex 占位符
            var placeholder = new Border
            {
                Background = Avalonia.Media.Brushes.LightGray
            };
            BoxPanel.SetFlex(placeholder, 1);

            AddPlaceholderBoxPanel.Children.Add(placeholder);
            _addedPlaceholders.Add(placeholder);

            AddFlexButton.Content = "Remove placeholder";
        }
        else
        {
            // 移除最后一个占位符
            var lastPlaceholder = _addedPlaceholders[_addedPlaceholders.Count - 1];
            AddPlaceholderBoxPanel.Children.Remove(lastPlaceholder);
            _addedPlaceholders.RemoveAt(_addedPlaceholders.Count - 1);

            if (_addedPlaceholders.Count == 0)
            {
                AddFlexButton.Content = "Add a placeholder flex";
            }
        }
    }
}
using System.Collections;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

internal class SelectResultOptionsBox : TemplatedControl
{
    public static readonly DirectProperty<SelectResultOptionsBox, IList?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<SelectResultOptionsBox, IList?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions,
            (o, v) => o.SelectedOptions = v);
    
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectResultOptionsBox, SelectMode>(nameof(Mode));
    
    public static readonly StyledProperty<bool> IsSearchEnabledProperty =
        Select.IsSearchEnabledProperty.AddOwner<SelectResultOptionsBox>();
    
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<SelectResultOptionsBox, bool>(nameof(IsDropDownOpen));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<SelectResultOptionsBox>();
    
    private IList? _selectedOptions;

    public IList? SelectedOptions
    {
        get => _selectedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _selectedOptions, value);
    }
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public bool IsSearchEnabled
    {
        get => GetValue(IsSearchEnabledProperty);
        set => SetValue(IsSearchEnabledProperty, value);
    }
    
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    private WrapPanel? _defaultPanel;
    private SelectSearchTextBox? _searchTextBox;
    private protected readonly Dictionary<object, IDisposable> _tagsBindingDisposables = new();

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedOptionsProperty)
        {
            HandleSelectedOptionsChanged();
        }
        else if (change.Property == IsSearchEnabledProperty ||
                 change.Property == ModeProperty)
        {
            ConfigureSearchTextControl();
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            ConfigureSearchTextReadOnly();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = false;
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _defaultPanel  = e.NameScope.Find<WrapPanel>(SelectResultOptionsBoxThemeConstants.DefaultPanelPart);
        _searchTextBox = new SelectSearchTextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        BindUtils.RelayBind(this, SizeTypeProperty, _searchTextBox, SizeTypeProperty);
        if (IsSearchEnabled)
        {
            if (Mode == SelectMode.Multiple)
            {
                _defaultPanel?.Children.Add(_searchTextBox);
            }
        }

        ConfigureSearchTextControl();
        HandleSelectedOptionsChanged();
    }

    private void HandleSelectedOptionsChanged()
    {
        if (_defaultPanel != null)
        {
            _searchTextBox?.Clear();
            _defaultPanel.Children.Clear();
            foreach (var entry in _tagsBindingDisposables)
            {
                entry.Value.Dispose();
            }
            _tagsBindingDisposables.Clear();
            if (_selectedOptions != null)
            {
                foreach (var item in _selectedOptions)
                {
                    if (item is SelectOption option)
                    {
                        var tag = new SelectTag
                        {
                            TagText = option.Header,
                            Option  = option
                        };
                        _tagsBindingDisposables.Add(tag, BindUtils.RelayBind(this, SizeTypeProperty, tag, SizeTypeProperty));
                        _defaultPanel.Children.Add(tag);
                    }
                }
            }

            if (_searchTextBox != null)
            {
                _defaultPanel.Children.Add(_searchTextBox);
                _searchTextBox.Focus();
            }
        }
    }
    
    private void ConfigureSearchTextControl()
    {
        if (_searchTextBox != null)
        {
            if (Mode == SelectMode.Multiple)
            {
                _searchTextBox.IsVisible = IsSearchEnabled;
            }
        }
    }

    private void ConfigureSearchTextReadOnly()
    {
        if (_searchTextBox != null)
        {
            if (IsDropDownOpen)
            {
                _searchTextBox.IsReadOnly = false;
            }
            else
            {
                _searchTextBox.Clear();
                _searchTextBox.IsReadOnly = true;
            }
        }
    }
}